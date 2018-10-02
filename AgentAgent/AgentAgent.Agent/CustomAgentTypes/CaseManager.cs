using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class CaseManager : AgentType
    {

        public CaseManager()
        {
            AgentTypeName = "Case Manager";
            Guid = "894462EF-395F-4527-A51B-8D328D229030";
            AlwaysNeeded = false;
            OffHoursAgent = true;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = false;
            EddsQueueName = null;
        }

        //The Case Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override List<AgentsDesired> DesiredAgentsPerPool()
        {
            List<AgentsDesired> outputList = new List<AgentsDesired>();

            AgentAgentAgent agent = new AgentAgentAgent();

            if (agent.IsOffHours())
            {
                AgentsDesired agentsPerPoolObject = new AgentsDesired
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
