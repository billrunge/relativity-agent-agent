using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class ProductionManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public ProductionManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "916CF88F-F8D0-4C65-9ECC-1BBFDF5E1515";
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
        }

        //Production managers are an at least one agent per resource pool, but only one manager per job
        //so the ideal situation here is one manager per job in the queue. 

        public override AgentsDesiredObject AgentsDesired()
        {
            string SQL = @"
                SELECT Count(P.[SetQueueId]) 
                FROM   [ProductionSetQueue] P WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON P.[WorkspaceArtifactId] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = jobCount
            };

            return agentsDesired;
        }
    }

}

