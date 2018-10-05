using AgentAgent.Agent.Objects;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class RunAgentDelete
    {
        private List<AgentsDesiredObject> _agentsToDelete;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentInformation _environmentInformation;

        public RunAgentDelete(IDBContext eddsDbContext, IEnvironmentInformation environmentInformation, List<AgentsDesiredObject> agentsToDelete)
        {
            _eddsDbContext = eddsDbContext;
            _environmentInformation = environmentInformation;
            _agentsToDelete = agentsToDelete;
        }

        public void Run()
        {
            int agentTypeId;
            int agentToDeleteId;
            List<SpotsPerServerObject> agentsPerServer = new List<SpotsPerServerObject>();
            DeleteAgent deleteAgent = new DeleteAgent(_eddsDbContext);
            foreach (var agent in _agentsToDelete)
            {                
                //set agentsPerServer = _environmentInformation.GetAgentsPerServerByPool();
                agentsPerServer = agentsPerServer.OrderByDescending(x => x.Spots).ToList();
                if (agentsPerServer[0].Spots > 0)
                {
                    agentTypeId = _environmentInformation.GetArtifactIdFromGuid(agent.Guid);
                    agentToDeleteId = deleteAgent.GetAgentIdToDelete(agentTypeId, agentsPerServer[0].AgentServerArtifactId);
                    deleteAgent.Delete(agentToDeleteId);
                    agent.Count -= 1;
                }
            }


        }
    }
}
