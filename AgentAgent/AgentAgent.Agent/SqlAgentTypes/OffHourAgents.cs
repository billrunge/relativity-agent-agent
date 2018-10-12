namespace AgentAgent.Agent.SqlAgentTypes
{
    class OffHourAgents
    {
        private readonly bool _isOffHours;

        public OffHourAgents(bool isOffHours)
        {
            _isOffHours = isOffHours;
        }

        public int GetAgentsDesired()
        {
            if (_isOffHours)
            {
                return 1;
            }

            return 0;
        }
    }
}
