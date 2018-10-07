namespace AgentAgent.Agent
{
    public class AgentServer
    {
        public int ArtifactID { get; set; }
        public string Hostname { get; set; }
        public bool Active { get; set; }
        public int ProcessorCores { get; set; }
        public long Memory { get; set; }
        public int AgentCount { get; set; }
    }
}
