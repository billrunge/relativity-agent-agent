using System;
using System.Data.SqlClient;
using AgentAgent.Agent.Interfaces;
using Relativity.API;

namespace AgentAgent.Agent
{
    class CreateAgentSql : ICreateAgent
    {
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

        public void Create(string agentTypeGuid, int agentServerArtifactId)
        {
            _agentTypeArtifactId = _environment.GetArtifactIdFromGuid(agentTypeGuid);
            _agentServerArtifactId = agentServerArtifactId;
            _runInterval = _environment.GetAgentRunIntervalByType(_agentTypeArtifactId);
            _agentName = CreateAgentName();
            _agentArtifactTypeId = _environment.GetAgentArtifactType();
            _systemContainerId = _environment.GetSystemContainerId();

            RunCreateAgentSql();
        }

        private void RunCreateAgentSql()
        {
            
            string SQL = @"
                BEGIN TRY 
                    BEGIN TRANSACTION 

                    --Get ArtifactID 
                    DECLARE @AgentArtifactID INT 

                    INSERT INTO [Artifact] 
                                ([ArtifactTypeID], 
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
                                 [DeleteFlag]) 
                    VALUES      ( @AgentArtifactTypeID, 
                                  @ParentContainerID, 
                                  1, 
                                  1, 
                                  GETUTCDATE(), 
                                  GETUTCDATE(), 
                                  777, 
                                  777, 
                                  @AgentName, 
                                  @ParentContainerID, 
                                  '', 
                                  'Created with Agent Agent', 
                                  0 ) 

                    SELECT @AgentArtifactID = Scope_identity() 

                    INSERT INTO [ArtifactAncestry] 
                                (ArtifactID, 
                                 AncestorArtifactID) 
                    VALUES      (@AgentArtifactID, 
                                 @ParentContainerID) 

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
                    VALUES      (@AgentName, 
                                 '', 
                                 GETUTCDATE(), 
                                 GETUTCDATE(), 
                                 '', 
                                 1, 
                                 @RunInterval, 
                                 '', 
                                 @AgentArtifactID, 
                                 @AgentTypeArtifactID, 
                                 @ServerArtifactID, 
                                 1, 
                                 1, 
                                 1) 
                    SELECT @AgentArtifactID
                    COMMIT TRANSACTION 

                END TRY 

                BEGIN CATCH 
                    SELECT 0

                    ROLLBACK TRANSACTION 
                END CATCH ";

            SqlParameter agentArtifactTypeIdParam = new SqlParameter("@AgentArtifactTypeID", System.Data.SqlDbType.Int)
            {
                Value = _agentArtifactTypeId
            };

            SqlParameter parentContainerIdParam = new SqlParameter("@ParentContainerID", System.Data.SqlDbType.Int)
            {
                Value = _systemContainerId
            };

            SqlParameter agentNameParam = new SqlParameter("@AgentName", System.Data.SqlDbType.NVarChar)
            {
                Value = _agentName
            };

            SqlParameter runIntervalParam = new SqlParameter("@RunInterval", System.Data.SqlDbType.Char)
            {
                Value = _runInterval
            };

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = _agentTypeArtifactId
            };

            SqlParameter agentServerArtifactIdParam = new SqlParameter("@ServerArtifactID", System.Data.SqlDbType.Int)
            {
                Value = _agentServerArtifactId
            };

            _agentArtifactId = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] 
            {
                agentArtifactTypeIdParam,
                parentContainerIdParam,
                agentNameParam,
                runIntervalParam,
                agentTypeArtifactIdParam,
                agentServerArtifactIdParam
            });

            if (_agentArtifactId > 0)
            {
                InsertAuditRecord(_agentName);
            }
            else
            {
                throw new Exception("SQL Agent creation Failed");
            }
        }      

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
    }
}




