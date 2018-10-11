using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class RelativityAnalyticsIndexProgressManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;
        private IEnvironmentHelper _environment;

        public RelativityAnalyticsIndexProgressManager(IDBContext eddsDbContext, IEnvironmentHelper environment, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            Guid = "BED2945C-7710-4BBC-A1CF-A768F2EA997C";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;
            int serverCount = 0;

            string SQL = @"
                SELECT Count(AIP.[IndexID]) 
                FROM   [AnalyticsIndexProgress] AIP WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON AIP.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
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
