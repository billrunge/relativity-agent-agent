using AgentAgent.Agent.Interfaces;

namespace AgentAgent.Agent
{
    abstract class AgentType : IAgentType
    {
        public string Guid { get; protected set; }
        public int AgentAgentResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public int PagesPerAgent { get; set; }
        public abstract AgentsDesired GetAgentsDesired();
    }
}
