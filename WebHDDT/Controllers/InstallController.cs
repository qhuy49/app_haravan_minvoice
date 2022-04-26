using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using WebHDDT.Models;
namespace WebHDDT.Controllers
{
    using RestSharp;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    public class InstallController : Controller
    {
        //private static string ID_ORD;
        Minvoice_HaravanEntities3 db = new Minvoice_HaravanEntities3();

        // GET: Install
        private static string Url_first = ConfigurationManager.AppSettings["Url_first"];
        private static string token_login = "";
        private static string URL_CallBack = ConfigurationManager.AppSettings["URL_CallBack"];
        private static string nonce = ConfigurationManager.AppSettings["nonce"];

        private static string URL_CallBack_install = ConfigurationManager.AppSettings["URL_CallBack_install"];
        private static string app_id = ConfigurationManager.AppSettings["app_id"];
        private static string app_secret = ConfigurationManager.AppSettings["app_secret"];

        private static string scope_login = ConfigurationManager.AppSettings["scope_login"];
        private static string scope_install = ConfigurationManager.AppSettings["scope_install"];

        private static string URL_getACTO = ConfigurationManager.AppSettings["URL_getACTO"];
        private static string URL_GETINFO = ConfigurationManager.AppSettings["URL_GETINFO"];
        private static string URL_GET_ORDER_DETAILS = ConfigurationManager.AppSettings["URL_GET_ORDER_DETAILS"];

        private static string URL_GET_THEMES = ConfigurationManager.AppSettings["URL_GET_THEMES"];
        private static string URL_UPDATE_CART = ConfigurationManager.AppSettings["URL_UPDATE_CART"];

        private static string username = ConfigurationManager.AppSettings["username"];
        private static string pass = ConfigurationManager.AppSettings["pass"];
        private static string ma_dvcs = ConfigurationManager.AppSettings["ma_dvcs"];
        private static string url_save = ConfigurationManager.AppSettings["URL_SAVE"];
        private static string url_login = ConfigurationManager.AppSettings["URL_LOGIN"];

        public string mst = "";


        // GET: Install
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Login(string orgid, string code, string id_token)
        {

            //kiểm tra có session không ?
            if (Session["expired"] != null)
            {
                // nếu có thì còn hạn hay không ?
                if (DateTime.Parse(Session["expired"].ToString()) >= DateTime.Now)
                {
                    //nếu còn hạn thì orgid có null không?
                    if (!string.IsNullOrWhiteSpace(orgid))
                    {
                        //nếu không null thì ktra có bằng orgid trong session không?
                        if (Session["orgid"].ToString() == orgid)
                        {
                            // kiểm tra xem có link trang hóa đơn chưa
                            User_info data_link = db.User_info.SingleOrDefault(n => n.orgid == orgid);
                            var ketqua = "";
                            //kiểm tra có dữ liệu data_link không ?
                            if (data_link != null)
                            {
                                if (data_link.link != null && data_link.link != "")
                                {
                                    // có thì đưa đến trang hđ

                                    Session["link"] = data_link.link;
                                    return Redirect(data_link.link);
                                }
                                // chưa có thì kiểm tra xem có mst chưa
                                else
                                {
                                    // kiểm tra có mst chưa
                                    if (data_link.mst != "" && data_link.mst != null)
                                    {
                                        // nếu có thì gọi api login kiểm tra có TBPH chưa
                                        Session["mst"] = data_link.mst;
                                        string link = $@"https://{data_link.mst}.minvoice.com.vn/api/Account/Login";
                                        var webClient = new WebClient
                                        {
                                            Encoding = Encoding.UTF8
                                        };
                                        webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
                                        JObject json = new JObject
                                        {
                                        {"username",username },
                                        {"password", pass },
                                        {"ma_dvcs", ma_dvcs }
                                        };
                                        ketqua = webClient.UploadString(link, json.ToString());

                                        JObject jObject = JObject.Parse(ketqua);
                                        if (jObject.ContainsKey("token") || jObject.ContainsKey("error"))
                                        {
                                            data_link.link = $"https://{mst}.minvoice.com.vn";
                                            // lưu link vô DB
                                            db.SaveChanges();

                                            Session["link"] = data_link.link;

                                            return Redirect(data_link.myharavan_domain);
                                            //string error = (string)jObject["inv_invoiceNumber"];

                                        }
                                        // chưa có TBPH hoặc user sai
                                        return RedirectToAction("Index", "Home");

                                    }
                                    // chưa có mst

                                    //đưa về trang chủ
                                    return Redirect("/");
                                }
                            }

                            return Redirect("/");

                        }
                        //nếu không bằng orgid trong session thì xóa hết thông tin và bắt đn lại
                        ///////////////////////////////////////////////////////////////////////////////////////////////
                        else
                        {
                            Session["address1"] = null;
                            Session["province"] = null;
                            Session["country"] = null;
                            Session["customer_email"] = null;
                            Session["myharavan_domain"] = null;
                            Session["phone"] = null;
                            Session["shop_owner"] = null;
                            Session["orgid"] = null;
                            //  Session["id_token"] = null;
                            //  Session["orgid"] = null;
                            Session["expired"] = null;
                            Session["link"] = null;
                            Session["mst"] = null;
                            Session["id_order"] = null;
                            Session["user_mst"] = null;
                            Session["user_username"] = null;
                            Session["user_password"] = null;
                            Session["authorize"] = null;
                        }
                    }
                    // orgid null thì xét tiếp
                    else
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////////////
                        //nếu có session orgid thì cho về trang chủ

                        //Session["orgid"] chỉ có được khi đã từng đăng nhập thành công!!!!
                        if (Session["orgid"] != null)
                        {
                            //orgid null
                            string session_org = Session["orgid"].ToString();
                            User_info data_link = db.User_info.SingleOrDefault(n => n.orgid == session_org);
                            var ketqua = "";
                            //kiểm tra có dữ liệu data_link không ?
                            if (data_link != null)
                            {
                                if (data_link.link != null && data_link.link != "")
                                {
                                    // có thì đưa đến trang hđ

                                    Session["link"] = data_link.link;
                                    return Redirect(data_link.myharavan_domain);
                                }
                                // chưa có thì kiểm tra xem có mst chưa
                                else
                                {
                                    // kiểm tra có mst chưa
                                    if (data_link.mst != "" && data_link.mst != null)
                                    {
                                        // nếu có thì gọi api login kiểm tra có TBPH chưa
                                        Session["mst"] = data_link.mst;
                                        string link = $@"https://{data_link.mst}.minvoice.com.vn/api/Account/Login";
                                        var webClient = new WebClient
                                        {
                                            Encoding = Encoding.UTF8
                                        };
                                        webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
                                        JObject json = new JObject
                                        {
                                        {"username",username },
                                        {"password", pass },
                                        {"ma_dvcs",ma_dvcs}
                                        };
                                        ketqua = webClient.UploadString(link, json.ToString());

                                        JObject jObject = JObject.Parse(ketqua);
                                        if (jObject.ContainsKey("token") || jObject.ContainsKey("error"))
                                        {
                                            data_link.link = $"https://{mst}.minvoice.com.vn";
                                            // lưu link vô DB
                                            db.SaveChanges();

                                            Session["link"] = data_link.link;

                                            return Redirect(data_link.myharavan_domain);
                                            //string error = (string)jObject["inv_invoiceNumber"];

                                        }
                                        // chưa có TBPH hoặc user sai
                                        return RedirectToAction("Index", "Home");

                                    }
                                    // chưa có mst

                                    //đưa về trang chủ
                                    return Redirect("/");
                                }
                            }

                            return Redirect("/");





                        }
                        if (Session["sid_install"] != null)
                        {
                            return Redirect("/");
                        }
                        return Redirect("/");
                    }



                }
                ///////////////////////////////////////////////////////////////////////////////////////////////
                // hết hạn đăng nhập cho đăng nhập lại
                else
                {
                    // hết phiên đăng nhập
                    Session["address1"] = null;
                    Session["province"] = null;
                    Session["country"] = null;
                    Session["customer_email"] = null;
                    Session["myharavan_domain"] = null;
                    Session["phone"] = null;
                    Session["shop_owner"] = null;
                    Session["expired"] = null;
                    // Session["id_token"] = null;
                    Session["orgid"] = null;
                    Session["link"] = null;
                    Session["mst"] = null;
                    Session["id_order"] = null;
                    Session["user_mst"] = null;
                    Session["user_username"] = null;
                    Session["user_password"] = null;
                    Session["authorize"] = null;
                    // xoa het thong tin 
                }
            }


            if (!string.IsNullOrWhiteSpace(orgid))
            {
                if (Session["orgid"] == null)
                {
                    //kiểm tra id_order có null k, nếu không null thì lưu vô session
                    //

                    return Redirect($"{Url_first}&scope={scope_login}&client_id={app_id}&redirect_uri={URL_CallBack}&nonce={nonce}&orgid={orgid}");
                }

            }
            else
            {
                if (string.IsNullOrWhiteSpace(orgid) && string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(id_token))
                {


                    return Redirect($"{Url_first}&scope={scope_login}&client_id={app_id}&redirect_uri={URL_CallBack}&nonce={nonce}");
                }
            }

            if (!string.IsNullOrWhiteSpace(id_token))
            {
                token_login = id_token;


                //  var model = new HaravanLoginModel();

                var obj = DeserializeObjectToken(id_token);
                JArray arr = new JArray();

                arr = obj["role"];
                string role = arr[0].ToString();
                //lấy orgid
                string orgid1 = obj["orgid"];
                string sid = obj["sid"];

                // lấy accesstoken từ DB ra để kiểm tra sid đã cài app chưa
                //var ACTO_install = from AC in db.Info_install where AC.sid == sid select AC.access_token;
                User_info ttuser = db.User_info.SingleOrDefault(n => n.orgid == orgid1);



                // Kiểm tra xem user đã cài app chưa
                Info_install ACTO_install = db.Info_install.SingleOrDefault(n => n.orgid == orgid1);
                if (ACTO_install != null)
                {
                    if (ACTO_install.access_token != null && ACTO_install.access_token != "")
                    {
                        string author = $"Bearer {ACTO_install.access_token}";
                        // gọi hàm api kiểm tra xem app còn được cài đặt không
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var client = new RestClient(URL_GETINFO);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Authorization", author);
                        request.AddParameter("application/json", "", ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                        HttpStatusCode statusCode = response.StatusCode;
                        int numericStatusCode = (int)statusCode;
                        //  JObject object_res = new JObject();
                        if (numericStatusCode == 200)
                        {
                            var object_res = JObject.Parse(response.Content);
                            var object_res2 = JObject.Parse(object_res.GetValue("shop").ToString());
                            //   TempData["address1"]
                            Session["address1"] = object_res2.GetValue("address1").ToString();
                            Session["province"] = object_res2.GetValue("province").ToString();
                            Session["country"] = object_res2.GetValue("country").ToString();
                            Session["customer_email"] = object_res2.GetValue("customer_email").ToString();
                            Session["myharavan_domain"] = object_res2.GetValue("myharavan_domain").ToString();
                            Session["phone"] = object_res2.GetValue("phone").ToString();
                            Session["shop_owner"] = object_res2.GetValue("shop_owner").ToString();

                            if (ttuser == null)
                            {



                                User_info user = new User_info();
                                user.id_token = id_token;
                                user.aud = obj["aud"];
                                user.auth_time = obj["auth_time"];
                                user.c_hash = obj["c_hash"];
                                user.exp = obj["exp"];
                                user.iat = obj["iat"];
                                user.idp = obj["idp"];
                                user.iss = obj["iss"];
                                user.name = obj["name"];
                                user.middle_name = obj["middle_name"];
                                user.orgcat = obj["orgcat"];
                                user.orgname = obj["orgname"];
                                user.orgid = obj["orgid"];
                                user.sid = obj["sid"];
                                user.sub = obj["sub"];
                                user.email = obj["email"];
                                user.myharavan_domain = object_res2.GetValue("myharavan_domain").ToString()+"/adminv2/orders";
                                user.customer_email = object_res2.GetValue("customer_email").ToString();
                                user.phone = object_res2.GetValue("phone").ToString();
                                db.User_info.Add(user);
                                db.SaveChanges();
                            }

                            if (ttuser != null)
                            {
                                Session["session_user"] = ttuser;
                                mst = ttuser.mst ?? "";
                            }
                            else
                            {
                                User_info ttuser1 = db.User_info.SingleOrDefault(n => n.orgid == orgid1);
                                mst = ttuser1.mst ?? "";
                                Session["session_user"] = ttuser1;
                            }
                            Session["sid_install"] = sid;
                            Session["expired"] = DateTime.Now.AddMinutes(60);
                            Session["orgid"] = orgid1;
                            Session["mst"] = mst;
                            Session["link"] = "";
                            //  kiểm tra xem mã đơn hàng có đc lưu tạm không?/////////////////////////////////////////////////////////////////////////
                            var check_order = await Check_Order(orgid1);

                            if (check_order != null && check_order.id_order != "")
                            {

                                // nếu có id đơn hàng , đưa về action lấy chi tiết đơn hàng
                                //Session["id_order"];
                                //return RedirectToAction("LoadingGetOrder", "Order", new { orgid = orgid1, id = check_order.ToString() });
                                return RedirectToAction("GetOrderDetails", new { orgid = orgid1, id = check_order.id_order.ToString() , inv_invoicecode_id =check_order.inv_invoicecode_id.ToString(), inv_invoiceseries=check_order.inv_invoiceseries.ToString(), mau_so=check_order.mau_so.ToString(), tt78= check_order.tt78 });
                            }
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            //User_info data_mst = db.User_info.SingleOrDefault(n => n.orgid == orgid1);
                            // lấy mst từ db kiểm tra TBPH
                            if (mst != "")
                            {
                                User_info data_link = db.User_info.SingleOrDefault(n => n.orgid == orgid1);
                                // GỌI API KIỂM TRA TBPH
                                var ketqua = "";
                                try
                                {
                                    if (data_link.link == null && data_link.link == "")
                                    {


                                        string link = $@"https://{mst}.minvoice.com.vn/api/Account/Login";
                                        var webClient = new WebClient
                                        {
                                            Encoding = Encoding.UTF8
                                        };
                                        webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
                                        JObject json = new JObject
                                        {
                                         {"username",username },
                                        {"password", pass },
                                        {"ma_dvcs",ma_dvcs}
                                        };
                                        ketqua = webClient.UploadString(link, json.ToString());

                                        JObject jObject = JObject.Parse(ketqua);
                                        if (jObject.ContainsKey("token") || jObject.ContainsKey("error"))
                                        {
                                            data_link.link = $"https://{mst}.minvoice.com.vn";

                                            db.SaveChanges();
                                            //link ok lưu session
                                            Session["link"] = data_link.link;
                                            return Redirect(data_link.myharavan_domain);
                                            //string error = (string)jObject["inv_invoiceNumber"];


                                        }
                                        // link hư hoặc không ok
                                        //??
                                        return RedirectToAction("Index", "Home");


                                    }
                                    else
                                    {
                                        Session["link"] = data_link.link;
                                        return Redirect(data_link.myharavan_domain);
                                    }
                                }
                                catch (Exception e)
                                {

                                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                                }
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        // xóa access_token của ng dùng đã gỡ app
                        else
                        {
                            if (numericStatusCode == 401)
                            {
                                db.Info_install.Remove(ACTO_install);
                                if (ttuser != null)
                                {
                                    db.User_info.Remove(ttuser);
                                }

                                db.SaveChanges();

                                // bắt cài lại app với quyền admin
                                if (role == "admin")
                                {
                                    return Redirect($"{Url_first}&scope={scope_install}&client_id={app_id}&redirect_uri={URL_CallBack_install}&nonce={nonce}&orgid={orgid1}"); /// lay ac
                                }
                                else
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                }
                            }

                        }




                    }

                }
                else
                {
                    //Info_install ACTO_install = db.Info_install.SingleOrDefault(n => n.sid == sid);
                    //if ()
                    //{
                    if (ttuser != null)
                    {
                        db.User_info.Remove(ttuser);
                    }

                    db.SaveChanges();
                    //}
                    if (role == "admin")
                    {
                        return Redirect($"{Url_first}&scope={scope_install}&client_id={app_id}&redirect_uri={URL_CallBack_install}&nonce={nonce}&orgid={orgid1}"); /// lay ac
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                }

            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);


        }

        public ActionResult GrandService(string code, string id_token)
        {
            if (!string.IsNullOrWhiteSpace(id_token))
            {
                // var context = HttpContext.Request;
                var object1 = DeserializeObjectToken(id_token);
                string orgid = object1["orgid"];
                string sid_install = object1["sid"];
                // lưu session sid của user vừa cài app và để xử lý khi ng dùng logout
                Session["sid_install"] = sid_install;

                if (!string.IsNullOrWhiteSpace(code))
                {
                    //var content = new FormUrlEncodedContent(new[]
                    //{
                    //new KeyValuePair<string, string>("username", email),
                    //new KeyValuePair<string, string>("password", password)
                    //});
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // gọi api lấy access_token và save vào DB
                    var client = new RestClient(URL_getACTO);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("code", code.ToString());
                    request.AddParameter("client_id", app_id);
                    request.AddParameter("client_secret", app_secret);
                    request.AddParameter("grant_type", "authorization_code");
                    request.AddParameter("redirect_uri", URL_CallBack_install);

                    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                    IRestResponse response = client.Execute(request);

                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    //  JObject object_res = new JObject();
                    if (numericStatusCode == 200)
                    {


                        var object_res = JObject.Parse(response.Content);
                        string access_token = object_res.GetValue("access_token").ToString();

                        //Lưu thông tin vào bảng user_install
                        Info_install info_Install = new Info_install();
                        info_Install.sid = sid_install;
                        info_Install.code = code;
                        info_Install.access_token = access_token;
                        info_Install.id_token = id_token;
                        info_Install.orgid = orgid;
                        db.Info_install.Add(info_Install);

                        db.SaveChanges();

                        //JWT token lúc đăng nhập để lấy thông tin user
                        var obj = DeserializeObjectToken(token_login);

                        string author = $"Bearer {access_token}";
                        // gọi hàm api kiểm tra xem app còn được cài đặt không
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var client1 = new RestClient(URL_GETINFO);
                        client1.Timeout = -1;
                        var request1 = new RestRequest(Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("Authorization", author);
                        request1.AddParameter("application/json", "", ParameterType.RequestBody);
                        IRestResponse response1 = client1.Execute(request1);

                        HttpStatusCode statusCode1 = response1.StatusCode;
                        int numericStatusCode1 = (int)statusCode1;
                        if (numericStatusCode1 == 200)
                        {
                            var object_res_view = JObject.Parse(response1.Content);
                            var object_res_view2 = JObject.Parse(object_res_view.GetValue("shop").ToString());
                            Session["address1"] = object_res_view2.GetValue("address1").ToString();
                            Session["province"] = object_res_view2.GetValue("province").ToString();
                            Session["country"] = object_res_view2.GetValue("country").ToString();
                            Session["customer_email"] = object_res_view2.GetValue("customer_email").ToString();
                            Session["myharavan_domain"] = object_res_view2.GetValue("myharavan_domain").ToString();
                            Session["phone"] = object_res_view2.GetValue("phone").ToString();
                            Session["shop_owner"] = object_res_view2.GetValue("shop_owner").ToString();

                            // Lưu thông tin vào bảng user_info
                            User_info ttuser = db.User_info.SingleOrDefault(n => n.orgid == orgid);
                            if (ttuser == null)
                            {


                                User_info user = new User_info();
                                user.id_token = token_login;
                                user.aud = obj["aud"];
                                user.auth_time = obj["auth_time"];
                                user.c_hash = obj["c_hash"];
                                user.exp = obj["exp"];
                                user.iat = obj["iat"];
                                user.idp = obj["idp"];
                                user.iss = obj["iss"];
                                user.name = obj["name"];
                                user.middle_name = obj["middle_name"];
                                user.orgcat = obj["orgcat"];
                                user.orgname = obj["orgname"];
                                user.orgid = obj["orgid"];
                                user.sid = obj["sid"];
                                user.sub = obj["sub"];
                                user.email = obj["email"];
                                user.myharavan_domain = object_res_view2.GetValue("myharavan_domain").ToString() +"/adminv2/orders";
                                user.customer_email = object_res_view2.GetValue("customer_email").ToString();
                                user.phone = object_res_view2.GetValue("phone").ToString();
                                db.User_info.Add(user);
                                db.SaveChanges();
                            }

                            if (ttuser != null)
                            {
                                Session["session_user"] = ttuser;
                                mst = ttuser.mst ?? "";
                            }
                            else
                            {
                                User_info ttuser1 = db.User_info.SingleOrDefault(n => n.orgid == orgid);
                                Session["session_user"] = ttuser1;
                                mst = ttuser1.mst ?? "";
                            }
                            // thiết lập session
                            Session["access_token"] = access_token;
                            Session["expired"] = DateTime.Now.AddMinutes(60);
                            Session["orgid"] = orgid;
                            Session["mst"] = mst;
                            Session["link"] = "";

                            //  kiểm tra xem mã đơn hàng có đc lưu tạm không?/////////////////////////////////////////////////////////////////////////
                            //string check_order = await Check_Order(orgid1);

                            //if (check_order != "")
                            //{

                            //    // nếu có id đơn hàng , đưa về action lấy chi tiết đơn hàng
                            //    //Session["id_order"];
                            //    return RedirectToAction("GetOrderDetails", new { orgid = orgid1, id = check_order.ToString() });
                            //}

                            // gọi api cài đặt code html cart
                            string author_theme = $"Bearer {access_token}";
                            string id_theme = "";
                            // gọi hàm api kiểm tra xem app còn được cài đặt không
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client_theme = new RestClient(URL_GET_THEMES);
                            client_theme.Timeout = -1;
                            var request_theme = new RestRequest(Method.GET);
                            request_theme.AddHeader("Authorization", author_theme);
                            request_theme.AddHeader("Content-Type", "application/json");
                            IRestResponse response_theme = client_theme.Execute(request_theme);

                            HttpStatusCode statusCode_theme = response_theme.StatusCode;
                            int numericStatusCode_theme = (int)statusCode_theme;
                            // get theme ID thành công
                            if (numericStatusCode_theme == 200)
                            {
                                var object_res_theme = JObject.Parse(response_theme.Content);
                                var object_res_themelist = JArray.Parse(object_res_theme.GetValue("themes").ToString());

                                if (object_res_themelist.Count > 0)
                                {
                                    
                                    foreach (var item in object_res_themelist)
                                    {
                                        if (item["role"].ToString() == "main")
                                        {
                                            id_theme = item["id"].ToString();
                                        }
                                    }
                                }



                            }
                            // gọi api add code html vào cart của shop

                            //URL_GET_ORDER_DETAILS + "" + id + ".json"
                            var client_cart = new RestClient(URL_UPDATE_CART +""+id_theme+"/assets.json");
                            client_cart.Timeout = -1;
                            var request_cart = new RestRequest(Method.PUT);
                            request_cart.AddHeader("Authorization", author_theme);
                            request_cart.AddHeader("Content-Type", "application/json");
                            request_cart.AddParameter("application/json", "{\r\n    \"asset\": {\r\n        \"content_type\": \"text/x-liquid\",\r\n        \"key\": \"snippets/bill-minvoice.liquid\",\r\n        \"value\": \"<div class=\\\"col-md-12\\\">\\n\\t<div class=\\\"r-bill\\\">\\n\\t\\t<div class=\\\"checkbox\\\">\\n\\t\\t\\t<input type=\\\"hidden\\\" name=\\\"attributes[invoice]\\\" id=\\\"re-checkbox-bill\\\" value=\\\"no\\\">\\n\\t\\t\\t<input type=\\\"checkbox\\\" id=\\\"checkbox-bill\\\" value=\\\"yes\\\" class=\\\"regular-checkbox\\\">\\n\\t\\t\\t<label for=\\\"checkbox-bill\\\" class=\\\"title\\\">Xuất hoá đơn công ty</label>\\n\\t\\t</div>\\n\\t\\t<div class=\\\"bill-field\\\" style=\\\"display: none;\\\">\\n\\t\\t\\t<div class=\\\"form-group\\\">\\n\\t\\t\\t\\t<input type=\\\"text\\\" class=\\\"form-control val-f\\\" name=\\\"attributes[invoice_namebuyer]\\\" value=\\\"\\\" placeholder=\\\"Tên người mua...\\\">\\n\\t\\t\\t</div>\\t\\n\\t\\t\\t<div class=\\\"form-group\\\">\\n\\t\\t\\t\\t<input type=\\\"text\\\" class=\\\"form-control val-f\\\" name=\\\"attributes[invoice_namelegal]\\\" value=\\\"\\\" placeholder=\\\"Tên công ty...\\\">\\n\\t\\t\\t</div>\\t\\n\\t\\t\\t<div class=\\\"form-group\\\">\\n\\t\\t\\t\\t<input type=\\\"number\\\" pattern=\\\".{10,}\\\" onkeydown=\\\"return FilterInput(event)\\\" onpaste=\\\"handlePaste(event)\\\" class=\\\"form-control val-f val-n\\\" name=\\\"attributes[invoice_tax]\\\" value=\\\"\\\" placeholder=\\\"Mã số thuế...\\\">\\n\\t\\t\\t</div>\\n\\t\\t\\t<div class=\\\"form-group\\\">\\n\\t\\t\\t\\t<input type=\\\"text\\\" class=\\\"form-control val-f\\\" name=\\\"attributes[invoice_address]\\\" value=\\\"\\\" placeholder=\\\"Địa chỉ công ty...\\\">\\n\\t\\t\\t</div>\\n\\t\\t\\t<div class=\\\"form-group\\\">\\n\\t\\t\\t\\t<input type=\\\"email\\\" class=\\\"form-control val-f\\\" name=\\\"attributes[invoice_email]\\\" value=\\\"\\\" placeholder=\\\"Địa chỉ email công ty...\\\">\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t</div>\\n</div>\\n<script>\\n\\t$('body').on('change','#checkbox-bill',function(){\\n\\t\\tdebugger\\n\\t\\tvar checked=$('#checkbox-bill');\\n\\t\\tif(checked.is(\\\":checked\\\")){\\n\\t\\t\\t$('#re-checkbox-bill').val('yes');\\n\\t\\t\\t$('.bill-field').show();\\n\\t\\t}else{\\n\\t\\t\\t$('#re-checkbox-bill').val('no');\\n\\t\\t\\t$('.bill-field').hide();\\n\\t\\t}\\n\\t})\\n\\t\\n</script>\"\r\n    }\r\n}", ParameterType.RequestBody);
                            IRestResponse response_cart = client_cart.Execute(request_cart);

                            HttpStatusCode statusCode_cart = response_cart.StatusCode;
                            int numericStatusCode_cart = (int)statusCode_cart;
                            // get theme ID thành công
                            if (numericStatusCode_cart == 201)
                            {



                                if (mst != "")
                                {
                                    User_info data_link = db.User_info.SingleOrDefault(n => n.orgid == orgid);
                                    // GỌI API KIỂM TRA TBPH
                                    var ketqua = "";
                                    try
                                    {
                                        if (data_link.link == null && data_link.link == "")
                                        {


                                            string link = $@"https://{mst}.minvoice.com.vn/api/Account/Login";
                                            var webClient = new WebClient
                                            {
                                                Encoding = Encoding.UTF8
                                            };
                                            webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
                                            JObject json = new JObject
                                        {
                                         {"username",username },
                                        {"password", pass },
                                        {"ma_dvcs",ma_dvcs}
                                        };
                                            ketqua = webClient.UploadString(link, json.ToString());

                                            JObject jObject = JObject.Parse(ketqua);
                                            if (jObject.ContainsKey("token") || jObject.ContainsKey("error"))
                                            {
                                                data_link.link = $"https://{mst}.minvoice.com.vn";

                                                db.SaveChanges();
                                                //link ok lưu session
                                                Session["link"] = data_link.link;
                                                return Redirect(data_link.myharavan_domain);
                                                //string error = (string)jObject["inv_invoiceNumber"];


                                            }
                                            // link hư hoặc không ok
                                            //??
                                            return RedirectToAction("Index", "Home");


                                        }
                                        else
                                        {
                                            Session["link"] = data_link.link;
                                            return Redirect(data_link.myharavan_domain);
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                        return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                                    }
                                }
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //info_Install.access_token;



        }

        private dynamic DeserializeObjectToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token).ToString();
                jsonToken = jsonToken.Replace(jsonToken.Substring(0, jsonToken.IndexOf("}.") + 2), "");
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonToken);
                return obj;
            }
            return null;
        }

        public ActionResult front_logout(string iss, string orgid, string sid)
        {
            var a = Session["sid_install"];
            var b = Session["orgid"];
            if (Session["sid_install"] != null && Session["orgid"] != null)
            {


                if (Session["sid_install"].ToString() == sid && Session["orgid"].ToString() == orgid)
                {
                    Session["address1"] = null;
                    Session["province"] = null;
                    Session["country"] = null;
                    Session["customer_email"] = null;
                    Session["myharavan_domain"] = null;
                    Session["phone"] = null;
                    Session["shop_owner"] = null;

                    Session["sid_install"] = null;
                    Session["orgid"] = null;
                    Session["session_user"] = null;
                    Session["expired"] = null;
                    Session["link"] = null;
                    Session["mst"] = null;
                }
            }
            return Redirect("https://accounts.haravan.com/connect/endsession");

        }

        public async Task<Temp_Order> Check_Order(string orgid)
        {
            var MaxDate = (from n in db.Temp_Order where n.orgid == orgid select n.date_add).Max();
            Temp_Order _temp_Order = db.Temp_Order.SingleOrDefault(n => n.orgid == orgid && n.date_add == MaxDate);
            if (_temp_Order != null)
            {
                if (_temp_Order.id_order != null)
                {
                    return _temp_Order;
                }
            }
            return null;

        }


        public async Task<bool> Add_TemOrder(string orgid, string id, string inv_invoicecode_id, string inv_invoiceseries, string mau_so, bool tt78)
        {
            //add id_order vô bảng tạm
            //await Task.Run();

            Temp_Order tem_order = db.Temp_Order.SingleOrDefault(n => n.id_order == id && n.orgid == orgid);
            if (tem_order == null)
            {
                Temp_Order temp = new Temp_Order()
                {
                    id_order = id,
                    orgid = orgid,
                    inv_invoicecode_id = inv_invoicecode_id,
                    inv_invoiceseries = inv_invoiceseries,
                    mau_so = mau_so,
                    date_add = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                    tt78 = tt78
                };
                db.Temp_Order.Add(temp);
                db.SaveChanges();
                return true;
            }

            return false;

        }

        // nhận thông tin về id của đơn hàng và get chi tiết đơn hàng
        public async Task<ActionResult> GetOrderDetails(string orgid, string id, string inv_invoicecode_id, string inv_invoiceseries, string mau_so, bool tt78)
        {

            if (!string.IsNullOrWhiteSpace(orgid) && !string.IsNullOrWhiteSpace(id))
            {
                //kiểm tra có session không ?
                if (Session["expired"] != null)
                {
                    // nếu có thì còn hạn hay không ?
                    if (DateTime.Parse(Session["expired"].ToString()) >= DateTime.Now)
                    {
                        //nếu còn hạn thì orgid có null không?
                        if (!string.IsNullOrWhiteSpace(orgid))
                        {
                            //nếu không null thì ktra có bằng orgid trong session không?
                            if (Session["orgid"].ToString() == orgid)
                            {
                                // lấy thông tin chi tiết đơn hàng

                                //xóa temp order 
                                //remove id_odder sau khi post xong
                                Temp_Order delete_tempOder = db.Temp_Order.SingleOrDefault(n => n.id_order == id && n.orgid == orgid);
                                if (delete_tempOder != null)
                                {
                                    db.Temp_Order.Remove(delete_tempOder);
                                    db.SaveChanges();
                                }
                                var data_api = await PostAPI_Haravan(orgid, id);
                                if (data_api == null)
                                {
                                    return Content("<script language='javascript' type='text/javascript'>alert('Xuất hóa đơn thất bại do không tìm thấy dữ liệu đơn hàng!');  window.close();</script>");
                                }

                                if (data_api != null)
                                {
                                    if (data_api.ContainsKey("errors"))
                                    {
                                        return Content("<script language='javascript' type='text/javascript'>alert('" + data_api.GetValue("errors").ToString() + "');  window.close(); close();</script>");
                                    }
                                    // xử lý gọi api Minvoice xuất hóa đơn
                                    if (data_api.ContainsKey("order"))
                                    {
                                        // lấy thông báo phát hành
                                        var note_order = JObject.Parse(data_api.GetValue("order").ToString());
                                        var note_attributes = JArray.Parse(note_order["note_attributes"].ToString());

                                        var mes = new JObject();
                                        mes.Add("name", "invoice_info");
                                        string username_1 = "";
                                        string password_1 = "";
                                        string taxcode_1 = "";
                                        ////string inv_invoiceseries = "";
                                        //string mau_hd = "";
                                        //string inv_invoicecode_id = "";
                                        if (note_attributes.Count > 0)
                                        {
                                            if (note_attributes.ToString().Contains("xuất hóa đơn điện tử thành công"))
                                            {
                                                return Content("<script language='javascript' type='text/javascript'>alert('Đơn hàng này đã xuất hóa đơn điện tử rồi!');  window.close();</script>");
                                            }
                                        }
                                        if (note_order["cancelled_status"].ToString() == "cancelled")
                                        {
                                            return Content("<script language='javascript' type='text/javascript'>alert('Đơn hàng này đã hủy không thể xuất hóa đơn!');  window.close();</script>");
                                        }
                                        //if (note_attributes.Count > 0)
                                        //{
                                        //    foreach (var attribute in note_attributes)
                                        //    {
                                        //        switch (attribute["name"].ToString().ToLower())
                                        //        {
                                        //            case "username": username = attribute["value"].ToString(); break;

                                        //            case "password": password = attribute["value"].ToString(); break;

                                        //            case "taxcode": taxcode = attribute["value"].ToString().Contains("-") ? attribute["value"].ToString().Replace("-", "") : attribute["value"].ToString(); break;

                                        //            case "series": inv_invoiceseries = attribute["value"].ToString(); break;

                                        //            case "templateno": mau_hd = attribute["value"].ToString(); break;
                                        //        }
                                        //        //if (attribute["name"].ToString() == "username")
                                        //        //{
                                        //        //    username = attribute["value"].ToString();

                                        //        //}
                                        //    }
                                        //}

                                        //var ctthongbao = db.ctthongbao.SingleOrDefault(n => n.orgid == orgid && n.mau_so == mau_hd && n.ky_hieu == inv_invoiceseries && n.hetso == "0");
                                        //if (ctthongbao != null)
                                        //{
                                        //    inv_invoicecode_id = ctthongbao.ctthongbao_id.ToString();
                                        //}
                                        //else
                                        //{
                                        //    var ctthongbao2 = db.ctthongbao.Where(n => n.orgid == orgid && n.mau_so == mau_hd && n.ky_hieu == inv_invoiceseries && n.hetso == "1").ToList();
                                        //    //lấy tb phát hành


                                        //    var webClient =  SetupWebClient(username, password, taxcode);

                                        //    var ctthongbao_id = await Get_TBPH(webClient, taxcode, orgid, mau_hd, inv_invoiceseries, ctthongbao2);
                                        //    inv_invoicecode_id = ctthongbao_id;

                                        //}
                                        ///////////////////////tt78
                                        var jObjectData = await WebHDDT.Models.JsonConvert.ConvertData(note_order, mau_so, inv_invoiceseries, inv_invoicecode_id,tt78);

                                        //var rsult = webClient.(URL_CallBack,);
                                        //var jObjectData = await WebHDDT.Models.JsonConvert.ConvertData(data_api);
                                        var jArrayData = new JArray { jObjectData };
                                        
                                        var jObjectMainData = new JObject
                                        {
                                            {"windowid", "WIN00187"},

                                            {"editmode", 1},
                                            {"data", jArrayData}
                                        };

                                        var dataRequest = jObjectMainData.ToString();
                                        // gọi api xuất hđ
                                        User_info _userinfo = db.User_info.SingleOrDefault(n=>n.orgid == orgid);
                                        if (_userinfo !=null)
                                        {
                                            username_1 = _userinfo.username;
                                            password_1 = _userinfo.password;
                                            taxcode_1 = _userinfo.mst;
                                        }
                                        var webClient2 =  SetupWebClient(username_1, password_1, taxcode_1);
                                        ///////////////////////////tt78
                                        if (tt78)
                                        {
                                           
                                            var result = await webClient2.UploadStringTaskAsync(url_save.Replace("testapi", taxcode_1), dataRequest);
                                            var resultResponse = JObject.Parse(result);
                                            if (resultResponse["code"].ToString() == "00" && resultResponse["data"] != null)
                                            {

                                                mes.Add("value", "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Đã xuất hóa đơn điện tử thành công!, Số hóa đơn: " + resultResponse["data"]["shdon"].ToString() + " , Ký hiệu: " + resultResponse["data"]["khieu"].ToString() + "");
                                                note_attributes.Add(mes);
                                                // update tags lên đơn hàng trên Haravan
                                                var kq = await API_UpdateOrder_Haravan(orgid, id, note_attributes.ToString());
                                                if (kq == "")
                                                {
                                                    return Content("<script language='javascript' type='text/javascript'>alert('Cập nhật trạng thái xuất hóa đơn thất bại, do không tìm thấy dữ liệu đơn hàng!');  window.close();</script>");
                                                }
                                                if (kq == "422")
                                                {
                                                    return Content("<script language='javascript' type='text/javascript'>alert('Đơn hàng không tồn tại');  window.close();</script>");
                                                }
                                                //if (kq == "OK")
                                                //{

                                                //    // cập nhật thành công
                                                //}
                                                Export_Invoice export_invoice = new Export_Invoice()
                                                {
                                                    orgid = orgid,
                                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                                                    so_don_hang = resultResponse["data"]["sdhang"].ToString(),
                                                    mau_so = mau_so,
                                                    ky_hieu = resultResponse["data"]["khieu"].ToString(),
                                                    inv_invoicenumber = resultResponse["data"]["shdon"].ToString(),
                                                    inv_InvoiceAuth_id = resultResponse["data"]["hoadon68_id"].ToString(),
                                                    inv_invoicecode_id = resultResponse["data"]["cctbao_id"].ToString(),
                                                    json_data = jObjectMainData.ToString(),
                                                    trang_thai_xuat = "Xuất thành công!"
                                                };
                                                db.Export_Invoice.Add(export_invoice);
                                                //update trạng thái hetso="1" khi đã hết số hóa đơn TT78 BỎ
                                                //var code_id = (Guid)resultResponse["data"]["inv_InvoiceCode_id"];
                                                //ctthongbao cttb = db.ctthongbao.SingleOrDefault(n => n.ctthongbao_id == code_id && n.orgid == orgid);
                                                //if (cttb != null)
                                                //{
                                                //    if (cttb.so_luong == int.Parse(resultResponse["data"]["inv_invoiceNumber"].ToString()))
                                                //    {
                                                //        cttb.hetso = "1";
                                                //    }
                                                //}
                                                db.SaveChanges();


                                                return Content("<script language='javascript' type='text/javascript'>alert('Xuất hóa đơn thành công!'); window.close(); window.location.href ='https://" + taxcode_1 + ".minvoice.com.vn'</script>");


                                            }
                                            else
                                            {
                                                // update tags lên đơn hàng trên Haravan 
                                                // thêm số hđ, mẫu số, ký hiệu hdđt lên số đơn hàng haravan
                                                mes.Add("value", "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Xuất hóa đơn điện tử thất bại!, Nguyên nhân: " + "'" + "" + resultResponse["message"] + "" + "'" + "");
                                                note_attributes.Add(mes);

                                                var kq = await API_UpdateOrder_Haravan(orgid, id, note_attributes.ToString());

                                                Export_Invoice export_invoice = new Export_Invoice()
                                                {
                                                    orgid = orgid,
                                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                                                    so_don_hang = !string.IsNullOrEmpty(note_order["id"].ToString()) ? note_order["id"].ToString() : "",
                                                    mau_so = mau_so,
                                                    ky_hieu = inv_invoiceseries,
                                                    inv_invoicecode_id = inv_invoicecode_id,
                                                    json_data = jObjectMainData.ToString(),
                                                    trang_thai_xuat = "Xuất thất bại!"
                                                };
                                                db.Export_Invoice.Add(export_invoice);
                                                db.SaveChanges();
                                                return Content("<script language='javascript' type='text/javascript'>alert('Xuất hóa đơn thất bại do " + resultResponse["message"] + "!'); window.close(); window.close();</script>");
                                            }

                                        }
                                        else //TT32
                                        {
                                            var url = $"https://{taxcode_1}.minvoice.com.vn/api/InvoiceAPI/Save";
                                            var result = await webClient2.UploadStringTaskAsync(url, dataRequest);
                                            var resultResponse = JObject.Parse(result);
                                            if (resultResponse.ContainsKey("ok") && resultResponse.ContainsKey("data"))
                                            {

                                                mes.Add("value", "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Đã xuất hóa đơn điện tử thành công!, Số hóa đơn: " + resultResponse["data"]["inv_invoiceNumber"].ToString() + " , Mẫu số: " + resultResponse["data"]["mau_hd"].ToString() + ", Ký hiệu: " + resultResponse["data"]["inv_invoiceSeries"].ToString() + "");
                                                note_attributes.Add(mes);
                                                // update tags lên đơn hàng trên Haravan
                                                var kq = await API_UpdateOrder_Haravan(orgid, id, note_attributes.ToString());
                                                if (kq == "")
                                                {
                                                    return Content("<script language='javascript' type='text/javascript'>alert('Cập nhật trạng thái xuất hóa đơn thất bại, do không tìm thấy dữ liệu đơn hàng!');  window.close();</script>");
                                                }
                                                if (kq == "422")
                                                {
                                                    return Content("<script language='javascript' type='text/javascript'>alert('Đơn hàng không tồn tại');  window.close();</script>");
                                                }
                                                //if (kq == "OK")
                                                //{

                                                //    // cập nhật thành công
                                                //}
                                                Export_Invoice export_invoice = new Export_Invoice()
                                                {
                                                    orgid = orgid,
                                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                                                    so_don_hang = resultResponse["data"]["so_benh_an"].ToString(),
                                                    mau_so = resultResponse["data"]["mau_hd"].ToString(),
                                                    ky_hieu = resultResponse["data"]["inv_invoiceSeries"].ToString(),
                                                    inv_invoicenumber = resultResponse["data"]["inv_invoiceNumber"].ToString(),
                                                    inv_InvoiceAuth_id = resultResponse["data"]["inv_InvoiceAuth_id"].ToString(),
                                                    inv_invoicecode_id = resultResponse["data"]["inv_InvoiceCode_id"].ToString(),
                                                    json_data = jObjectMainData.ToString(),
                                                    trang_thai_xuat = "Xuất thành công!"
                                                };
                                                db.Export_Invoice.Add(export_invoice);
                                                //update trạng thái hetso="1" khi đã hết số hóa đơn 
                                                var code_id = (Guid)resultResponse["data"]["inv_InvoiceCode_id"];
                                                ctthongbao cttb = db.ctthongbao.SingleOrDefault(n => n.ctthongbao_id == code_id && n.orgid == orgid);
                                                if (cttb != null)
                                                {
                                                    if (cttb.so_luong == int.Parse(resultResponse["data"]["inv_invoiceNumber"].ToString()))
                                                    {
                                                        cttb.hetso = "1";
                                                    }
                                                }
                                                db.SaveChanges();


                                                return Content("<script language='javascript' type='text/javascript'>alert('Xuất hóa đơn thành công!'); window.close(); window.location.href ='https://" + taxcode_1 + ".minvoice.com.vn'</script>");


                                            }
                                            if (resultResponse.ContainsKey("error"))
                                            {
                                                // update tags lên đơn hàng trên Haravan 
                                                // thêm số hđ, mẫu số, ký hiệu hdđt lên số đơn hàng haravan
                                                mes.Add("value", "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Xuất hóa đơn điện tử thất bại!, Nguyên nhân: " + "'" + "" + resultResponse["error"] + "" + "'" + "");
                                                note_attributes.Add(mes);

                                                var kq = await API_UpdateOrder_Haravan(orgid, id, note_attributes.ToString());

                                                Export_Invoice export_invoice = new Export_Invoice()
                                                {
                                                    orgid = orgid,
                                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                                                    so_don_hang = !string.IsNullOrEmpty(note_order["id"].ToString()) ? note_order["id"].ToString() : "",
                                                    mau_so = mau_so,
                                                    ky_hieu = inv_invoiceseries,
                                                    inv_invoicecode_id = inv_invoicecode_id,
                                                    json_data = jObjectMainData.ToString(),
                                                    trang_thai_xuat = "Xuất thất bại!"
                                                };
                                                db.Export_Invoice.Add(export_invoice);
                                                db.SaveChanges();
                                                return Content("<script language='javascript' type='text/javascript'>alert('Xuất hóa đơn thất bại do " + resultResponse["error"] + "!'); window.close(); window.close();</script>");
                                            }

                                        }
                                       


                                    }



                                    //var node_order = JObject.Parse(data_api.GetValue("order").ToString());
                                    ////   TempData["address1"]
                                    //Session["address1"] = object_res2.GetValue("address1").ToString();
                                    //Session["province"] = object_res2.GetValue("province").ToString();
                                    //Session["country"] = object_res2.GetValue("country").ToString();
                                    //Session["customer_email"] = object_res2.GetValue("customer_email").ToString();
                                    //Session["myharavan_domain"] = object_res2.GetValue("myharavan_domain").ToString();
                                    //Session["phone"] = object_res2.GetValue("phone").ToString();
                                    //Session["shop_owner"] = object_res2.GetValue("shop_owner").ToString();
                                }



                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //return View();
                            }

                            //nếu không bằng orgid trong session thì xóa hết thông tin và bắt đn lại
                            ///////////////////////////////////////////////////////////////////////////////////////////////
                            else
                            {
                                Session["address1"] = null;
                                Session["province"] = null;
                                Session["country"] = null;
                                Session["customer_email"] = null;
                                Session["myharavan_domain"] = null;
                                Session["phone"] = null;
                                Session["shop_owner"] = null;
                                Session["orgid"] = null;
                                //  Session["id_token"] = null;
                                //  Session["orgid"] = null;
                                Session["expired"] = null;
                                Session["link"] = null;
                                Session["mst"] = null;
                            }

                        }

                    }
                    else
                    {
                        // hết phiên đăng nhập
                        Session["address1"] = null;
                        Session["province"] = null;
                        Session["country"] = null;
                        Session["customer_email"] = null;
                        Session["myharavan_domain"] = null;
                        Session["phone"] = null;
                        Session["shop_owner"] = null;
                        Session["expired"] = null;
                        // Session["id_token"] = null;
                        Session["orgid"] = null;
                        Session["link"] = null;
                        Session["mst"] = null;
                        Session["user_mst"] = null;
                        Session["user_username"] = null;
                        Session["user_password"] = null;
                        Session["authorize"] = null;
                        // xoa het thong tin 
                    }
                }
                else
                {
                    var add_order_temp = await Add_TemOrder(orgid, id, inv_invoicecode_id,inv_invoiceseries,mau_so, tt78);
                    return RedirectToAction("Login", new { orgid = orgid });
                    //gọi đến action login để login, trong action login đăt 1 biến bool để ktra id của mã đơn hàng, nếu có id chuyển về action order detail tiếp
                }
            }
            return Redirect("/");
            //public async Task<string> SendAsyncJson(string url, string json , string content)
            //{


            //    Console.WriteLine($"Starting connect {url}");
            //    try
            //    {
            //        HttpClient httpClient = new HttpClient();
            //        var request = new HttpRequestMessage(HttpMethod.Post, url);
            //        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            //        request.Content = httpContent;
            //        var response = await httpClient.SendAsync(request, content );
            //        var rcontent = await response.Content.ReadAsStringAsync();
            //        return rcontent;

            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //        throw e;
            //    }
            //}
        }


        public async Task<JObject> PostAPI_Haravan(string orgid, string id)
        {
            Info_install ACTO_install = db.Info_install.SingleOrDefault(n => n.orgid == orgid);
            if (ACTO_install != null)
            {
                if (ACTO_install.access_token != null && ACTO_install.access_token != "")
                {

                    string author = $"Bearer {ACTO_install.access_token}";
                    // gọi hàm api láy thông tin chi tiết đơn hàng
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    var client = new RestClient(URL_GET_ORDER_DETAILS + "" + id + ".json");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", author);
                    request.AddParameter("application/json", "", ParameterType.RequestBody);
                    IRestResponse response = await client.ExecuteAsync(request);

                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    //  JObject object_res = new JObject();
                    if (numericStatusCode == 200)
                    {
                        var object_res = JObject.Parse(response.Content);
                        return object_res;



                    }
                    if (numericStatusCode == 422)
                    {
                        var object_res = JObject.Parse(response.Content);


                        return object_res;

                    }

                }
            }
            return null;
        }
        public static WebClient SetupWebClient(string username, string password, string taxcode)
        {

            var webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");

            CreateAuthorization(webClient, username, password, taxcode);
            return webClient;
        }
        public static void CreateAuthorization(WebClient webClient, string username, string pass, string taxcode)
        {
            try
            {
                var tokenJson = Login_Minvoice(username, pass, taxcode);
                if (tokenJson != null)
                {
                    var authorization = "Bear " + tokenJson["token"] + ";VP;vi";
                    webClient.Headers[HttpRequestHeader.Authorization] = authorization;

                }

            }
            catch (Exception ex)
            {
                throw new Exception();
                //XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static JObject Login_Minvoice(string username, string password, string taxcode)
        {
            var client = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            JObject json = new JObject
            {
                {"username",username },
                {"password",password },
                {"ma_dvcs","VP" }
            };

            //var urlLogin = BaseConfig.UrlLogin;
            var token = client.UploadString(url_login.Replace("testapi", taxcode), json.ToString());
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            return JObject.Parse(token);
        }

        public async Task<string> Get_TBPH(WebClient webClient, string taxcode, string orgid, string mau_hd, string inv_invoiceseries, List<ctthongbao> ctthongbao2)
        {
            //var webClient = SetupWebClient(username, password, taxcode);
            try
            {


                var result = await webClient.DownloadStringTaskAsync(new Uri("https://" + taxcode + ".minvoice.com.vn/api/System/GetDataReferencesByRefId?refId=RF00027"));
                var resultResponse = JArray.Parse(result);
                if (resultResponse.Count > 0)
                {
                    if (resultResponse.Count == 1)
                    {
                        if (ctthongbao2.Count > 0)
                        {
                            foreach (var jToken in resultResponse)
                            {
                                // đúng với mẫu số ký hiệu truyền sang

                                if (mau_hd == jToken["mau_so"].ToString() && inv_invoiceseries == jToken["ky_hieu"].ToString() && ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false)
                                {
                                    ctthongbao cttb = new ctthongbao
                                    {
                                        ky_hieu = jToken["ky_hieu"].ToString(),
                                        mau_so = jToken["mau_so"].ToString(),
                                        ctthongbao_id = (Guid)jToken["ctthongbao_id"],
                                        so_luong = (int)jToken["so_luong"],
                                        tu_so = jToken["tu_so"].ToString(),
                                        den_so = jToken["den_so"].ToString(),
                                        ngay_bd_sd = (DateTime)jToken["ngay_bd_sd"],
                                        hetso = "0",
                                        orgid = orgid,
                                    };

                                    db.ctthongbao.Add(cttb);
                                    db.SaveChanges();
                                    return ((Guid)jToken["ctthongbao_id"]).ToString();
                                }
                                else
                                {
                                    //k đúng với mẫu số truyền sang
                                    return "";
                                }
                            }
                        }
                        else
                        {
                            foreach (var jToken in resultResponse)
                            {
                                // đúng với mẫu số ký hiệu truyền sang

                                if (mau_hd == jToken["mau_so"].ToString() && inv_invoiceseries == jToken["ky_hieu"].ToString())
                                {
                                    ctthongbao cttb = new ctthongbao
                                    {
                                        ky_hieu = jToken["ky_hieu"].ToString(),
                                        mau_so = jToken["mau_so"].ToString(),
                                        ctthongbao_id = (Guid)jToken["ctthongbao_id"],
                                        so_luong = (int)jToken["so_luong"],
                                        tu_so = jToken["tu_so"].ToString(),
                                        den_so = jToken["den_so"].ToString(),
                                        ngay_bd_sd = (DateTime)jToken["ngay_bd_sd"],
                                        hetso = "0",
                                        orgid = orgid,
                                    };

                                    db.ctthongbao.Add(cttb);
                                    db.SaveChanges();
                                    return ((Guid)jToken["ctthongbao_id"]).ToString();
                                }
                                else
                                {
                                    //k đúng với mẫu số truyền sang
                                    return "";
                                }
                            }
                        }

                    }
                    else
                    {
                        //kq trả về hơn 1 dải hđ
                        if (ctthongbao2.Count > 0)
                        {
                            // nếu ctthongbao2 có giá trị(có dải dã hết số)
                            foreach (var jToken in resultResponse)
                            {

                                if (mau_hd == jToken["mau_so"].ToString() && inv_invoiceseries == jToken["ky_hieu"].ToString() && ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false)
                                {
                                    ctthongbao cttb = new ctthongbao
                                    {
                                        ky_hieu = jToken["ky_hieu"].ToString(),
                                        mau_so = jToken["mau_so"].ToString(),
                                        ctthongbao_id = (Guid)jToken["ctthongbao_id"],
                                        so_luong = (int)jToken["so_luong"],
                                        tu_so = jToken["tu_so"].ToString(),
                                        den_so = jToken["den_so"].ToString(),
                                        ngay_bd_sd = (DateTime)jToken["ngay_bd_sd"],
                                        hetso = "0",
                                        orgid = orgid,
                                    };

                                    db.ctthongbao.Add(cttb);
                                    db.SaveChanges();
                                    return ((Guid)jToken["ctthongbao_id"]).ToString();
                                }
                            }
                            return "";
                        }
                        else
                        {
                            foreach (var jToken in resultResponse)
                            {

                                if (mau_hd == jToken["mau_so"].ToString() && inv_invoiceseries == jToken["ky_hieu"].ToString())
                                {
                                    ctthongbao cttb = new ctthongbao
                                    {
                                        ky_hieu = jToken["ky_hieu"].ToString(),
                                        mau_so = jToken["mau_so"].ToString(),
                                        ctthongbao_id = (Guid)jToken["ctthongbao_id"],
                                        so_luong = (int)jToken["so_luong"],
                                        tu_so = jToken["tu_so"].ToString(),
                                        den_so = jToken["den_so"].ToString(),
                                        ngay_bd_sd = (DateTime)jToken["ngay_bd_sd"],
                                        hetso = "0",
                                        orgid = orgid,
                                    };

                                    db.ctthongbao.Add(cttb);
                                    db.SaveChanges();
                                    return ((Guid)jToken["ctthongbao_id"]).ToString();
                                }
                            }
                            return "";
                        }
                    }

                }
                // chưa có tbph......



                return "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<string> API_UpdateOrder_Haravan(string orgid, string id, string message)
        {
            Info_install ACTO_install = db.Info_install.SingleOrDefault(n => n.orgid == orgid);
            if (ACTO_install != null)
            {
                if (ACTO_install.access_token != null && ACTO_install.access_token != "")
                {

                    string author = $"Bearer {ACTO_install.access_token}";
                    // gọi hàm api láy thông tin chi tiết đơn hàng
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    var client = new RestClient(URL_GET_ORDER_DETAILS + "" + id + ".json");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.PUT);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", author);
                    request.AddParameter("application/json", "{\r\n \"order\": {\r\n   \"note_attributes\": " + message+"\r\n }\r\n}", ParameterType.RequestBody);
                    IRestResponse response = await client.ExecuteAsync(request);

                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    //  JObject object_res = new JObject();
                    if (numericStatusCode == 200)
                    {
                        //var object_res = JObject.Parse(response.Content);
                        return "OK";



                    }
                    if (numericStatusCode == 422)
                    {
                        //var object_res = JObject.Parse(response.Content);


                        return "422";

                    }

                }
            }
            return "";
        }
    }
}
