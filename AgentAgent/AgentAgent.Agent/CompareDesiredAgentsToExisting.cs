using AgentAgent.Agent.Objects;
using Relativity.API;
using System.Collections.Generic;

namespace AgentAgent.Agent
{
    class CompareDesiredAgentsToExisting
    {
        private List<AgentsDesiredObject> _agentsDesiredObjects;
        private readonly int _poolArtifactId;
        private IEnvironmentInformation _environmentInformation;
        private IAPILog _logger;

        public CompareDesiredAgentsToExisting(List<AgentsDesiredObject> agentsDesiredObjects, int poolArtifactId, IEnvironmentInformation environmentInformation, IAPILog logger)
        {
            _agentsDesiredObjects = agentsDesiredObjects;
            _poolArtifactId = poolArtifactId;
            _environmentInformation = environmentInformation;
            _logger = logger;
        }

        public List<AgentsDesiredObject> Compare()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();
            string logString = "";

            int counter = _agentsDesiredObjects.Count - 1;
            while (counter >= 0)
            {
                string agentGuid = _agentsDesiredObjects[counter].Guid;
                int agentTypeId = _environmentInformation.GetArtifactIdFromGuid(agentGuid);
                int currentCount = _environmentInformation.GetAgentCountByPool(agentTypeId, _poolArtifactId);
                int desiredCount = _agentsDesiredObjects[counter].Count;
                int difference = desiredCount - currentCount;

                logString += string.Format("AgentGuid: {0}, AgentTypeID {1}, Current Count: {2}, Desired Count: {3} Difference: {4} | ", agentGuid, agentTypeId, currentCount, desiredCount, difference);

                AgentsDesiredObject agent = new AgentsDesiredObject
                {
                    Guid = _agentsDesiredObjects[counter].Guid,
                    RespectsResourcePool = _agentsDesiredObjects[counter].RespectsResourcePool,
                    Count = difference
                };

                outputList.Add(agent);
                counter -= 1;
            }
            _logger.LogVerbose("Compare output: " + logString);
            return outputList;
        }
    }
}
