using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class RelativityAnalyticsIndexManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;
        private IEnvironmentHelper _environment;

        public RelativityAnalyticsIndexManager(IDBContext eddsDbContext, IEnvironmentHelper environment, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            Guid = "EE159966-D74C-49D5-8FE8-B6AD1CC8009C";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;
            int serverCount = 0;

            string SQL = @"
                SELECT Count(CAI.[CaseArtifactID]) 
                FROM   [ContentAnalystIndexJob] CAI WITH(NOLOCK)
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON CAI.[CaseArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                       AND [Status] IN ( 0, 1 )";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });
            serverCount = _environment.GetAnalyticsServerCountByResourcePool(AgentAgentResourcePool);

            if (jobCount < serverCount)
            {
                agentCount = jobCount;
            }
            else
            {
                agentCount = serverCount;
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
