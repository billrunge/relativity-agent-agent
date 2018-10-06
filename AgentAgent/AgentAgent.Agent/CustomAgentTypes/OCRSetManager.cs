using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class OCRSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public OCRSetManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "OCR Set Manager";
            Guid = "42BC48A4-4638-4653-8C2F-C1444D272F84";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "OCRSetQueue";
        }

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                SELECT Count(O.[WorkspaceArtifactID]) 
                FROM   [OCRSetQueue] O 
                       INNER JOIN [Case] C 
                               ON O.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            if (jobCount > 0)
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesiredObject = new AgentsDesiredObject()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount
            };

            outputList.Add(agentsDesiredObject);

            return outputList;
        }
    }

}