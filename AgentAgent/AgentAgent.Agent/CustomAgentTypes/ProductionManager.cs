using AgentAgent.Agent.Objects;
using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProductionManager : AgentType
    {
        private IDBContext _eddsDbContext;

        public ProductionManager(IDBContext eddsDbContext, int poolArtifactId)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Production Manager";
            Guid = "916CF88F-F8D0-4C65-9ECC-1BBFDF5E1515";
            AlwaysNeeded = false;
            OffHoursAgent = false;
            MaxPerInstance = 0;
            MaxPerResourcePool = 1;
            AgentAgentResourcePool = poolArtifactId;
            RespectsResourcePool = true;
            UsesEddsQueue = true;
            EddsQueueName = "ProductionSetQueue";
        }

        //Production managers are an at least one agent per resource pool, but only one manager per job
        //so the ideal situation here is one manager per job in the queue. 

        public override List<AgentsDesiredObject> AgentsDesired()
        {
            List<AgentsDesiredObject> outputList = new List<AgentsDesiredObject>();

            string SQL = @"
                SELECT Count(P.[SetQueueId]) 
                FROM   [ProductionSetQueue] P 
                       INNER JOIN [Case] C 
                               ON P.[WorkspaceArtifactId] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter resourcePoolArtifactIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Char)
            {
                Value = AgentAgentResourcePool
            };

            int jobCount = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { resourcePoolArtifactIdParam });

            AgentsDesiredObject agentsDesiredObject = new AgentsDesiredObject()
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = jobCount
            };

            outputList.Add(agentsDesiredObject);

            return outputList;
        }
    }

}

