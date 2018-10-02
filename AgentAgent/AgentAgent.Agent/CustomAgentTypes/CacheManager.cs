using AgentAgent.Agent.Objects;
using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class CacheManager : AgentType
    {

        public CacheManager(int resourcePoolArtifactId)
        {
            AgentTypeName = "Cache Manager";
            Guid = "505B1655-2B80-45F5-8DE8-8F26442A6E07";
            AlwaysNeeded = false;
            OffHoursAgent = true;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            AgentAgentResourcePool = resourcePoolArtifactId;
            RespectsResourcePool = false;
            UsesEddsQueue = false;
            EddsQueueName = null;
        }

        //The Cache Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            AgentAgentAgent agent = new AgentAgentAgent();
            int agentCount = 0;

            if (agent.IsOffHours())
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject()
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool
            };

            outputList.Add(agentsDesired);
            return outputList;
        }
    }
}
