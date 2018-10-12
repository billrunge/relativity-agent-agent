using AgentAgent.Agent.Interfaces;
using Relativity.API;
using System.Data.SqlClient;
using Relativity.Services.Agent;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class DeleteAgentApi : IDeleteAgent
    {
        private readonly IDBContext _eddsDbContext;
        private readonly IAgentHelper _helper;
        private int _agentArtifactId;

        public DeleteAgentApi(IDBContext eddsDbContext, IAgentHelper helper)
        {
            _eddsDbContext = eddsDbContext;
            _helper = helper;

        }

        public void Delete(int agentArtifactId)
        {
            _agentArtifactId = agentArtifactId;
            ApiDelete().GetAwaiter();
        }

        private async Task ApiDelete()
        {
            using (IAgentManager agentManager = _helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.CurrentUser))
            {
                await agentManager.DeleteSingleAsync(_agentArtifactId);
            };
        }
        public int GetAgentIdToDelete(int agentTypeId, int serverArtifactId)
        {
            /* Ordering by LastUpdate descending under the assumption 
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
