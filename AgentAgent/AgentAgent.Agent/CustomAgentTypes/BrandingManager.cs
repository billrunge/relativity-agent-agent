using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{ 

    class BrandingManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public BrandingManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "29F92E0F-05C8-4DA2-ACCB-7C2ACAF2860A";
            AgentAgentResourcePool = poolId;
            RespectsResourcePool = true;
            PagesPerAgent = 50000;
        }

        //The processing set queue has a column that shows how many images are remaining in a Processing set. This is very
        //Useful for determining the amount of branding managers needs. Just divide the image sumby the PagesPerAgent variable
        //to determine the amount of agents desired
        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int poolImageCount = 0;

            string SQL = @"
                SELECT IIF (SUM(PSQ.[ImagesRemaining]) IS NULL, 0, (SUM(PSQ.[ImagesRemaining]))) AS [ImagesRemaining]
                FROM   [ProductionSetQueue] PSQ WITH(NOLOCK)
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON PSQ.[WorkspaceArtifactID] = C.[ArtifactID]
                       INNER JOIN [Code] CO WITH(NOLOCK)
                               ON PSQ.[Status] = CO.[ArtifactID] 
                       INNER JOIN [CodeType] CT WITH(NOLOCK)
                               ON CO.[CodeTypeID] = CT.[CodeTypeID] 
                WHERE  CO.[Name] = 'Branding' 
                       AND CT.[Name] = 'ProductionSetQueueStatus'
					   AND C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            poolImageCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });            
            
            if (poolImageCount > 0)
            {
                agentCount = poolImageCount / PagesPerAgent;

                if (agentCount < 1)
                {
                    agentCount = 1;
                }
            }

            return new AgentsDesired()
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool
            };
        }
    }
}
