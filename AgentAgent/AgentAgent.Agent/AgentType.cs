using System.Collections.Generic;

namespace AgentAgent.Agent
{
    interface IAgentType
    {
        AgentsDesiredObject AgentsDesired();
    }

    abstract class AgentType : IAgentType
    {
        public string Guid { get; protected set; }
        public int AgentAgentResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public int PagesPerAgent { get; set; }
        public abstract AgentsDesiredObject AgentsDesired();
    }
}
