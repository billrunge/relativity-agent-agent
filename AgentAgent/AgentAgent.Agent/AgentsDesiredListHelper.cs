using System.Collections.Generic;
using System.Linq;

namespace AgentAgent.Agent
{
    class AgentsDesiredListHelper
    {
        private readonly List<AgentsDesired> _agentsDesiredList;

        public AgentsDesiredListHelper(List<AgentsDesired> agentsDesiredList)
        {
            _agentsDesiredList = agentsDesiredList;
        }

        public List<AgentsDesired> GetAgentCreateList()
        {
            return _agentsDesiredList.Select(x => new AgentsDesired()
            {
                Guid = x.Guid,
                RespectsResourcePool = x.RespectsResourcePool,
                Count = x.Count

            }).Where(x => x.Count > 0).ToList<AgentsDesired>();
        }

        public List<AgentsDesired> GetAgentDeleteList()
        {
            return _agentsDesiredList.Select(x => new AgentsDesired()
            {
                Guid = x.Guid,
                RespectsResourcePool = x.RespectsResourcePool,
                Count = x.Count

            }).Where(x => x.Count < 0).ToList<AgentsDesired>();
        }
    }
}
