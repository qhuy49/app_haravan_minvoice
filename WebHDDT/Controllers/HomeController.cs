using System.Web.Mvc;
using System.Configuration;
namespace WebHDDT.Controllers
{
    using Newtonsoft.Json.Linq;
    using RestClient.Net.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using WebHDDT.Models;

    public class HomeController : Controller
    {
        Minvoice_HaravanEntities3 db = new Minvoice_HaravanEntities3();
        private static string subject = ConfigurationManager.AppSettings["subject"];



        private static string FromMail = ConfigurationManager.AppSettings["FromMail"];
        private static string password = ConfigurationManager.AppSettings["password"];
        private static string emailTo = ConfigurationManager.AppSettings["emailTo"];
        private static string bcc = ConfigurationManager.AppSettings["bcc"];
        private static string smtp = ConfigurationManager.AppSettings["smtp"];
        private static int port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private static bool ssl = bool.Parse(ConfigurationManager.AppSettings["ssl"]);
        private static string link_checkout = ConfigurationManager.AppSettings["link_checkout"];

        //public async Task<HttpResponseMessage> LoginAsync(string username, string password)
        //{

        //    HttpClient client = new HttpClient { BaseAddress = new Uri("https://0315827587.minvoice.com.vn/") };
        //    var content = new FormUrlEncodedContent(new[]
        //    {
        //          new KeyValuePair<string, string>("username", "VNYI"),
        //          new KeyValuePair<string, string>("password", "123456"),
        //          new KeyValuePair<string, string>("madvcs", null)
        //    });

        //    HttpResponseMessage respon = await client.PostAsync("api/Account/Login", content);
        //    string conten = await respon.Content.ReadAsStringAsync();
        //    JObject json = JObject.Parse(conten);


        //    //System.Diagnostics.Debug.WriteLine(token);
        //    return new HttpResponseMessage(HttpStatusCode.OK);
        //}



        public async Task<ActionResult> Index()
         {
            //HttpResponseMessage x =  await LoginAsync("","");

            if (Session["address1"] != null)
            {
                ViewBag.address1 = Session["address1"].ToString();
            }
            else
            {
                ViewBag.address1 = "";
            }

            //
            if (Session["province"] != null)
            {
                ViewBag.province = Session["province"].ToString();
            }
            else
            {
                ViewBag.province = "";
            }
            //
            if (Session["country"] != null)
            {
                ViewBag.country = Session["country"].ToString();
            }
            else
            {
                ViewBag.country = "";
            }
            //
            if (Session["customer_email"] != null)
            {
                ViewBag.customer_email = Session["customer_email"].ToString();
            }
            else
            {
                ViewBag.customer_email = "";
            }
            //

            if (Session["myharavan_domain"] != null)
            {
                ViewBag.myharavan_domain = Session["myharavan_domain"].ToString();
            }
            else
            {
                ViewBag.myharavan_domain = "";
            }
            //
            if (Session["phone"] != null)
            {
                ViewBag.phone = Session["phone"].ToString();
            }
            else
            {
                ViewBag.phone = "";
            }
            //
            if (Session["shop_owner"] != null)
            {
                ViewBag.shop_owner = Session["shop_owner"].ToString();
            }
            else
            {
                ViewBag.shop_owner = "";
            }
            if (Session["mst"] != null)
            {
                ViewBag.mst = Session["mst"].ToString();
            }
            else
            {
                ViewBag.mst = "";
            }
            //ViewBag.province = TempData["province"].ToString();
            //    ViewBag.country = TempData["country"].ToString();
            //    ViewBag.customer_email = TempData["customer_email"].ToString();
            //    ViewBag.myharavan_domain = TempData["myharavan_domain"].ToString();
            //    ViewBag.phone = TempData["phone"].ToString();
            //    ViewBag.shop_owner = TempData["shop_owner"].ToString();
            return View();


        }
        public JsonResult Check_Link(string link)
        {
            var orgid = Session["orgid"] ?? "";
            User_info _user = db.User_info.SingleOrDefault(n => n.orgid == orgid.ToString());
            if (_user != null)
            {
                _user.link = link;
                db.SaveChanges();
                Session["link"] = link;
                return Json(_user.link);
            }

            return Json("");
        }
        public JsonResult Check_MST(string mst, string username, string password)
        {
            var orgid = Session["orgid"] ?? "";
            //User_info _user = (from x in db.User_info
            //                   where x.mst == "1"
            //              select x).First();
            //_user.mst = mst;
            //db.SaveChanges();

            User_info _user = db.User_info.SingleOrDefault(n => n.orgid == orgid.ToString());
            if (_user != null)
            {
                _user.mst = mst;
                _user.username = username;
                _user.password = password;
                Session["mst"] = mst;
                db.SaveChanges();
                return Json(_user.mst);
            }

            return Json("");
        }
        public ActionResult GetThongTin(FormCollection f)
        {
            if (f.Count != 0)
            {


                var hoten = f["nf-field-103"] ?? "";
                var sdt = f["nf-field-100"] ?? "";
                var email = f["nf-field-98"] ?? "";
                var goi_hd = f["nf-field-102"] ?? "";
                var mst = f["nf-field-109"] ?? "";

                var address1 = f["address1"] ?? "";
                var province = f["province"] ?? "";
                var country = f["country"] ?? "";
                var myharavan_domain = f["myharavan_domain"] ?? "";
                var orgname = f["orgname"] ?? "";
                var orgcat = f["orgcat"] ?? "";


                var diachi = address1 + province + country;
                sendEmailViaWebApi(hoten, sdt, email, goi_hd, diachi, myharavan_domain, orgname, orgcat, mst);

                if (link_checkout != "")
                {
                    return Redirect(link_checkout);
                }
                return View();
            }
            return View();
        }
        private void sendEmailViaWebApi(string hoten, string sdt, string email, string goi_hd, string diachi, string myharavan_domain, string orgname, string orgcat, string mst)
        {

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(smtp);
            mail.From = new MailAddress(FromMail);
            mail.To.Add(emailTo);
            mail.Bcc.Add(bcc);
            mail.Subject = subject;
            mail.Body = $"Tôi tên là: {hoten},\nSDT: {sdt}, \nEmai: {email}, \nMST Công ty: {mst}, \nTên Công ty: {orgname}, \nĐịa chỉ: {diachi},\nDịch vụ KD: {orgcat}, \nTên miền trên Haravan: {myharavan_domain}, \nĐây là email gửi từ động từ app HDDT trên Haravan , tôi có quan tâm đến gói hóa đơn '{goi_hd}'. Nếu nhận được email hãy tư vấn giúp tôi nhé!!!";
            SmtpServer.Port = port;
            SmtpServer.Credentials = new System.Net.NetworkCredential(FromMail, password);
            SmtpServer.EnableSsl = ssl;
            SmtpServer.Send(mail);
        }

    }
}
