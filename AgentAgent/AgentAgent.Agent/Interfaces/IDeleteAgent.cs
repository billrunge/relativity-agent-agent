namespace AgentAgent.Agent.Interfaces
{
    interface IDeleteAgent
    {
        void Delete(int agentArtifactID);
        int GetAgentIdToDelete(int agentTypeId, int serverArtifactId);
    }
}
