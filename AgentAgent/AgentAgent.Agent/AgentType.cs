﻿using System.Collections.Generic;

namespace AgentAgent.Agent
{
    interface IAgentType
    {
        List<AgentsDesiredObject> AgentsDesired();
    }

    abstract class AgentType : IAgentType
    {
        public string Guid { get; protected set; }
        public int AgentAgentResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public abstract List<AgentsDesiredObject> AgentsDesired();        
    }
}
