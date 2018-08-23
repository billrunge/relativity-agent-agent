using System;
using Relativity.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentAgent.Agent
{
    class EnvironmentInformation
    {
        private IDBContext _eddsDbContext;

        public EnvironmentInformation(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
        }

        public int GetAgentArtifactType()
        {
            int agentArtifactType = _eddsDbContext.ExecuteSqlStatementAsScalar<Int32>(@"
                SELECT TOP 1 [ArtifactTypeID] 
                FROM   [EDDS].[eddsdbo].[ArtifactType] 
                WHERE  ArtifactType = 'Agent' ");

            if (agentArtifactType == 0)
            {
                throw new Exception("Unable to retrieve agent artifact type from database");
            }
            else
            {
                return agentArtifactType;
            }
        }

        //Get system container ID
        public int GetSystemContainerId()
        {
            int systemContainerId = _eddsDbContext.ExecuteSqlStatementAsScalar<Int32>(@"
                SELECT TOP 1 [ArtifactID]
                FROM [EDDS].[eddsdbo].[Artifact]
                WHERE TextIdentifier = 'System'
                ORDER BY[ArtifactID] ASC");

            if (systemContainerId == 0)
            {
                throw new Exception("Unable to retrieve system container ID from database");
            }
            else
            {
                return systemContainerId;
            }
        }

    }
}
