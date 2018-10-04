using System;
using System.Collections.Generic;
using AgentAgent.Agent.CustomAgentTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;
using AgentAgent.Agent.Objects;

namespace AgentAgent.Agent
{
    class RunAgentTypeLogic
    {
        public List<AgentsDesiredObject> AgentsPerServerObject { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private readonly int _resourcePoolId;

        public RunAgentTypeLogic(IDBContext eddsDbContext, int resourcePoolId)
        {
           AgentsPerServerObject = new List<AgentsDesiredObject>();
            _eddsDbContext = eddsDbContext;
            _resourcePoolId = resourcePoolId;
            Run();
        }

        private void Run() {

            //Create agent type objects
            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);

            BrandingManager brandMan = new BrandingManager(_eddsDbContext, _resourcePoolId);

            CacheManager cacheMan = new CacheManager(_resourcePoolId);

            CaseManager caseMan = new CaseManager(_resourcePoolId);
  
            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager();
 
            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);

            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);

            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext);

            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext);

            ProductionManager prodMan = new ProductionManager(_eddsDbContext);

            ServerManager servMan = new ServerManager();

            //Run queue checking logic for all agent type objects
            AgentsPerServerObject.AddRange(assistRevMan.AgentsDesiredObject());
            AgentsPerServerObject.AddRange(brandMan.AgentsDesiredObject());
            AgentsPerServerObject.AddRange(cacheMan.AgentsDesiredObject());
            AgentsPerServerObject.AddRange(caseMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(caseStatsMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(clustUpWork.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(distJobMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(OcrSetMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(procMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(prodMan.DesiredAgentsPerPool());
            AgentsPerServerObject.AddRange(servMan.DesiredAgentsPerPool());

        }

    }
}
