using Relativity.API;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class DtSearchIndexWorker : AgentType
    {
        private IAgentHelper _agentHelper;
        private IDBContext _eddsDbContext;

        public DtSearchIndexWorker(IAgentHelper agentHelper, int poolId)
        {
            _agentHelper = agentHelper;
            _eddsDbContext = _agentHelper.GetDBContext(-1);
            Guid = "78880FC6-0A4A-43E5-87EF-9BEB5E55717D";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT D.[WorkspaceArtifactID], 
                       D.[SetArtifactID] 
                FROM   [dtSearchIndexQueue] D WITH(NOLOCK) 
                       INNER JOIN [Case] C WITH(NOLOCK)
                               ON D.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            DataTable queueResults = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { poolIdParam });

            if (queueResults.Rows.Count > 0)
            {
                foreach (DataRow row in queueResults.Rows)
                {
                    int workspaceId;
                    int setId;

                    if (!int.TryParse(row["WorkspaceArtifactID"].ToString(), out workspaceId))
                    {
                        throw new Exception("Unable to cast WorkspaceArtifactID returned from database to Int32");
                    }

                    if (!int.TryParse(row["SetArtifactID"].ToString(), out setId))
                    {
                        throw new Exception("Unable to cast SetArtifactId returned from database to Int32");
                    }

                    IDBContext workspaceDbContext = _agentHelper.GetDBContext(workspaceId);

                    SQL = @"
                        SELECT Count(ID) 
                        FROM   [dtSearchSubIndex] WITH(NOLOCK) 
                        WHERE  [dtSearchIndexID] = @DtSearchIndexID";

                    SqlParameter jobIdParam = new SqlParameter("@DtSearchIndexID", System.Data.SqlDbType.Char)
                    {
                        Value = setId
                    };

                    agentCount += workspaceDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { jobIdParam });
                }
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
