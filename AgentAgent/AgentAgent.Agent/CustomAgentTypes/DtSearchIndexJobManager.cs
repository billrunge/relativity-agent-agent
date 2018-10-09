using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class DtSearchIndexJobManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public DtSearchIndexJobManager(IDBContext eddsDbContext, int poolId)
        {
            Guid = "DE7F5902-413E-47FF-B8DB-4BAAAD702E65";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
            _eddsDbContext = eddsDbContext;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT Count(D.[setartifactid]) 
                FROM   [dtsearchindexqueue] D WITH(nolock) 
                       INNER JOIN [case] C WITH(nolock) 
                               ON D.[workspaceartifactid] = C.[artifactid] 
                WHERE  C.[resourcegroupartifactid] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });

            if (jobCount > 0)
            {
                agentCount = 1;
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
