﻿using kCura.Agent;
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
                //Adding message so time of last check-in updates
                this.RaiseMessage("Managing the agents.", 10);

                //ResourcePoolArtifactID is hard coded for now (Default pool), but will get pulled from an instance setting and then eventually from the UI
                int poolArtifactId = 1015040;

                //Adjustment factor is also hardcoded but will be an instance setting and then UI
                int adjFactor = 10;

                logger.LogDebug("Getting EDDS database context");
                IAgentHelper helper = Helper;
                IDBContext eddsDbContext = helper.GetDBContext(-1);

                logger.LogDebug("Generating new EnvironmentInformation object");
                EnvironmentHelper environment = new EnvironmentHelper(eddsDbContext);

                //Run all of the classes for agents to tell us how many agent spots they want per resource pool
                logger.LogDebug("Creating GetAgentsDesiredList object");
                GetAgentsDesiredList desiredAgents = new GetAgentsDesiredList(eddsDbContext, environment, poolArtifactId, IsOffHours());
                logger.LogDebug("Getting AgentsDesiredList from GetAgentsDesiredList object");
                List<AgentsDesired> desiredAgentsList = desiredAgents.AgentsPerServerObjectList;

                //Create CompareDesiredAgentsToExisting Object and run it
                logger.LogDebug("Creating CompareDesiredAgentsToExisting object");
                CompareDesiredAgentsToExisting compare = new CompareDesiredAgentsToExisting(desiredAgentsList, poolArtifactId, environment, logger);
                List<AgentsDesired> comparedList = compare.Compare();                           

                //Use agents desired list helper to generate agent create and agent delete lists
                logger.LogDebug("Generate list helper object");
                AgentsDesiredListHelper listHelper = new AgentsDesiredListHelper(comparedList);

                //Create a create list
                logger.LogDebug("Generate create list");
                List<AgentsDesired> createList = listHelper.GetAgentCreateList();

                string listString = "Create List: ";
                foreach (AgentsDesired cL in createList)
                {
                    listString += string.Format("{0} - {1} - {2}\r\n", cL.Guid, cL.Count, cL.RespectsResourcePool);
                }
                logger.LogDebug(listString);

                //Create a delete list
                logger.LogDebug("Generate delete list");
                List<AgentsDesired> deleteList = listHelper.GetAgentDeleteList();

                listString = "Delete List: ";
                foreach (AgentsDesired dL in deleteList)
                {
                    listString += string.Format("{0} - {1} - {2}\r\n", dL.Guid, dL.Count, dL.RespectsResourcePool);
                }
                logger.LogDebug(listString);

                //Get available agent spots per server list
                logger.LogDebug("Generate spots per server object");
                GetSpotsPerServerList getSpotsPerServer = new GetSpotsPerServerList(eddsDbContext, environment, adjFactor, poolArtifactId);                

                logger.LogDebug("Generate spots per server list and populate it with the spots per server object");
                List<SpotsPerServerObject> spotsPerServerList = getSpotsPerServer.SpotsPerServerList;

                listString = "Spots Per Server List: ";
                foreach (SpotsPerServerObject sP in spotsPerServerList)
                {
                    listString += string.Format("{0} - {1}\r\n", sP.AgentServerArtifactId, sP.Spots);
                }
                logger.LogDebug(listString);

                //Create and run agent create object
                logger.LogDebug("Generate agent create object");
                RunAgentCreate agentCreate = new RunAgentCreate(eddsDbContext, environment, createList, spotsPerServerList, logger);
                logger.LogDebug("Run agent create logic");
                agentCreate.Run();

                //Create and run agent delete object
                logger.LogDebug("Generate agent delete object");
                RunAgentDelete agentDelete = new RunAgentDelete(eddsDbContext, environment, poolArtifactId, deleteList, logger);
                logger.LogDebug("Run agent delete logic");
                agentDelete.Run();                            


            }
            catch (Exception ex)
            {
                //Your Agent caught an exception
                string fullError = string.Format("Message: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex.InnerException, ex.StackTrace);

                RaiseError(ex.Message, fullError);
                logger.LogError(fullError);
                this.RaiseMessage("There has been an error. See log for more details", 1);
                this.RaiseError(ex.Message, fullError);
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