using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class TranscriptManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public TranscriptManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "37CADC21-83AA-422F-B37F-BA498903687D";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(T.[ApplicationArtifactID]) 
                FROM   [TranscriptQueue] T 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON T.[ApplicationArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
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
