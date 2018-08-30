using Relativity.API;
using System;
using System.Data;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProcessingSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        //The Processing set manager is a one agent per resource pool agent
        //Which makes determining the amount of agents desired fairly easy
        public ProcessingSetManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Processing Set Manager";
            Guid = "8326948B-32E1-4911-AC08-DA9130D38AF1";
        }

        public override int DesiredAgentCount()
        {
            string SQL = @"
                SELECT COUNT(*)
                FROM [ProcessingSetQueue]";

            int queueDepth = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (queueDepth > 0)
            {
                return 1;
            }
            else {
                return 0;
            }

        }

        private ResourcePoolList PoolsWithJobsInQueue()
        {
            ResourcePoolList poolList = new ResourcePoolList();
            string SQL = @"
                SELECT DISTINCT(RG.[ArtifactID]) 
                FROM   [ResourceGroup] RG 
                       INNER JOIN [Case] C 
                               ON C.[ResourceGroupArtifactID] = RG.[ArtifactID] 
                       INNER JOIN [ProcessingSetQueue] PSQ 
                               ON PSQ.[WorkspaceArtifactId] = C.[ArtifactID] ";

            DataTable poolTable = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL);

            if (poolTable.Rows.Count == 0)
            {
                return null;
            } else
            {
                foreach (DataRow row in poolTable.Rows)
                {

                    if (!int.TryParse(row["ArtifactID"].ToString(), out int resourcePoolArtifactId))
                    {
                        throw new Exception("Unable to cast Resource Pool artifactID returned from database to int");
                    }
                    ResourcePoolObject rp = new ResourcePoolObject(resourcePoolArtifactId);
                    poolList.Add(rp);
                }

                return poolList;
}


        }
    }
}
