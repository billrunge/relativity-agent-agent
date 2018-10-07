namespace AgentAgent.Agent
{
    class CaseManager : AgentType
    {
        private readonly bool _isOffHours;

        public CaseManager(bool isOffHours)
        {
            Guid = "894462EF-395F-4527-A51B-8D328D229030";
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
