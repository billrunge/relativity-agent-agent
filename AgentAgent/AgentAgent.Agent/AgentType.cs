using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class AgentType
    {
        public string Guid { get; protected set; }
        public bool AlwaysNeeded { get; protected set; }
        public int MaxPerInstance { get; protected set; }
        public int MaxPerResourcePool { get; protected set; }
        public bool RespectsResourcePool { get; protected set; }
        public bool UsesQueue { get; protected set; }
        public string QueueName { get; protected set; }
        
    }
}
