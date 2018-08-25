using kCura.Agent;
using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Objects;
using System;

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

                //Temporary test code to create an OCR worker
                //All we need at this point to create an agent is EDDS db context,
                //The agent type GUID, and a server artifact ID
                CreateAgent agentCreator = new CreateAgent(eddsDbContext, "9541B7E8-569E-4995-B826-65437AAC26B9", 1016713);

                agentCreator.Create();


                logger.LogVerbose("Log information throughout execution.");
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