using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Storage.Table
{
    public class UserdataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserdataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable");
            _table.CreateIfNotExists();
        }

        public bool Exists(string UserNo)
        {
            return RetrieveAllUsers().Where(s => s.RowKey == UserNo).FirstOrDefault() != null;
        }

        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }

        public bool AuthenticateUser(string email, string lozinka)
        {
            return RetrieveAllUsers().Where(s => s.Email == email && s.Password == lozinka).FirstOrDefault() != null;
        }

        public class Mail : TableEntity
        {
            public Mail() { PartitionKey = "email"; RowKey = Guid.NewGuid().ToString(); }
            public string Address { get; set; }
        }
        public List<string> Get()
        {
            return _table.CreateQuery<Mail>().Where(x => x.PartitionKey == "mail").Select(x => x.Address).ToList();
        }


        public void AddUser(User newUser)
        {
            if (Exists(newUser.Email))
            {
                throw new Exception("Email already exists");
            }
            else
            {
                TableOperation insertOperation = TableOperation.Insert(newUser);
                _table.Execute(insertOperation);
            }
        }

        public User GetUserByEmail(string email)
        {
            IQueryable<User> result = from g in _table.CreateQuery<User>()
                         where g.PartitionKey == "User" && g.Email == email
                         select g;

            return result.SingleOrDefault();
        }




        //TO DO:
        public void EditUser(User newUser)
        {
            if (newUser.PartitionKey == "User" && newUser.RowKey == newUser.Email)
            {
                newUser.ETag = "*";
                TableOperation update = TableOperation.Replace(newUser);
                _table.Execute(update);
            }
            else
            {
                throw new Exception("User doesn't exists");
            }

        }
        public void UserData(User user)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>("User", user.Email);
            TableResult retrievedResult = _table.Execute(retrieveOperation);
            Console.WindowTop.ToString("kurcuba");
            Console.WriteLine(retrievedResult + " ret rez");

            if (retrievedResult.Result != null)
            {
                User existingUser = (User)retrievedResult.Result;
                user.FirstName = existingUser.FirstName;
                user.LastName = existingUser.LastName;
                user.Address = existingUser.Address;
                user.City = existingUser.City;
                user.Country = existingUser.Country;
                user.PhoneNumber = existingUser.PhoneNumber;
                user.Email = existingUser.Email;
                user.Password = existingUser.Password;
                user.Image = existingUser.Image;
            }
        }

       
    }
}
