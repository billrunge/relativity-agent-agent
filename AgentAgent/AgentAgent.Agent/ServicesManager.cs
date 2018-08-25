using System;
using System.ServiceProcess;

namespace AgentAgent.Agent
{
    class ServicesManager
    {
        public int Timeout { get; set; } = 30; //30 seconds is the default service start/stop timeout

        private ServiceController _serviceControl;
        private readonly string _agentServiceName = "kCura EDDS Agent Manager";
        private readonly string _serviceHostServiceName = "kCura Service Host Manager";
        private readonly string _webProcessingServiceName = "kCura EDDS Web Processing Manager";

        public ServicesManager()
        {
            _serviceControl = new ServiceController();
        }

        private void StopService (string serviceName, string agentServerHostname)
        {
            _serviceControl = new ServiceController(serviceName, agentServerHostname);

            try
            {
                // Stop the service, and wait until its status is "Stopped".
                _serviceControl.Stop();
                var timeout = new TimeSpan(0, 0, Timeout);
                _serviceControl.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                throw new Exception("Timeout stopping service");
            }
        }

        private void StartService(string serviceName, string agentServerHostname)
        {
            _serviceControl = new ServiceController(serviceName, agentServerHostname);

            try
            {
                // Start the service, and wait until its status is "Running".
                _serviceControl.Start();
                var timeout = new TimeSpan(0, 0, Timeout);
                _serviceControl.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                throw new Exception("Timeout starting service");
            }


        }

        private void RestartService(string serviceName, string agentServerHostname)
        {
            StopService(serviceName, agentServerHostname);
            StartService(serviceName, agentServerHostname);

        }

        public void StartAgentService(string agentServerHostname)
        {
            StartService(_agentServiceName, agentServerHostname);
        }

        public void StopAgentService(string agentServerHostname)
        {
            StopService(_agentServiceName, agentServerHostname);
        }

        public void RestartAgentService(string agentServerHostName)
        {
            RestartService(_agentServiceName, agentServerHostName);
        }

        public void StartServiceHostService(string agentServerHostname)
        {
            StartService(_serviceHostServiceName, agentServerHostname);
        }

        public void StopServiceHostService(string agentServerHostname)
        {
            StopService(_serviceHostServiceName, agentServerHostname);
        }

        public void RestartServiceHostService(string agentServerHostName)
        {
            RestartService(_serviceHostServiceName, agentServerHostName);
        }

        public void StartWebProcessingService(string agentServerHostname)
        {
            StartService(_webProcessingServiceName, agentServerHostname);
        }

        public void StopWebProcessingService(string agentServerHostname)
        {
            StopService(_webProcessingServiceName, agentServerHostname);
        }

        public void RestartWebProcessingService(string agentServerHostName)
        {
            RestartService(_webProcessingServiceName, agentServerHostName);
        }

    }
}
