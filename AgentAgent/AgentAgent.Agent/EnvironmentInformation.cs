using System;
using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{

    public interface IEnvironmentInformation
    {
        int GetAgentArtifactType();
        int GetSystemContainerId();
        int GetArtifactIdFromGuid(string Guid);
        string GetTextIdByArtifactId(int artifactId);
        int GetAgentCount(int agentTypeArtifactId);
        int GetAgentRunIntervalByType(int agentTypeArtifactId);
    }

    /// <summary>
    /// A class to hold a set of methods that allow you to get information about the Relativity environmentk
    /// </summary>
    class EnvironmentInformation : IEnvironmentInformation
    {
        private IDBContext _eddsDbContext;

        public EnvironmentInformation(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
        }

        //Get agent artifact type id from artifact type table
        public int GetAgentArtifactType()
        {
            int agentArtifactType = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(@"
                SELECT TOP 1 [ArtifactTypeID] 
                FROM [ArtifactType] 
                WHERE  ArtifactType = 'Agent' ");

            if (agentArtifactType == 0)
            {
                throw new Exception("Unable to retrieve agent artifact type ID from database");
            }
            else
            {
                return agentArtifactType;
            }
        }

        //Get system container ID from artifact table
        public int GetSystemContainerId()
        {
            int systemContainerId = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(@"
                SELECT TOP 1 [ArtifactID]
                FROM [Artifact]
                WHERE TextIdentifier = 'System'
                ORDER BY [ArtifactID] ASC");

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
                FROM [ArtifactGuid]
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

        //Get text identifier for an artifact from the artifact table
        public string GetTextIdByArtifactId(int artifactId)
        {
            string textIdentifier;
            string SQL = @"
                SELECT TOP 1 [TextIdentifier]
                FROM [Artifact]
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

        //Get current deployed agent count for a specific agent type
        public int GetAgentCount(int agentTypeArtifactId)
        {
            int agentCount;

            string SQL = @"
                SELECT COUNT(*)
                FROM [Agent]
                WHERE [AgentTypeArtifactID] = @AgentTypeArtifactID";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };

            agentCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentTypeArtifactIdParam });
            return agentCount;

        }

        //Get default run interval for a specific agent type from agent type table
        public int GetAgentRunIntervalByType(int agentTypeArtifactId)
        {
            int runInterval;
            string SQL = @"
                SELECT[DefaultInterval]
                FROM [AgentType]
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
