using System.Collections.Generic;
using Relativity.API;

namespace AgentAgent.Agent
{
    class GetAgentsDesiredList
    {
        public List<AgentsDesiredObject> AgentsPerServerObject { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private readonly int _resourcePoolId;
        private readonly IEnvironmentInformation _environment;
        private readonly bool _isOffHours;

        public GetAgentsDesiredList(IDBContext eddsDbContext, IEnvironmentInformation environment, int resourcePoolId, bool isOffHours)
        {
           AgentsPerServerObject = new List<AgentsDesiredObject>();
            _eddsDbContext = eddsDbContext;
            _resourcePoolId = resourcePoolId;
            _environment = environment;
            _isOffHours = isOffHours;
            Run();
        }

        private void Run() {

            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            AgentsPerServerObject.Add(assistRevMan.AgentsDesired());

            BrandingManager brandMan = new BrandingManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(brandMan.AgentsDesired());

            CacheManager cacheMan = new CacheManager(_isOffHours);
            AgentsPerServerObject.Add(cacheMan.AgentsDesired());

            CaseManager caseMan = new CaseManager(_isOffHours);
            AgentsPerServerObject.Add(caseMan.AgentsDesired());

            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager(_isOffHours);
            AgentsPerServerObject.Add(caseStatsMan.AgentsDesired());

            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            AgentsPerServerObject.Add(clustUpWork.AgentsDesired());

            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            AgentsPerServerObject.Add(distJobMan.AgentsDesired());

            FileDeletionManager fileDelMan = new FileDeletionManager(_isOffHours);
            AgentsPerServerObject.Add(fileDelMan.AgentsDesired());

            IntegrationPointsAgent RipAgent = new IntegrationPointsAgent(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(RipAgent.AgentsDesired());

            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(OcrSetMan.AgentsDesired());

            OCRWorker OcrWorker = new OCRWorker(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(OcrWorker.AgentsDesired());

            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(procMan.AgentsDesired());

            ProductionManager prodMan = new ProductionManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(prodMan.AgentsDesired());

            SearchTermsReportManager STRMan = new SearchTermsReportManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObject.Add(STRMan.AgentsDesired());

            ServerManager servMan = new ServerManager();
            AgentsPerServerObject.Add(servMan.AgentsDesired());
        }
    }
}
