using System.Collections.Generic;
using AgentAgent.Agent.CustomAgentTypes;
using Relativity.API;

namespace AgentAgent.Agent
{
    class GetAgentsDesiredList
    {
        public List<AgentsDesiredObject> AgentsPerServerObject { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private readonly int _resourcePoolId;

        public GetAgentsDesiredList(IDBContext eddsDbContext, int resourcePoolId)
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
            CacheManager cacheMan = new CacheManager();
            CaseManager caseMan = new CaseManager();  
            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager(); 
            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext, _resourcePoolId);
            OCRWorker OcrWorker = new OCRWorker(_eddsDbContext, _resourcePoolId);
            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext, _resourcePoolId);
            ProductionManager prodMan = new ProductionManager(_eddsDbContext, _resourcePoolId);
            ServerManager servMan = new ServerManager();

            //Run queue checking logic for all agent type objects

            AgentsPerServerObject.AddRange(assistRevMan.AgentsDesired());
            AgentsPerServerObject.AddRange(brandMan.AgentsDesired());
            AgentsPerServerObject.AddRange(cacheMan.AgentsDesired());
            AgentsPerServerObject.AddRange(caseMan.AgentsDesired());
            AgentsPerServerObject.AddRange(caseStatsMan.AgentsDesired());
            AgentsPerServerObject.AddRange(clustUpWork.AgentsDesired());
            AgentsPerServerObject.AddRange(distJobMan.AgentsDesired());
            AgentsPerServerObject.AddRange(OcrSetMan.AgentsDesired());
            AgentsPerServerObject.AddRange(OcrWorker.AgentsDesired());
            AgentsPerServerObject.AddRange(procMan.AgentsDesired());
            AgentsPerServerObject.AddRange(prodMan.AgentsDesired());
            AgentsPerServerObject.AddRange(servMan.AgentsDesired());
        }

    }
}
