using System;
using System.Data.SqlClient;
using Relativity.API;

namespace AgentAgent.Agent
{
    class CreateAgentSql : ICreateAgent
    {
        //private AgentObject _agent;
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private string _agentName;
        private int _agentArtifactId;
        private int _agentServerArtifactId;
        private int _agentTypeArtifactId;
        private int _agentArtifactTypeId;
        private int _systemContainerId;
        private int _runInterval;


        public CreateAgentSql(IDBContext eddsDbContext, IEnvironmentHelper environment)
        {
            _eddsDbContext = eddsDbContext;
            _environment = environment;

        }

        //Inserts row to the ArtifactID table, which generates a new artifact ID
        private void InsertArtifact()
        {
            //Create a new ArtifactID, and return the created value
            string SQL = @"
                DECLARE @SQL NVARCHAR(2000)
                DECLARE @AgentArtifactID NVARCHAR(10)

                SET @SQL = '
                INSERT INTO [Artifact] 
                            ( 
                                        [ArtifactTypeID], 
                                        [ParentArtifactID], 
                                        [AccessControlListID], 
                                        [AccessControlListIsInherited], 
                                        [CreatedOn], 
                                        [LastModifiedOn], 
                                        [LastModifiedBy], 
                                        [CreatedBy], 
                                        [TextIdentifier], 
                                        [ContainerID], 
                                        [Keywords], 
                                        [Notes], 
                                        [DeleteFlag] 
                            ) 
                            VALUES 
                            ( 
                                        '+@AgentArtifactType+', 
                                        '+@ParentContainerID+', 
                                        1, 
                                        1, 
                                        CONVERT(nvarchar(25),Getutcdate(),121), 
                                        CONVERT(nvarchar(25),Getutcdate(),121), 
                                        777, 
                                        777, 
                                        '''+@AgentName+''', 
                                        '+@ParentContainerID+', 
                                        '''', 
                                        ''Created with Agent Agent'', 
                                        0 
                            )SELECT @AgentArtifactID = Scope_identity()'

                EXEC SP_ExecuteSQL @SQL, N'@AgentArtifactID INTEGER OUTPUT', @AgentArtifactID OUTPUT

                SELECT @AgentArtifactID";

            //Gather values to input into above script
            SqlParameter agentName = new SqlParameter("@AgentName", System.Data.SqlDbType.Char)
            {
                Value = _agentName
            };

            SqlParameter agentArtifactTypeID = new SqlParameter("@AgentArtifactType", System.Data.SqlDbType.Char)
            {
                Value = _agentArtifactTypeId
            };

            SqlParameter parentContainerID = new SqlParameter("@ParentContainerID", System.Data.SqlDbType.Char)
            {
                Value = _systemContainerId
            };

            //Run SQL to create ArtifactID/Artifact table entry and return ArtifactID
            int artifactId = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentName, agentArtifactTypeID, parentContainerID });

            if (artifactId == 0)
            {
                throw new Exception("Artifact creation failed");
            }
            else
            {
                _agentArtifactId = artifactId;
            }

        }

        //Insert row into the Artifact Ancestry table
        private void InsertArtifactAncestry()
        {
            string SQL = @"
                DECLARE @SQL NVARCHAR(255)
                SET @SQL = '
                INSERT INTO [ArtifactAncestry] 
                           (ArtifactID, 
                            AncestorArtifactID) 
                VALUES     ('+@AgentArtifactID+', 
                            '+@ParentContainerID+')'

                EXEC SP_ExecuteSQL @SQL";

            SqlParameter artifactID = new SqlParameter("@AgentArtifactID", System.Data.SqlDbType.Char)
            {
                Value = _agentArtifactId
            };

            SqlParameter parentContainerID = new SqlParameter("@ParentContainerID", System.Data.SqlDbType.Char)
            {
                Value = _systemContainerId
            };

            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { artifactID, parentContainerID });

        }

        //Insert row into the agent table
        private void InsertAgentTable()
        {
            string SQL = @"
                DECLARE @SQL NVARCHAR(2000)
                SET @SQL = '
                INSERT INTO [Agent] 
                           ([Name], 
                            [Message], 
                            [MessageTime], 
                            [LastUpdate], 
                            [MessageType], 
                            [Enabled], 
                            [Interval], 
                            [DetailMessage], 
                            [ArtifactID], 
                            [AgentTypeArtifactID], 
                            [ServerArtifactID], 
                            [Updated], 
                            [Safe], 
                            [LoggingLevel]) 
                VALUES     ('''+@AgentName+''', 
                            '''', 
                            CONVERT(nvarchar(25), Getutcdate(), 121), 
                            CONVERT(nvarchar(25), Getutcdate(), 121), 
                            '''', 
                            1, 
                            '+@RunInterval+', 
                            '''', 
                            '+@AgentArtifactID+', 
                            '+@AgentTypeArtifact+', 
                            '+@ServerArtifactID+', 
                            1, 
                            1, 
                            1)'

                EXEC SP_ExecuteSQL @SQL";

            //Gather values to input into above script
            SqlParameter agentServerArtifactID = new SqlParameter("@ServerArtifactID", System.Data.SqlDbType.Char)
            {
                Value = _agentServerArtifactId
            };

            SqlParameter agentName = new SqlParameter("@AgentName", System.Data.SqlDbType.Char)
            {
                Value = _agentName
            };

            SqlParameter agentTypeArtifactID = new SqlParameter("@AgentTypeArtifact", System.Data.SqlDbType.Char)
            {
                Value = _agentTypeArtifactId
            };

            SqlParameter artifactID = new SqlParameter("@AgentArtifactID", System.Data.SqlDbType.Char)
            {
                Value = _agentArtifactId
            };

            SqlParameter runInterval = new SqlParameter("@RunInterval", System.Data.SqlDbType.Char)
            {
                Value = _runInterval
            };

            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentServerArtifactID, agentName, agentTypeArtifactID, runInterval, artifactID });

            //Audit the agent's creation
            InsertAuditRecord(_agentName);
        }

        //Insert row into EDDS's AuditRecord table
        private void InsertAuditRecord(string agentName)
        {
            string SQL = @"
                INSERT INTO [AuditRecord] 
                            ([ArtifactID], 
                             [Action], 
                             [Details], 
                             [UserID], 
                             [TimeStamp], 
                             [RequestOrigination], 
                             [RecordOrigination]) 
                VALUES      (@ArtifactID, 
                             @Action, 
                             '<auditElement>' + @AgentName + ' created with Agent Agent.</auditElement>', 
                             777, 
                             Getutcdate(), 
                             '<auditElement></auditElement>', 
                             '<auditElement></auditElement>') ";

            //Gather values to input into above script
            SqlParameter agentArtifactIdParam = new SqlParameter("@ArtifactID", System.Data.SqlDbType.Char)
            {
                Value = _agentArtifactId
            };

            SqlParameter agentNameParam = new SqlParameter("@AgentName", System.Data.SqlDbType.Char)
            {
                Value = agentName
            };

            SqlParameter actionParam = new SqlParameter("@Action", System.Data.SqlDbType.Char)
            {
                //2 is create
                Value = 2
            };

            _eddsDbContext.ExecuteNonQuerySQLStatement(SQL, new SqlParameter[] { agentArtifactIdParam, agentNameParam, actionParam });

        }

        //Using the agent type name and current count of that agent type in the system, generate a name for generated agents
        //There is no real issue with having agents with the same name, but in the future would like to add logic
        //to make sure that the agent name doesn't already exist
        private string CreateAgentName()
        {
            int agentCount = _environment.GetAgentCount(_agentTypeArtifactId);
            string agentTypeName = _environment.GetTextIdByArtifactId(_agentTypeArtifactId);
            return string.Format("{0} ({1})", agentTypeName, agentCount + 1);
        }

        //Run the methods to create the agent
        public void Create(string agentTypeGuid, int agentServerArtifactId)
        {
            _agentTypeArtifactId = _environment.GetArtifactIdFromGuid(agentTypeGuid);
            _agentServerArtifactId = agentServerArtifactId;
            _runInterval = _environment.GetAgentRunIntervalByType(_agentTypeArtifactId);
            _agentName = CreateAgentName();
            _agentArtifactTypeId = _environment.GetAgentArtifactType();
            _systemContainerId = _environment.GetSystemContainerId();

            //These three methods must be ran in this order
            //Or at the very least, InsertArtifact first
            InsertArtifact();
            InsertArtifactAncestry();
            InsertAgentTable();
        }

    }
}




