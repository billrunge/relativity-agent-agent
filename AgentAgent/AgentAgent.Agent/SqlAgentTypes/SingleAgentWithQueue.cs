using Relativity.API;

namespace AgentAgent.Agent.SqlAgentTypes
{
    class SingleAgentWithQueue
    {
        private readonly IDBContext _eddsDbContext;
        private readonly IEnvironmentHelper _environment;
        private readonly string _eddsQueueTable;

        public SingleAgentWithQueue(IDBContext eddsDbContext, IEnvironmentHelper environment, string eddsQueuTable)
        {
            _eddsDbContext = eddsDbContext;
            _environment = environment;


        }

        public int GetAgentsDesiredList()
        {
            string SQL = string.Format(@"
                SELECT Count(*) 
                FROM   [{0}] T 
                       INNER JOIN [Case] C WITH(NOLOCK) 
                               ON T.[{1}] = C.[ArtifactID] 
                WHERE  C.[ResourceGroupArtifactID] = @ResourceGroupArtifactID", );

        }
    }
}
