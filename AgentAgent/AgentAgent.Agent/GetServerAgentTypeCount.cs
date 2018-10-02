using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class takes two input lists: a list of pools, agent types, and desired counts
//and a list of pools, agent servers assigned to those pools, and available spots
//Using this information, it will output a list of Resource Pools, Agent Servers, and allowed spots
//Then use this to determine the best arrangment of [Agent Server - Agent Type - Count]


namespace AgentAgent.Agent
{
    class GetServerAgentTypeCount
    {
        //[Resource Pool - Agent Type - Desired Spots]
        private List<AgentsDesired> _inputDesiredAgentsPerPool;

        // [Resource Pool - Agent Server ID - Spots]
        private List<SpotsPerPoolObject> _inputSpotsPerPool;

        //Intermediate Object
        private List<SpotsPerPoolObject> _desiredSpotsPerPool;

        //Output list
        public List<AgentTypePerServerObject> AgentTypePerServer { get; private set; }

        private EnvironmentInformation _environmentInformation;
        public bool RunComplete { get; private set; }

        public GetServerAgentTypeCount(IDBContext eddsDbContext, List<AgentsDesired> agentsPerPool, List<SpotsPerPoolObject> spotsPerPool)
        {
            _inputDesiredAgentsPerPool = agentsPerPool;
            _inputSpotsPerPool = spotsPerPool;
            _desiredSpotsPerPool = new List<SpotsPerPoolObject>();
            _environmentInformation = new EnvironmentInformation(eddsDbContext);
            AgentTypePerServer = new List<AgentTypePerServerObject>();
            RunComplete = false;
        }

        private void InputListConsolidater()
        {
            //Consilidate instances where ResourcePool and Agent Types match by summing desired agent count
            _inputDesiredAgentsPerPool = _inputDesiredAgentsPerPool
                .GroupBy(x => new
                {
                    x.ResourcePoolArtifactId,
                    x.AgentTypeGuid

                }).Select(x => new AgentsDesired
                {
                    ResourcePoolArtifactId = x.Key.ResourcePoolArtifactId,
                    AgentTypeGuid = x.Key.AgentTypeGuid,
                    AgentCount = x.Sum(xs => xs.AgentCount)

                }).ToList<AgentsDesired>();
        }

        private void AccountForExistingAgents()
        {
            //Subtract existing agent counts against desired agent counts in the consilidated input list
            int agentTypeId = 0;
            foreach (AgentsDesired desiredAgentPerPool in _inputDesiredAgentsPerPool)
            {
                agentTypeId = _environmentInformation.GetArtifactIdFromGuid(desiredAgentPerPool.AgentTypeGuid);
                desiredAgentPerPool.AgentCount -= _environmentInformation.GetAgentCountByPool(agentTypeId, desiredAgentPerPool.ResourcePoolArtifactId);
            }
        }

        private void GetDesiredSpotsPerPool()
        {
            //populate the intermediate object that just shows how many spots each resource pool wants
            //based off the consolidated list with existing agents accounted for
            _desiredSpotsPerPool = _inputDesiredAgentsPerPool
                .GroupBy(x => x.ResourcePoolArtifactId)
                .Select(x => new SpotsPerPoolObject
                {
                    ResourcePoolArtifactId = x.Key,
                    Spots = x.Sum(xs => xs.AgentCount)

                }).ToList<SpotsPerPoolObject>();
        }

        private void SinglePoolAgentDistributor()
        {
            //get a sequence of agent server artifact IDs that only occur in the list once (only belong to one resource pool)
            var singlePoolAgents = _inputSpotsPerPool
                .GroupBy(x => x.AgentServerArtifactId)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key);

            //Loop through each server in sequence
            foreach (var server in singlePoolAgents)
            {
                //Loop through each spot in the input spots per pool list
                foreach (var poolAgentSpot in _inputSpotsPerPool)
                {
                    //If the spot on the input list matches one of the unique entries
                    if (poolAgentSpot.AgentServerArtifactId == server)
                    {
                        bool breakLoop;
                        while (poolAgentSpot.Spots > 0)
                        {
                            //Create flag to see if we need to exit from while loop to avoid scenario 
                            //where there are agent spots remaining but no more agents are desired
                            breakLoop = true;
                            //Loop through each input desired agent
                            foreach (var desiredAgentCount in _inputDesiredAgentsPerPool)
                            {
                                if ((poolAgentSpot.ResourcePoolArtifactId == desiredAgentCount.ResourcePoolArtifactId) && (desiredAgentCount.AgentCount > 0 && poolAgentSpot.Spots > 0))
                                {
                                    //If we enter this loop, the while loop should continue
                                    breakLoop = false;

                                    //Subtract one desired agent to achieve a round robin like effect
                                    desiredAgentCount.AgentCount -= 1;

                                    //Subtract the spot from available spots
                                    poolAgentSpot.Spots -= 1;

                                    //Create agent creation record
                                    //Since we are adding these one at a time, we will have to consolidate at some point
                                    AgentTypePerServerObject agentTypePerServerObject = new AgentTypePerServerObject()
                                    {
                                        AgentServerArtifactId = poolAgentSpot.AgentServerArtifactId,
                                        AgentTypeGuid = desiredAgentCount.AgentTypeGuid,
                                        Count = 1
                                    };

                                    //Add agent creation record to output list
                                    AgentTypePerServer.Add(agentTypePerServerObject);

                                    //If there are no more desired agents required for the entry, remove it
                                    if (desiredAgentCount.AgentCount < 1)
                                    {
                                        _inputDesiredAgentsPerPool.Remove(desiredAgentCount);
                                    }

                                    //If there are no more available spots, break the loop
                                    if (poolAgentSpot.Spots < 1)
                                    {
                                        _inputSpotsPerPool.Remove(poolAgentSpot);
                                        break;
                                    }
                                }

                            }

                            if (breakLoop == true)
                            {
                                break;
                            }

                        }

                    }
                }

            }
        }

        private void CleanUpInputServerList()
        {
            //Clean up input server list to make sure it only shows resource pools with jobs
            //Creating a flag to trigger clean up
            bool removeServer;

            //Loop through each available server
            foreach (var poolAgentSpot in _inputSpotsPerPool)
            {
                //Set flag to true
                removeServer = true;
                //Loop through each desired job
                foreach (var desiredAgentCount in _inputDesiredAgentsPerPool)
                {
                    //If there are any applicable jobs, set remove flag to false
                    if (desiredAgentCount.ResourcePoolArtifactId == poolAgentSpot.ResourcePoolArtifactId)
                    {
                        removeServer = false;
                    }
                }

                //Remove entry from list if flag is tre
                if (removeServer)
                {
                    _inputSpotsPerPool.Remove(poolAgentSpot);
                }

            }

        }

        private void SharedPoolAgentDistributor()
        {
            //If there were enough servers dedicated to one resource pool to meet the needs of all jobs
            //No need to do anything

            if (_inputDesiredAgentsPerPool.Count() > 0)
            {
                CleanUpInputServerList();

                //get a sequence of agent server artifact IDs that only occur in the list once (only belong to one resource pool)
                var sharedPoolAgents = _inputSpotsPerPool
                    .GroupBy(x => x.AgentServerArtifactId)
                    .Where(x => x.Count() > 1)
                    .Select(x => new ServerPoolCountObject
                    {
                        AgentServerArtifactId = x.Key,
                        ResourcePoolCount = x.Count()
                    });

                var listOfPoolsFromDesiredList = _inputDesiredAgentsPerPool
                    .GroupBy(x => x.ResourcePoolArtifactId)
                    .Select(x => x.First())
                    .ToList();


                //Loop through sequence
                foreach (var server in sharedPoolAgents)
                {
                    //Loop through each server on server input list
                    foreach (var spotsPerPool in _inputSpotsPerPool)
                    {
                        //if the sequence's server artifactID == lists artifact ID
                        if (server.AgentServerArtifactId == spotsPerPool.AgentServerArtifactId)
                        {
                            //Divide available spots on the server by the amount of resource pools the agent belongs to
                            int distributedSpotsPerPool = spotsPerPool.Spots / server.ResourcePoolCount;

                            //Begin a loop to distribute the distributed spots
                            bool breakLoop;
                            while (distributedSpotsPerPool > 0)
                            {
                                breakLoop = true;
                                foreach (var desiredAgentsPerPool in _inputDesiredAgentsPerPool)
                                {
                                    if ((desiredAgentsPerPool.ResourcePoolArtifactId == spotsPerPool.ResourcePoolArtifactId) && (desiredAgentsPerPool.AgentCount > 0 && distributedSpotsPerPool > 0 && spotsPerPool.Spots > 0))
                                    {
                                        //If we enter this loop, the while loop should continue
                                        breakLoop = false;

                                        //Subtract one desired agent to achieve a round robin like effect
                                        desiredAgentsPerPool.AgentCount -= 1;

                                        //Subtract from spot allowance
                                        distributedSpotsPerPool -= 1;

                                        //Subtract the spot from available spots
                                        spotsPerPool.Spots -= 1;

                                        //Create agent creation record
                                        //Since we are adding these one at a time, we will have to consolidate at some point
                                        AgentTypePerServerObject agentTypePerServerObject = new AgentTypePerServerObject()
                                        {
                                            AgentServerArtifactId = spotsPerPool.AgentServerArtifactId,
                                            AgentTypeGuid = desiredAgentsPerPool.AgentTypeGuid,
                                            Count = 1
                                        };

                                        //Add agent creation record to output list
                                        AgentTypePerServer.Add(agentTypePerServerObject);

                                        //If there are no more desired agents required for the entry, remove it
                                        if (desiredAgentsPerPool.AgentCount < 1)
                                        {
                                            _inputDesiredAgentsPerPool.Remove(desiredAgentsPerPool);
                                        }

                                        //If there are no more available spots, break the loop
                                        if (spotsPerPool.Spots < 1)
                                        {

                                            _inputSpotsPerPool.Remove(spotsPerPool);
                                            break;
                                        }

                                        if (distributedSpotsPerPool < 1)
                                        {
                                            break;
                                        }


                                    }

                                }

                                if (breakLoop == true){
                                    break;
                                }
                            }


                        }

                    }


                }
            }

        }

        private void OutputListConsilidater()
        {
            //Consilidate instances where ResourcePool and Agent Types match by summing desired agent count
            AgentTypePerServer = AgentTypePerServer
                .GroupBy(x => new
                {
                    x.AgentServerArtifactId,
                    x.AgentTypeGuid

                }).Select(x => new AgentTypePerServerObject
                {
                    AgentServerArtifactId = x.Key.AgentServerArtifactId,
                    AgentTypeGuid = x.Key.AgentTypeGuid,
                    Count = x.Sum(xs => xs.Count)

                }).ToList<AgentTypePerServerObject>();
        }

        public void Run()
        {
            InputListConsolidater();
            AccountForExistingAgents();
            GetDesiredSpotsPerPool();
            SinglePoolAgentDistributor();
            CleanUpInputServerList();
            SharedPoolAgentDistributor();
            OutputListConsilidater();
            RunComplete = true;
            
        }
    }
}

