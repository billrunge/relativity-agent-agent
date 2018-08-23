using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class AgentServerList
    {
        public List<AgentServer> OutputList {get; }

        public AgentServerList()
        {
            this.OutputList = new List<AgentServer>();
        }

        public void Add(AgentServer server)
        {        
            OutputList.Add(server);
        }
                     
    }
}
