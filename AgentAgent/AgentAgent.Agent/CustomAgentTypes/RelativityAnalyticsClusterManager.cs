using Relativity.API;
using System.Data.SqlClient;


namespace AgentAgent.Agent
{
    class RelativityAnalyticsClusterManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public RelativityAnalyticsClusterManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "2E668D59-137C-42BA-98EB-CE678EA90AB0";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(CACJ.[ClusterID]) 
                FROM   [ContentAnalystClusterJob] CACJ WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON CACJ.[CaseArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });

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
