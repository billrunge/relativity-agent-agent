using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class TransformSetManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        //Even though documentation does not say that TransformSetManagers run in a resource pool
        //It does specify to only have one. So if there is already one in any resource pool, let's not create any
        //but let's be selfish and only create one if there's a pending job in our pool

        public TransformSetManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "47B4787D-6DB8-4CF3-9274-31FBDBDF850C";
            RespectsResourcePool = false;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(D.[CaseArtifactID]) 
                FROM   [DataTransformQueue] D WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON D.[CaseArtifactID] = C.[ArtifactID] 
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
