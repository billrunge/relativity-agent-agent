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
        public List<AgentsDesired> AgentsPerPoolObject { get; private set; }
        public List<string> AllAgentGuids { get; private set; }
        private readonly IDBContext _eddsDbContext;

        public RunAgentTypeLogic(IDBContext eddsDbContext)
        {
           AgentsPerPoolObject = new List<AgentsDesired>();
            _eddsDbContext = eddsDbContext;
            Run();
        }

        private void Run() {

            //Create agent type objects
            AssistedReviewManager assistRevMan = new AssistedReviewManager(_eddsDbContext);
            AllAgentGuids.Add(assistRevMan.Guid);
            BrandingManager brandMan = new BrandingManager(_eddsDbContext);
            AllAgentGuids.Add(brandMan.Guid);
            CacheManager cacheMan = new CacheManager();
            AllAgentGuids.Add(cacheMan.Guid);
            CaseManager caseMan = new CaseManager();
            AllAgentGuids.Add(caseMan.Guid);
            CaseStatisticsManager caseStatsMan = new CaseStatisticsManager();
            AllAgentGuids.Add(caseStatsMan.Guid);
            ClusterUpgradeWorker clustUpWork = new ClusterUpgradeWorker(_eddsDbContext);
            AllAgentGuids.Add(clustUpWork.Guid);
            DistributedJobManager distJobMan = new DistributedJobManager(_eddsDbContext);
            AllAgentGuids.Add(distJobMan.Guid);
            OCRSetManager OcrSetMan = new OCRSetManager(_eddsDbContext);
            AllAgentGuids.Add(OcrSetMan.Guid);
            ProcessingSetManager procMan = new ProcessingSetManager(_eddsDbContext);
            AllAgentGuids.Add(procMan.Guid);
            ProductionManager prodMan = new ProductionManager(_eddsDbContext);
            AllAgentGuids.Add(prodMan.Guid);
            ServerManager servMan = new ServerManager();
            AllAgentGuids.Add(servMan.Guid);

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
