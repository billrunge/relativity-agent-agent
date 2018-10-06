using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ClusterUpgradeWorker : AgentType
    {
        private IDBContext _eddsDbContext;

        public ClusterUpgradeWorker(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "50BE7641-0604-4BD2-9381-F0540301A87E";
            RespectsResourcePool = false;
        }

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            string SQL = @"
                SELECT COUNT(*)
                FROM [ClusterUpgradeJobs]";
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
