using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProductionManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public ProductionManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Production Manager";
            Guid = "916CF88F-F8D0-4C65-9ECC-1BBFDF5E1515";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProductionSetQueue";
        }

        //Production managers are an at least one agent per resource pool, but only one manager per job
        //so the ideal situation here is one manager per job in the queue. 

        public override List<AgentsDesired> DesiredAgentsPerPool()
        {
            List<AgentsDesired> poolsWithJobsList = new List<AgentsDesired>();
            //Select distinct Resource Pool Artifact IDs that have a job in the queue

            //Todo: remove ResourceGroup table from join (there's a column on case for that)

            string SQL = @"
                SELECT COUNT(RG.[ArtifactID]) AS [JobCount], 
                       RG.[ArtifactID] 
                FROM   [ResourceGroup] RG 
                       INNER JOIN [Case] C 
                               ON C.[ResourceGroupArtifactID] = RG.[ArtifactID] 
                       INNER JOIN [ProductionSetQueue] PSQ 
                               ON PSQ.[WorkspaceArtifactId] = C.[ArtifactID] 
                GROUP  BY RG.[ArtifactID]";

            DataTable poolsWithJobsTable = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL);

            //If nothing is returned, there are no jobs in the queue
            if (poolsWithJobsTable.Rows.Count == 0)
            {
                return null;
            }
            //Otherwise let's iterate through the results and add to the AgentPerPoolList
            else
            {

                foreach (DataRow row in poolsWithJobsTable.Rows)
                {

                    if (!int.TryParse(row["ArtifactID"].ToString(), out int resourcePoolArtifactId))
                    {
                        throw new Exception("Unable to cast Resource Pool artifactID returned from database to int");
                    }

                    if (!int.TryParse(row["JobCount"].ToString(), out int resourcePoolJobCount))
                    {
                        throw new Exception("Unable to cast Resource Pool job count returned from database to int");
                    }

                    AgentsDesired agentsPerPoolObject = new AgentsDesired
                    {
                        AgentCount = resourcePoolJobCount,
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
