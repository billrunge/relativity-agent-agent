using System.Collections.Generic;


namespace AgentAgent.Agent
{
    class AgentServerList
    {
        public List<AgentServerObject> OutputList {get; }

        public AgentServerList()
        {
            OutputList = new List<AgentServerObject>();
        }

        public void Add(AgentServerObject server)
        {        
            OutputList.Add(server);
        }
                     
    }
}
