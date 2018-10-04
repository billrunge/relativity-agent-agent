using AgentAgent.Agent.Objects;
using System.Collections.Generic;

namespace AgentAgent.Agent
{
    class CompareDesiredAgentsToExisting
    {
        private readonly List<AgentsDesiredObject> _agentsDesiredObjects;
        private readonly int _poolArtifactId;
        private IEnvironmentInformation _environmentInformation;

        public CompareDesiredAgentsToExisting(List<AgentsDesiredObject> agentsDesiredObjects, int poolArtifactId, IEnvironmentInformation environmentInformation)
        {
            _agentsDesiredObjects = agentsDesiredObjects;
            _poolArtifactId = poolArtifactId;
            _environmentInformation = environmentInformation;
        }

        public List<AgentsDesiredObject> Compare()
        {
            foreach (AgentsDesiredObject agentsDesired in _agentsDesiredObjects)
            {
                int agentTypeId = _environmentInformation.GetArtifactIdFromGuid(agentsDesired.Guid);
                int currentCount = _environmentInformation.GetAgentCountByPool(agentTypeId, _poolArtifactId);
                agentsDesired.Count -= currentCount;
            }
            return _agentsDesiredObjects;
        }
    }
}
