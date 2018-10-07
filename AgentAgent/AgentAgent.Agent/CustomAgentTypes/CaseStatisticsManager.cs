namespace AgentAgent.Agent
{
    class CaseStatisticsManager : AgentType
    {
        private readonly bool _isOffHours;

        public CaseStatisticsManager(bool isOffHours)
        {
            Guid = "336F88BD-2D9B-4DB2-BB8A-6AA48C4F13D0";
            RespectsResourcePool = false;
            _isOffHours = isOffHours;
        }

        //The Case Statistics Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override AgentsDesired AgentsDesired()
        {
            int agentCount = 0;

            if (_isOffHours)
            {
                agentCount = 1;
            }

            return new AgentsDesired()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount

            };
        }
    }
}
