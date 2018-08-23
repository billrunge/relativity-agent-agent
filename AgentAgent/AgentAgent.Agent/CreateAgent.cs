using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace AgentAgent.Agent
{
    class CreateAgent
    {
        private readonly string _agentName;
        private readonly IDBContext _eddsDbContext;
        private readonly int _agentTypeArtifactId;
        private readonly int _systemContainerId;

        private int _artifactId;
        

        public CreateAgent(IDBContext eddsDbContext, string agentName)
        {            
            _agentName = agentName;
            _eddsDbContext = eddsDbContext;

            EnvironmentInformation environmentInformation = new EnvironmentInformation(_eddsDbContext);

            _agentTypeArtifactId = environmentInformation.GetAgentArtifactType();
            _systemContainerId = environmentInformation.GetSystemContainerId();

        }


        private void ArtifactID()
        {
            

            //Create a new ArtifactID, and return the created value
            string SQL = @"
                DECLARE @SQL NVARCHAR(1000)
                DECLARE @AgentArtifactID NVARCHAR(10)

                SET @SQL = '
                INSERT INTO[EDDS].[eddsdbo].[Artifact] 
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
                                        ''created with dynamic agent'', 
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
                Value = _agentTypeArtifactId
            };

            SqlParameter parentContainerID = new SqlParameter("@ParentContainerID", System.Data.SqlDbType.Char)
            {
                Value = _systemContainerId
            };

            //Run SQL to create ArtifactID/Artifact table entry and return ArtifactID
            int artifactId = _eddsDbContext.ExecuteSqlStatementAsScalar<Int32>(SQL, new SqlParameter[] { agentName, agentArtifactTypeID, parentContainerID });

            if (artifactId == 0)
            {
                throw new Exception("Artifact creation failed");
            }
            else
            {
                _artifactId = artifactId;
            }
            
        }


    }
}
