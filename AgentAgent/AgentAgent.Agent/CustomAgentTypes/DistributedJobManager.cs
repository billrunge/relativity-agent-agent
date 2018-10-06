using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class DistributedJobManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public DistributedJobManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "E7EBE10A-CC1A-4D3F-A3D6-F9B6B2069B09";
            RespectsResourcePool = false;
        }

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            string SQL = @"
                SELECT COUNT(*)
                FROM [DistributedJob]";

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);
            
            if (jobCount > 0)
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesiredObject = new AgentsDesiredObject
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
