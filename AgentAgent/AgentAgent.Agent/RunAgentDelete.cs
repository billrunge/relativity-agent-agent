﻿using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgentAgent.Agent
{
    class RunAgentDelete
    {
        private readonly int _resourcePoolId;
        private List<AgentsDesired> _agentsToDelete;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private readonly IAPILog _logger;
        
        public RunAgentDelete(IDBContext eddsDbContext, IEnvironmentHelper environment, int resourcePoolId, List<AgentsDesired> agentsToDelete, IAPILog logger)
        {
            _eddsDbContext = eddsDbContext;
            _environment = environment;
            _agentsToDelete = agentsToDelete;
            _resourcePoolId = resourcePoolId;
            _logger = logger;
        }

        public void Run()
        {
            int agentTypeId;
            int agentToDeleteId;
            int counter;
            List<SpotsPerServerObject> agentsPerServer = new List<SpotsPerServerObject>();
            DeleteAgent deleteAgent = new DeleteAgent(_eddsDbContext);

            while (_agentsToDelete.Count > 0)
            {
                _logger.LogDebug(string.Format("{0} AgentsDesiredObjects to be deleted", _agentsToDelete.Count));

                counter = _agentsToDelete.Count - 1;
                while (counter >= 0)
                {
                    _agentsToDelete[counter].Count = Math.Abs(_agentsToDelete[counter].Count);
                    agentTypeId = _environment.GetArtifactIdFromGuid(_agentsToDelete[counter].Guid);
                    agentsPerServer = _environment.GetAgentsPerServerByPool(agentTypeId, _resourcePoolId);
                    if (agentsPerServer.Count > 0)
                    {
                        agentsPerServer = agentsPerServer.OrderByDescending(x => x.Spots).ToList();
                        if (agentsPerServer[0].Spots > 0)
                        {
                            agentToDeleteId = deleteAgent.GetAgentIdToDelete(agentTypeId, agentsPerServer[0].AgentServerArtifactId);
                            deleteAgent.Delete(agentToDeleteId);
                            _agentsToDelete[counter].Count -= 1;

                            if (_agentsToDelete[counter].Count < 1)
                            {
                                _agentsToDelete.Remove(_agentsToDelete[counter]);
                            }

                        }
                        else
                        {
                            _agentsToDelete.Remove(_agentsToDelete[counter]);
                        }
                    }
                    else
                    {
                        _agentsToDelete.Remove(_agentsToDelete[counter]);
                    }

                    counter -= 1;
                }
            }
        }
    }
}
