using System.Collections.Generic;


namespace AgentAgent.Agent
{
    class AgentServerList
    {
        public List<AgentServer> OutputList {get; }

        public AgentServerList()
        {
            OutputList = new List<AgentServer>();
        }

        public void Add(AgentServer server)
        {        
            OutputList.Add(server);
        }
                     
    }
}
