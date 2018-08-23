using System;
using Relativity.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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
            int agentArtifactType = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(@"
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
            int systemContainerId = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(@"
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

        //Get Agent Type ID for specific agent type (i.e. Production Manager) by its GUID
        public int GetArtifactIdFromGuid(string Guid)
        {
            int artifactId;

            string SQL = @"
                DECLARE @SQL NVARCHAR(500) =
                'SELECT TOP 1 ArtifactID
                FROM[EDDS].[eddsdbo].[ArtifactGuid]
                WHERE ArtifactGuid = ''' + @Guid + ''''
                EXEC SP_ExecuteSQL @SQL";

            SqlParameter artifactGuid = new SqlParameter("@Guid", System.Data.SqlDbType.Char)
            {
                Value = Guid
            };

            artifactId = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { artifactGuid });

            if (artifactId == 0)
            {
                throw new Exception("Unable to retrieve agent type artifactID by its GUID from the database");
            }
            else
            {
                return artifactId;
            }
        }

        public string GetTextIdByArtifactId(int artifactId)
        {
            string textIdentifier;
            string SQL = @"
                SELECT TOP 1 [TextIdentifier]
                FROM [EDDS].[eddsdbo].[Artifact]
                WHERE [ArtifactID] = @ArtifactID";

            SqlParameter artifactIdParam = new SqlParameter("@ArtifactID", System.Data.SqlDbType.Char)
            {
                Value = artifactId
            };

            textIdentifier = _eddsDbContext.ExecuteSqlStatementAsScalar<string>(SQL, new SqlParameter[] { artifactIdParam });

            if (textIdentifier == null || textIdentifier == "")
            {
                throw new Exception("Unable to retrieve text identifier from database.");
            }
            else
            {
                return textIdentifier;
            }



        }

        public int GetAgentCount(int agentTypeArtifactId)
        {
            int agentCount;

            string SQL = @"
                SELECT COUNT(*)
                FROM [EDDS].[eddsdbo].[Agent]
                WHERE [AgentTypeArtifactID] = @AgentTypeArtifactID";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };

            agentCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentTypeArtifactIdParam });
            return agentCount;

        }

        public int GetAgentRunIntervalByType(int agentTypeArtifactId)
        {
            int runInterval;
            string SQL = @"
                SELECT[DefaultInterval]
                FROM[EDDS].[eddsdbo].[AgentType]
                WHERE[ArtifactID] = @AgentTypeArtifactID";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };

            runInterval = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentTypeArtifactIdParam });

            if (runInterval == 0)
            {
                throw new Exception("Unable to retrieve agent run interval from the database");
            }
            else
            {
                return runInterval;
            }

        }

    }
}
