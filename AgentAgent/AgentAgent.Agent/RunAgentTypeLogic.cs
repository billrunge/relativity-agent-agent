using System;
using System.Collections.Generic;
using AgentAgent.Agent.CustomAgentTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace AgentAgent.Agent
{
    class RunAgentTypeLogic
    {
        public List<AgentsPerPoolObject> AgentsPerPoolObject { get; private set; }
        private readonly IDBContext _eddsDbContext;

        public RunAgentTypeLogic(IDBContext eddsDbContext)
        {
           AgentsPerPoolObject = new List<AgentsPerPoolObject>();
            _eddsDbContext = eddsDbContext;

            Run();
        }

        private void Run() {

            //Create agent type objects
            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            BrandingManager brandMan = new BrandingManager(_eddsDbContext);
            CacheManager cacheMan = new CacheManager();
            CaseManager caseMan = new CaseManager();
            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager();
            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext);
            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext);
            ProductionManager prodMan = new ProductionManager(_eddsDbContext);
            ServerManager servMan = new ServerManager();

            //Run queue checking logic for all agent type objects
            AgentsPerPoolObject.AddRange(assistRevMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(brandMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(cacheMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(caseMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(caseStatsMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(clustUpWork.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(distJobMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(OcrSetMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(procMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(prodMan.DesiredAgentsPerPool());
            AgentsPerPoolObject.AddRange(servMan.DesiredAgentsPerPool());

        }

    }
}
