using AgentAgent.Agent.CustomAgentTypes;
using AgentAgent.Agent.Objects;
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
                IDBContext eddsDbContext = Helper.GetDBContext(-1);

                //Generate Environment Information object
                EnvironmentInformation environment = new EnvironmentInformation(eddsDbContext);

                //ResourcePoolArtifactID is hard coded for now, but will get pulled from an instance setting and then eventually from the UI
                int poolArtifactId = 1015040;

                //Adjustment factor is also hardcoded but will be an instance setting and then UI
                int adjFactor = 10;

                //Run all of the classes for agents to tell us how many agent spots they want per resource pool
                GetAgentsDesiredList desiredAgents = new GetAgentsDesiredList(eddsDbContext, poolArtifactId);
                List<AgentsDesiredObject> desiredAgentsList = desiredAgents.AgentsPerServerObject;

                //Use agents desired list helper to generate agent create and agent delete lists
                AgentsDesiredListHelper listHelper = new AgentsDesiredListHelper(desiredAgentsList);

                List<AgentsDesiredObject> createList = listHelper.GetAgentCreateList();
                List<AgentsDesiredObject> deleteList = listHelper.GetAgentDeleteList();

                //Get available agent spots per server list
                GetSpotsPerServerList getSpotsPerServer = new GetSpotsPerServerList(eddsDbContext, adjFactor, poolArtifactId);
                List<SpotsPerServerObject> spotsPerServerList = new List<SpotsPerServerObject>();
                spotsPerServerList = getSpotsPerServer.SpotsPerServerList;

                //Create and run agent create object
                RunAgentCreate agentCreate = new RunAgentCreate(eddsDbContext, environment, createList, spotsPerServerList);
                agentCreate.Run();

                //Create and run agent delete object
                RunAgentDelete agentDelete = new RunAgentDelete(eddsDbContext, environment, deleteList);

            }
            catch (Exception ex)
            {
                //Your Agent caught an exception
                RaiseError(ex.Message, ex.Message);
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