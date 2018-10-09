using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class TextExtractionManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public TextExtractionManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "9DFA232F-8753-4E3C-B695-AB09934CC1E0";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(*) 
                FROM   [TextExtractionQueue] T WITH(NOLOCK)
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON T.[ApplicationID] = C.[ArtifactID] 
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
