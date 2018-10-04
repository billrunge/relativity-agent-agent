using AgentAgent.Agent.Objects;
using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class AssistedReviewManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public AssistedReviewManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Assisted Review Manager";
            Guid = "6D19C56E-2C1E-4BF6-A7A8-EDBD9DEF0588";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = true;
            EddsQueueName = "AssistedReviewMasterQueue";
        }

        public override List<AgentsDesiredObject> AgentsDesiredObject()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            string SQL = @"
                SELECT COUNT(*)
                FROM [AssistedReviewMasterQueue]";
            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (jobCount > 0)
            {
                agentCount = 1;
            }
            AgentsDesiredObject AgentsDesiredObject = new AgentsDesiredObject
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool

            };
            outputList.Add(AgentsDesiredObject);
            return outputList;
        }



    }
}
