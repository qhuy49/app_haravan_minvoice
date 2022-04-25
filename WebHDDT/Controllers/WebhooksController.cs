using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebHDDT.Models;

namespace WebHDDT.Controllers
{
    using RestSharp;
    public class WebhooksController : ApiController
    {
        Minvoice_HaravanEntities3 db = new Minvoice_HaravanEntities3();
        private static string Verify_token = ConfigurationManager.AppSettings["Verify_token"];
        private static string app_secret = ConfigurationManager.AppSettings["app_secret"];
        private static string URL_GET_ORDER_DETAILS = ConfigurationManager.AppSettings["URL_GET_ORDER_DETAILS"];
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        public async Task<HttpResponse> LoginAsync(string username, string password)
        {
            
            HttpClient client = new HttpClient { BaseAddress = new Uri("https://0315827587.minvoice.com.vn/") };
            var content = new FormUrlEncodedContent(new[]
            {
                  new KeyValuePair<string, string>("username", "VNYI"),
                  new KeyValuePair<string, string>("password", "123456"),
                  new KeyValuePair<string, string>("madvcs", null)
            });

            HttpResponseMessage respon = await client.PostAsync("api/Account/Login", content);
            string conten = await respon.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(conten);
          

            //System.Diagnostics.Debug.WriteLine(token);
            return new HttpResponse();
        }

        #region Get Request  

        [HttpGet]
        [Route("api/webhooks/get")]
        public HttpResponseMessage Get()
        {
            try
            {


                string verify_token_request = System.Web.HttpContext.Current.Request.QueryString["hub.verify_token"].ToString();
                if (verify_token_request == Verify_token)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(System.Web.HttpContext.Current.Request.QueryString["hub.challenge"])
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }


        }

#endregion Get Request  

#region Post Request  

        [HttpPost]
        [Route("api/webhooks/get")]
        public async Task<HttpResponseMessage> Get([FromBody]string data1)
        {
            //HttpRequestMessage request = new HttpRequestMessage();
            //return CreateHttpResponse(request, () => {
            //    HttpResponseMessage response = null;
            //    response = request.CreateResponse(HttpStatusCode.Created , data1);
            //    return response;
            //});
            try
            {
                var topic = System.Web.HttpContext.Current.Request.Headers["x-haravan-topic"];
                var org_id = System.Web.HttpContext.Current.Request.Headers["x-haravan-org-id"];
                var Hmacsha256 = System.Web.HttpContext.Current.Request.Headers["x-haravan-Hmacsha256"];

                HttpRequest request = HttpContext.Current.Request;
                    string body = "";
                    using (StreamReader stream = new StreamReader(request.GetBufferedInputStream(), Encoding.UTF8))
                    {
                        body = await stream.ReadToEndAsync();
                    }
                //body = "{\n    \"order\": {\n        \"billing_address\": {\n            \"address1\": \"123 Lê Duẩn\",\n            \"address2\": null,\n            \"city\": \"\",\n            \"company\": null,\n            \"country\": \"Vietnam\",\n            \"first_name\": \"Huy\",\n            \"id\": 1044182168,\n            \"last_name\": \"Đinh Quang\",\n            \"phone\": \"0963105785\",\n            \"province\": \"Cao Bằng\",\n            \"zip\": \"\",\n            \"name\": \"Đinh Quang Huy\",\n            \"province_code\": \"CB\",\n            \"country_code\": \"VN\",\n            \"default\": true,\n            \"district\": \"Huyện Bảo Lạc\",\n            \"district_code\": \"CB133\",\n            \"ward\": null,\n            \"ward_code\": null\n        },\n        \"browser_ip\": null,\n        \"buyer_accepts_marketing\": false,\n        \"cancel_reason\": \"inventory\",\n        \"cancelled_at\": \"2021-04-28T07:35:37.887Z\",\n        \"cart_token\": \"e17671625bf246baa3a9de0ca2089afa\",\n        \"checkout_token\": \"e17671625bf246baa3a9de0ca2089afa\",\n        \"client_details\": {\n            \"accept_language\": null,\n            \"browser_ip\": null,\n            \"session_hash\": null,\n            \"user_agent\": null,\n            \"browser_height\": null,\n            \"browser_width\": null\n        },\n        \"closed_at\": null,\n        \"created_at\": \"2021-04-28T07:34:45.633Z\",\n        \"currency\": \"VND\",\n        \"customer\": {\n            \"accepts_marketing\": false,\n            \"addresses\": [\n                {\n                    \"address1\": \"Số 2 Nguyễn Thế Lộc Phường 12 Quận Tân Bình TP.HCM\",\n                    \"address2\": null,\n                    \"city\": \"\",\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075238783,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Cao Bằng\",\n                    \"zip\": \"\",\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"CB\",\n                    \"country_code\": \"vn\",\n                    \"default\": true,\n                    \"district\": \"Huyện Hà Quảng\",\n                    \"district_code\": \"CB134\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                },\n                {\n                    \"address1\": \"123 Lê Duẩn\",\n                    \"address2\": null,\n                    \"city\": null,\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075931075,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Cao Bằng\",\n                    \"zip\": null,\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"CB\",\n                    \"country_code\": \"vn\",\n                    \"default\": false,\n                    \"district\": \"Huyện Bảo Lạc\",\n                    \"district_code\": \"CB133\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                },\n                {\n                    \"address1\": \"Số 2, Nguyễn Thế Lộc, Phường 12, Quận Tân Bình, TP.HCM\",\n                    \"address2\": null,\n                    \"city\": null,\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075611097,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Cần Thơ\",\n                    \"zip\": null,\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"CN\",\n                    \"country_code\": \"vn\",\n                    \"default\": false,\n                    \"district\": \"Quận Thốt Nốt\",\n                    \"district_code\": \"CN637\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                },\n                {\n                    \"address1\": \"Số 2, Nguyễn Thế Lộc, Phường 12, Quận Tân Bình, TP.HCM\",\n                    \"address2\": null,\n                    \"city\": null,\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075609908,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Cao Bằng\",\n                    \"zip\": null,\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"CB\",\n                    \"country_code\": \"vn\",\n                    \"default\": false,\n                    \"district\": \"Huyện Hà Quảng\",\n                    \"district_code\": \"CB134\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                },\n                {\n                    \"address1\": \"Số 2, Nguyễn Thế Lộc, Phường 12, Quận Tân Bình, TP.HCM\",\n                    \"address2\": null,\n                    \"city\": null,\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075450044,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Cà Mau\",\n                    \"zip\": null,\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"CM\",\n                    \"country_code\": \"vn\",\n                    \"default\": false,\n                    \"district\": \"Huyện Thới Bình\",\n                    \"district_code\": \"CM667\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                },\n                {\n                    \"address1\": \"Số 2, Nguyễn Thế Lộc, Phường 12, Quận Tân Bình, TP.HCM\",\n                    \"address2\": null,\n                    \"city\": null,\n                    \"company\": null,\n                    \"country\": \"Vietnam\",\n                    \"first_name\": \"Huy\",\n                    \"id\": 1075295145,\n                    \"last_name\": \"Đinh Quang\",\n                    \"phone\": \"0963105785\",\n                    \"province\": \"Đắk Nông\",\n                    \"zip\": null,\n                    \"name\": \"Đinh Quang Huy\",\n                    \"province_code\": \"DO\",\n                    \"country_code\": \"vn\",\n                    \"default\": false,\n                    \"district\": \"Huyện Đắk Mil\",\n                    \"district_code\": \"DO460\",\n                    \"ward\": null,\n                    \"ward_code\": null\n                }\n            ],\n            \"created_at\": \"2021-04-03T06:27:25.335Z\",\n            \"default_address\": {\n                \"address1\": \"Số 2 Nguyễn Thế Lộc Phường 12 Quận Tân Bình TP.HCM\",\n                \"address2\": null,\n                \"city\": \"\",\n                \"company\": null,\n                \"country\": \"Vietnam\",\n                \"first_name\": \"Huy\",\n                \"id\": 1075238783,\n                \"last_name\": \"Đinh Quang\",\n                \"phone\": \"0963105785\",\n                \"province\": \"Cao Bằng\",\n                \"zip\": \"\",\n                \"name\": \"Đinh Quang Huy\",\n                \"province_code\": \"CB\",\n                \"country_code\": \"vn\",\n                \"default\": true,\n                \"district\": \"Huyện Hà Quảng\",\n                \"district_code\": \"CB134\",\n                \"ward\": null,\n                \"ward_code\": null\n            },\n            \"email\": \"huydq@minvoice.vn\",\n            \"phone\": \"0963105785\",\n            \"first_name\": \"Huy\",\n            \"id\": 1044182168,\n            \"multipass_identifier\": null,\n            \"last_name\": \"Đinh Quang\",\n            \"last_order_id\": 1193364260,\n            \"last_order_name\": \"#100048\",\n            \"note\": null,\n            \"orders_count\": 15,\n            \"state\": \"Disabled\",\n            \"tags\": null,\n            \"total_spent\": 0.0000,\n            \"total_paid\": 0.0,\n            \"updated_at\": \"2021-04-28T06:50:53Z\",\n            \"verified_email\": false,\n            \"send_email_invite\": false,\n            \"send_email_welcome\": false,\n            \"password\": null,\n            \"password_confirmation\": null,\n            \"group_name\": null,\n            \"birthday\": null,\n            \"gender\": null,\n            \"last_order_date\": null\n        },\n        \"discount_codes\": [],\n        \"email\": \"qhuylk49@gmail.com\",\n        \"financial_status\": \"pending\",\n        \"fulfillments\": [],\n        \"fulfillment_status\": \"notfulfilled\",\n        \"tags\": null,\n        \"gateway\": \"Thanh toán khi giao hàng (COD)\",\n        \"gateway_code\": \"COD\",\n        \"id\": 1193409002,\n        \"landing_site\": \"/collections/all\",\n        \"landing_site_ref\": null,\n        \"source\": \"web\",\n        \"line_items\": [\n            {\n                \"fulfillable_quantity\": 1,\n                \"fulfillment_service\": null,\n                \"fulfillment_status\": \"notfulfilled\",\n                \"grams\": 123.0000,\n                \"id\": 1227059616,\n                \"price\": 50000.0000,\n                \"price_original\": 50000.0000,\n                \"price_promotion\": 0.0,\n                \"product_id\": 1031734483,\n                \"quantity\": 1,\n                \"requires_shipping\": true,\n                \"sku\": \"adad\",\n                \"title\": \"áo\",\n                \"variant_id\": 1069476149,\n                \"variant_title\": \"Default Title\",\n                \"vendor\": \"adidas\",\n                \"type\": \"Khác\",\n                \"name\": \"áo - Default Title\",\n                \"gift_card\": false,\n                \"taxable\": true,\n                \"tax_lines\": null,\n                \"product_exists\": true,\n                \"barcode\": null,\n                \"properties\": null,\n                \"total_discount\": 0.0000,\n                \"applied_discounts\": [],\n                \"image\": null,\n                \"not_allow_promotion\": false,\n                \"ma_cost_amount\": 0.00\n            },\n            {\n                \"fulfillable_quantity\": 1,\n                \"fulfillment_service\": null,\n                \"fulfillment_status\": \"notfulfilled\",\n                \"grams\": 0.0000,\n                \"id\": 1227059617,\n                \"price\": 100000.0000,\n                \"price_original\": 100000.0000,\n                \"price_promotion\": 0.0,\n                \"product_id\": 1030537835,\n                \"quantity\": 1,\n                \"requires_shipping\": true,\n                \"sku\": null,\n                \"title\": \"sách\",\n                \"variant_id\": 1066849825,\n                \"variant_title\": \"Default Title\",\n                \"vendor\": \"Khác\",\n                \"type\": \"Khác\",\n                \"name\": \"sách - Default Title\",\n                \"gift_card\": false,\n                \"taxable\": true,\n                \"tax_lines\": null,\n                \"product_exists\": true,\n                \"barcode\": \"25888880000000001\",\n                \"properties\": null,\n                \"total_discount\": 0.0000,\n                \"applied_discounts\": [],\n                \"image\": {\n                    \"src\": \"https://product.hstatic.net/200000230425/product/z2253826157811_adda6c2538f5a4cd4da3e30447072682_7ff0952c51d84852b1a67651404be92e.jpg\",\n                    \"attactment\": null,\n                    \"filename\": null\n                },\n                \"not_allow_promotion\": false,\n                \"ma_cost_amount\": 0.00\n            }\n        ],\n        \"name\": \"#100049\",\n        \"note\": \"Hàng dễ vỡ\",\n        \"number\": 1193409002,\n        \"order_number\": \"#100049\",\n        \"processing_method\": null,\n        \"referring_site\": \"https://hddt-minvoice-1.myharavan.com/cart\",\n        \"refunds\": [\n            {\n                \"created_at\": \"2021-04-28T07:35:37.901Z\",\n                \"id\": 1009114033,\n                \"note\": \"abc\",\n                \"refund_line_items\": null,\n                \"restock\": null,\n                \"user_id\": 200000510447,\n                \"order_id\": 1193409002,\n                \"transactions\": []\n            }\n        ],\n        \"shipping_address\": {\n            \"address1\": \"123 Lê Duẩn\",\n            \"address2\": null,\n            \"city\": \"\",\n            \"company\": null,\n            \"country\": \"Vietnam\",\n            \"first_name\": \"Huy\",\n            \"last_name\": \"Đinh Quang\",\n            \"latitude\": 0.00000000,\n            \"longitude\": 0.00000000,\n            \"phone\": \"0963105785\",\n            \"province\": \"Cao Bằng\",\n            \"zip\": \"\",\n            \"name\": \"Đinh Quang Huy\",\n            \"province_code\": \"CB\",\n            \"country_code\": \"VN\",\n            \"district_code\": \"CB133\",\n            \"district\": \"Huyện Bảo Lạc\",\n            \"ward_code\": null,\n            \"ward\": null\n        },\n        \"shipping_lines\": [\n            {\n                \"code\": null,\n                \"price\": 0.0000,\n                \"source\": null,\n                \"title\": null\n            }\n        ],\n        \"source_name\": \"web\",\n        \"subtotal_price\": 150000.0000,\n        \"tax_lines\": null,\n        \"taxes_included\": false,\n        \"token\": \"e17671625bf246baa3a9de0ca2089afa\",\n        \"total_discounts\": 0.0000,\n        \"total_line_items_price\": 150000.0000,\n        \"total_price\": 150000.0000,\n        \"total_tax\": 0.0,\n        \"total_weight\": 123.0000,\n        \"updated_at\": \"2021-04-28T07:35:37.901Z\",\n        \"transactions\": [\n            {\n                \"amount\": 150000.0000,\n                \"authorization\": null,\n                \"created_at\": \"2021-04-28T07:34:45.814Z\",\n                \"device_id\": null,\n                \"gateway\": \"Thanh toán khi giao hàng (COD)\",\n                \"id\": 1078317990,\n                \"kind\": \"pending\",\n                \"order_id\": 1193409002,\n                \"receipt\": null,\n                \"status\": null,\n                \"test\": false,\n                \"user_id\": 200000510447,\n                \"location_id\": 975782,\n                \"payment_details\": null,\n                \"parent_id\": null,\n                \"currency\": \"VND\",\n                \"haravan_transaction_id\": null,\n                \"external_transaction_id\": null,\n                \"send_email\": false\n            }\n        ],\n        \"note_attributes\": [\n            {\n                \"name\": \"invoice\",\n                \"value\": \"yes\"\n            },\n            {\n                \"name\": \"invoice_namebuyer\",\n                \"value\": \"Nguyễn Văn A\"\n            },\n            {\n                \"name\": \"invoice_namelegal\",\n                \"value\": \"công ty TNHH T&T\"\n            },\n            {\n                \"name\": \"invoice_tax\",\n                \"value\": \"0106026485\"\n            },\n            {\n                \"name\": \"invoice_address\",\n                \"value\": \"098 Lê Văn Sỹ, Phường 14, Tân Bình, TP.HCM\"\n            },\n            {\n                \"name\": \"invoice_email\",\n                \"value\": \"nva@gmail.com\"\n            }\n        ],\n        \"confirmed_at\": \"2021-04-28T07:34:46.047Z\",\n        \"closed_status\": \"unclosed\",\n        \"cancelled_status\": \"cancelled\",\n        \"confirmed_status\": \"confirmed\",\n        \"user_id\": 200000510447,\n        \"device_id\": null,\n        \"location_id\": 975782,\n        \"ref_order_id\": 1193364260,\n        \"ref_order_number\": \"#100048\",\n        \"utm_source\": null,\n        \"utm_medium\": null,\n        \"utm_campaign\": null,\n        \"utm_term\": null,\n        \"utm_content\": null,\n        \"redeem_model\": null,\n        \"omni_order_status\": 0,\n        \"omni_order_status_name\": null,\n        \"location_assigned_at\": null,\n        \"in_stock_at\": null,\n        \"out_of_stock_at\": null,\n        \"export_at\": null,\n        \"complete_at\": null\n    }\n}";
                SHA256CryptoServiceProvider x1 = new SHA256CryptoServiceProvider();

                byte[] bs1 = System.Text.Encoding.UTF8.GetBytes(body+app_secret);
                bs1 = x1.ComputeHash(bs1);
                System.Text.StringBuilder s1 = new System.Text.StringBuilder();

                foreach (byte b in bs1)
                {
                    s1.Append(b.ToString("x2").ToLower());
                }
                

                Console.WriteLine(s1.ToString());
                var encode_data = Convert.ToBase64String(Encoding.UTF8.GetBytes(s1.ToString()));
                if (encode_data == Hmacsha256)
                {
                    // đã xác thực webhook

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {

                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                //if (topic == "orders/cancelled")
                //{
                //    //HttpRequestMessage
                //    HttpRequest request = HttpContext.Current.Request;
                //    string body = "";
                //    using (StreamReader stream = new StreamReader(request.GetBufferedInputStream(), Encoding.UTF8))
                //    {
                //        body = await stream.ReadToEndAsync();
                //    }

                //    if (body.Contains("\"cancelled_status\":\"cancelled\""))
                //    {
                //        string[] arrayStr = body.Split(',');
                //        var note = arrayStr.Where(n => n.Contains("\"note\"")).ToList();
                //        var ghi_chu = note[note.Count-1].Substring(note[note.Count-1].IndexOf(":") + 1);
                //        var ghi_chu2 = ghi_chu.Substring(1, ghi_chu.Length - 2);
                //        var order = arrayStr.Where(n => n.Contains("\"order_id")).ToList();

                //        var id_order = order[0].Substring(order[0].IndexOf(":") + 1);
                //        var tags = arrayStr.Where(n => n.Contains("\"tags")).ToList();
                //        // kiểm tra đã hủy hóa đơn chưa
                //        if (tags[0].Contains("Đã hủy hóa đơn".ToLower()))
                //        {
                //            return new HttpResponseMessage(HttpStatusCode.OK);
                //        }
                //        else
                //        {
                //            var invoice = db.Export_Invoice.Where(n => n.orgid == org_id && n.so_don_hang == id_order && n.inv_invoicenumber != null).FirstOrDefault();
                //            if (invoice != null)
                //            {
                //                // gọi api hủy hóa đơn
                //                var inv_invoiceauth_id = !string.IsNullOrEmpty(invoice.inv_InvoiceAuth_id) ? invoice.inv_InvoiceAuth_id : "";
                //                User_info _userinfo = db.User_info.SingleOrDefault(n => n.orgid == org_id);
                //                if (_userinfo != null)
                //                {
                //                    var webClient = new WebClient
                //                    {
                //                        Encoding = Encoding.UTF8
                //                    };

                //                    webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
                //                    var tokenJson = Login_Minvoice(_userinfo.username, _userinfo.password, _userinfo.mst);
                //                    if (tokenJson != null)
                //                    {
                //                        if (tokenJson.ContainsKey("token"))
                //                        {
                //                            var authorization = "Bear " + tokenJson["token"] + ";VP;vi";
                //                            webClient.Headers[HttpRequestHeader.Authorization] = authorization;


                //                            var jObjectMainData = new JObject
                //                            {
                //                                {"inv_InvoiceAuth_id", inv_invoiceauth_id},
                //                                {"sovb", "BBXB/"+invoice.inv_invoicenumber+"/"+DateTime.Now.Day+"/"+DateTime.Now.Month+"/"+DateTime.Now.Year+""},
                //                                {"ngayvb", DateTime.Now.ToString("yyyy-MM-dd")},
                //                                {"ghi_chu", ghi_chu2}
                //                            };

                //                            var dataRequest = jObjectMainData.ToString();

                //                            var url = $"https://{_userinfo.mst}.minvoice.com.vn/api/Invoice/xoaboHD";
                //                            var result = await webClient.UploadStringTaskAsync(url, dataRequest);
                //                            var resultResponse = JObject.Parse(result);
                //                            // hủy thành công
                //                            if (resultResponse.ContainsKey("ok"))
                //                            {
                //                                var kq = await API_UpdateOrder_Haravan(org_id, id_order, "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Đã hủy hóa đơn điện tử thành công!, Số hóa đơn: " + invoice.inv_invoicenumber.ToString() + " , Mẫu số: " + invoice.mau_so.ToString()+ ", Ký hiệu: " + invoice.ky_hieu.ToString() + "");
                //                                if (kq == "")
                //                                {
                //                                    return new HttpResponseMessage(HttpStatusCode.BadGateway);
                //                                }
                //                                if (kq == "422")
                //                                {
                //                                    return new HttpResponseMessage(HttpStatusCode.BadGateway);
                //                                }
                //                                //if (kq == "OK")
                //                                //{

                //                                //    // cập nhật thành công
                //                                //}
                //                                Export_Invoice export_invoice = new Export_Invoice()
                //                                {
                //                                    orgid = org_id,
                //                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                //                                    so_don_hang = invoice.so_don_hang.ToString(),
                //                                    mau_so = invoice.mau_so,
                //                                    ky_hieu = invoice.ky_hieu,
                //                                    inv_InvoiceAuth_id = invoice.inv_InvoiceAuth_id,
                //                                    inv_invoicecode_id = invoice.inv_invoicecode_id,
                //                                    json_data = jObjectMainData.ToString(),
                //                                    trang_thai_xuat = "Hủy hóa đơn thành công!"
                //                                };
                //                                db.Export_Invoice.Add(export_invoice);
                //                                db.SaveChanges();
                //                                return new HttpResponseMessage(HttpStatusCode.OK);
                //                            }
                //                            if (resultResponse.ContainsKey("error"))
                //                            {
                //                                var kq = await API_UpdateOrder_Haravan(org_id, id_order, "" + Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH: mm:ss")) + " " + "Hủy hóa đơn điện tử thất bại!, Nguyên nhân: " + "'" + "" + resultResponse["error"] + "" + "'" + "");

                //                                Export_Invoice export_invoice = new Export_Invoice()
                //                                {
                //                                    orgid = org_id,
                //                                    ngay_tao = Convert.ToDateTime(DateTime.Now.ToString("yyyy MM dd HH:mm:ss")),
                //                                    so_don_hang = invoice.so_don_hang.ToString(),
                //                                    mau_so = invoice.mau_so,
                //                                    ky_hieu = invoice.ky_hieu,
                //                                    inv_InvoiceAuth_id = invoice.inv_InvoiceAuth_id,
                //                                    inv_invoicecode_id = invoice.inv_invoicecode_id,
                //                                    json_data = jObjectMainData.ToString(),
                //                                    trang_thai_xuat = "Hủy hóa đơn thất bại!"
                //                                };
                //                                db.Export_Invoice.Add(export_invoice);
                //                                db.SaveChanges();

                //                                return new HttpResponseMessage(HttpStatusCode.OK);
                //                            }


                //                        }
                //                    }

                //                }
                //            }

                //            //int index = body.IndexOf("cancelled_status") - 2;
                //            //int index2 = body.IndexOf(',', body.IndexOf(',') + 2);
                //            //var abc = body[index2];
                //            //if (body.StartsWith())
                //            //{

                //            //}



                //        }
                //        //dynamic data = JsonConvert.DeserializeObject(body);


                //        //cancelled_status = "cancelled";
                //        //using (var reader = new StreamReader(
                //        //             Request.Body.,
                //        //             encoding: Encoding.UTF8,
                //        //             detectEncodingFromByteOrderMarks: false
                //        //      ))
                //        //{
                //        //    var bodyString = reader.ReadToEnd();

                //        //    var value = JsonConvert.DeserializeObject<dynamic>(bodyString);

                //        //    // use the body, process the stuff...x`
                //        //}
                //        //}

                //        //var signatureHashHexExpected = "559bd871bfd21ab76ad44513ed5d65774f9954d3232ab68dab1806163f806447";
                //        //var signature = "123456:some-string:2016-04-12T12:44:16Z";
                //        //var key = "AgQGCAoMDhASFAIEBggKDA4QEhQCBAYICgwOEBIUAgQ=";
                //        //byte[] bytes = Encoding.UTF8.GetBytes(body+app_secret);

                //        //using (HMACSHA256 hmac = new HMACSHA256(bytes))
                //        //{
                //        //    // Create an array to hold the keyed hash value read from the file.
                //        //    byte[] storedHash = new byte[hmac.HashSize / 8];
                //        //    // Create a FileStream for the source file.

                //        //}

                //        //var sha256 = sh();

                //        //var shaKeyBytes = Convert.FromBase64String(app_secret);
                //        //using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(shaKeyBytes))
                //        //{
                //        //    var signatureBytes = System.Text.Encoding.UTF8.GetBytes(body);
                //        //    var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                //        //    var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2")));

                //        //    System.Diagnostics.Debug.Assert(signatureHashHex == Hmacsha256);
                //        //}



                //        //Console.WriteLine(System.Web.HttpContext.Current.Request.);
                //        //You got the data do whatever you want here!!!Happy programming!!  

                //    }
                //}
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
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
                {"username",!string.IsNullOrEmpty(username) ? username :"123" },
                {"password",!string.IsNullOrEmpty(password) ? password :"123"  },
                {"ma_dvcs","VP" }
            };
            var mst = !string.IsNullOrEmpty(taxcode) ? taxcode : "123";
            //var urlLogin = BaseConfig.UrlLogin;
            var token = client.UploadString($"https://{taxcode}.minvoice.com.vn/api/Account/Login", json.ToString());
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
        
            return JObject.Parse(token);
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
                    request.AddParameter("application/json", "{\r\n \"order\": {\r\n   \"note_attributes\": " + message + "\r\n }\r\n}", ParameterType.RequestBody);
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
        //public void Get(string res)
        //{
        //    string verify_token_request = Request.QueryString["hub.verify_token"].ToString();
        //    if (verify_token_request == Verify_token)
        //    {
        //        Response.StatusCode = 200;
        //        Response.Headers.AllKeys[1];
        //    }
        //    else
        //    {
        //        new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        //    }

        //}
        #endregion
    }

}

