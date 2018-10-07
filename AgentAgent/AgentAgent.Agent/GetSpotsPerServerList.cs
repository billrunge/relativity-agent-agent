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
        private List<AgentServer> agentServerList;
        public List<SpotsPerServer> SpotsPerServerList { get; private set; }

        public GetSpotsPerServerList(IDBContext eddsDbContext, IEnvironmentHelper environment, float adjustmentFactor, int resourcePoolId)
        {
            agentServerList = new List<AgentServer>();
            SpotsPerServerList = new List<SpotsPerServer>();
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            _adjustmentFactor = adjustmentFactor;
            _resourcePoolId = resourcePoolId;
            Run();
        }

        private void Run()
        {
            agentServerList = _environment.GetPoolAgentServerList(_resourcePoolId);
            foreach (AgentServer agent in agentServerList)
            {
                GetSpotsPerServer getSpotsPerServer = new GetSpotsPerServer(_eddsDbContext, _environment, agent.ArtifactID, _adjustmentFactor);
                SpotsPerServer spotsPerServerObject = new SpotsPerServer()
                {
                    AgentServerArtifactId = agent.ArtifactID,
                    Spots = getSpotsPerServer.Spots
                };
                SpotsPerServerList.Add(spotsPerServerObject);
            }
        }
    }
}
