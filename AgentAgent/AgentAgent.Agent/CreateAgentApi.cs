using System;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Services.Agent;
using Relativity.Services.ResourceServer;
using AgentAgent.Agent.Interfaces;



namespace AgentAgent.Agent
{
    class CreateAgentApi : ICreateAgent
    {
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private readonly IAgentHelper _helper;
        private int _agentTypeArtifactId;
        private int _agentServerArtifactId;
        private int _runInterval;

        public CreateAgentApi(IDBContext eddsDbContext, IEnvironmentHelper environment, IAgentHelper helper)
        {
            _helper = helper;
            _eddsDbContext = eddsDbContext;
            _environment = environment;
        }

        private string CreateAgentName()
        {
            int agentCount = _environment.GetAgentCount(_agentTypeArtifactId);
            string agentTypeName = _environment.GetTextIdByArtifactId(_agentTypeArtifactId);
            return string.Format("{0} ({1})", agentTypeName, agentCount + 1);
        }

        public void Create(string agentTypeGuid, int agentServerArtifactId)
        {
            _agentServerArtifactId = agentServerArtifactId;
            _agentTypeArtifactId = _environment.GetArtifactIdFromGuid(agentTypeGuid);
            _runInterval = _environment.GetAgentRunIntervalByType(_agentTypeArtifactId);


            int agentId = ApiCreate().GetAwaiter().GetResult();

            if (agentId < 1)
            {
                throw new Exception(string.Format("API agent create failed. Returned agent ID: {0}", agentId));
            }
        }

        public async Task<int> ApiCreate()
        {
            using (IAgentManager agentManager = _helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.CurrentUser))
            {
                Relativity.Services.Agent.Agent newAgent = new Relativity.Services.Agent.Agent
                {
                    AgentType = new AgentTypeRef(_agentTypeArtifactId),
                    Enabled = true,
                    Interval = _runInterval,
                    Name = CreateAgentName(),
                    LoggingLevel = Relativity.Services.Agent.Agent.LoggingLevelEnum.Critical,
                    Server = new ResourceServerRef
                    {
                        ArtifactID = _agentServerArtifactId
                    }
                };
                return await agentManager.CreateSingleAsync(newAgent);
            }
        }
    }
}
