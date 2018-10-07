﻿namespace AgentAgent.Agent
{
    class ServerManager : AgentType
    {
        public ServerManager()
        {
            Guid = "FBE5DF61-E8CF-4D9C-AD03-03C6100CED48";
            RespectsResourcePool = false;
        }

        //You always need a server manager.
        public override AgentsDesiredObject AgentsDesired()
        {
            AgentsDesiredObject agentsDesired = new AgentsDesiredObject
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = 1
            };

            return agentsDesired;
        }
    }
}
