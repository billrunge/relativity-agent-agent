using AgentAgent.Agent.Objects;
using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class BrandingManager : AgentType
    {
        private IDBContext _eddsDbContext;
        public int PagesPerAgent { get; set; }

        public BrandingManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Branding Manager";
            Guid = "29F92E0F-05C8-4DA2-ACCB-7C2ACAF2860A";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 0;
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProductionSetQueue";
            PagesPerAgent = 50000;

        }
        //The processing set queue has a column that shows how many images are remaining in a Processing set. This is very
        //Useful for determining the amount of branding managers needs. Just divide the image sumby the PagesPerAgent variable
        //to determine the amount of agents desired

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                SELECT IIF (SUM(PSQ.[ImagesRemaining]) IS NULL, 0, (SUM(PSQ.[ImagesRemaining]))) AS [ImagesRemaining]
                FROM   [ProductionSetQueue] PSQ 
                       INNER JOIN [Case] C 
                               ON PSQ.[WorkspaceArtifactID] = C.[ArtifactID] 
                       INNER JOIN [Code] CO 
                               ON PSQ.[Status] = CO.[ArtifactID] 
                       INNER JOIN [CodeType] CT 
                               ON CO.[CodeTypeID] = CT.[CodeTypeID] 
                WHERE  CO.[Name] = 'Branding' 
                       AND CT.[Name] = 'ProductionSetQueueStatus'
					   AND C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int poolImageCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });            
            
            if (poolImageCount > 0)
            {
                agentCount = poolImageCount / PagesPerAgent;

                if (agentCount < 1)
                {
                    agentCount = 1;
                }
            }

            AgentsDesiredObject agentsDesiredObject = new AgentsDesiredObject()
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool
            };
            outputList.Add(agentsDesiredObject);
            return outputList;
        }
    }
}
