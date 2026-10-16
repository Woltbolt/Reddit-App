using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService.Models
{
    public class RegistrovaniKorisnici
    {
       
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Adresa { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public string BrojTelefona { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        // public HttpPostedFileBase Slika { get; set; } // slika ce se cuvati kao niz bajtova
        public RegistrovaniKorisnici() { }
    }
}