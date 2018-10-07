using Relativity.API;

namespace AgentAgent.Agent
{
    class AssistedReviewManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public AssistedReviewManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "6D19C56E-2C1E-4BF6-A7A8-EDBD9DEF0588";
            RespectsResourcePool = false;
        }

        public override AgentsDesiredObject AgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT COUNT(*)
                FROM [AssistedReviewMasterQueue] WITH(NOLOCK)";

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (jobCount > 0)
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount

            };

            return agentsDesired;
        }
    }
}
