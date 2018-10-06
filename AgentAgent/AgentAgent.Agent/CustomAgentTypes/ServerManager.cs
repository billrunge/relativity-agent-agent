using System.Collections.Generic;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ServerManager : AgentType
    {
        public ServerManager()
        {
            AgentTypeName = "Server Manager";
            Guid = "FBE5DF61-E8CF-4D9C-AD03-03C6100CED48";
            AlwaysNeeded = true;
            OffHoursAgent = false;
            MaxPerInstance = 1;
            MaxPerResourcePool = 0;
            RespectsResourcePool = false;
            UsesEddsQueue = false;
            EddsQueueName = null;
        }

        //You always need a server manager.
        public override List<AgentsDesiredObject> AgentsDesired()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            AgentsDesiredObject AgentsDesiredObject = new AgentsDesiredObject
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = 1
            };
            outputList.Add(AgentsDesiredObject);
            return outputList;
        }
    }
}
