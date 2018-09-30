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

                //EnvironmentInformation environmentInformation = new EnvironmentInformation(eddsDbContext);
                //CreateAgent agentCreator = new CreateAgent(eddsDbContext, environmentInformation, procManagerType.Guid, 1016713);
                //DeleteAgent agentDeleter = new DeleteAgent(eddsDbContext);

                List<AgentsPerPoolObject> agentsPerPoolObject = new List<AgentsPerPoolObject>();
                RunAgentTypeLogic runAgentLogic = new RunAgentTypeLogic(eddsDbContext);
                agentsPerPoolObject = runAgentLogic.AgentsPerPoolObject;





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