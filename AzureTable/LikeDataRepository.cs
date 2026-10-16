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
    public class LikeDataRepository
    {
        private CloudStorageAccount _storageLikes;
        private CloudTable _tableLikes;


        public LikeDataRepository()
        {
            _storageLikes = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageLikes.TableEndpoint.AbsoluteUri), _storageLikes.Credentials);
            _tableLikes = tableClient.GetTableReference("LikeTable");
            _tableLikes.CreateIfNotExists();
        }
        public IQueryable<Like> RetrieveAllLikes()
        {
            var results = from g in _tableLikes.CreateQuery<Like>()
                          where g.PartitionKey == "Like"
                          select g;
            return results;
        }

        public void AddLike(Like newLike)
        {
            TableOperation add = TableOperation.Insert(newLike);
            _tableLikes.Execute(add);
        }

        public IQueryable<Like> RetrieveLikeId(string id)
        {
            var results = from g in _tableLikes.CreateQuery<Like>()
                          where g.PartitionKey == "Like" && g.Id == id
                          select g;

            return results;
        }
    }
}
