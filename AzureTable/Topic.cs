using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Storage.Table
{
    public class Topic : TableEntity
    {
        public string Id { get; set; }
        public string Tema { get; set; }
        public string Naslov { get; set; }
        public string Tekst { get; set; }
        public string Objavio { get; set; }

        public Topic() { }

        public Topic(string id)
        {
            PartitionKey = "Topic";
            RowKey = id;
        }
    }
}
