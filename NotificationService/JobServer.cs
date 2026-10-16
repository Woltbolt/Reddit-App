//using Common;
/*using Microsoft.WindowsAzure.ServiceRuntime;
using NotificationService;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace RedditService
{
    public class JobServer
    {
        private ServiceHost serviceHost;
        private string endPointName = "health-monitoring";
        public JobServer()
        {
            RoleInstanceEndpoint inputEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InternalRequest"];
            string endpoint = string.Format("net.tcp://{0}/{1}", inputEndPoint.IPEndpoint, endPointName);
            serviceHost = new ServiceHost(typeof(HealthMonitoring));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding,endpoint);
        }
        public void Open()
        {
            try
            {
                serviceHost.Open();
                Trace.TraceInformation(string.Format("Host for {0} endpoint type opened successfully at {1}", endPointName, DateTime.Now));
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Host open error for {0} endpoint type. Error message is: {1}. ", endPointName, e.Message);
            }
        }
        public void Close()
        {
            try
            {
                serviceHost.Close();
                Trace.TraceInformation(string.Format("Host for {0} endpoint type closed successfully at {1}", endPointName, DateTime.Now));
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Host close error for {0} endpoint type. Error message is: {1}. ", endPointName, e.Message);
            }
        }
    }
}*/