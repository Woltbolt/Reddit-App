using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

//namespace NotificationService

    [ServiceContract]
    public interface IHealthMonitoring
    {
        [OperationContract]
        bool HealthCheck();
    }

