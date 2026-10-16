using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTable
{
    public class SubscribeDataRepository
    {
        private CloudStorageAccount _storageSub;
        private CloudTable _tableSub;


        public SubscribeDataRepository()
        {
            _storageSub = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageSub.TableEndpoint.AbsoluteUri), _storageSub.Credentials);
            _tableSub = tableClient.GetTableReference("SubTable");
            _tableSub.CreateIfNotExists();
        }

        public IQueryable<Sub> RetrieveAllSubs()
        {
            var results = from g in _tableSub.CreateQuery<Sub>()
                          where g.PartitionKey == "Sub"
                          select g;
            return results;
        }

        public void AddSub(Sub newSub)
        {
            TableOperation add = TableOperation.Insert(newSub);
            _tableSub.Execute(add);
        }

        public Sub GetSubByUserAndPub(string sub, string pub)
        {
            IQueryable<Sub> result = from g in _tableSub.CreateQuery<Sub>()
                                     where g.PartitionKey == "Sub" && g.Subscriber == sub && g.Publisher == pub
                                     select g;
            return result.FirstOrDefault();
        }


        public void RemoveSub(Sub sub)
        {
            TableOperation remove = TableOperation.Delete(sub);
            _tableSub.Execute(remove);
        }

        public bool IsUserSubscribed(string sub, string pub)
        {
            return RetrieveAllSubs().Where(s => s.Subscriber == sub && s.Publisher == pub).FirstOrDefault() != null;
        }

    }
}