﻿using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class OCRWorker : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public OCRWorker(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "9541B7E8-569E-4995-B826-65437AAC26B9";            
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolArtifactId;
            PagesPerAgent = 2500;
        }

        //Determining the amount of OCR Workers needed is a just a matter 
        //of getting the count of pages from the Worker Queue
        public override List<AgentsDesiredObject> AgentsDesired()
        {
            int agentCount = 0;
            int imageCount = 0;
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                SELECT Count(O.[SetArtifactId]) 
                FROM   [OCRWorkerQueue] O 
                       INNER JOIN [Case] C 
                               ON O.[WorkspaceArtifactID] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            imageCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            if (imageCount > 0)
            {
                if ((imageCount / PagesPerAgent) < 1)
                {
                    agentCount += 1;
                }
                else
                {
                    agentCount += imageCount / PagesPerAgent;
                }
            }

            AgentsDesiredObject agentsDesired = new AgentsDesiredObject
            {
                Guid = Guid,
                Count = agentCount,
                RespectsResourcePool = RespectsResourcePool
            };

            outputList.Add(agentsDesired);
            return outputList;
        }
    }
}