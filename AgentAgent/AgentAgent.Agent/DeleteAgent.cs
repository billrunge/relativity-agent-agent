using AgentAgent.Agent.Objects;
using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class DeleteAgent
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

            //Gather values to input into above script
            SqlParameter agentArtifactIdParam = new SqlParameter("@ArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentArtifactId
            };
            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentArtifactIdParam });
        }

        public int GetAgentIdToDelete (int agentTypeId, int serverArtifactId)
        {
            string SQL = @"
                SELECT TOP 1 AG.[ArtifactID] 
                FROM   [Agent] AG 
                       INNER JOIN [Artifact] A 
                               ON AG.[ArtifactID] = A.[ArtifactID] 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                       AND AG.[ServerArtifactID] = @ServerArtifactID 
                       AND A.[DeleteFlag] = 0";

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



        //Todo: integrate logic to force delete agents
        public void ForceDeleteAgent (int agentArtifactId)
        {

            string SQL = @"
                DELETE FROM [Agent] 
                WHERE  [ArtifactID] = @AgentArtifactID 

                DELETE FROM [ArtifactAncestry] 
                WHERE  [ArtifactID] = @AgentArtifactID 

                DELETE FROM [Artifact] 
                WHERE  [ArtifactID] = @AgentArtifactID";

            //Gather values to input into above script
            SqlParameter agentArtifactIdParam = new SqlParameter("@AgentArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentArtifactId
            };

            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentArtifactIdParam });

        }

    }
}
