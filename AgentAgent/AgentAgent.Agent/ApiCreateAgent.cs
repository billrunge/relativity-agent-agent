using Relativity.API;
using Relativity.Services.Agent;
using Relativity.Services.ResourceServer;
using Relativity.Services.ServiceProxy;
using System;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class ApiCreateAgent : ICreateAgent
    {
        private AgentObject _agent;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;

        public ApiCreateAgent(IDBContext eddsDbContext, IEnvironmentHelper environment, string agentTypeGuid, int agentServerArtifactId)
        {
            _agent = new AgentObject();
            _eddsDbContext = eddsDbContext;
            _agent.AgentTypeGuid = agentTypeGuid;
            _agent.AgentServerArtifactId = agentServerArtifactId;
            _environment = environment;
            _agent.AgentTypeArtifactId = _environment.GetArtifactIdFromGuid(_agent.AgentTypeGuid);
            _agent.AgentArtifactTypeId = _environment.GetAgentArtifactType();
            _agent.SystemContainerId = _environment.GetSystemContainerId();
            _agent.RunInterval = _environment.GetAgentRunIntervalByType(_agent.AgentTypeArtifactId);
        }

        private string CreateAgentName()
        {
            int agentCount = _environment.GetAgentCount(_agent.AgentTypeArtifactId);
            string agentTypeName = _environment.GetTextIdByArtifactId(_agent.AgentTypeArtifactId);
            return string.Format("{0} ({1})", agentTypeName, agentCount + 1);
        }

        public void Create()
        {
            int agentId = ApiCreate().GetAwaiter().GetResult();

            if (agentId < 1)
            {
                throw new Exception(string.Format("API agent create failed. Returned agent ID: {0}", agentId));
            }
        }

        public async Task<int> ApiCreate()
        {
            ServiceFactorySettings settings = new ServiceFactorySettings(
                                                              new Uri("net.pipe://localhost/relativity.services/"),
                                                              new Uri("http://localhost/Relativity.Rest/API"),
                                                              new IntegratedAuthCredentials());

            using (IAgentManager agentManager = new ServiceFactory(settings).CreateProxy<IAgentManager>())
            {
                Relativity.Services.Agent.Agent newAgent = new Relativity.Services.Agent.Agent
                {
                    AgentType = new AgentTypeRef(_agent.AgentTypeArtifactId),
                    Enabled = true,
                    Interval = _agent.RunInterval,
                    Name = CreateAgentName(),
                    LoggingLevel = Relativity.Services.Agent.Agent.LoggingLevelEnum.Critical,
                    Server = new ResourceServerRef
                    {
                        ArtifactID = _agent.AgentServerArtifactId
                    }
                };
                return await agentManager.CreateSingleAsync(newAgent);                
            }
        }
    }
}
