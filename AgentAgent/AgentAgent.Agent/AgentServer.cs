namespace AgentAgent.Agent
{
    class AgentServer
    {
        public int ArtifactID { get; }
        public string Hostname { get;  }
        public bool Active { get;  }
        public int ProcessorCores { get; }
        public long Memory { get;  }
        public int AgentCount { get; }

        public AgentServer(int artifactId, string hostname, bool active, int processorCores, long memory, int agentCount)
        {
            ArtifactID = artifactId;
            Hostname = hostname;
            Active = active;
            ProcessorCores = processorCores;
            Memory = memory;
            AgentCount = agentCount;
        }
    }
}
