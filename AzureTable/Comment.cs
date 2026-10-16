using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Storage.Table
{
    public class Comment : TableEntity
    {
        public string Id_komentara { get; set; }
        public string Id_objave { get; set; }
        public string Tekst { get; set; }
        public string Napisao { get; set; }
        public string Objavu_objavio { get; set; }
        public string Naziv_Objave { get; set; }
        public string Naziv_Teme { get; set; }
        public string Jedinstveni_id { get; set; }

        public Comment() { }

        public Comment(string idkomentara)
        {
            PartitionKey = "Comment";
            RowKey = idkomentara;
        }
        public Comment(string id_komentara, string id_objave, string tekst, string napisao, string objavio, string naziv, string nt, string jedinstveni)
        {
            Id_komentara = id_komentara;
            Id_objave = id_objave;
            Tekst = tekst;
            Napisao = napisao;
            Objavu_objavio = objavio;
            Naziv_Objave = naziv;
            Naziv_Teme = nt;
            Jedinstveni_id = jedinstveni;
        }
    }
}
