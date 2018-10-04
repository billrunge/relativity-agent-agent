﻿using AgentAgent.Agent.Objects;
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
            AgentTypeName = "Cluster Upgrade Worker";
            Guid = "50BE7641-0604-4BD2-9381-F0540301A87E";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = true;
            EddsQueueName = "ClusterUpgradeJobs";
        }

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            string SQL = @"
                SELECT COUNT(*)
                FROM [ClusterUpgradeJobs]";
            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);
            int agentCount = 0;

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
