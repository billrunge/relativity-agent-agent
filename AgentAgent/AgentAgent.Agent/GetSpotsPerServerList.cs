using Relativity.API;
using System.Collections.Generic;
using AgentAgent.Agent.Objects;

namespace AgentAgent.Agent
{
    class GetSpotsPerServerList
    {
        private readonly IDBContext _eddsDbContext;
        private readonly EnvironmentInformation _environmentInformation;
        private readonly ServerInformation _serverInformation;
        private readonly float _adjustmentFactor;
        private readonly int _resourcePoolId;
        private List<AgentServerObject> agentServerList;
        public List<SpotsPerServerObject> SpotsPerServerList { get; private set; }

        public GetSpotsPerServerList(IDBContext eddsDbContext, float adjustmentFactor, int resourcePoolId)
        {
            agentServerList = new List<AgentServerObject>();
            SpotsPerServerList = new List<SpotsPerServerObject>();

            _eddsDbContext = eddsDbContext;
            _environmentInformation = new EnvironmentInformation(_eddsDbContext);
            _serverInformation = new ServerInformation(_eddsDbContext);
            _adjustmentFactor = adjustmentFactor;
            _resourcePoolId = resourcePoolId;
            Run();
        }

        private void Run()
        {
            agentServerList = _serverInformation.GetPoolAgentServerList(_resourcePoolId);
            foreach (AgentServerObject agent in agentServerList)
            {
                GetSpotsPerServer getSpotsPerServer = new GetSpotsPerServer(_eddsDbContext, agent.ArtifactID, _adjustmentFactor);
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
