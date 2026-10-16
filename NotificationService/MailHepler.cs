using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using AzureTable;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace NotificationService
{
    class MailHelper
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
}
