using kCura.Agent;
using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Objects;
using System;

namespace AgentAgent.Agent
{
    [kCura.Agent.CustomAttributes.Name("Agent Agent Agent")]
    [System.Runtime.InteropServices.Guid("0c1f7839-e571-497f-ba87-32bdd899953b")]
    public class Agent : AgentBase
    {
        /// <summary>
        /// Agent logic goes here
        /// </summary>
        public override void Execute()
        {
            IAPILog logger = Helper.GetLoggerFactory().GetLogger();

            try
            {
                //Get the current Agent artifactID
                int agentArtifactId = AgentID;

                //Get a dbContext for the EDDS database
                IDBContext eddsDbContext = Helper.GetDBContext(-1);
                
                ServerInformation serverInfo = new ServerInformation(eddsDbContext);
                AgentServerList serverList = new AgentServerList();

                serverList = serverInfo.GetAgentServerList();

                
                                
                //Get GUID for an artifact
                //int testArtifactId = 10101010;
                //Guid guidForTestArtifactId = Helper.GetGuid(workspaceArtifactId, testArtifactId);

                //Display a message in Agents Tab and Windows Event Viewer


                RaiseMessage("Server hostname is " + serverList.OutputList[0].Hostname, 1);

                ////The Object Manager is the newest and preferred way to interact with Relativity instead of the Relativity Services API(RSAPI). 
                ////The RSAPI will be scheduled for depreciation after the Object Manager reaches feature party with it.
                //using (IObjectManager objectManager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
                //{

                //}

                ////Setting up an RSAPI Client
                //using (IRSAPIClient rsapiClient = Helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.CurrentUser))
                //{
                //    //Set the proxy to use the current workspace
                //    //rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;

                //    //Add code for working with RSAPIClient
                //}

                //logger.LogVerbose("Log information throughout execution.");
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