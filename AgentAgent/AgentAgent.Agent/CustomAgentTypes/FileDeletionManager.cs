namespace AgentAgent.Agent
{
    class FileDeletionManager : AgentType
    {
        private readonly bool _isOffHours;

        public FileDeletionManager(bool isOffHours)
        {
            Guid = "6A17A769-1BF2-4480-B8A8-D4000CF48143";
            RespectsResourcePool = false;
            _isOffHours = isOffHours;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            if (_isOffHours)
            {
                agentCount = 1;
            }

            return new AgentsDesired()
            {
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool,
                Guid = Guid
            };
        }
    }
}
