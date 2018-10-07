using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent
{
    class GetSpotsPerServerList
    {
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private readonly float _adjustmentFactor;
        private readonly int _resourcePoolId;
        private List<AgentServerObject> agentServerList;
        public List<SpotsPerServerObject> SpotsPerServerList { get; private set; }

        public GetSpotsPerServerList(IDBContext eddsDbContext, IEnvironmentHelper environment, float adjustmentFactor, int resourcePoolId)
        {
            agentServerList = new List<AgentServerObject>();
            SpotsPerServerList = new List<SpotsPerServerObject>();
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            _adjustmentFactor = adjustmentFactor;
            _resourcePoolId = resourcePoolId;
            Run();
        }

        private void Run()
        {
            agentServerList = _environment.GetPoolAgentServerList(_resourcePoolId);
            foreach (AgentServerObject agent in agentServerList)
            {
                GetSpotsPerServer getSpotsPerServer = new GetSpotsPerServer(_eddsDbContext, _environment, agent.ArtifactID, _adjustmentFactor);
                SpotsPerServerObject spotsPerServerObject = new SpotsPerServerObject()
                {
                    AgentServerArtifactId = agent.ArtifactID,
                    Spots = getSpotsPerServer.Spots
                };
                SpotsPerServerList.Add(spotsPerServerObject);
            }
        }
    }
}
