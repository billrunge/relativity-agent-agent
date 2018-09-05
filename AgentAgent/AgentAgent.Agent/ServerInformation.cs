﻿using System;
using System.Collections.Generic;
using System.Data;
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

        public List<AgentServerObject> GetAgentServerList()
        {
            List<AgentServerObject> outputList = new List<AgentServerObject>();

            string SQL = @"
                SELECT [ArtifactID],
                       [Name]                                  as [Hostname],
                       IIF([Status] = 'Active', 'True', 'False') as [Status],
                       [ProcessorCores],
                       [Memory],
                       [NumberOfAgents]
                FROM   [ExtendedResourceServer]
                WHERE  [Type] = 'Agent'";

            DataTable agentServerDataTable = _eddsDBContext.ExecuteSqlStatementAsDataTable(SQL);

            if (agentServerDataTable == null)
            {
                throw new Exception("This environment contains no agent servers or retrieval from DB failed");
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

    }
}
