using System.Collections.Generic;


namespace AgentAgent.Agent
{
    class AgentsPerPoolList
    {
        public List<AgentsPerPoolObject> OutputList { get; }

        public AgentsPerPoolList()
        {
            OutputList = new List<AgentsPerPoolObject>();
        }

        public void Add(AgentsPerPoolObject agentsPerPoolObject)
        {
            OutputList.Add(agentsPerPoolObject);
        }
    }
}
