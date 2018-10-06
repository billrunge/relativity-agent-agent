using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using AgentAgent.Agent.Objects;

namespace AgentAgent.Agent
{
    class Powershell
    {
        private string RunPowershell(string script)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                string output = "";
                shell.Commands.AddScript(script);

                Collection<PSObject> results = shell.Invoke();

                foreach (PSObject result in results)
                {
                    output += result.ToString();
                }
                return output;
            }
        }


    }
}
