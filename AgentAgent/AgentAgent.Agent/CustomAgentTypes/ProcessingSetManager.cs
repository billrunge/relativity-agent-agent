using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProcessingSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public ProcessingSetManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Processing Set Manager";
            Guid = "8326948B-32E1-4911-AC08-DA9130D38AF1";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProcessingSetQueue";

        }


        //The Processing set manager is a one agent per resource pool agent
        //Which makes determining the amount of agents per pool desired fairly easy

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> poolsWithJobsList = new List<AgentsPerPoolObject>();
            //Select distinct Resource Pool Artifact IDs that have a job in the queue

            //Todo: remove ResourceGroup table from join (there's a column on case for that)


            string SQL = @"
                SELECT DISTINCT(RG.[ArtifactID]) 
                FROM   [ResourceGroup] RG 
                       INNER JOIN [Case] C 
                               ON C.[ResourceGroupArtifactID] = RG.[ArtifactID] 
                       INNER JOIN [ProcessingSetQueue] PSQ 
                               ON PSQ.[WorkspaceArtifactId] = C.[ArtifactID]";

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

