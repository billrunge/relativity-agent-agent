using System.Collections.Generic;
using Relativity.API;

namespace AgentAgent.Agent
{
    class GetAgentsDesiredList
    {
        public List<AgentsDesired> AgentsPerServerObjectList { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private readonly int _resourcePoolId;
        private readonly IEnvironmentHelper _environment;
        private readonly bool _isOffHours;

        public GetAgentsDesiredList(IDBContext eddsDbContext, IEnvironmentHelper environment, int resourcePoolId, bool isOffHours)
        {
           AgentsPerServerObjectList = new List<AgentsDesired>();
            _eddsDbContext = eddsDbContext;
            _resourcePoolId = resourcePoolId;
            _environment = environment;
            _isOffHours = isOffHours;
            Run();
        }

        private void Run() {

            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            AgentsPerServerObjectList.Add(assistRevMan.AgentsDesired());

            BrandingManager brandMan = new BrandingManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(brandMan.AgentsDesired());

            CacheManager cacheMan = new CacheManager(_isOffHours);
            AgentsPerServerObjectList.Add(cacheMan.AgentsDesired());

            CaseManager caseMan = new CaseManager(_isOffHours);
            AgentsPerServerObjectList.Add(caseMan.AgentsDesired());

            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager(_isOffHours);
            AgentsPerServerObjectList.Add(caseStatsMan.AgentsDesired());

            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            AgentsPerServerObjectList.Add(clustUpWork.AgentsDesired());

            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            AgentsPerServerObjectList.Add(distJobMan.AgentsDesired());

            FileDeletionManager fileDelMan = new FileDeletionManager(_isOffHours);
            AgentsPerServerObjectList.Add(fileDelMan.AgentsDesired());

            IntegrationPointsAgent RipAgent = new IntegrationPointsAgent(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(RipAgent.AgentsDesired());

            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(OcrSetMan.AgentsDesired());

            OCRWorker OcrWorker = new OCRWorker(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(OcrWorker.AgentsDesired());

            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(procMan.AgentsDesired());

            ProductionManager prodMan = new ProductionManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(prodMan.AgentsDesired());

            SearchTermsReportManager STRMan = new SearchTermsReportManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(STRMan.AgentsDesired());

            ServerManager servMan = new ServerManager();
            AgentsPerServerObjectList.Add(servMan.AgentsDesired());
        }
    }
}
