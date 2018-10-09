using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{

    class AutoBatchManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        //Auto Batch Managers are scheduled jobs that can run on a set interval of minutes. To prevent scheduled
        //jobs from causing agents continuously be created and destroyed, I put in a time buffer. 
        //For now setting it to 5 minutes but will probably make it adjustable from the UI at some point
        private readonly int _timeBuffer;

        public AutoBatchManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            _timeBuffer = 5;
            Guid = "82A921DF-ED11-44E5-8A24-2400D1CD465F";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;

        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(A.[CaseArtifactID]) 
                FROM   [AutoBatchQueue] A WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON A.[CaseArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                       AND A.[StartTime] < Dateadd(mi, Cast(@AutoBatchBuffer AS INT), 
                                           Getutcdate())";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            SqlParameter bufferParam = new SqlParameter("@AutoBatchBuffer", System.Data.SqlDbType.Int)
            {
                Value = _timeBuffer
            };

            jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam, bufferParam });

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
