using Relativity.API;

namespace AgentAgent.Agent
{
    class ClusterUpgradeWorker : AgentType
    {
        private IDBContext _eddsDbContext;

        public ClusterUpgradeWorker(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "50BE7641-0604-4BD2-9381-F0540301A87E";
            RespectsResourcePool = false;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT COUNT(*)
                FROM [ClusterUpgradeJobs] WITH(NOLOCK)";
            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (jobCount > 0)
            {
                agentCount = 1;
            }

            return new AgentsDesired()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount
            };
        }
    }
}
