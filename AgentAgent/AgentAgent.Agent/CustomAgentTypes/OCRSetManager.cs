using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class OCRSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public OCRSetManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "42BC48A4-4638-4653-8C2F-C1444D272F84";
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
        }

        public override AgentsDesired AgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT Count(O.[WorkspaceArtifactID]) 
                FROM   [OCRSetQueue] O WITH(NOLOCK)
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON O.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

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