using Relativity.API;


namespace AgentAgent.Agent.CustomAgentTypes
{
    class ClusterUpgradeWorker : AgentType
    {
        private IDBContext _eddsDbContext;

        public ClusterUpgradeWorker(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Cluster Upgrade Worker";
            Guid = "50BE7641-0604-4BD2-9381-F0540301A87E";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = true;
            EddsQueueName = "ClusterUpgradeJobs";
        }

        public override AgentsPerPoolList DesiredAgentsPerPool()
        {
            AgentsPerPoolList outputList = new AgentsPerPoolList();
            string SQL = @"
                SELECT COUNT(*)
                FROM [ClusterUpgradeJobs]";
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
