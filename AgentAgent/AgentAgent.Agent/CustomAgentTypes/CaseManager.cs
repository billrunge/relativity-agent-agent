using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class CaseManager : AgentType
    {

        public CaseManager()
        {
            Guid = "894462EF-395F-4527-A51B-8D328D229030";
            RespectsResourcePool = false;
        }

        //The Case Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            AgentAgentAgent agentHelper = new AgentAgentAgent();            

            if (agentHelper.IsOffHours())
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesiredObject = new AgentsDesiredObject
            {
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool,
                Guid = Guid
            };

            outputList.Add(agentsDesiredObject);
            return outputList;
        }
    }
}
