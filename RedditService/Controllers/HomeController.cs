using AzureTable;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using RedditService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using NotificationService;
using System.ServiceModel;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace RedditService.Controllers
{
    public class HomeController : Controller
    {
        




        public ActionResult Index()
        {
            return View();
        }

        UserdataRepository repo = new UserdataRepository();
        TopicsDataRepository topicRepo = new TopicsDataRepository();
        CommentDataRepository commentRepo = new CommentDataRepository();
        LikeDataRepository likeRepo = new LikeDataRepository();
        SubscribeDataRepository subRepo = new SubscribeDataRepository();
        // public Dictionary<int,string> replycomments = new Dictionary<int, string>();




        public ActionResult Register(string ime, string prezime, string adresa, string grad, string drzava, string telefon, string email, string lozinka, HttpPostedFileBase slika)
{
    try
    {
        TempData["Message"] = $"Uspješna registracija korisnika '{ime} {prezime}'!";
        string RowKey = email;
        if (repo.Equals(RowKey))
        {
            return View();
        }

        if (slika != null && slika.ContentLength > 0)
        {
            string uniqueBlobName = $"image_{RowKey}{Path.GetExtension(slika.FileName)}";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("vezba");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueBlobName);
            blockBlob.UploadFromStream(slika.InputStream);

            string imageUrl = blockBlob.Uri.ToString();
            ViewBag.trenutnaSlika = imageUrl;

            User k = new User(RowKey)
            {
                FirstName = ime,
                LastName = prezime,
                Address = adresa,
                City = grad,
                Country = drzava,
                PhoneNumber = telefon,
                Email = email,
                Password = lozinka,
                Image = imageUrl
            };

            repo.AddUser(k);

            CloudQueue red = QueueHelper.GetQueueReference("vezba");
            red.AddMessage(new CloudQueueMessage(RowKey), null, TimeSpan.FromMilliseconds(30));

            return RedirectToAction("Contact");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Please upload an image.");
            return RedirectToAction("Registracija");
        }
    }
    catch (Exception ex)
    {
        ModelState.AddModelError(string.Empty, ex.Message);
        return RedirectToAction("Registracija");
    }
}
        static User trenutnikorisnik = new User();

        [HttpPost]
        public ActionResult Login(string email, string lozinka)
        {

            bool isAuthenticated = repo.AuthenticateUser(email, lozinka);
            ViewBag.subbd = "true";
            if (isAuthenticated)
            {
                trenutnikorisnik.Email = email;
                trenutnikorisnik.Password = lozinka;
                return RedirectToAction("Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public class MailHelper
        {
            public const string From = "kuravicaCloud@outlook.com";
            public const string Password = "12345678Aa!";
            public static void SendMail(string subject, string body, string to)
            {
                try
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(From),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(to);
                    var smtpClient = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(From, Password),
                        EnableSsl = true,
                    };
                    smtpClient.Send(mailMessage);
                }
                catch (System.Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }
        }

        //TO DO:

        //[HttpPut]
        public ActionResult EditProfile()
        {
            ViewBag.Message = trenutnikorisnik;
           
            return View();
        }
        public ActionResult AddPost()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Registracija()
        {
            return View();
        }
        UserdataRepository reponovi = new UserdataRepository();
        public ActionResult Home()
        {
            User user = repo.GetUserByEmail(trenutnikorisnik.Email);
            var topics = topicRepo.RetrieveAllTopics();
            User korisnik = new User();
            korisnik.Email = trenutnikorisnik.Email;
            reponovi.UserData(korisnik);
            trenutnikorisnik.FirstName = korisnik.FirstName;
            trenutnikorisnik.LastName = korisnik.LastName;
            trenutnikorisnik.Address = korisnik.Address;
            trenutnikorisnik.City = korisnik.City;
            trenutnikorisnik.Country = korisnik.Country;
            trenutnikorisnik.PhoneNumber = korisnik.PhoneNumber;
            trenutnikorisnik.Password = korisnik.Password;
            trenutnikorisnik.Image = korisnik.Image;
            
            ViewBag.trenutnaSlika = trenutnikorisnik.Image;
            ViewBag.Message = $"Welcome {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} on your account!";
            ViewBag.CurrentUserName = user.FirstName;
            ViewBag.CurrentUserEmail= user.Email;
            ViewBag.allcomments = "";



            return View(topics);
        }

        
        [HttpPost]
        public ActionResult UpdatedUser(string ime, string prezime, string adresa, string grad, string drzava, string telefon, string sifra, string image)
        {
            try
            {
                string RowKey = trenutnikorisnik.Email;
                User newuser = new User(RowKey);
                newuser.FirstName = ime;
                newuser.LastName = prezime;
                newuser.Address = adresa;
                newuser.City = grad;
                newuser.Country = drzava;
                newuser.PhoneNumber = telefon;
                newuser.Email = trenutnikorisnik.Email;
                newuser.Password = sifra;
                newuser.Image = image;

                // Check if a new image is uploaded
               
                    

                    // Store the image URL in ViewBag if needed
                    ViewBag.trenutnaSlika = image;
                

                // Edit the user in the repository
                repo.EditUser(newuser);

                // Optionally, perform other actions or redirect to a success view
                ViewBag.allcomments = "";
                return View("SucssesfullyUpdatedUser");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the process
                ModelState.AddModelError(string.Empty, ex.Message);
                // Redirect to the appropriate view with an error message
                return RedirectToAction("ErrorView");
            }
        }


        public ActionResult Topics(string themes)
        {
            var topics = topicRepo.RetrieveAllTopics();
            ViewBag.Message = $"Welcome {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} on your account!";
            ViewBag.CurrentUserEmail = trenutnikorisnik.Email;
            switch (themes)
            {
                case "games":
                    TempData["all"] = "Games";
                    ViewBag.all = "Games";
                    break;
                case "technology":
                    TempData["all"] = "Technology";
                    ViewBag.all = "Technology";
                    break;
                case "movies_tv":
                    TempData["all"] = "Movies & TV";
                    ViewBag.all = "Movies & TV";
                    break;
                case "arts":
                    TempData["all"] = "Arts";
                    ViewBag.all = "Arts";
                    break;
                case "business":
                    TempData["all"] = "Business";
                    ViewBag.all = "Business";
                    break;
                case "places_travel":
                    TempData["all"] = "Places & Travel";
                    ViewBag.all = "Places & Travel";
                    break;
                case "science":
                    TempData["all"] = "Science";
                    ViewBag.all = "Science";
                    break;
                case "sport":
                    TempData["all"] = "Sport";
                    ViewBag.all = "Sport";
                    break;
                case "vehicle":
                    TempData["all"] = "Vehicle";
                    ViewBag.all = "Vehicle";
                    break;
                default:
                    break;
            }
            return View("Home",topics);
        }

        public string RandomId()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
        [HttpPost]
        public ActionResult Add(string tema, string naslov, string tekst)
        {
            User user = repo.GetUserByEmail(trenutnikorisnik.Email);

            try
            {
                string id = RandomId();

                Topic topic = new Topic(id)
                {
                    Id = id, 
                    Tema = tema,
                    Naslov = naslov,
                    Tekst = tekst,
                    Objavio = user.Email,
                };

                topicRepo.AddTopic(topic);
                return RedirectToAction("Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Contact");
            }
        }
        /*
        public string RandomIdComment()
        {
            Random rand = new Random();
            int randID = rand.Next(1, 500);
            return
        }
        */
        public ActionResult Notifications(string publisher, string komentar, string action, string id, string nameOfPost, string topic/*dodatak*/, string tekst_komentara, string id_komentara, string naziv_teme, string komentar_napisao, string jedinstveni)
        {
            // ViewBag.replydic = replycomments;
            ViewBag.sub = "";
            string idSub = RandomId();
            ViewBag.idkomentara = "";
            ViewBag.Reply = "";
            ViewBag.replykomentar = "";
            string rowkey = RandomId();
            string jedinstveniid = RandomId();
            var topics = topicRepo.RetrieveAllTopics();
            ViewBag.id = id;
            ViewBag.CurrentUserEmail = trenutnikorisnik.Email;

            if (action == "comment")
            {
                Comment newcomment = new Comment(rowkey);
                newcomment.Id_komentara = rowkey;
                newcomment.Id_objave = id;
                newcomment.Napisao = trenutnikorisnik.Email;
                newcomment.Objavu_objavio = publisher;
                newcomment.Tekst = komentar;
                newcomment.Naziv_Objave = nameOfPost;
                newcomment.Naziv_Teme = topic;
                newcomment.Jedinstveni_id = jedinstveniid;

                commentRepo.AddComment(newcomment);


                ViewBag.objava = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} commented on {publisher}'s post:";

                UserdataRepository user = new UserdataRepository();
                //foreach (var US in user.RetrieveAllUsers().Where(s => s.FirstName == publisher))
                //{
                    MailHelper.SendMail("Komentar", $"Korisnik {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} je komentarisao: {newcomment.Tekst} ", publisher);
                //}
                
                ViewBag.komentar = komentar;
                ViewBag.alllikes = "";

                return View("Home", topics);
            }
            else if (action == "like")
            {
                var lajkovi = likeRepo.RetrieveAllLikes();

                Like newLike = new Like(rowkey);
                newLike.Email = trenutnikorisnik.Email;
                newLike.Ime = trenutnikorisnik.FirstName;
                newLike.Prezime = trenutnikorisnik.LastName;
                newLike.Id_topic = id;
                newLike.Id = rowkey;

                Like bezrow = new Like() { RowKey = "", Id = "", Email = trenutnikorisnik.Email, Ime = trenutnikorisnik.FirstName, Prezime = trenutnikorisnik.LastName, Id_topic = id };

                List<Like> lista_lajkova = new List<Like>();
                foreach (var l in lajkovi)
                {
                    lista_lajkova.Add(new Like { RowKey = "", Id = "", Email = l.Email, Ime = l.Ime, Prezime = l.Prezime, Id_topic = l.Id_topic });
                }


                foreach (var a in lista_lajkova)
                {
                    if (a.Email == bezrow.Email && a.Id_topic == bezrow.Id_topic)
                    {
                        ViewBag.ima = "ima";
                    }
                }

                if (ViewBag.ima != "ima")
                {
                    likeRepo.AddLike(newLike);
                    ViewBag.objava = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} liked {publisher}'s post.";

                    UserdataRepository user = new UserdataRepository();
                    //foreach (string email in user.Get())
                    //{
                        MailHelper.SendMail("Lajk", $"Korisnik {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} je lajkovao post od {publisher}", publisher);
                    //}

                    ViewBag.komentar = "";
                    ViewBag.alllikes = "";
                }
                else
                {
                    ViewBag.alllikes = "";
                    ViewBag.PorukaVazna = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} already liked this post.";
                }
                return View("Home", topics);
            }else if(action == "subscribe")
            {
                ViewBag.sub = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} subscribed to {publisher}";
                MailHelper.SendMail("Subscrajb", $"Korisnik {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} je subscrajbovao na: {publisher} ", publisher);
                Sub newSub = new Sub(idSub)
                {
                    Subscriber = trenutnikorisnik.Email,
                    Publisher = publisher,
                };

                bool subbed = subRepo.IsUserSubscribed(trenutnikorisnik.Email, publisher);
                ViewBag.subbd = subbed.ToString();
                if (!subbed)
                {
                    subRepo.AddSub(newSub);
                }
                else
                {
                    ViewBag.sub = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} is already subscribed on {publisher} channel!";
                }
                return View("Home", topics);
            }
            else if (action == "unsubscribe")
            {
                ViewBag.sub = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} unsubscribed {publisher}";
                Sub subRemove = subRepo.GetSubByUserAndPub(trenutnikorisnik.Email, publisher);

                bool subbed = subRepo.IsUserSubscribed(trenutnikorisnik.Email, publisher);


                if (subbed)
                {
                    subRepo.RemoveSub(subRemove);
                }
                else
                {
                    ViewBag.sub = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} is not subscribed on {publisher} channel!";
                }
                
                return View("Home", topics);
            }
            else if (action == "viewcomments")
            {
                List<Comment> komentari = new List<Comment>();
                var allcomments = commentRepo.RetrieveAllComments();
                foreach (var k in allcomments)
                {
                    if (k.Id_objave == id)
                    {
                        ViewBag.naziteme = topic;
                        komentari.Add(new Comment { Id_komentara = k.Id_komentara, Id_objave = id, Tekst = k.Tekst, Napisao = k.Napisao, Objavu_objavio = publisher, Naziv_Objave = nameOfPost, Naziv_Teme = k.Naziv_Teme, Jedinstveni_id = k.Jedinstveni_id });
                    }
                }
                List<Comment> komentari_bez_teme = new List<Comment>();

                foreach (var kom in komentari)
                {
                    if (kom.Naziv_Teme == "")
                    {
                        komentari_bez_teme.Add(kom);
                    }
                }

                List<Comment> redom = new List<Comment>();
                Dictionary<string, Comment> dic = new Dictionary<string, Comment>();
                foreach (var a in komentari)
                {
                    if (komentari_bez_teme == null || komentari_bez_teme.Count == 0)
                    {
                        redom.Add(a);
                    }
                    else
                    {
                        foreach (var b in komentari_bez_teme)
                        {
                            if (a.Id_komentara != b.Id_komentara && a.Naziv_Teme != "")
                            {

                                if (!redom.Contains(a))
                                {
                                    redom.Add(a);
                                }
                                //ispisi a
                            }
                            else if (a.Id_komentara == b.Id_komentara && a.Naziv_Teme != "")
                            {
                                //ispisi a
                                if (redom.Contains(a))
                                {
                                    foreach (var c in komentari_bez_teme)
                                    {
                                        if (c.Id_komentara == a.Id_komentara)
                                        {
                                            //ispisi sve c
                                            if (!redom.Contains(c))
                                            {
                                                redom.Add(c);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    redom.Add(a); foreach (var c in komentari_bez_teme)
                                    {
                                        if (c.Id_komentara == a.Id_komentara)
                                        {
                                            //ispisi sve c
                                            if (redom.Contains(c))
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                redom.Add(c);
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                var likes = likeRepo.RetrieveAllLikes();
                List<Like> lajkovi = new List<Like>();
                foreach (var l in likes)
                {
                    if (l.Id_topic == id)
                    {
                        lajkovi.Add(new Like { Id = l.Id, Email = l.Email, Ime = l.Ime, Prezime = l.Prezime, Id_topic = l.Id_topic });
                    }

                }
                ViewBag.trenutniemail = trenutnikorisnik.Email;
                ViewBag.alllikes = lajkovi;
                ViewBag.allcomments = redom;
                return View("Home",topics);
            }
            else if (action == "reply")
            {
                ViewBag.Reply = "reply";
                List<Comment> komentari = new List<Comment>();
                var allcomments = commentRepo.RetrieveAllComments();
                foreach (var k in allcomments)
                {
                    if (k.Id_objave == id)
                    {
                        ViewBag.naziteme = topic;
                        komentari.Add(new Comment { Id_komentara = k.Id_komentara, Id_objave = id, Tekst = k.Tekst, Napisao = k.Napisao, Objavu_objavio = publisher, Naziv_Objave = nameOfPost, Naziv_Teme = k.Naziv_Teme });
                    }
                }
              
                foreach (var i in komentari)
                {
                    if(i.Id_komentara == id_komentara)
                    {
                        ViewBag.idkomentara = i.Id_komentara;
                        ViewBag.kljuc = i.RowKey;
                    }
                }

                List<Comment> komentari_bez_teme = new List<Comment>();
                foreach (var kom in komentari)
                {
                    if (kom.Naziv_Teme == "")
                    {
                        komentari_bez_teme.Add(kom);
                    }
                }

                List<Comment> redom = new List<Comment>();
                Dictionary<string, Comment> dic = new Dictionary<string, Comment>();
                foreach (var a in komentari)
                {
                    if (komentari_bez_teme == null || komentari_bez_teme.Count == 0)
                    {
                        redom.Add(a);
                    }
                    else
                    {
                        foreach (var b in komentari_bez_teme)
                        {
                            if (a.Id_komentara != b.Id_komentara && a.Naziv_Teme != "")
                            {

                                if (!redom.Contains(a))
                                {
                                    redom.Add(a);
                                }

                                //ispisi a


                            }
                            else if (a.Id_komentara == b.Id_komentara && a.Naziv_Teme != "")
                            {
                                //ispisi a
                                if (redom.Contains(a))
                                {
                                    foreach (var c in komentari_bez_teme)
                                    {
                                        if (c.Id_komentara == a.Id_komentara)
                                        {
                                            //ispisi sve c
                                            if (!redom.Contains(c))
                                            {
                                                redom.Add(c);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    redom.Add(a); foreach (var c in komentari_bez_teme)
                                    {
                                        if (c.Id_komentara == a.Id_komentara)
                                        {
                                            //ispisi sve c
                                            if (redom.Contains(c))
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                redom.Add(c);
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                var likes = likeRepo.RetrieveAllLikes();
                List<Like> lajkovi = new List<Like>();
                foreach (var l in likes)
                {
                    if (l.Id_topic == id)
                    {
                        lajkovi.Add(new Like { Id = l.Id, Email = l.Email, Ime = l.Ime, Prezime = l.Prezime, Id_topic = l.Id_topic });
                    }
                }
                ViewBag.alllikes = lajkovi;

                ViewBag.allcomments = redom;
                return View("Home", topics);
            }else if(action== "replycomment")
            {
                Comment newcomment = new Comment(rowkey);
                newcomment.Id_komentara = id_komentara;
                newcomment.Id_objave = id;
                newcomment.Napisao = trenutnikorisnik.Email;
                newcomment.Objavu_objavio = publisher;
                newcomment.Tekst = komentar;
                newcomment.Naziv_Objave = nameOfPost;
                newcomment.Naziv_Teme = "";
                newcomment.Jedinstveni_id = jedinstveniid;

                commentRepo.AddComment(newcomment);

                ViewBag.odgovoriola = komentar_napisao;
                ViewBag.objava = $"User {trenutnikorisnik.FirstName} {trenutnikorisnik.LastName} commented on {komentar_napisao}'s comment:";
                ViewBag.komentar = komentar;
                ViewBag.alllikes = "";
                return View("Home", topics);
            }else if(action== "delete")
            {
                var komentari = commentRepo.RetrieveAllComments();
                foreach(var i in komentari)
                {
                    if(i.Jedinstveni_id == jedinstveni && i.Naziv_Teme=="")
                    {
                        commentRepo.DeleteComment(i);
                    }else if(i.Jedinstveni_id == jedinstveni && i.Naziv_Teme == naziv_teme && i.Id_komentara == id_komentara)
                    {
                        commentRepo.DeleteComment(i);
                        foreach(var g in komentari)
                        {
                            if(g.Id_komentara == id_komentara)
                            {
                                commentRepo.DeleteComment(g);
                            }
                        }
                    }
                }

                var likes = likeRepo.RetrieveAllLikes();
                List<Like> lajkovi = new List<Like>();
                foreach (var l in likes)
                {
                    if (l.Id_topic == id)
                    {
                        lajkovi.Add(new Like { Id = l.Id, Email = l.Email, Ime = l.Ime, Prezime = l.Prezime, Id_topic = l.Id_topic });
                    }
                }
                ViewBag.alllikes = lajkovi;
                return View("Home", topics);
            }else if(action == "delete2")
            {

                var komentari = commentRepo.RetrieveAllComments();
                foreach (var i in komentari)
                {
                    if (i.Id_objave == id)
                    {
                        commentRepo.DeleteComment(i);
                    }
                }
                foreach (var top in topics)
                {
                    if(top.Id == id)
                    {
                        topicRepo.RemoveTopic(top);
                    }
                }
                return View("Home",topics);
            }
            return View("Home");
            
        }

        /*
        public ActionResult NotificationReply(string komentar_napisao, string tekst_komentara, string id_objave, string naziv_objave, string objavu_objavio, string id_komentara, string naziv_teme)
        {
            var topics = topicRepo.RetrieveAllTopics();
            ViewBag.id = id_objave;
            ViewBag.CurrentUserEmail = trenutnikorisnik.Email;

            List<Comment> komentari = new List<Comment>();
            var allcomments = commentRepo.RetrieveAllComments();
            foreach (var k in allcomments)
            {
                if (k.Id_objave == id_objave)
                {
                    ViewBag.naziteme = naziv_teme;
                    komentari.Add(new Comment { Id_komentara = k.Id_komentara, Id_objave = id_objave, Tekst = k.Tekst, Napisao = k.Napisao, Objavu_objavio = objavu_objavio, Naziv_Objave = naziv_objave });
                }
            }
            ViewBag.allcomments = komentari;
            return View("SucssesfullyUpdatedUser");




        }*/

    }
}