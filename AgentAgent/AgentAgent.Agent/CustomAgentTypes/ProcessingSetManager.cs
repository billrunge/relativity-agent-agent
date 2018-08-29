using Relativity.API;

namespace AgentAgent.Agent.CustomAgentTypes
{
    class ProcessingSetManager : AgentType
    {
        private IDBContext _eddsDbContext;

        //The Processing set manager is a one agent per resource pool agent
        //Which makes determining the amount of agents desired fairly easy
        public ProcessingSetManager(IDBContext eddsDbContext)
        {
            _eddsDbContext = eddsDbContext;
            AgentTypeName = "Processing Set Manager";
            Guid = "8326948B-32E1-4911-AC08-DA9130D38AF1";
        }

        public override int DesiredAgentCount()
        {
            string SQL = @"
                SELECT COUNT(*)
                FROM [ProcessingSetQueue]";

            int queueDepth = _eddsDbContext.ExecuteSqlStatementAsScalar<int>(SQL);

            if (queueDepth > 0)
            {
                return 1;
            }
            else {
                return 0;
            }

        }
    }
}
