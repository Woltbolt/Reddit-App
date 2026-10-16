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
    public class TopicsDataRepository
    {
        private CloudStorageAccount _storageTopics;
        private CloudTable _tableTopics;
        

        public TopicsDataRepository()
        {
            _storageTopics = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageTopics.TableEndpoint.AbsoluteUri), _storageTopics.Credentials);
            _tableTopics = tableClient.GetTableReference("TopicTable");
            _tableTopics.CreateIfNotExists();
        }

        public IQueryable<Topic> RetrieveAllTopics()
        {
            var results = from g in _tableTopics.CreateQuery<Topic>()
                          where g.PartitionKey == "Topic"
                          select g;
            return results;
        }

        public void AddTopic(Topic newTopic)
        {
            TableOperation add = TableOperation.Insert(newTopic);
            _tableTopics.Execute(add);
        }

        public IQueryable<Topic> RetrieveTopicTheme(string tema)
        {
            var results = from g in _tableTopics.CreateQuery<Topic>()
                          where g.PartitionKey == "Topic" && g.Tema == tema
                          select g;

            return results;
        } 

        public void RemoveTopic(Topic remTopic)
        {
            TableOperation remove = TableOperation.Delete(remTopic);
            _tableTopics.Execute(remove);
        }
    }
}
