namespace AgentAgent.Agent
{
    class CacheManager : AgentType
    {
        private readonly bool _isOffHours;

        public CacheManager(bool isOffHours)
        {
            Guid = "505B1655-2B80-45F5-8DE8-8F26442A6E07";
            RespectsResourcePool = false;
            _isOffHours = isOffHours;
        }

        //The Cache Manager agent is a single, off hour agent 
        //Check if it is off hours, report one agent needed in any resource pool
        //If it is not off hours, no agent needed

        public override AgentsDesiredObject AgentsDesired()
        {
            int agentCount = 0;        

            if (_isOffHours)
            {
                agentCount = 1;
            }

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject()
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool
            };

            return agentsDesired;
        }
    }
}
