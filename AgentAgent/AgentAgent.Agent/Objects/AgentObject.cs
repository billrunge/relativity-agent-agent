namespace AgentAgent.Agent
{
    public class AgentObject
    {
        public string AgentName { get; set; }
        public int AgentArtifactTypeId { get; set; }
        public int SystemContainerId { get; set; }
        public int ArtifactId { get; set; }
        public int AgentServerArtifactId { get; set; }
        public int AgentTypeArtifactId { get; set; }
        public int RunInterval { get; set; }
        public string AgentTypeGuid { get; set; }
    }
}
