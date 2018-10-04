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

        public override List<AgentsDesiredObject> DesiredAgentsPerPool()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            AgentsDesiredObject AgentsDesiredObject = new AgentsDesiredObject
            {
                AgentCount = 1,
                AgentTypeGuid = Guid,
                ResourcePoolArtifactId = 0
            };
            outputList.Add(AgentsDesiredObject);
            return outputList;
        }
    }
}
