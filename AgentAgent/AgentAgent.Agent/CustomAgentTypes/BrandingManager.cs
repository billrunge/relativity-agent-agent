using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class BrandingManager : AgentType
    {
        private IDBContext _eddsDbContext;
        public int PagesPerAgent { get; set; }

        public BrandingManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Branding Manager";
            Guid = "29F92E0F-05C8-4DA2-ACCB-7C2ACAF2860A";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 0;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProductionSetQueue";
            PagesPerAgent = 50000;

        }
        //The processing set queue has a column that shows how many images are remaining in a Processing set. This is very
        //Useful for determining the amount of branding managers needs. Any jobs in a status of branding are going to have
        //their images remaining summed and grouped by resource pool. The image sum is then divided by the PagesPerAgent variable
        //to determine the amount of agents desired per resouce pool

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> poolsWithJobsList = new List<AgentsPerPoolObject>();

            string SQL = @"
                SELECT SUM(PSQ.[ImagesRemaining]) AS [ImagesRemaining], 
                       C.[ResourceGroupArtifactID] 
                FROM   [ProductionSetQueue] PSQ 
                       INNER JOIN [Case] C 
                               ON PSQ.[WorkspaceArtifactID] = C.[ArtifactID] 
                       INNER JOIN [Code] CO 
                               ON PSQ.[Status] = CO.[ArtifactID] 
                       INNER JOIN [CodeType] CT 
                               ON CO.[CodeTypeID] = CT.[CodeTypeID] 
                WHERE  CO.[Name] = 'Branding' 
                       AND CT.[Name] = 'ProductionSetQueueStatus' 
                GROUP  BY C.[ResourceGroupArtifactID]";

            DataTable poolImageCount = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL);

            //If nothing is returned, there are no jobs in the queue
            if (poolImageCount.Rows.Count == 0)
            {
                return null;
            }
            //Otherwise let's iterate through the results and add to the AgentPerPoolList
            else
            {

                foreach (DataRow row in poolImageCount.Rows)
                {
                    if (!int.TryParse(row["ResourceGroupArtifactID"].ToString(), out int resourcePoolArtifactId))
                    {
                        throw new Exception("Unable to cast ResourceGroupArtifactID returned from database to int");
                    }

                    if (!int.TryParse(row["ImagesRemaining"].ToString(), out int imagesRemaining))
                    {
                        throw new Exception("Unable to cast ImagesRemaining returned from database to int");
                    }

                    AgentsPerPoolObject agentsPerPoolObject = new AgentsPerPoolObject
                    {
                        AgentCount = imagesRemaining / PagesPerAgent,
                        AgentTypeGuid = Guid,
                        ResourcePoolArtifactId = resourcePoolArtifactId
                    };
                    poolsWithJobsList.Add(agentsPerPoolObject);
                }

                return poolsWithJobsList;
            }


        }

    }
}
