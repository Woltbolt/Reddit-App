using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
//using System;
//using System.Net.Http.Headers;
using System.ServiceModel;

namespace NotificationService
{
    public static class WcfClientHelper
    {
        public static TService Connect<TService>(string serviceName, string route)
        {
            Random random = new Random();
            int instanceIndex = random.Next(RoleEnvironment.Roles[serviceName].Instances.Count - 1);
            var endpoint = RoleEnvironment.Roles[serviceName].Instances[instanceIndex].InstanceEndpoints["InternalRequest"];
            string fullEndpoint = string.Format("net.tcp://{0}/{1}", endpoint.IPEndpoint, route);
            ChannelFactory<TService> factory = new ChannelFactory<TService>(new NetTcpBinding(), new EndpointAddress(fullEndpoint));
            return factory.CreateChannel();
        }
    }
}
