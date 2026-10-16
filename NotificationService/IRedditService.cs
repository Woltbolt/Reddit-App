using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    [ServiceContract]
    public interface IRedditService
    {
        [OperationContract]
        bool Register(User user);
        [OperationContract]
        User Login(string email, string password);
        [OperationContract]
        bool Update(User user);
        [OperationContract]
        void AddAlarm(string email, double limit, string currency);
    }
}
