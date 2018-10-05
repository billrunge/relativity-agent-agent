using Relativity.API;
using AgentAgent.Agent.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AgentAgent.Agent
{
    class RunAgentCreate
    {
        private List<AgentsDesiredObject> _agentsDesired;
        private List<SpotsPerServerObject> _spotsPerServer;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentInformation _environmentInformation;

        public RunAgentCreate(IDBContext eddsDbContext, IEnvironmentInformation environmentInformation, List<AgentsDesiredObject> agentsDesired, List<SpotsPerServerObject> spotsPerServer)
        {
            _eddsDbContext = eddsDbContext;
            _environmentInformation = environmentInformation;
            _agentsDesired = agentsDesired;
            _spotsPerServer = spotsPerServer;
        }

        public void Run()
        {
            bool outOfServers = false;
            while (_agentsDesired.Count > 0 && outOfServers == false)
            {
                foreach (var agent in _agentsDesired)
                {
                    if (agent.Count > 0)
                    {
                        if (_spotsPerServer.Count > 0)
                        {
                            //Order the agent servers by spots descending, and then select first object
                            _spotsPerServer = _spotsPerServer.OrderByDescending(x => x.Spots).ToList();
                            if (_spotsPerServer[0].Spots > 0)
                            {
                                CreateAgent createAgent = new CreateAgent(_eddsDbContext, _environmentInformation, agent.Guid, _spotsPerServer[0].AgentServerArtifactId);
                                createAgent.Create();
                                _spotsPerServer[0].Spots -= 1;
                                agent.Count -= 1;

                                if (agent.Count < 1)
                                {
                                    _agentsDesired.Remove(agent);
                                }

                                if (_spotsPerServer[0].Spots < 1)
                                {
                                    _spotsPerServer.Remove(_spotsPerServer[0]);
                                }
                            }
                            else
                            {
                                _spotsPerServer.Remove(_spotsPerServer[0]);
                            }
                        }
                        else
                        {
                            //Out of servers! 
                            //Log message about being out of servers
                            outOfServers = true;
                            break;
                        }
                    }
                    else
                    {
                        _agentsDesired.Remove(agent);
                    }
                }
            }
        }
    }
}
