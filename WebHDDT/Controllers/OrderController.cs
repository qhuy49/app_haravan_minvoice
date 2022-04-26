using Newtonsoft.Json.Linq;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebHDDT.Models;


namespace WebHDDT.Controllers
{
    public class OrderController : Controller
    {
        Minvoice_HaravanEntities3 db = new Minvoice_HaravanEntities3();
        private static string mst_config = ConfigurationManager.AppSettings["mst"];
        private static string url_series = ConfigurationManager.AppSettings["URL_GET_SERIES"];
        private static string url_series32 = ConfigurationManager.AppSettings["URL_GET_SERIES32"];
        private static string url_save = ConfigurationManager.AppSettings["URL_SAVE"];
        private static string url_login = ConfigurationManager.AppSettings["URL_LOGIN"];
        // GET: Order
        public async Task<ActionResult> LoadingGetOrder(string orgid, string id)
        {
            var list_cttb = new List<ctthongbao>();
            if (!string.IsNullOrWhiteSpace(orgid))
            {
                Session["org"] = orgid;
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                Session["id"] = id;
            }
            var user_info = db.User_info.SingleOrDefault(n => n.orgid == orgid);
            if (user_info != null)

            {
                Session["no_userinfo"] = null;
                Session["user_information"] = user_info;
              
                //var login = SetupWebClient(user_info.username,user_info.password,user_info.mst);
                var webClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                webClient.Headers.Add("Content-Type", "application/json; charset=utf-8");

                var tokenJson = Login_Minvoice(user_info.username ?? "abc", user_info.password ?? "123", user_info.mst ?? mst_config);
                if (tokenJson != null)
                {
                    if (tokenJson.ContainsKey("token"))
                    {
                       
                        Session["error"] = null;
                        var authorization = "Bear " + tokenJson["token"] + ";VP;vi";
                        webClient.Headers[HttpRequestHeader.Authorization] = authorization;

                        Session["user_mst"] = user_info.mst;
                        Session["user_username"] = user_info.username;
                        Session["user_password"] = user_info.password;
                        Session["authorize"] = authorization;


                        //select ktra có dùng 78 chưa ck

                        var check_tt = db.ctthongbao.Where(n => n.orgid == orgid).ToList();//tồn tại
                        var check_78 = db.ctthongbao.Where(n => n.orgid == orgid && n.tt78 == true).ToList();//TT78
                        
                        if (check_tt.Count > 0) 
                        {
                            if (check_78.Count > 0)//TT78
                            {
                                //check năm bao nhiêu để get sêries
                                //int getYear = Convert.ToInt32(DateTime.Today.Year.ToString().Substring(2, 2));

                                    // lấy dữ liệu trực tiếp từ data
                                    return View(check_78);

                            }
                            //TT32
                            else
                            { //TT32
                              //check hết số hay còn số
                                var ctthongbao = db.ctthongbao.Where(n => n.orgid == orgid && n.hetso == "0").ToList();//còn số
                                var ctthongbao2 = db.ctthongbao.Where(n => n.orgid == orgid && n.hetso == "1").ToList();//hết số
                                var countt = (from c in db.ctthongbao
                                              where c.orgid == orgid
                                              group c by new { c.mau_so, c.ky_hieu } into grp
                                              where grp.Count() > 1
                                              select new { grp.Key.mau_so, grp.Key.ky_hieu }).ToList(); //lấy ra 2 dải trùng nhau hay k


                                if ((ctthongbao.Count == 1 && ctthongbao2.Count == 1 && countt.Count == 1) ||
                                    (ctthongbao.Count > 0 && ctthongbao2.Count == 0) ||
                                    (ctthongbao.Count == 1 && ctthongbao2.Count == 2 && countt.Count == 1) ||
                                    (ctthongbao.Count == 2 && ctthongbao2.Count == 1 && countt.Count == 1))
                                {
                                    // lấy dữ liệu trực tiếp từ data
                                    if (ctthongbao.Count == 1 && ctthongbao2.Count == 2 && countt.Count == 1)
                                    {
                                        var countt2 = (from c in db.ctthongbao
                                                       where c.orgid == orgid
                                                       group c by new { c.mau_so, c.ky_hieu } into grp
                                                       where grp.Count() > 2
                                                       select new { grp.Key.mau_so, grp.Key.ky_hieu }).ToList();
                                        if (ctthongbao.Count == 1 && ctthongbao2.Count == 2 && countt2.Count == 1)
                                        {
                                            // lấy dữ liệu trực tiếp từ data
                                            return View(ctthongbao);
                                        }
                                        else
                                        {
                                            // gọi api lất cttbph
                                            //gọi api lấy tbph
                                            var result = await webClient.DownloadStringTaskAsync(new Uri("https://" + user_info.mst + ".minvoice.com.vn/api/System/GetDataReferencesByRefId?refId=RF00027"));
                                            var resultResponse = JArray.Parse(result);

                                            if (resultResponse.Count > 0)
                                            {
                                                if (ctthongbao.Count == 0 && ctthongbao2.Count == 0)
                                                {
                                                    foreach (var jToken in resultResponse)
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
                                                            value = jToken["value"].ToString()
                                                        };
                                                        db.ctthongbao.Add(cttb);
                                                        db.SaveChanges();
                                                        list_cttb.Add(cttb);

                                                    }

                                                }
                                                if (ctthongbao.Count > 0 && ctthongbao2.Count > 0)
                                                {
                                                    foreach (var jToken in resultResponse)
                                                    {

                                                        if ((ctthongbao.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false) && (ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false))
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
                                                                value = jToken["value"].ToString()
                                                            };
                                                            db.ctthongbao.Add(cttb);
                                                            db.SaveChanges();
                                                            list_cttb.Add(cttb);
                                                        }
                                                    }

                                                }
                                                if (ctthongbao.Count == 0 && ctthongbao2.Count > 0)
                                                {
                                                    foreach (var jToken in resultResponse)
                                                    {
                                                        if (ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false)
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
                                                                value = jToken["value"].ToString()
                                                            };
                                                            db.ctthongbao.Add(cttb);
                                                            db.SaveChanges();
                                                            list_cttb.Add(cttb);
                                                        }
                                                    }
                                                }


                                                return View(list_cttb);
                                            }



                                        }
                                    }
                                    return View(ctthongbao);


                                }
                                else
                                {
                                    //gọi api lấy tbph
                                    var result = await webClient.DownloadStringTaskAsync(new Uri(url_series32.Replace("testapi", user_info.mst)));


                                    //var result = await webClient.DownloadStringTaskAsync(new Uri("https://" + user_info.mst + ".minvoice.com.vn/api/System/GetDataReferencesByRefId?refId=RF00027"));
                                    var resultResponse = JArray.Parse(result);

                                    if (resultResponse.Count > 0)
                                    {
                                        if (ctthongbao.Count == 0 && ctthongbao2.Count == 0)
                                        {
                                            foreach (var jToken in resultResponse)
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
                                                    value = jToken["value"].ToString()
                                                };
                                                db.ctthongbao.Add(cttb);
                                                db.SaveChanges();
                                                list_cttb.Add(cttb);

                                            }

                                        }
                                        if (ctthongbao.Count > 0 && ctthongbao2.Count > 0)
                                        {
                                            foreach (var jToken in resultResponse)
                                            {

                                                if ((ctthongbao.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false) && (ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false))
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
                                                        value = jToken["value"].ToString()
                                                    };
                                                    db.ctthongbao.Add(cttb);
                                                    db.SaveChanges();
                                                    list_cttb.Add(cttb);
                                                }
                                            }

                                        }
                                        if (ctthongbao.Count == 0 && ctthongbao2.Count > 0)
                                        {
                                            foreach (var jToken in resultResponse)
                                            {
                                                if (ctthongbao2.Any(s => s.ctthongbao_id.ToString() == jToken["ctthongbao_id"].ToString()) == false)
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
                                                        value = jToken["value"].ToString()
                                                    };
                                                    db.ctthongbao.Add(cttb);
                                                    db.SaveChanges();
                                                    list_cttb.Add(cttb);
                                                }
                                            }
                                        }


                                        return View(list_cttb);
                                    }

                                }
                            }

                        }
                        else //CHƯA CÓ THÌ CALL API 78 GET SERIES
                        {
                            //call API 78
                            var result = await webClient.DownloadStringTaskAsync(new Uri(url_series.Replace("testapi", user_info.mst) ));
                            var JOBJECT = JObject.Parse(result);
                            var resultResponse = JArray.Parse(JOBJECT["data"].ToString());

                            if (resultResponse.Count > 0)
                            {

                                    foreach (var jToken in resultResponse)
                                    {
                                        ctthongbao cttb = new ctthongbao
                                        {
                                            ky_hieu = jToken["khhdon"].ToString(),
                                            mau_so = jToken["lhdon"].ToString(),
                                            ctthongbao_id = (Guid)jToken["quanlykyhieu68_id"],
                                            so_luong = null,
                                            tu_so = null,
                                            den_so = null,
                                            ngay_bd_sd = (DateTime)jToken["date_new"],
                                            hetso = "0",
                                            orgid = orgid,
                                            value = jToken["value"].ToString(),
                                            tt78=true,
                                        };
                                        db.ctthongbao.Add(cttb);
                                        db.SaveChanges();
                                        list_cttb.Add(cttb);

                                    }
                            }
                            return View(list_cttb);
                            //chưa mua hóa đơn 78
                            // thêm ~/Content vào html
                        }
                       
                       
                    }
                    if (tokenJson.ContainsKey("error"))
                    {
                        Session["error"] = "error";
                        return View(list_cttb);
                    }



                }
            }
            else
            {
                Session["no_userinfo"] = "no_user";
                return View(list_cttb);
            }
            return View(list_cttb);
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

        // nhận thông tin về id của đơn hàng và get chi tiết đơn hàng
        //public ActionResult GetDetailOrder()
        //{

        //}
    }
}