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
        private readonly int _resourcePoolId;
        private readonly bool _isOffHours;

        public GetAgentsDesiredList(IAgentHelper agentHelper, IEnvironmentHelper environment, int resourcePoolId, bool isOffHours)
        {
           AgentsPerServerObjectList = new List<AgentsDesired>();
            _agentHelper = agentHelper;
            _eddsDbContext = _agentHelper.GetDBContext(-1);
            _resourcePoolId = resourcePoolId;
            _environment = environment;
            _isOffHours = isOffHours;
            Run();
        }

        private void Run() {
            
            //There has to be a better way to do this. Perhaps with a delegate?

            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            AgentsPerServerObjectList.Add(assistRevMan.GetAgentsDesired());

            BrandingManager brandMan = new BrandingManager(_eddsDbContext, _resourcePoolId);
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

            DtSearchIndexJobManager dtJobMan = new DtSearchIndexJobManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(dtJobMan.GetAgentsDesired());

            DtSearchIndexWorker dtJobWorker = new DtSearchIndexWorker(_agentHelper, _resourcePoolId);
            AgentsPerServerObjectList.Add(dtJobWorker.GetAgentsDesired());

            FileDeletionManager fileDelMan = new FileDeletionManager(_isOffHours);
            AgentsPerServerObjectList.Add(fileDelMan.GetAgentsDesired());

            IntegrationPointsAgent RipAgent = new IntegrationPointsAgent(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(RipAgent.GetAgentsDesired());

            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(OcrSetMan.GetAgentsDesired());

            OCRWorker OcrWorker = new OCRWorker(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(OcrWorker.GetAgentsDesired());

            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(procMan.GetAgentsDesired());

            ProductionManager prodMan = new ProductionManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(prodMan.GetAgentsDesired());

            SearchTermsReportManager STRMan = new SearchTermsReportManager(_eddsDbContext, _resourcePoolId);
            AgentsPerServerObjectList.Add(STRMan.GetAgentsDesired());

            ServerManager servMan = new ServerManager();
            AgentsPerServerObjectList.Add(servMan.GetAgentsDesired());
        }
    }
}
