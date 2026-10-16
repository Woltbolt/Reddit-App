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
    public class CommentDataRepository
    {
        private CloudStorageAccount _storageComments;
        private CloudTable _tableComments;


        public CommentDataRepository()
        {
            _storageComments = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageComments.TableEndpoint.AbsoluteUri), _storageComments.Credentials);
            _tableComments = tableClient.GetTableReference("CommentTable");
            _tableComments.CreateIfNotExists();
        }

        public IQueryable<Comment> RetrieveAllComments()
        {
            var results = from g in _tableComments.CreateQuery<Comment>()
                          where g.PartitionKey == "Comment"
                          select g;
            return results;
        }

        public void AddComment(Comment newComment)
        {
            TableOperation add = TableOperation.Insert(newComment);
            _tableComments.Execute(add);
        }

        public IQueryable<Comment> RetrieveCommentId(string id)
        {
            var results = from g in _tableComments.CreateQuery<Comment>()
                          where g.PartitionKey == "Comment" && g.Id_komentara == id
                          select g;

            return results;
        }
        public void DeleteComment(Comment comment)
        {
            TableOperation delete = TableOperation.Delete(comment);
            _tableComments.Execute(delete);
        }
    }
}
