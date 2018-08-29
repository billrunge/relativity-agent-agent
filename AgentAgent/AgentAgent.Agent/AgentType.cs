

namespace AgentAgent.Agent
{
    interface IAgentType
    {
        int DesiredAgentCount();
    }

    abstract class AgentType : IAgentType
    {
        public string AgentTypeName { get; protected set; }
        public string Guid { get; protected set; }
        public bool AlwaysNeeded { get; protected set; }
        public int MaxPerInstance { get; protected set; }
        public int MaxPerResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public bool UsesQueue { get; protected set; }
        public string QueueName { get; protected set; }

        public abstract int DesiredAgentCount();
        
    }
}
