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

        public override List<AgentsPerPoolObject> DesiredAgentsPerPool()
        {
            List<AgentsPerPoolObject> outputList = new List<AgentsPerPoolObject>();
            AgentsPerPoolObject agentsPerPoolObject = new AgentsPerPoolObject
            {
                AgentCount = 1,
                AgentTypeGuid = Guid,
                ResourcePoolArtifactId = 0
            };
            outputList.Add(agentsPerPoolObject);
            return outputList;
        }
    }
}
