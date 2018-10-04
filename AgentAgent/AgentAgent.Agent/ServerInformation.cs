using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Relativity.API;

namespace AgentAgent.Agent
{
    class ServerInformation
    {
        private readonly IDBContext _eddsDBContext;

        public ServerInformation(IDBContext eddsDBContext)
        {
            _eddsDBContext = eddsDBContext;
        }

        public AgentServerObject GetAgentServerObject(int agentServerArtifactId)
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

            DataTable agentServerDataTable = _eddsDBContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { serverArtifactIdParam });

            if (agentServerDataTable == null)
            {
                throw new Exception("This environment contains no agent servers or retrieval from DB failed");
            }
            else if (agentServerDataTable.Rows.Count > 1)
            {
                throw new Exception("Multiple server entries returned for agent server Artifact ID");
            }
            else
            {
                if (!int.TryParse(agentServerDataTable.Rows[0]["ArtifactID"].ToString(), out int artifactId))
                {
                    throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                }

                string hostname = agentServerDataTable.Rows[0]["Hostname"].ToString();

                if (!bool.TryParse(agentServerDataTable.Rows[0]["Status"].ToString(), out bool active))
                {
                    throw new Exception("Unable to cast agent server status returned from database to Boolean");
                }

                if (!int.TryParse(agentServerDataTable.Rows[0]["ProcessorCores"].ToString(), out int cores))
                {
                    throw new Exception("Unable to cast agent server core count returned from database to Int32");
                }

                if (!long.TryParse(agentServerDataTable.Rows[0]["Memory"].ToString(), out long memory))
                {
                    throw new Exception("Unable to cast agent server memory count returned from database to Int64");
                }

                if (!int.TryParse(agentServerDataTable.Rows[0]["NumberOfAgents"].ToString(), out int agentCount))
                {
                    throw new Exception("Unable to cast count of agents on server returned from database to Int32");
                }

                AgentServerObject agentServer = new AgentServerObject(artifactId, hostname, active, cores, memory, agentCount);

                return agentServer;

            }



        }

        public List<AgentServerObject> GetPoolAgentServerList(int resourcePoolArtifactId)
        {
            List<AgentServerObject> outputList = new List<AgentServerObject>();

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

            SqlParameter poolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = resourcePoolArtifactId
            };

            DataTable agentServerDataTable = _eddsDBContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { poolArtifactIdParam });

            if (agentServerDataTable == null)
            {
                throw new Exception("The Agent Agent Resource Pools contains no agent servers or retrieval from DB failed");
            }
            else
            {
                foreach (DataRow row in agentServerDataTable.Rows)
                {

                    if (!int.TryParse(row["ArtifactID"].ToString(), out int artifactId))
                    {
                        throw new Exception("Unable to cast agent server ArtifactID returned from database to Int32");
                    }

                    string hostname = row["Hostname"].ToString();

                    if (!bool.TryParse(row["Status"].ToString(), out bool active))
                    {
                        throw new Exception("Unable to cast agent server status returned from database to Boolean");
                    }

                    if (!int.TryParse(row["ProcessorCores"].ToString(), out int cores))
                    {
                        throw new Exception("Unable to cast agent server core count returned from database to Int32");
                    }

                    if (!long.TryParse(row["Memory"].ToString(), out long memory))
                    {
                        throw new Exception("Unable to cast agent server memory count returned from database to Int64");
                    }

                    if (!int.TryParse(row["NumberOfAgents"].ToString(), out int agentCount))
                    {
                        throw new Exception("Unable to cast count of agents on server returned from database to Int32");
                    }

                    AgentServerObject agentServer = new AgentServerObject(artifactId, hostname, active, cores, memory, agentCount);

                    outputList.Add(agentServer);
                }

                return outputList;
            }
        }

        public List<int> GetPoolIDsByServerId(int agentServerArtifactId)
        {
            List<int> outputList = new List<int>();

            string SQL = @"
                SELECT [ResourceGroupArtifactID]
                FROM[ServerResourceGroup]
                WHERE[ResourceServerArtifactID] = @AgentServerArtifactID";

            SqlParameter serverArtifactIdParam = new SqlParameter("@AgentServerArtifactID", System.Data.SqlDbType.Char)
            {
                Value = agentServerArtifactId
            };

            DataTable poolDataTable = _eddsDBContext.ExecuteSqlStatementAsDataTable(SQL, new SqlParameter[] { serverArtifactIdParam });

            if (poolDataTable == null)
            {
                throw new Exception("This environment contains has no agent servers in resource pools or retrieval from DB failed");
            }

            foreach (DataRow pool in poolDataTable.Rows)
            {
                if (!int.TryParse(pool["ResourceGroupArtifactID"].ToString(), out int resourcePoolId))
                {
                    throw new Exception("Unable to cast resource pool ID returned from database to Int32");
                }

                outputList.Add(resourcePoolId);
            }

            return outputList;

        }
    }
}

