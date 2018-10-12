using System;
using Relativity.API;
using System.Data.SqlClient;
using System.Collections.Generic;
using AgentAgent.Agent.Interfaces;
using System.Data;

namespace AgentAgent.Agent
{
    /// <summary>
    /// A helper class to get information about the Relativity environment
    ///</summary>

    class EnvironmentHelper : IEnvironmentHelper
    {
        private IDBContext _eddsDbContext;

        public EnvironmentHelper(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
        }

        //Get agent artifact type id from artifact type table
        public int GetAgentArtifactType()
        {
            int agentArtifactType = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(@"
                SELECT TOP 1 [ArtifactTypeID] 
                FROM [ArtifactType] WITH(NOLOCK)
                WHERE  ArtifactType = 'Agent'");

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
                FROM [Artifact] WITH(NOLOCK)
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
                SELECT TOP 1 ArtifactID
                FROM [ArtifactGuid] WITH(NOLOCK)
                WHERE ArtifactGuid = @Guid";

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
                FROM [Artifact] WITH(NOLOCK)
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
                SELECT Count(AG.[ArtifactID]) 
                FROM   [Agent] AG WITH(NOLOCK) 
                       INNER JOIN [Artifact] A WITH(NOLOCK) 
                               ON AG.[ArtifactID] = A.[ArtifactID] 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                       AND A.[DeleteFlag] = 0";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };

            agentCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentTypeArtifactIdParam });
            return agentCount;

        }

        //Get current deployed agent count for a specific agent type and resource pool
        public int GetAgentCountByPool (int agentTypeArtifactId, int poolId)
        {
            int agentCount;

            string SQL = @"
                SELECT Count(AG.[ArtifactID]) 
                FROM   [Agent] AG WITH(NOLOCK) 
                        INNER JOIN [ServerResourceGroup] S WITH(NOLOCK) 
                                ON AG.[ServerArtifactID] = S.[ResourceServerArtifactID] 
                        INNER JOIN [Artifact] A WITH(NOLOCK) 
                                ON AG.[ArtifactID] = A.[ArtifactID] 
                WHERE  AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                        AND S.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                        AND [DeleteFlag] = 0";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = poolId
            };

            agentCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { agentTypeArtifactIdParam, poolIdParam });
            return agentCount;
        }

        public List<SpotsPerServer> GetAgentsPerServerByPool(int agentTypeArtifactId, int poolId)
        {
            List<SpotsPerServer> outputList = new List<SpotsPerServer>();

            string SQL = @"
                SELECT AG.[ServerArtifactID], 
                       Count(AG.[ArtifactID]) AS [Count] 
                FROM   [Agent] AG WITH(NOLOCK)
                       INNER JOIN [Artifact] A WITH(NOLOCK)
                               ON AG.[ArtifactID] = A.[ArtifactID] 
                       INNER JOIN [ServerResourceGroup] S WITH(NOLOCK)
                               ON AG.[ServerArtifactID] = S.[ResourceServerArtifactID] 
                WHERE  A.[DeleteFlag] = 0 
                       AND AG.[AgentTypeArtifactID] = @AgentTypeArtifactID 
                       AND S.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                GROUP  BY AG.[ServerArtifactID]";

            SqlParameter agentTypeArtifactIdParam = new SqlParameter("@AgentTypeArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentTypeArtifactId
            };
            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = poolId
            };

            DataTable results = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { agentTypeArtifactIdParam, poolIdParam });

            if (results != null)
            {
                foreach (DataRow row in results.Rows)
                {
                    int serverArtifactId;
                    int count; 

                    if (!int.TryParse(row["ServerArtifactID"].ToString(), out serverArtifactId))
                    {
                        throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                    }

                    if (!int.TryParse(row["Count"].ToString(), out count))
                    {
                        throw new Exception("Unable to cast agent count returned from database to Int32");
                    }

                    outputList.Add(new SpotsPerServer()
                    {
                        AgentServerArtifactId = serverArtifactId,
                        Spots = count
                    });
                }
            }
            return outputList;

        } 

        //Get default run interval for a specific agent type from agent type table
        public int GetAgentRunIntervalByType(int agentTypeArtifactId)
        {
            int runInterval;
            string SQL = @"
                SELECT [DefaultInterval]
                FROM [AgentType] WITH(NOLOCK)
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

        //Get count of Analytics servers in a specific respource pool
        public int GetAnalyticsServerCountByResourcePool (int poolId)
        {
            int serverCount = 0;

            string SQL = @"
                SELECT Count(RS.[ArtifactID]) 
                FROM   [ResourceServer] RS WITH(NOLOCK) 
                       INNER JOIN [ResourceGroupAnalyticsServers] RGAS WITH(NOLOCK) 
                               ON RS.[ArtifactID] = RGAS.[AnalyticsServerArtifactID] 
                WHERE  RGAS.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = poolId
            };

            serverCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });

            return serverCount;

        }

        //Get an object holding information about agent servers by its ArtifactID
        public AgentServer GetAgentServerObject(int agentServerArtifactId)
        {
            string SQL = @"
				SELECT [ArtifactID],
                       [Name]                                  as [Hostname],
                       IIF([Status] = 'Active', 'True', 'False') as [Status],
                       [ProcessorCores],
                       [Memory],
                       [NumberOfAgents]
                FROM   [ExtendedResourceServer]
                WHERE  [ArtifactID] = @ServerArtifactID";

            SqlParameter serverArtifactIdParam = new SqlParameter("@ServerArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentServerArtifactId
            };

            DataTable agentServerDataTable = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { serverArtifactIdParam });

            if (agentServerDataTable == null)
            {
                throw new Exception(String.Format("This environment does not contain an agent server with Artifat ID: {0} retrieval from DB failed", agentServerArtifactId));
            }
            else if (agentServerDataTable.Rows.Count > 1)
            {
                throw new Exception(String.Format("Multiple server entries returned for agent server Artifact ID: {0}", agentServerArtifactId));
            }
            else
            {
                int artifactId;
                bool active;
                int cores;
                long memory;
                int agentCount;

                if (!int.TryParse(agentServerDataTable.Rows[0]["ArtifactID"].ToString(), out artifactId))
                {
                    throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                }

                string hostname = agentServerDataTable.Rows[0]["Hostname"].ToString();

                if (!bool.TryParse(agentServerDataTable.Rows[0]["Status"].ToString(), out active))
                {
                    throw new Exception("Unable to cast agent server status returned from database to Boolean");
                }

                if (!int.TryParse(agentServerDataTable.Rows[0]["ProcessorCores"].ToString(), out cores))
                {
                    throw new Exception("Unable to cast agent server core count returned from database to Int32");
                }

                if (!long.TryParse(agentServerDataTable.Rows[0]["Memory"].ToString(), out memory))
                {
                    throw new Exception("Unable to cast agent server memory count returned from database to Int64");
                }

                if (!int.TryParse(agentServerDataTable.Rows[0]["NumberOfAgents"].ToString(), out agentCount))
                {
                    throw new Exception("Unable to cast count of agents on server returned from database to Int32");
                }

                return new AgentServer
                {
                    ArtifactID = artifactId,
                    Hostname = hostname,
                    Active = active,
                    ProcessorCores = cores,
                    Memory = memory,
                    AgentCount = agentCount
                };                   
            }
        }

        //Get a list of AgentServerObjects by Resource Pool
        public List<AgentServer> GetPoolAgentServerList(int poolId)
        {
            List<AgentServer> outputList = new List<AgentServer>();

            string SQL = @"
                SELECT E.[ArtifactID], 
                       E.[Name]                                    as [Hostname], 
                       IIF(E.[Status] = 'Active', 'True', 'False') as [Status], 
                       E.[ProcessorCores], 
                       E.[Memory], 
                       E.[NumberOfAgents] 
                FROM   [ExtendedResourceServer] E 
                       INNER JOIN [ServerResourceGroup] S 
                               ON E.[ArtifactID] = S.[ResourceServerArtifactID] 
                WHERE  [Type] = 'Agent' 
                       AND S.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = poolId
            };

            DataTable agentServerDataTable = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { poolIdParam });

            if (agentServerDataTable == null)
            {
                throw new Exception("The Agent Agent Resource Pools contains no agent servers or retrieval from DB failed");
            }
            else
            {
                foreach (DataRow row in agentServerDataTable.Rows)
                {
                    int artifactId;
                    bool active;
                    int cores;
                    long memory;
                    int agentCount;

                    if (!int.TryParse(row["ArtifactID"].ToString(), out artifactId))
                    {
                        throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                    }

                    string hostname = row["Hostname"].ToString();

                    if (!bool.TryParse(row["Status"].ToString(), out active))
                    {
                        throw new Exception("Unable to cast agent server status returned from database to Boolean");
                    }

                    if (!int.TryParse(row["ProcessorCores"].ToString(), out cores))
                    {
                        throw new Exception("Unable to cast agent server core count returned from database to Int32");
                    }

                    if (!long.TryParse(row["Memory"].ToString(), out memory))
                    {
                        throw new Exception("Unable to cast agent server memory count returned from database to Int64");
                    }

                    if (!int.TryParse(row["NumberOfAgents"].ToString(), out agentCount))
                    {
                        throw new Exception("Unable to cast count of agents on server returned from database to Int32");
                    }                   

                    outputList.Add(new AgentServer {
                    ArtifactID = artifactId,
                    Hostname = hostname,
                    Active = active,
                    ProcessorCores = cores,
                    AgentCount = agentCount,
                    Memory = memory});
                }
                return outputList;
            }
        }

        //Get a list of AgentServerObjects by Resource Pool that do not conatin the dtSearch search agent
        public List<AgentServer> GetPoolAgentServerListNoDtSearch(int poolId)
        {
            List<AgentServer> outputList = new List<AgentServer>();
            //A0683637-4690-49CE-B8BE-156324FC64F0 is the GUID for the dtSearch Search Agent
            string SQL = @"
                SELECT E.[ArtifactID], 
                       E.[Name]                                    as [Hostname], 
                       IIF(E.[Status] = 'Active', 'True', 'False') as [Status], 
                       E.[ProcessorCores], 
                       E.[Memory], 
                       E.[NumberOfAgents] 
                FROM   [ExtendedResourceServer] E WITH(NOLOCK) 
                       INNER JOIN [ServerResourceGroup] S WITH(NOLOCK) 
                               ON E.[ArtifactID] = S.[ResourceServerArtifactID] 
                WHERE  E.[Type] = 'Agent' 
                       AND S.[ResourceGroupArtifactID] = @ResourceGroupArtifactID 
                       AND E.[ArtifactID] NOT IN (SELECT A.[ServerArtifactID] 
                                                  FROM   [Agent] A WITH(NOLOCK) 
                                                         INNER JOIN [AgentType] AGT WITH(NOLOCK) 
                                                                 ON A.[AgentTypeArtifactID] = 
                                                                    AGT.[ArtifactID] 
                                                  WHERE 
                               AGT.[Guid] = 'A0683637-4690-49CE-B8BE-156324FC64F0')";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = poolId
            };

            DataTable agentServerDataTable = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { poolIdParam });

            if (agentServerDataTable == null)
            {
                throw new Exception("The Agent Agent Resource Pools contains no agent servers or retrieval from DB failed");
            }
            else
            {
                foreach (DataRow row in agentServerDataTable.Rows)
                {
                    int artifactId;
                    bool active;
                    int cores;
                    long memory;
                    int agentCount;

                    if (!int.TryParse(row["ArtifactID"].ToString(), out artifactId))
                    {
                        throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                    }

                    string hostname = row["Hostname"].ToString();

                    if (!bool.TryParse(row["Status"].ToString(), out active))
                    {
                        throw new Exception("Unable to cast agent server status returned from database to Boolean");
                    }

                    if (!int.TryParse(row["ProcessorCores"].ToString(), out cores))
                    {
                        throw new Exception("Unable to cast agent server core count returned from database to Int32");
                    }

                    if (!long.TryParse(row["Memory"].ToString(), out memory))
                    {
                        throw new Exception("Unable to cast agent server memory count returned from database to Int64");
                    }

                    if (!int.TryParse(row["NumberOfAgents"].ToString(), out agentCount))
                    {
                        throw new Exception("Unable to cast count of agents on server returned from database to Int32");
                    }

                    outputList.Add(new AgentServer
                    {
                        ArtifactID = artifactId,
                        Hostname = hostname,
                        Active = active,
                        ProcessorCores = cores,
                        AgentCount = agentCount,
                        Memory = memory
                    });
                }
                return outputList;
            }
        }
    }
}
