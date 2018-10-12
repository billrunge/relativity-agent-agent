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
                /* Adding message so time of last check-in updates */
                RaiseMessage("Managing the agents.", 10);

                /* Resource Pool ArtifactID is hard coded for now (Default pool)
                 * Will eventually make this accessible in the UI */
                int poolId = 1015040;

                /* Agent Server Adjustment factor. 
                 * Will eventually make this accessible in the UI */
                int adjFactor = 10;

                /* This switch tells Agent Agent to not create any agents on a server containing the dtSearch search agent. 
                 * Will eventually make this accessible in the UI*/
                bool ignoreSearchServer = true;

                /*Use API to create agents instead of manually creating in SQL*/
                bool useApiCreate = true;

                /* Making stuff */
                logger.LogVerbose("Creating new {objectName}", "Agent Helper");
                IAgentHelper agentHelper = Helper;

                logger.LogVerbose("Creating new {objectName}", "EDDS DB Context");
                IDBContext eddsDbContext = agentHelper.GetDBContext(-1);

                logger.LogVerbose("Creating new {objectName}", "Environment Helper");
                EnvironmentHelper environment = new EnvironmentHelper(eddsDbContext);

                logger.LogVerbose("Creating new {objectName}", "Desired Agents List");
                //List<AgentsDesired> desiredAgentsList = new GetAgentsDesiredList(agentHelper, environment, poolId, IsOffHours()).AgentsPerServerObjectList;
                List<AgentsDesired> desiredAgentsList = new GetSqlAgentsDesiredList(agentHelper, environment, poolId, IsOffHours()).AgentsDesiredList;


                logger.LogVerbose("Creating new {objectName}", "Compared List");
                List<AgentsDesired> comparedList = new CompareDesiredAgentsToExisting(desiredAgentsList, poolId, environment, logger).Compare();

                logger.LogVerbose("Creating new {objectName}", "List Helper");
                AgentsDesiredListHelper listHelper = new AgentsDesiredListHelper(comparedList);

                logger.LogVerbose("Creating new {objectName}", "Create List");
                List<AgentsDesired> createList = listHelper.GetAgentCreateList();

                logger.LogVerbose("Creating new {objectName}", "Delete List");
                List<AgentsDesired> deleteList = listHelper.GetAgentDeleteList();

                logger.LogVerbose("Creating new {objectName}", "Spots Per Server List");
                List<SpotsPerServer> spotsPerServerList = new GetSpotsPerServerList(eddsDbContext, environment, adjFactor, poolId, ignoreSearchServer).SpotsPerServerList;

                logger.LogVerbose("Creating {objectName}", "Agent Create");
                ICreateAgent createAgent = new CreateAgentSql(eddsDbContext, environment);
                if (useApiCreate)
                {
                    createAgent = new CreateAgentApi(eddsDbContext, environment, agentHelper);
                }
                
                logger.LogVerbose("Creating {objectName}", "Agent Delete");
                IDeleteAgent deleteAgent = new DeleteAgent(eddsDbContext);


                /* Log createList, deleteList, and spotsPerServerList contents */
                string createListString = "";
                string deleteListString = "";
                string spotsPerServerListString = "";

                foreach (AgentsDesired cL in createList)
                {
                    createListString += string.Format("{0} - {1} - {2}\r\n", cL.Guid, cL.Count, cL.RespectsResourcePool);
                }

                foreach (AgentsDesired dL in deleteList)
                {
                    deleteListString += string.Format("{0} - {1} - {2}\r\n", dL.Guid, dL.Count, dL.RespectsResourcePool);
                }

                foreach (SpotsPerServer sP in spotsPerServerList)
                {
                    spotsPerServerListString += string.Format("{0} - {1}\r\n", sP.AgentServerArtifactId, sP.Spots);
                }

                logger.LogDebug("Delete List : {deleteList}", deleteListString);
                logger.LogDebug("Create List: {createList}", createListString);
                logger.LogDebug("Spots Per Server List = {spotsPerServerList}", spotsPerServerListString);


                /* Create */
                RunAgentCreate agentCreate = new RunAgentCreate(eddsDbContext, environment, createAgent, createList, spotsPerServerList, logger);
                logger.LogVerbose("Running {objectName}", "Agent Create");
                agentCreate.Run();
                logger.LogVerbose("Completed {objectName}", "Agent Create");
                

                /* Delete */
                RunAgentDelete agentDelete = new RunAgentDelete(eddsDbContext, environment, deleteAgent, poolId, deleteList, logger);
                logger.LogVerbose("Running {objectName}", "Agent Delete");
                agentDelete.Run();
                logger.LogVerbose("Completed {objectName}", "Agent Delete");

            }
            catch (Exception ex)
            {
                RaiseError(ex.Message, ex.StackTrace);
                logger.LogError("Exception during agent Execute(): {ex}", ex);
                RaiseMessage("There has been an error. See log for more details", 1);
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