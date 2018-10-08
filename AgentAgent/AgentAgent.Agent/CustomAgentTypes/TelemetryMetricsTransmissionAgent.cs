namespace AgentAgent.Agent
{
    class TelemetryMetricsTransmissionAgent : AgentType
    {
        public TelemetryMetricsTransmissionAgent()
        {
            Guid = "120546D6-6C68-4879-AF26-09BE6F9862B7";
            RespectsResourcePool = false;
        }

        //You always need a telemetry metrics transmisison agent manager.
        public override AgentsDesired GetAgentsDesired()
        {
            return new AgentsDesired()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = 1
            };

        }
    }
}
