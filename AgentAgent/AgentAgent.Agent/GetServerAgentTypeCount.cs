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
        private List<AgentsPerPoolObject> _inputDesiredAgentsPerPool;

        // [Resource Pool - Agent Server ID - Spots]
        private List<SpotsPerPoolObject> _inputSpotsPerPool;

        //Intermediate Object
        private List<SpotsPerPoolObject> _desiredSpotsPerPool;

        private EnvironmentInformation _environmentInformation;

        public GetServerAgentTypeCount(IDBContext eddsDbContext, List<AgentsPerPoolObject> agentsPerPool, List<SpotsPerPoolObject> spotsPerPool)
        {
            _inputDesiredAgentsPerPool = agentsPerPool;
            _inputSpotsPerPool = spotsPerPool;
            _desiredSpotsPerPool = new List<SpotsPerPoolObject>();
            _environmentInformation = new EnvironmentInformation(eddsDbContext);
        }

        private void InputListConsolidater()
        {
            //Consilidate instances where ResourcePool and Agent Types match by summing desired agent count
            _inputDesiredAgentsPerPool = _inputDesiredAgentsPerPool
                .GroupBy(x => new
            {
                x.ResourcePoolArtifactId,
                x.AgentTypeGuid

            }).Select(x => new AgentsPerPoolObject
            {
                ResourcePoolArtifactId = x.Key.ResourcePoolArtifactId,
                AgentTypeGuid = x.Key.AgentTypeGuid,
                AgentCount = x.Sum(xs => xs.AgentCount)

            }).ToList<AgentsPerPoolObject>();
        }

        private void AccountForExistingAgents()
        {
            //Subtract existing agent counts against desired agent counts in the consilidated input list
            int agentTypeId = 0;
            foreach (AgentsPerPoolObject desiredAgentPerPool in _inputDesiredAgentsPerPool)
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
                        //Loop through each entry in the desired input table
                        foreach (var desiredAgentCount in _inputDesiredAgentsPerPool)
                        {
                            if (poolAgentSpot.ResourcePoolArtifactId == desiredAgentCount.ResourcePoolArtifactId)
                            {

                            }

                        }
                    }
                }

            }
        }


    }
}


//two input buckets(lists)

//[Resource Pool - Agent Type - Desired Spots]
//[Resource Pool - Agent Server ID - Spots]

//Output object
//[Resource Pool - Agent Id - Spots]

//Intermediate objects
//[Resoure Pool - Agent Count]

//1.
//- Determine total agent counts required by each resource pool
//--New object[Resource Pool - Agent Count]

//SELECT[ResourcePoolArtifactID],
//SUM([DesiredCount]) AS[TotalDesiredCount]
//FROM[EDDS].[eddsdbo].[PoolAgentTypeCount]
//GROUP BY[ResourcePoolArtifactID]

//2.
//- Loop through each agent server that belong to exactly 1 resource pool
//-- Subtract spots from[Resource Pool - Agent Count]
//-- Add spots to[Resource Pool - AgentServerID - Spots + new spots]
//-- Remove server from[Resource Pool - Agent Server ID - Spots]

//3.
//- Are there any resource pools left that need agents?
//-- If so, loop through all shared agent servers
//--- How many resource pools do they belong to with jobs?
//---- Loop through each resource pool with their share of the share resource pool
//---- If[Resource Pool - AgentServerID - Spots].spots / pools with jobs <= [Resource Pool - Agent Server ID - Spots].spots 
//----- Add spots to[Resource Pool - AgentServerID - Spots + new spots]
//----- Remove server from[Resource Pool - Agent Server ID - Spots]
//---- If[Resource Pool - AgentServerID - Spots + new spots].spots <= [Resource Pool - Agent Server ID - Spots].spots
//-----

