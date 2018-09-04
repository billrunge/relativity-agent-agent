

namespace AgentAgent.Agent
{
    interface IAgentType
    {
       AgentsPerPoolList DesiredAgentsPerPool();
    }

    abstract class AgentType : IAgentType
    {
        public string AgentTypeName { get; protected set; }
        public string Guid { get; protected set; }
        public bool AlwaysNeeded { get; protected set; }
        public int MaxPerInstance { get; protected set; }
        public int MaxPerResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public bool UsesEddsQueue { get; protected set; }
        public string EddsQueueName { get; protected set; }

        public abstract AgentsPerPoolList DesiredAgentsPerPool();
        
    }
}
