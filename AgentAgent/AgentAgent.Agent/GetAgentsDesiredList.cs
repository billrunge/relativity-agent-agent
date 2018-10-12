using System.Collections.Generic;
using Relativity.API;

namespace AgentAgent.Agent
{
    class GetAgentsDesiredList
    {
        public List<AgentsDesired> AgentsPerServerObjectList { get; private set; }
        private readonly IDBContext _eddsDbContext;
        private IAgentHelper _agentHelper;
        private readonly IEnvironmentHelper _environment;
        private readonly int _poolId;
        private readonly bool _isOffHours;

        public GetAgentsDesiredList(IAgentHelper agentHelper, IEnvironmentHelper environment, int poolId, bool isOffHours)
        {
           AgentsPerServerObjectList = new List<AgentsDesired>();
            _agentHelper = agentHelper;
            _eddsDbContext = _agentHelper.GetDBContext(-1);
            _poolId = poolId;
            _environment = environment;
            _isOffHours = isOffHours;
            Run();
        }

        private void Run() {

            /* There has to be a better way to do this. Perhaps with a delegate? */

            ApplicationInstallationManager appInstMan = new ApplicationInstallationManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(appInstMan.GetAgentsDesired());

            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            AgentsPerServerObjectList.Add(assistRevMan.GetAgentsDesired());

            AutoBatchManager autoBatMan = new AutoBatchManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(autoBatMan.GetAgentsDesired());

            BrandingManager brandMan = new BrandingManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(brandMan.GetAgentsDesired());

            CacheManager cacheMan = new CacheManager(_isOffHours);
            AgentsPerServerObjectList.Add(cacheMan.GetAgentsDesired());

            CaseManager caseMan = new CaseManager(_isOffHours);
            AgentsPerServerObjectList.Add(caseMan.GetAgentsDesired());

            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager(_isOffHours);
            AgentsPerServerObjectList.Add(caseStatsMan.GetAgentsDesired());

            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            AgentsPerServerObjectList.Add(clustUpWork.GetAgentsDesired());

            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            AgentsPerServerObjectList.Add(distJobMan.GetAgentsDesired());

            DtSearchIndexJobManager dtJobMan = new DtSearchIndexJobManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(dtJobMan.GetAgentsDesired());

            DtSearchIndexWorker dtJobWorker = new DtSearchIndexWorker(_agentHelper, _poolId);
            AgentsPerServerObjectList.Add(dtJobWorker.GetAgentsDesired());

            FileDeletionManager fileDelMan = new FileDeletionManager(_isOffHours);
            AgentsPerServerObjectList.Add(fileDelMan.GetAgentsDesired());

            IntegrationPointsAgent RipAgent = new IntegrationPointsAgent(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(RipAgent.GetAgentsDesired());

            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(OcrSetMan.GetAgentsDesired());

            OCRWorker OcrWorker = new OCRWorker(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(OcrWorker.GetAgentsDesired());

            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(procMan.GetAgentsDesired());

            ProductionManager prodMan = new ProductionManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(prodMan.GetAgentsDesired());

            RelativityAnalyticsCategorizationManager relAnalyticsCatMan = new RelativityAnalyticsCategorizationManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(relAnalyticsCatMan.GetAgentsDesired());

            RelativityAnalyticsClusterManager relAnalyticsClusterMan = new RelativityAnalyticsClusterManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(relAnalyticsClusterMan.GetAgentsDesired());

            RelativityAnalyticsIndexManager relAnalyticsIndexMan = new RelativityAnalyticsIndexManager(_eddsDbContext, _environment, _poolId);
            AgentsPerServerObjectList.Add(relAnalyticsIndexMan.GetAgentsDesired());

            RelativityAnalyticsIndexProgressManager relAnalyticsIndexProgMan = new RelativityAnalyticsIndexProgressManager(_eddsDbContext, _environment, _poolId);
            AgentsPerServerObjectList.Add(relAnalyticsIndexProgMan.GetAgentsDesired());

            SearchTermsReportManager STRMan = new SearchTermsReportManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(STRMan.GetAgentsDesired());

            ServerManager servMan = new ServerManager();
            AgentsPerServerObjectList.Add(servMan.GetAgentsDesired());

            TelemetryMetricsTransmissionAgent telMetTransAgent = new TelemetryMetricsTransmissionAgent();
            AgentsPerServerObjectList.Add(telMetTransAgent.GetAgentsDesired());

            TextExtractionManager textExMan = new TextExtractionManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(textExMan.GetAgentsDesired());

            TranscriptManager transcriptMan = new TranscriptManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(transcriptMan.GetAgentsDesired());

            TransformSetManager transformMan = new TransformSetManager(_eddsDbContext, _poolId);
            AgentsPerServerObjectList.Add(transformMan.GetAgentsDesired());

        }
    }
}
