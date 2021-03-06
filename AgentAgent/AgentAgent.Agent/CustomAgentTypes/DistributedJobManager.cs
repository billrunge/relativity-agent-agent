﻿using Relativity.API;

namespace AgentAgent.Agent
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

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            string SQL = @"
                SELECT COUNT(*)
                FROM [DistributedJob] WITH(NOLOCK)";

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);
            
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
