using AgentAgent.Agent.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgentAgent.Agent
{
    class AgentsDesiredListHelper
    {
        private readonly List<AgentsDesiredObject> _agentsDesiredObjects;

        public AgentsDesiredListHelper(List<AgentsDesiredObject> agentsDesiredObjects)
        {
            _agentsDesiredObjects = agentsDesiredObjects;
        }

        public List<AgentsDesiredObject> GetAgentCreateList()
        {
            return _agentsDesiredObjects.Select(x => new AgentsDesiredObject()
            {
                Guid = x.Guid,
                RespectsResourcePool = x.RespectsResourcePool,
                Count = x.Count

            }).Where(x => x.Count > 0).ToList<AgentsDesiredObject>();            

        }

        public List<AgentsDesiredObject> GetAgentDeleteList()
        {
            return _agentsDesiredObjects.Select(x => new AgentsDesiredObject()
            {
                Guid = x.Guid,
                RespectsResourcePool = x.RespectsResourcePool,
                Count = x.Count

            }).Where(x => x.Count < 0).ToList<AgentsDesiredObject>();
        }



    }
}
