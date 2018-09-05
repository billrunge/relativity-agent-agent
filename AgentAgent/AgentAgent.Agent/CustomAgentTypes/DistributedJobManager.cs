using Relativity.API;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class DistributedJobManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public DistributedJobManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Distributed Job Manager";
            Guid = "E7EBE10A-CC1A-4D3F-A3D6-F9B6B2069B09";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = true;
            EddsQueueName = "DistributedJob";
        }

        public override AgentsPerPoolList DesiredAgentsPerPool()
        {
            AgentsPerPoolList outputList = new AgentsPerPoolList();
            string SQL = @"
                SELECT COUNT(*)
                FROM [DistributedJob]";
            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (jobCount > 0)
            {
                AgentsPerPoolObject agentsPerPoolObject = new AgentsPerPoolObject
                {
                    AgentCount = 1,
                    AgentTypeGuid = Guid,
                    ResourcePoolArtifactId = 0
                };
                outputList.Add(agentsPerPoolObject);
                return outputList;
            }
            else
            {
                return null;
            }
        }
    }
}
