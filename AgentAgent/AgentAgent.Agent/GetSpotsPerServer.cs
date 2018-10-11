using Relativity.API;
using System;

namespace AgentAgent.Agent
{
    /// <summary>
    /// This class will tell you how many spots a specific agent server has
    ///</summary>
    class GetSpotsPerServer
    {
        private AgentServer _agentServer;
        private readonly int _agentServerArtifactId;
        private readonly IEnvironmentHelper _environment;
        private readonly float _adjustmentFactor;
        public int Spots { get; private set; }

        public GetSpotsPerServer(IDBContext eddsDbContext, IEnvironmentHelper environment, int agentServerArtifactId, float adjustmentFactor)
        {
            _agentServerArtifactId = agentServerArtifactId;
            _environment = environment;
            _adjustmentFactor = adjustmentFactor;
            Run();
        }

        private void Run()
        {
            _agentServer = _environment.GetAgentServerObject(_agentServerArtifactId);

            int ram = Convert.ToInt32(_agentServer.Memory / 1024 / 1024 / 1024);
            int cores = _agentServer.ProcessorCores;
            int agentCount = _agentServer.AgentCount;

            if (ram > cores)
            {
                Spots = Convert.ToInt32((cores * _adjustmentFactor) - agentCount);
            }

            else if (cores > ram)
            {
                Spots = Convert.ToInt32((ram * _adjustmentFactor) - agentCount);
            }
        }
    }
}
