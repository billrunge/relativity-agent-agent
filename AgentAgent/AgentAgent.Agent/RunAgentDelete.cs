using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgentAgent.Agent
{
    class RunAgentDelete
    {
        private readonly int _resourcePoolId;
        private List<AgentsDesiredObject> _agentsToDelete;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentInformation _environmentInformation;
        

        public RunAgentDelete(IDBContext eddsDbContext, IEnvironmentInformation environmentInformation, int resourcePoolId, List<AgentsDesiredObject> agentsToDelete)
        {
            _eddsDbContext = eddsDbContext;
            _environmentInformation = environmentInformation;
            _agentsToDelete = agentsToDelete;
            _resourcePoolId = resourcePoolId;
        }

        public void Run()
        {
            int agentTypeId;
            int agentToDeleteId;
            int counter;
            List<SpotsPerServerObject> agentsPerServer = new List<SpotsPerServerObject>();
            DeleteAgent deleteAgent = new DeleteAgent(_eddsDbContext);

            while (_agentsToDelete.Count > 0)
            {
                counter = _agentsToDelete.Count - 1;
                while (counter >= 0)
                {
                    _agentsToDelete[counter].Count = Math.Abs(_agentsToDelete[counter].Count);
                    agentTypeId = _environmentInformation.GetArtifactIdFromGuid(_agentsToDelete[counter].Guid);
                    agentsPerServer = _environmentInformation.GetAgentsPerServerByPool(agentTypeId, _resourcePoolId);
                    if (agentsPerServer.Count > 0)
                    {
                        agentsPerServer = agentsPerServer.OrderByDescending(x => x.Spots).ToList();
                        if (agentsPerServer[0].Spots > 0)
                        {
                            agentToDeleteId = deleteAgent.GetAgentIdToDelete(agentTypeId, agentsPerServer[0].AgentServerArtifactId);
                            deleteAgent.Delete(agentToDeleteId);
                            _agentsToDelete[counter].Count -= 1;

                            if (_agentsToDelete[counter].Count < 1)
                            {
                                _agentsToDelete.Remove(_agentsToDelete[counter]);
                            }

                        }
                        else
                        {
                            _agentsToDelete.Remove(_agentsToDelete[counter]);
                        }
                    }

                    counter -= 1;
                }

            }


        }
    }
}
