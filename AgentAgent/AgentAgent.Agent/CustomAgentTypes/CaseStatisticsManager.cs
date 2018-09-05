using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class CaseStatisticsManager : AgentType
    {
        public CaseStatisticsManager()
        {
            AgentTypeName = "Case Statistics Manager";
            Guid = "336F88BD-2D9B-4DB2-BB8A-6AA48C4F13D0";
            AlwaysNeeded = false;
            OffHoursAgent = true;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = false;
            EddsQueueName = null;
        }

        //The Case Statistics Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> outputList = new List<AgentsPerPoolObject>();
            AgentAgentAgent agent = new AgentAgentAgent();

            if (agent.IsOffHours())
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
