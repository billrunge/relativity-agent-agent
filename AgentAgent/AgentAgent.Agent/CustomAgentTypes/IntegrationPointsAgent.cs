using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
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

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List <AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                IF ( EXISTS (SELECT * 
                             FROM   INFORMATION_SCHEMA.TABLES 
                             WHERE  TABLE_SCHEMA = 'eddsdbo' 
                                    AND TABLE_NAME = 
                            'ScheduleAgentQueue_08C0CE2D-8191-4E8F-B037-899CEAEE493D') ) 
                  BEGIN 
                      SELECT IIF(Count(SAQ.[JobID]) > 3, 4, Count(SAQ.[JobID])) 
                      FROM   [ScheduleAgentQueue_08C0CE2D-8191-4E8F-B037-899CEAEE493D] SAQ 
                             INNER JOIN [Case] C 
                                     ON SAQ.[WorkspaceID] = C.[ArtifactID] 
                      WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                  END 
                ELSE 
                  BEGIN 
                      SELECT 0 
                  END ";

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

            outputList.Add(agentsDesired);
            return outputList;
        }

    }
}
