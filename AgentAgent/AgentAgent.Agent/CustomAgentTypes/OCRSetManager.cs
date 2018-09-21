using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class OCRSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public OCRSetManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "OCR Set Manager";
            Guid = "42BC48A4-4638-4653-8C2F-C1444D272F84";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "OCRSetQueue";

        }

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> poolsWithJobsList = new List<AgentsPerPoolObject>();
            //Select distinct Resource Pool Artifact IDs that have a job in the queue
            string SQL = @"
                SELECT DISTINCT( C.[ResourceGroupArtifactID] ) 
                FROM   [OCRSetQueue] OCR 
                       INNER JOIN [Case] C 
                               ON OCR.[WorkspaceArtifactID] = C.[ArtifactID] ";

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

                    if (!int.TryParse(row["ResourceGroupArtifactID"].ToString(), out int resourcePoolArtifactId))
                    {
                        throw new Exception("Unable to cast ResourceGroupArtifactID returned from database to int");
                    }

                    AgentsPerPoolObject agentsPerPoolObject = new AgentsPerPoolObject
                    {
                        AgentCount = 1,
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
