using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProcessingSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public ProcessingSetManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Processing Set Manager";
            Guid = "8326948B-32E1-4911-AC08-DA9130D38AF1";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProcessingSetQueue";
        }


        //The Processing set manager is a one agent per resource pool agent
        //Which makes determining the amount of agents per pool desired easy

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                SELECT Count(P.[SetQueueID]) 
                FROM   [ProcessingSetQueue] P 
                       INNER JOIN [Case] C 
                               ON P.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            //If nothing is returned, there are no jobs in the queue
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