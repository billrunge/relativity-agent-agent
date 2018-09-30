using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class GetSpotsPerServer
    {
        private AgentServerObject _agentServer;
        private readonly int _agentServerArtifactId;
        private readonly ServerInformation _serverInformation;
        private readonly float _adjustmentFactor;
        public int Spots { get; private set; }

        public GetSpotsPerServer(IDBContext eddsDbContext, int agentServerArtifactId, float adjustmentFactor)
        {
            _agentServerArtifactId = agentServerArtifactId;
            _serverInformation = new ServerInformation(eddsDbContext);            
            _adjustmentFactor = adjustmentFactor;
            Spots = 0;
            Run();
        }

        private void Run()
        {
            _agentServer = _serverInformation.GetAgentServerObject(_agentServerArtifactId);
            
            int gbOfRam = Convert.ToInt32(_agentServer.Memory / 1024 / 1024 / 1024);
            int procCores = _agentServer.ProcessorCores;
            int agentCount = _agentServer.AgentCount;

            if (gbOfRam > procCores)
            {
                Spots = Convert.ToInt32((procCores * _adjustmentFactor) - agentCount);
            }

            else if (procCores > gbOfRam)
            {
                Spots = Convert.ToInt32((gbOfRam * _adjustmentFactor) - agentCount);
            }
        }

    }
}
