﻿using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class SearchTermsReportManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public SearchTermsReportManager(IDBContext eddsDbContext, int poolArtifacId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "CD4545F1-4370-4FF2-B2CD-4709B159112D";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolArtifacId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;
            int jobCount = 0;

            string SQL = @"
                SELECT Count(STRQ.[CaseArtifactID]) 
                FROM   [SearchTermsReportQueue] STRQ 
                       INNER JOIN [Case] C 
                               ON STRQ.[CaseArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            if (jobCount > 0)
            {
                agentCount = 1;
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
