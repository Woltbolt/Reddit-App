//using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    internal class HealthMonitoring : IHealthMonitoring
    {
        public bool HealthCheck()
        {
            Console.WriteLine("Worker role checked in.");
            return true;
        }
    }
}