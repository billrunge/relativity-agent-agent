using Relativity.API;
using System.Collections.Generic;

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
            _eddsDbContext = eddsDbContext;
            _environmentInformation = new EnvironmentInformation(_eddsDbContext);
            _serverInformation = new ServerInformation(_eddsDbContext);
            _adjustmentFactor = adjustmentFactor;
            agentServerList = new List<AgentServerObject>();
            _resourcePoolId = resourcePoolId;

        }

        private void Run()
        {
            agentServerList = _serverInformation.GetPoolAgentServerList(_resourcePoolId);

            foreach (var agent in agentServerList)
            {
                GetSpotsPerServer getSpotsPerServer = new GetSpotsPerServer(_eddsDbContext, agent.ArtifactID, _adjustmentFactor);
                SpotsPerServerObject serverSpots = new SpotsPerServerObject()
                {
                    AgentServerArtifactId = agent.ArtifactID,
                    ResourcePoolArtifactId = _resourcePoolId,
                    Spots = getSpotsPerServer.Spots
                };

                SpotsPerServerList.Add(serverSpots);


            }


        }
    }
}
