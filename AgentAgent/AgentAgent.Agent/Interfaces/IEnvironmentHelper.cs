using System.Collections.Generic;

namespace AgentAgent.Agent.Interfaces
{
    public interface IEnvironmentHelper
    {
        List<SpotsPerServer> GetAgentsPerServerByPool(int agentTypeArtifactId, int poolId);
        int GetAgentArtifactType();
        int GetSystemContainerId();
        int GetArtifactIdFromGuid(string Guid);
        string GetTextIdByArtifactId(int artifactId);
        int GetAgentCount(int agentTypeArtifactId);
        int GetAgentCountByPool(int agentTypeArtifactId, int poolId);
        int GetAgentRunIntervalByType(int agentTypeArtifactId);
        int GetAnalyticsServerCountByResourcePool(int poolId);
        AgentServer GetAgentServerObject(int agentServerArtifactId);
        List<AgentServer> GetPoolAgentServerList(int poolId);
        List<AgentServer> GetPoolAgentServerListNoDtSearch(int poolId);
    }
}
