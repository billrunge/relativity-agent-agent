using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent.Objects
{
    class AgentsDesiredObject
    {
        public string Guid { get; set; }
        public int Count { get; set; }
        public bool RespectsResourcePool { get; set; }
    }
}
