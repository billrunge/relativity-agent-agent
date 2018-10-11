using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent
{
    /// <summary>
    /// This class outputs a list of SpotsPerServer objects
    ///</summary>
    class GetSpotsPerServerList
    {
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private readonly float _adjustmentFactor;
        private readonly int _resourcePoolId;
        private readonly bool _ignoreSearchServers;
        private List<AgentServer> agentServerList;
        public List<SpotsPerServer> SpotsPerServerList { get; private set; }

        public GetSpotsPerServerList(IDBContext eddsDbContext, IEnvironmentHelper environment, float adjustmentFactor, int resourcePoolId, bool ignoreSearchServers)
        {
            agentServerList = new List<AgentServer>();
            SpotsPerServerList = new List<SpotsPerServer>();
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            _adjustmentFactor = adjustmentFactor;
            _resourcePoolId = resourcePoolId;
            _ignoreSearchServers = ignoreSearchServers;
            Run();
        }

        private void Run()
        {
            if (_ignoreSearchServers)
            {
                agentServerList = _environment.GetPoolAgentServerList(_resourcePoolId);
            }
            else
            {
                agentServerList = _environment.GetPoolAgentServerListNoDtSearch(_resourcePoolId);
            }
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
