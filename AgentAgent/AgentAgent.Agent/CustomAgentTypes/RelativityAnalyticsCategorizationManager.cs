using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class RelativityAnalyticsCategorizationManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;
        public RelativityAnalyticsCategorizationManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "D236A596-A732-4885-B05F-38F60E3C2691";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            /* Categorization Set Managers are max two per resource pool */

            string SQL = @"
                SELECT IIF(Count(ACSQ.[SetQueueId]) > 2, 2, Count(ACSQ.[SetQueueId])) 
                FROM   [AnalyticsCategorizationSetQueue] ACSQ WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON ACSQ.[WorkspaceArtifactId] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });

            if (jobCount > 0)
            {
                agentCount = jobCount;
            }

            return new AgentsDesired
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount
            };
        }
    }
}
