using System.Collections.Generic;
using System.Data;
using Relativity.API;

namespace AgentAgent.Agent
{
    class GetSqlAgentsDesiredList
    {
        public List<AgentsDesired> AgentsDesiredList { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private IAgentHelper _agentHelper;
        private readonly IEnvironmentHelper _environment;
        private readonly int _resourcePoolId;
        private readonly bool _isOffHours;

        public GetSqlAgentsDesiredList(IAgentHelper agentHelper, IEnvironmentHelper environment, int resourcePoolId, bool isOffHours)
        {
            AgentsDesiredList = new List<AgentsDesired>();
            _agentHelper = agentHelper;
            _eddsDbContext = _agentHelper.GetDBContext(-1);
            _resourcePoolId = resourcePoolId;
            _environment = environment;
            _isOffHours = isOffHours;
            Run();
        }

        private void Run()
        {

            /* There has to be a better way to do this. Perhaps with a delegate? */

            OffHoursAgents();


        }

        private void OffHoursAgents()
        {
            string SQL = @"
                SELECT [Guid], 
                       [RespectsResourcePool] 
                FROM   [AgentAgent] 
                WHERE  [OffHoursAgent] = 1";

            DataTable results = _eddsDbContext.ExecuteSqlStatementAsDataTable(SQL);

            if (results.Rows.Count > 0)
            {
                foreach (DataRow row in results.Rows)
                {
                    int agentCount = 0;
                    bool respectsResourcePool = false;

                    string guid = row["Guid"].ToString();
                    if (row["RespectsResourcePool"].ToString() == "1")
                    {
                        respectsResourcePool = true;
                    }

                    if (_isOffHours)
                    {
                        agentCount = 1;
                    }

                    AgentsDesiredList.Add(new AgentsDesired()
                    {
                        Guid = guid,
                        Count = agentCount,
                        RespectsResourcePool = respectsResourcePool
                    });

                }

            }


        }



    }
}
