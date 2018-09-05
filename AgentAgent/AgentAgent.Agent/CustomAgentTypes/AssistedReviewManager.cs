﻿using Relativity.API;
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

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> outputList = new List<AgentsPerPoolObject>();
            string SQL = @"
                SELECT COUNT(*)
                FROM [AssistedReviewMasterQueue]";
            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (jobCount > 0)
            {
                AgentsPerPoolObject agentsPerPoolObject = new AgentsPerPoolObject
                {
                    AgentCount = 1,
                    AgentTypeGuid = Guid,
                    ResourcePoolArtifactId = 0
                };
                outputList.Add(agentsPerPoolObject);
                return outputList;
            }
            else
            {
                return null;
            }
        }



    }
}
