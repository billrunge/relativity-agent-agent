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

        public void DeleteAgentByArtifactId(int agentArtifactId)
        {            
            string SQL = @"
                UPDATE [eddsdbo].[Artifact]
                SET [DeleteFlag] = 1
                WHERE ArtifactID = @ArtifactID";

            //Gather values to input into above script
            SqlParameter agentArtifactIdParam = new SqlParameter("@ArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentArtifactId
            };
            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentArtifactIdParam });
        }

        public void DeleteAgentsByAgentType (int agentTypeId)
        {
            string SQL = @"
                UPDATE A 
                SET    A.[DeleteFlag] = 1 
                FROM   [eddsdbo].[Agent] AG 
                       INNER JOIN [eddsdbo].[Artifact] A 
                               ON A.[ArtifactID] = AG.ArtifactID 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID";

            //Gather values to input into above script
            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeId
            };
            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentTypeArtifactIdParam });
        }

        public void DeleteAgentsByTypeAndResourcePool(int agentTypeId, int resourcePoolId)
        {
            string SQL = @"
                UPDATE A 
                SET    [DeleteFlag] = 1 
                FROM   [eddsdbo].[Agent] AG 
                       INNER JOIN [eddsdbo].[ExtendedResourceGroupServers] ERGS 
                               ON AG.[ServerArtifactID] = ERGS.[ServerArtifactID] 
                       INNER JOIN [eddsdbo].[Artifact] A 
                               ON AG.[ArtifactID] = A.[ArtifactID] 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                       AND ERGS.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            //Gather values to input into above script
            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeId
            };

            SqlParameter resourcePoolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = resourcePoolId
            };

            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentTypeArtifactIdParam, resourcePoolIdParam });

        }

        public void ForceDeleteAgent (int agentArtifactId)
        {

            string SQL = @"
                DELETE FROM [eddsdbo].[Agent] 
                WHERE  [ArtifactID] = @AgentArtifactID 

                DELETE FROM [eddsdbo].[ArtifactAncestry] 
                WHERE  [ArtifactID] = @AgentArtifactID 

                DELETE FROM [eddsdbo].[Artifact] 
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
