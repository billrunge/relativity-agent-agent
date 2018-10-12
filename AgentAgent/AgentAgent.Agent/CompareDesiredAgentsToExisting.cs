using Relativity.API;
using System.Collections.Generic;
using AgentAgent.Agent.Interfaces;

namespace AgentAgent.Agent
{
    class CompareDesiredAgentsToExisting
    {
        private List<AgentsDesired> _agentsDesiredList;
        private readonly int _poolId;
        private IEnvironmentHelper _environment;
        private IAPILog _logger;

        public CompareDesiredAgentsToExisting(List<AgentsDesired> agentsDesiredList, int poolId, IEnvironmentHelper environment, IAPILog logger)
        {
            _agentsDesiredList = agentsDesiredList;
            _poolId = poolId;
            _environment = environment;
            _logger = logger;
        }

        public List<AgentsDesired> Compare()
        {
            List<AgentsDesired> outputList = new List<AgentsDesired>();
            string logString = "";

            int counter = _agentsDesiredList.Count - 1;
            while (counter >= 0)
            {
                //Declaring all of these to variables to make logging easier
                int currentCount;
                string agentGuid = _agentsDesiredList[counter].Guid;
                int agentTypeId = _environment.GetArtifactIdFromGuid(agentGuid);
                int desiredCount = _agentsDesiredList[counter].Count;

                //Account for agents Resource Pool policy
                if (_agentsDesiredList[counter].RespectsResourcePool)
                {
                    currentCount = _environment.GetAgentCountByPool(agentTypeId, _poolId);
                }
                else
                {
                    currentCount = _environment.GetAgentCount(agentTypeId);
                }

                int difference = desiredCount - currentCount;

                logString += string.Format("AgentGuid: {0}, AgentTypeID {1}, Current Count: {2}, Desired Count: {3} Difference: {4}\r\n", agentGuid, agentTypeId, currentCount, desiredCount, difference);

                outputList.Add(new AgentsDesired
                {
                    Guid = _agentsDesiredList[counter].Guid,
                    RespectsResourcePool = _agentsDesiredList[counter].RespectsResourcePool,
                    Count = difference
                });

                counter -= 1;
            }
            _logger.LogDebug("Compare output--\r\n" + logString);
            return outputList;
        }
    }
}
