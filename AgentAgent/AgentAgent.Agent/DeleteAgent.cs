using Relativity.API;
using System.Data.SqlClient;
using AgentAgent.Agent.Interfaces;

namespace AgentAgent.Agent
{
    class DeleteAgent : IDeleteAgent
    {
        private readonly IDBContext _eddsDbContext;

        public DeleteAgent(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
        }

        public void Delete(int agentArtifactId)
        {
            string SQL = @"
                UPDATE [Artifact]
                SET [DeleteFlag] = 1
                WHERE ArtifactID = @ArtifactID";

            SqlParameter agentArtifactIdParam = new SqlParameter("@ArtifactID", System.Data.SqlDbType.Int)
            {
                Value = agentArtifactId
            };

            //Auditing here would create duplicate delete records, 
            //as Relativity will audit the delete if initiated by updating Artifact
            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentArtifactIdParam });
        }

        public int GetAgentIdToDelete(int agentTypeId, int serverArtifactId)
        {
            /*Ordering by LastUpdate descending under the assumption 
             * that agents that are busy will check in less frequently */

            string SQL = @"
                SELECT TOP 1 AG.[ArtifactID] 
                FROM   [Agent] AG WITH(NOLOCK)
                       INNER JOIN [Artifact] A WITH(NOLOCK)
                               ON AG.[ArtifactID] = A.[ArtifactID] 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                       AND AG.[ServerArtifactID] = @ServerArtifactID 
                       AND A.[DeleteFlag] = 0 
                ORDER  BY AG.[LastUpdate] DESC";

            SqlParameter agentTypeIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeId
            };

            SqlParameter serverArtifactIdParam = new SqlParameter("@ServerArtifactID", System.Data.SqlDbType.Char)
            {
                Value = serverArtifactId
            };

            return _eddsDbContext.ExecuteSqlStatementAsScalar<int?>(SQL, new SqlParameter[] { agentTypeIdParam, serverArtifactIdParam }).GetValueOrDefault();

        }

    }
}
