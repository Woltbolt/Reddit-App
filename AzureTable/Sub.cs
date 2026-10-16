using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Storage.Table
{
    public class Sub : TableEntity
    {
        public string Publisher { get; set; }
        public string Subscriber { get; set; }


        public Sub() { }

        public Sub(string id)
        {
            PartitionKey = "Sub";
            RowKey = id;
        }
    }
}
