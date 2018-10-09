using Relativity.API;
using System.Data.SqlClient;

namespace AgentAgent.Agent
{
    class ApplicationInstallationManager : AgentType
    {
        private readonly IDBContext _eddsDbContext;

        public ApplicationInstallationManager(IDBContext eddsDbContext, int poolId)
        {
            _eddsDbContext = eddsDbContext;
            Guid = "05DEFFFD-085F-47FA-8519-F155987E5A97";
            RespectsResourcePool = true;
            AgentAgentResourcePool = poolId;
        }

        public override AgentsDesired GetAgentsDesired()
        {
            int agentCount = 0;

            //ApplicationInstall statuses we are excluding;
            //    Errors = 1
            //    OutOfDate = 4
            //    Modified = 5
            //    Installed = 6
            //    Cancelled = 7
            //    Uninstalled = 10
            //    ErrorInUninstall = 11

            string SQL = @"
                SELECT Count(AI.[ApplicationInstallID]) 
                FROM   [ApplicationInstall] AI WITH(NOLOCK) 
                       INNER JOIN [CaseApplication] CA WITH(NOLOCK) 
                               ON AI.[CaseApplicationID] = CA.[CaseApplicationID] 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON CA.[CaseID] = C.[ArtifactID] 
                WHERE  AI.[Status] NOT IN ( 1, 4, 5, 6, 
                                            7, 10, 11 ) 
                       AND C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID";

            SqlParameter poolIdParam = new SqlParameter("@ResourceGroupArtifactID", System.Data.SqlDbType.Int)
            {
                Value = AgentAgentResourcePool
            };

            agentCount += _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL, new SqlParameter[] { poolIdParam });

            return new AgentsDesired
            {
                Guid = Guid,
                RespectsResourcePool = RespectsResourcePool,
                Count = agentCount
            };
        }
    }
}
