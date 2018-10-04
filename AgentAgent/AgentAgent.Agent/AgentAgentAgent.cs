using AgentAgent.Agent.CustomAgentTypes;
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

                //Run all of the classes for agents to tell us how many agent spots they want per resource pool
                RunAgentTypeLogic agentTypeLogic = new RunAgentTypeLogic(eddsDbContext);

                //Get a list of the agentGuids that Agent Agent manages
                List<string> agentGuidList = agentTypeLogic.AllAgentGuids;

                //Use that list to determine how many of each of these agents exist per resource pool


                //Get a list of all spots desired for each agent type by Resource Pool
                List<AgentsDesiredObject> desiredAgentList = agentTypeLogic.AgentsDesiredObjectObject;







                List<AgentsDesiredObject> AgentsDesiredObjectObject = new List<AgentsDesiredObject>();
                RunAgentTypeLogic runAgentLogic = new RunAgentTypeLogic(eddsDbContext);
                AgentsDesiredObjectObject = runAgentLogic.AgentsDesiredObjectObject;





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