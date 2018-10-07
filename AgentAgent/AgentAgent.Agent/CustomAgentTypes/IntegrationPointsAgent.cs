using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class IntegrationPointsAgent : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public IntegrationPointsAgent(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "08C0CE2D-8191-4E8F-B037-899CEAEE493D";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolArtifactId;
        }

        public override AgentsDesiredObject AgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                IF ( EXISTS (SELECT * 
                             FROM   INFORMATION_SCHEMA.TABLES WITH(NOLOCK)
                             WHERE  TABLE_SCHEMA = 'eddsdbo' 
                                    AND TABLE_NAME = 
                            'ScheduleAgentQueue_08C0CE2D-8191-4E8F-B037-899CEAEE493D') ) 
                  BEGIN 
                      SELECT IIF(Count(SAQ.[JobID]) > 3, 4, Count(SAQ.[JobID])) 
                      FROM   [ScheduleAgentQueue_08C0CE2D-8191-4E8F-B037-899CEAEE493D] SAQ WITH(NOLOCK)
                             INNER JOIN [Case] C WITH(NOLOCK)
                                     ON SAQ.[WorkspaceID] = C.[ArtifactID] 
                      WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                  END 
                ELSE 
                  BEGIN 
                      SELECT 0 
                  END";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            agentCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount
            };

            return agentsDesired;
        }

    }
}
