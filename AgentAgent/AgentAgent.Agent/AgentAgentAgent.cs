using kCura.Agent;
using Relativity.API;
using System;
using System.Collections.Generic;
using AgentAgent.Agent.Interfaces;

namespace AgentAgent.Agent
{
    [kCura.Agent.CustomAttributes.Name("Agent Agent Agent")]
    [System.Runtime.InteropServices.Guid("0c1f7839-e571-497f-ba87-32bdd899953b")]
    public class AgentAgentAgent : AgentBase
    {
        public override void Execute()
        {
            IAPILog logger = Helper.GetLoggerFactory().GetLogger();
            try
            {

                //Adding message so time of last check-in updates
                this.RaiseMessage("Managing the agents.", 10);

                //ResourcePoolArtifactID is hard coded for now (Default pool), but will get pulled from an instance setting and then eventually from the UI
                int poolArtifactId = 1015040;

                //Adjustment factor is also hardcoded but will be an instance setting and then UI
                int adjFactor = 10;

                //This switch tells Agent Agent to not create any agents on a server containing the dtSearch search agent
                //will eventually make this accessible in the UI
                bool ignoreSearchServer = true;

                //Use API to create agents instead of manually creating in SQL
                bool useApiCreate = false;

                logger.LogVerbose("Creating new {objectName}", "Agent Helper");
                IAgentHelper agentHelper = Helper;

                logger.LogVerbose("Creating new {objectName}", "EDDS DB Context");
                IDBContext eddsDbContext = agentHelper.GetDBContext(-1);

                logger.LogVerbose("Creating new {objectName}", "Environment Helper");
                EnvironmentHelper environment = new EnvironmentHelper(eddsDbContext);

                logger.LogVerbose("Creating new {objectName}", "Desired Agents List");
                List<AgentsDesired> desiredAgentsList = new GetAgentsDesiredList(agentHelper, environment, poolArtifactId, IsOffHours()).AgentsPerServerObjectList;

                logger.LogVerbose("Creating new {objectName}", "Compared List");
                List<AgentsDesired> comparedList = new CompareDesiredAgentsToExisting(desiredAgentsList, poolArtifactId, environment, logger).Compare();

                logger.LogVerbose("Creating new {objectName}", "List Helper");
                AgentsDesiredListHelper listHelper = new AgentsDesiredListHelper(comparedList);

                logger.LogVerbose("Creating new {objectName}", "Create List");
                List<AgentsDesired> createList = listHelper.GetAgentCreateList();

                string createListString = "";
                foreach (AgentsDesired cL in createList)
                {
                    createListString += string.Format("{0} - {1} - {2}\r\n", cL.Guid, cL.Count, cL.RespectsResourcePool);
                }
                logger.LogDebug("Create List: {createList}", createListString);
                
                logger.LogVerbose("Creating new {objectName}", "Delete List");
                List<AgentsDesired> deleteList = listHelper.GetAgentDeleteList();

                string deleteListString = "";
                foreach (AgentsDesired dL in deleteList)
                {
                    deleteListString += string.Format("{0} - {1} - {2}\r\n", dL.Guid, dL.Count, dL.RespectsResourcePool);
                }
                logger.LogDebug("Delete List : {deleteList}", deleteListString);                

                logger.LogVerbose("Creating new {objectName}", "Spots Per Server List");
                List<SpotsPerServer> spotsPerServerList = new GetSpotsPerServerList(eddsDbContext, environment, adjFactor, poolArtifactId, ignoreSearchServer).SpotsPerServerList;

                string spotsPerServerListString = "";
                foreach (SpotsPerServer sP in spotsPerServerList)
                {
                    spotsPerServerListString += string.Format("{0} - {1}\r\n", sP.AgentServerArtifactId, sP.Spots);
                }
                logger.LogDebug("Spots Per Server List = {spotsPerServerList}", spotsPerServerListString);

                logger.LogVerbose("Creating {objectName}", "Agent Create");

                ICreateAgent createAgent = new CreateAgentSql(eddsDbContext, environment);

                if (useApiCreate)
                {
                    createAgent = new CreateAgentApi(eddsDbContext, environment);
                }

                RunAgentCreate agentCreate = new RunAgentCreate(eddsDbContext, environment, createAgent, createList, spotsPerServerList, logger);
                logger.LogVerbose("Running {objectName}", "Agent Create");
                agentCreate.Run();
                logger.LogVerbose("Completed {objectName}", "Agent Create");

                logger.LogVerbose("Creating {objectName}", "Agent Delete");

                RunAgentDelete agentDelete = new RunAgentDelete(eddsDbContext, environment, poolArtifactId, deleteList, logger);
                logger.LogVerbose("Running {objectName}", "Agent Delete");
                agentDelete.Run();
                logger.LogVerbose("Completed {objectName}", "Agent Delete");

            }
            catch (Exception ex)
            {
                string fullError = string.Format("Message: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex.InnerException, ex.StackTrace);
                RaiseError(ex.Message, fullError);
                logger.LogError("Exception during agent Execute(): {ex}", ex);
                this.RaiseMessage("There has been an error. See log for more details", 1);
                this.RaiseError(ex.Message, fullError);
            }
        }

        public override string Name
        {
            get
            {
                return "Agent Agent Agent";
            }
        }        
    }
}