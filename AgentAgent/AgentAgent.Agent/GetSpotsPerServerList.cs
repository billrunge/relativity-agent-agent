using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class GetSpotsPerServerList
    {
        private readonly IDBContext _eddsDbContext;
        private readonly EnvironmentInformation _environmentInformation;
        private readonly ServerInformation _serverInformation;
        private readonly float _adjustmentFactor;
        private List<AgentServerObject> agentServerList;
        public List<SpotsPerServerObject> SpotsPerServerList { get; private set; }

        public GetSpotsPerServerList(IDBContext eddsDbContext, float adjustmentFactor)
        {
            _eddsDbContext = eddsDbContext;
            _environmentInformation = new EnvironmentInformation(_eddsDbContext);
            _serverInformation = new ServerInformation(_eddsDbContext);
            _adjustmentFactor = adjustmentFactor;
            agentServerList = new List<AgentServerObject>();
        }

        private void Run()
        {
            agentServerList = _serverInformation.GetAgentServerList();          

            foreach (var agent in agentServerList)
            {
                List<int> poolIdList = _serverInformation.GetPoolIDsByServerId(agent.ArtifactID);

                foreach (int poolId in poolIdList)
                {
                    GetSpotsPerServer getSpotsPerServer = new GetSpotsPerServer(_eddsDbContext, agent.ArtifactID, _adjustmentFactor);
                    SpotsPerServerObject serverSpotsObject = new SpotsPerServerObject()
                    {
                        AgentServerArtifactId = agent.ArtifactID,
                        ResourcePoolArtifactId = poolId,
                        Spots = getSpotsPerServer.Spots
                    };

                    SpotsPerServerList.Add(serverSpotsObject);
                }


            }

            
        }
    }
}
