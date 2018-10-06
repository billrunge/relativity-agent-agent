using kCura.Agent;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace AgentAgent.Agent
{
    [kCura.Agent.CustomAttributes.Name("Agent Agent Agent")]
    [System.Runtime.InteropServices.Guid("0c1f7839-e571-497f-ba87-32bdd899953b")]
    public class AgentAgentAgent : AgentBase
    {
        /// <summary>
        /// Main entry point for agent logic
        /// </summary>
        public override void Execute()
        {
            IAPILog logger = Helper.GetLoggerFactory().GetLogger();
            try
            {
                //Get a dbContext for the EDDS database
                logger.LogVerbose("Getting EDDS database context");
                IDBContext eddsDbContext = Helper.GetDBContext(-1);

                //Generate Environment Information object
                logger.LogVerbose("Generating new EnvironmentInformation object");
                EnvironmentInformation environment = new EnvironmentInformation(eddsDbContext);

                //ResourcePoolArtifactID is hard coded for now, but will get pulled from an instance setting and then eventually from the UI
                int poolArtifactId = 1015040;

                //Adjustment factor is also hardcoded but will be an instance setting and then UI
                int adjFactor = 10;

                //Run all of the classes for agents to tell us how many agent spots they want per resource pool
                logger.LogVerbose("Creating GetAgentsDesiredList object");
                GetAgentsDesiredList desiredAgents = new GetAgentsDesiredList(eddsDbContext, poolArtifactId);
                logger.LogVerbose("Getting AgentsDesiredList from GetAgentsDesiredList object");
                List<AgentsDesiredObject> desiredAgentsList = desiredAgents.AgentsPerServerObject;

                //Create CompareDesiredAgentsToExisting Object and run it
                logger.LogVerbose("Creating CompareDesiredAgentsToExisting object");
                CompareDesiredAgentsToExisting compare = new CompareDesiredAgentsToExisting(desiredAgentsList, poolArtifactId, environment, logger);
                List<AgentsDesiredObject> comparedList = compare.Compare();                           

                //Use agents desired list helper to generate agent create and agent delete lists
                logger.LogVerbose("Generate list helper object");
                AgentsDesiredListHelper listHelper = new AgentsDesiredListHelper(comparedList);

                //Create a create list
                logger.LogVerbose("Generate create list");
                List<AgentsDesiredObject> createList = listHelper.GetAgentCreateList();

                string listString = "Create List: ";
                foreach (AgentsDesiredObject cL in createList)
                {
                    listString += string.Format("{0} - {1} - {2} | ", cL.Guid, cL.Count, cL.RespectsResourcePool);
                }
                logger.LogVerbose(listString);

                //Create a delete list
                logger.LogVerbose("Generate delete list");
                List<AgentsDesiredObject> deleteList = listHelper.GetAgentDeleteList();

                listString = "Delete List: ";
                foreach (AgentsDesiredObject dL in deleteList)
                {
                    listString += string.Format("{0} - {1} - {2} | ", dL.Guid, dL.Count, dL.RespectsResourcePool);
                }
                logger.LogVerbose(listString);

                //Get available agent spots per server list
                logger.LogVerbose("Generate spots per server object");
                GetSpotsPerServerList getSpotsPerServer = new GetSpotsPerServerList(eddsDbContext, adjFactor, poolArtifactId);                

                logger.LogVerbose("Generate spots per server list and populate it with the spots per server object");
                List<SpotsPerServerObject> spotsPerServerList = getSpotsPerServer.SpotsPerServerList;

                listString = "Spots Per Server List: ";
                foreach (SpotsPerServerObject sP in spotsPerServerList)
                {
                    listString += string.Format("{0} - {1} | ", sP.AgentServerArtifactId, sP.Spots);
                }
                logger.LogVerbose(listString);

                //Create and run agent create object
                logger.LogVerbose("Generate agent create object");
                RunAgentCreate agentCreate = new RunAgentCreate(eddsDbContext, environment, createList, spotsPerServerList, logger);
                logger.LogVerbose("Run agent create logic");
                agentCreate.Run();

                //Create and run agent delete object
                logger.LogVerbose("Generate agent delete object");
                RunAgentDelete agentDelete = new RunAgentDelete(eddsDbContext, environment, poolArtifactId ,deleteList);
                logger.LogVerbose("Run agent delete logic");
                agentDelete.Run();


            }
            catch (Exception ex)
            {
                //Your Agent caught an exception
                RaiseError(ex.Message, string.Format("StackTrace: {0}, InnerException: {1}, Source: {2}, TargetSite: {3}", ex.StackTrace, ex.InnerException, ex.Source, ex.TargetSite));
                logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Returns the name of agent
        /// </summary>
        public override string Name
        {
            get
            {
                return "Agent Agent Agent";
            }



        }
        
    }
}