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
        private readonly IAPILog _logger;

        public RunAgentCreate(IDBContext eddsDbContext, IEnvironmentInformation environmentInformation, List<AgentsDesiredObject> agentsDesired, List<SpotsPerServerObject> spotsPerServer, IAPILog logger)
        {
            _eddsDbContext = eddsDbContext;
            _environmentInformation = environmentInformation;
            _agentsDesired = agentsDesired;
            _spotsPerServer = spotsPerServer;
            _logger = logger;
        }

        public void Run()
        {
            int counter;
            bool outOfServers = false;

            //while there are rows in the agent desired list and we have not run out of servers
            while (_agentsDesired.Count > 0 && outOfServers == false)
            {
                _logger.LogVerbose(string.Format("Agents Desired Count = {0}", _agentsDesired.Count));
                //Create index counter for _agentsDesired
                counter = _agentsDesired.Count() - 1;

                //before counter gets to negative index range
                while (counter >= 0)
                {
                    //Are there any agents to be created for this object?
                    if (_agentsDesired[counter].Count > 0)
                    {
                        //Are there any servers available?
                        if (_spotsPerServer.Count > 0)
                        {
                            //Order the agent servers by spots descending, and then select first object
                            _spotsPerServer = _spotsPerServer.OrderByDescending(x => x.Spots).ToList();
                            if (_spotsPerServer[0].Spots > 0)
                            {
                                CreateAgent createAgent = new CreateAgent(_eddsDbContext, _environmentInformation, _agentsDesired[counter].Guid, _spotsPerServer[0].AgentServerArtifactId);
                                createAgent.Create();
                                _spotsPerServer[0].Spots -= 1;
                                _agentsDesired[counter].Count -= 1;

                                if (_agentsDesired[counter].Count < 1)
                                {
                                    _agentsDesired.Remove(_agentsDesired[counter]);
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
                        _agentsDesired.Remove(_agentsDesired[0]);
                    }

                    counter -= 1;
                }
            }
        }
    }
}
