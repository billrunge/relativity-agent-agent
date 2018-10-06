﻿using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class CaseStatisticsManager : AgentType
    {
        public CaseStatisticsManager()
        {
            Guid = "336F88BD-2D9B-4DB2-BB8A-6AA48C4F13D0";
            RespectsResourcePool = false;
        }

        //The Case Statistics Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            AgentAgentAgent agent = new AgentAgentAgent();

            if (agent.IsOffHours())
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
