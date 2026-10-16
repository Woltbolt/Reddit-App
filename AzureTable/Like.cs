using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Storage.Table
{
    public class Like : TableEntity
    {
        public string Id { get; set; }
        public string Id_topic { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }

        public Like() { }

        public Like(string rowkey)
        {
            PartitionKey = "Like";
            RowKey = rowkey;
        }
        public Like(string rk,string iid,string e, string i, string p, string id)
        {
            RowKey = rk;
            Id = iid;
            Email = e;
            Ime = i;
            Prezime = p;
            Id_topic = id;
        }
        public override string ToString()
        {
            return $"Id: {Id}, Id_topic: {Id_topic}, Ime: {Ime}, Prezime: {Prezime}, Email: {Email}";
        }
    }
}
