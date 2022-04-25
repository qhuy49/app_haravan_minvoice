using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using ZaloCSharpSDK;
using ZaloDotNetSDK;

namespace WebHDDT.Controllers
{
    public class StudentDataController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {

          
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {

        }

        //private void sendEmailViaWebApi()
        //{
        //    string subject = "TEST gửi tin nhắn Minvoice-Haravan";
        //    string body = "Tôi tên là ABC";
        //    string FromMail = "ctycpquangdung1@gmail.com";
        //    string emailTo = "qhuylk49@gmail.com";
        //    MailMessage mail = new MailMessage();
        //    SmtpClient SmtpServer = new SmtpClient("mail.reckonbits.com.pk");
        //    mail.From = new MailAddress(FromMail);
        //    mail.To.Add(emailTo);
        //    mail.Subject = subject;
        //    mail.Body = body;
        //    SmtpServer.Port = 587;
        //    SmtpServer.Credentials = new System.Net.NetworkCredential("ctycpquangdung1@gmail.com", "quangdung123");
        //    SmtpServer.EnableSsl = true;
        //    SmtpServer.Send(mail);
        //}
    }
}