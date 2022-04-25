using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Threading.Tasks;

namespace WebHDDT.Models
{

    public class JsonConvert
    {

        
        public static async Task<JObject> ConvertMasterObject(JObject data_api, string mau_hd, string inv_invoiceseries, string inv_invoicecode_id, Guid inv_InvoiceAuth_id)
        {

            try
            {

                var jObject = new JObject();

                



                jObject.Add("inv_InvoiceAuth_id", inv_InvoiceAuth_id);


                jObject.Add("inv_invoiceType", "01GTKT");

                if (!string.IsNullOrWhiteSpace(inv_invoicecode_id))
                {
                    jObject.Add("inv_InvoiceCode_id", inv_invoicecode_id);
                }
                jObject.Add("inv_invoiceSeries", inv_invoiceseries);

                jObject.Add("inv_invoiceName", "Hóa đơn giá trị gia tăng");
                jObject.Add("inv_invoiceIssuedDate", DateTime.Now.ToString("yyyy-MM-dd"));
                jObject.Add("inv_currencyCode", !string.IsNullOrEmpty(data_api["currency"].ToString()) ? data_api["currency"].ToString() : "VND");
                jObject.Add("inv_exchangeRate", 1);
                jObject.Add("inv_adjustmentType", 1);

                var data_address = JObject.Parse(data_api.GetValue("billing_address").ToString());
              //  var billing_address = JObject.Parse(data_api.GetValue("billing_address").ToString());
                //
                var note_attributes = JArray.Parse(data_api["note_attributes"].ToString());
                string invoice_namebuyer = "";
                string invoice_tax = "";
                string invoice_address = "";
                string invoice_email = "";
                string invoice = "";
                string invoice_namelegal = "";
                if (note_attributes.Count > 0)
                {
                    foreach (var attribute in note_attributes)
                    {
                        switch (attribute["name"].ToString().ToLower())
                        {
                            case "invoice_namebuyer": invoice_namebuyer = attribute["value"].ToString(); break;

                            case "invoice_tax": invoice_tax = attribute["value"].ToString(); break;

                            case "invoice_address": invoice_address = attribute["value"].ToString(); break;

                            case "invoice_email": invoice_email = attribute["value"].ToString(); break;

                            case "invoice": invoice = attribute["value"].ToString(); break;

                            case "invoice_namelegal": invoice_namelegal = attribute["value"].ToString(); break;
                                //case "invoice": invoice = attribute["value"].ToString(); break;
                        }
                        //if (attribute["name"].ToString() == "username")
                        //{
                        //    username = attribute["value"].ToString();

                        //}
                    }
                }


    
                /////
                // nếu khách k điền thông tin mua hàng , k xuất hđ cho công ty thỳ lấy mặc định thông tin giao hàng của khách
                if (note_attributes.Count == 0 || invoice == "no")
                {
                     jObject.Add("inv_buyerDisplayName", (!string.IsNullOrEmpty(data_address["name"].ToString()) ? data_address["name"].ToString() : ""));
                    jObject.Add("inv_buyerLegalName", (!string.IsNullOrEmpty(data_address["company"].ToString()) ? data_address["company"].ToString() : ""));
                    jObject.Add("inv_buyerAddressLine", data_address["address1"].ToString() + (!string.IsNullOrEmpty(data_address["ward"].ToString()) ? ", " + data_address["ward"].ToString() : "") + (!string.IsNullOrEmpty(data_address["district"].ToString()) ? ", " + data_address["district"].ToString() :"") + (!string.IsNullOrEmpty(data_address["province"].ToString()) ? ", " + data_address["province"].ToString():"") + (!string.IsNullOrEmpty(data_address["country"].ToString()) ? ", " + data_address["country"].ToString():""));

                    jObject.Add("phone_number", !string.IsNullOrEmpty(data_address["phone"].ToString()) ? data_address["phone"].ToString() : "");

                   // var customer = JObject.Parse(data_api.GetValue("customer").ToString());

                    jObject.Add("inv_buyerEmail", !string.IsNullOrEmpty(data_api["contact_email"].ToString()) ? data_api["contact_email"].ToString() : "");
                }
                else
                {
                    jObject.Add("inv_buyerDisplayName", invoice_namebuyer);
                    jObject.Add("ma_dt", "");


                    jObject.Add("inv_buyerLegalName", invoice_namelegal);

                    jObject.Add("inv_buyerTaxCode", invoice_tax);


                    jObject.Add("inv_buyerAddressLine", invoice_address);


                    jObject.Add("inv_buyerEmail", invoice_email);
                }
              

                jObject.Add("inv_buyerBankAccount", "");


                jObject.Add("inv_buyerBankName", "");

                var httt = data_api["gateway"].ToString();
                if (httt.StartsWith("Chuyển khoản"))
                {
                    jObject.Add("inv_paymentMethodName", "Chuyển khoản");
                }
                else
                {
                    jObject.Add("inv_paymentMethodName", "TM/CK");
                }


                jObject.Add("inv_invoiceNote", data_api["note"].ToString());

                jObject.Add("inv_sellerBankAccount", "");
                jObject.Add("inv_sellerBankName", "");

                jObject.Add("so_benh_an", (!string.IsNullOrEmpty(data_api["id"].ToString()) ? data_api["id"].ToString() : ""));


                jObject.Add("trang_thai", "Chờ ký");
                jObject.Add("nguoi_ky", "");
                jObject.Add("sobaomat", "");
                jObject.Add("trang_thai_hd", 1);
                jObject.Add("in_chuyen_doi", false);
                jObject.Add("ngay_ky", null);
                jObject.Add("nguoi_in_cdoi", "");
                jObject.Add("ngay_in_cdoi", null);
                jObject.Add("mau_hd", mau_hd);
                jObject.Add("ma_ct", "HDDT");


                return jObject;
            }


            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        //public static JObject ConvertDetailJObjectSaVoucherDetail(DataRow row)
        //{
        //    try
        //    {
        //        var vatAmount = !string.IsNullOrEmpty(row["VATAmountOC"].ToString())
        //            ? double.Parse(row["VATAmountOC"].ToString())
        //            : 0;

        //        var discountAmount = !string.IsNullOrEmpty(row["DiscountAmount"].ToString())
        //            ? double.Parse(row["DiscountAmount"].ToString())
        //            : 0;

        //        var totalAmountWithoutVat = !string.IsNullOrEmpty(row["AmountOC"].ToString())
        //            ? double.Parse(row["AmountOC"].ToString())
        //            : 0;

        //        var totalAmount = totalAmountWithoutVat + vatAmount - discountAmount;

        //        var jObject = new JObject();
        //        jObject.Add("inv_InvoiceAuthDetail_id", row["RefDetailID"].ToString());
        //        jObject.Add(
        //            "inv_InvoiceAuth_id",
        //            !string.IsNullOrEmpty(row["SAInvoiceRefID"].ToString())
        //                ? row["SAInvoiceRefID"].ToString()
        //                : null
        //        );
        //        jObject.Add("stt_rec0", !string.IsNullOrEmpty(row["SortOrder"].ToString()) ? row["SortOrder"].ToString() : null);
        //        jObject.Add("inv_itemCode", !string.IsNullOrEmpty(row["InventoryItemCode"].ToString()) ? row["InventoryItemCode"].ToString() : null);
        //        jObject.Add(
        //            "inv_itemName",
        //            !string.IsNullOrEmpty(row["Description"].ToString()) ? row["Description"].ToString() : null
        //        );
        //        jObject.Add("inv_unitCode", !string.IsNullOrEmpty(row["UnitName"].ToString()) ? row["UnitName"].ToString() : null);
        //        jObject.Add("inv_unitName", !string.IsNullOrEmpty(row["UnitName"].ToString()) ? row["UnitName"].ToString() : null);
        //        Log.Debug(row["UnitPrice"].ToString());
        //        Log.Debug(row["Quantity"].ToString());
        //        Log.Debug(row["AmountOC"].ToString());

        //        jObject.Add(
        //            "inv_unitPrice",
        //            !string.IsNullOrEmpty(row["UnitPrice"].ToString()) ? CommonService.ConvertNumber2(row["UnitPrice"].ToString()) : null
        //        );
        //        jObject.Add(
        //            "inv_quantity",
        //            !string.IsNullOrEmpty(row["Quantity"].ToString()) ? CommonService.ConvertNumber2(row["Quantity"].ToString()) : null
        //        );
        //        jObject.Add(
        //            "inv_TotalAmountWithoutVat",
        //            !string.IsNullOrEmpty(row["AmountOC"].ToString()) ? CommonService.ConvertNumber2(row["AmountOC"].ToString()) : null
        //        );
        //        jObject.Add(
        //            "inv_vatPercentage",
        //            !string.IsNullOrEmpty(row["VATRate"].ToString()) ? CommonService.ConvertNumber2(row["VATRate"].ToString()) : null
        //        );
        //        jObject.Add(
        //            "inv_vatAmount",
        //            !string.IsNullOrEmpty(row["VATAmountOC"].ToString()) ? CommonService.ConvertNumber2(row["VATAmountOC"].ToString()) : null
        //        );
        //        jObject.Add("inv_TotalAmount", totalAmount);
        //        jObject.Add("inv_promotion", false);
        //        jObject.Add(
        //            "inv_discountPercentage",
        //            !string.IsNullOrEmpty(row["DiscountRate"].ToString()) ? CommonService.ConvertNumber2(row["DiscountRate"].ToString()) : null
        //        );
        //        jObject.Add(
        //            "inv_discountAmount",
        //            !string.IsNullOrEmpty(row["DiscountAmount"].ToString())
        //                ? CommonService.ConvertNumber2(row["DiscountAmount"].ToString())
        //                : null
        //        );
        //        jObject.Add(
        //            "ma_thue",
        //            !string.IsNullOrEmpty(row["VATRate"].ToString())
        //                ? CommonService.ConvertNumber2(row["VATRate"].ToString())
        //                : null
        //        );

        //        Log.Debug(jObject.ToString());

        //        return jObject;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Tên hàng: {row["Description"].ToString()} --- Thứ tự: {row["SortOrder"].ToString()}");
        //        Log.Error(ex.Message);
        //        throw new Exception(ex.Message);
        //    }
        //}
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static JObject ConvertDetailJObjectSaInvoiceDetail(JToken jtoken, Guid inv_InvoiceAuth_id, int count)
        {
            try
            {
                double vatAmount = 0;
                double totalAmountWithoutVat = 0.0;
                double totalAmount = 0.0;
                double unit_price = 0.0;
                double discountpercentage = 0.0;

                if (!string.IsNullOrEmpty(jtoken["quantity"].ToString()) && !string.IsNullOrEmpty(jtoken["price"].ToString()))
                {
                    totalAmount = (double)jtoken["quantity"] * (double)jtoken["price"];
                }

                var discountAmount = !string.IsNullOrEmpty(jtoken["total_discount"].ToString()) ? double.Parse(jtoken["total_discount"].ToString()) : 0;

              
                    vatAmount = (totalAmount / 1.1) * 10 / 100;

                totalAmountWithoutVat = (totalAmount / 1.1) + discountAmount;

                if (totalAmountWithoutVat!=0)
                {
                    discountpercentage = (discountAmount * 100 / totalAmountWithoutVat);
                }
                
          
                unit_price = totalAmountWithoutVat / (double)jtoken["quantity"];
                
               
               

                var jObject = new JObject();
                //jObject.Add("inv_InvoiceAuthDetail_id", row["RefDetailID"].ToString());
                jObject.Add(
                    "inv_InvoiceAuth_id", inv_InvoiceAuth_id);
                string stt_rec0 = count.ToString().PadLeft(8,'0');
                jObject.Add("stt_rec0", stt_rec0);



                jObject.Add("inv_itemCode", !string.IsNullOrEmpty(jtoken["sku"].ToString()) ? jtoken["sku"].ToString() : null);
                jObject.Add(
                    "inv_itemName",
                    !string.IsNullOrEmpty(jtoken["name"].ToString()) ? jtoken["name"].ToString() : null);

                jObject.Add("inv_unitCode", null);
                jObject.Add("inv_unitName", null);
                jObject.Add("inv_unitPrice", unit_price);
                jObject.Add(
                    "inv_quantity",
                    !string.IsNullOrEmpty(jtoken["quantity"].ToString()) ? jtoken["quantity"].ToString() : null);

                jObject.Add("inv_TotalAmountWithoutVat", totalAmountWithoutVat);
              
                jObject.Add( "inv_vatPercentage",  "10");
                jObject.Add( "ma_thue", "10");


                

                jObject.Add( "inv_vatAmount", vatAmount);

                jObject.Add("inv_TotalAmount", totalAmount);

                jObject.Add("inv_promotion", false);

                jObject.Add( "inv_discountPercentage", discountpercentage );
                jObject.Add( "inv_discountAmount", discountAmount);

                return jObject;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public static async Task<JObject> ConvertData(JObject data_api, string mau_hd, string inv_invoiceseries, string inv_invoicecode_id)
        {
            try
            {
                var inv_TotalAmount = 0.0;
                var inv_TotalAmountWithoutVat = 0.0;
                var inv_vatAmount = 0.0;
                var inv_discountAmount = 0.0;
                //double thue = !string.IsNullOrEmpty(row["VAT"].ToString()) ? Convert.ToDouble(row["VAT"].ToString()) : 0.0;
                Guid inv_InvoiceAuth_id = Guid.NewGuid();
                var masterJObject = await ConvertMasterObject(data_api, mau_hd, inv_invoiceseries, inv_invoicecode_id, inv_InvoiceAuth_id);

                //var SoPhieu = row["SoPhieu"].ToString();
                var jArrayDetail = GetJArrayDetail(data_api,out inv_TotalAmount,out inv_TotalAmountWithoutVat,out inv_vatAmount,out inv_discountAmount, inv_InvoiceAuth_id);


                masterJObject.Add("inv_TotalAmount", inv_TotalAmount);
                masterJObject.Add("inv_TotalAmountWithoutVat", inv_TotalAmountWithoutVat);
                masterJObject.Add("inv_vatAmount", inv_vatAmount);
                masterJObject.Add("inv_discountAmount", inv_discountAmount);

                //if (!string.IsNullOrEmpty(row["SuaTienThue1"].ToString()) && bool.Parse(row["SuaTienThue1"].ToString()) == true)
                //{


                //    masterJObject.Add("inv_TotalAmountWithoutVat", inv_TotalAmountWithoutVat);

                //    inv_vatAmount = !string.IsNullOrEmpty(row["TienThue"].ToString()) ? double.Parse(row["TienThue"].ToString()) : 0;

                //    masterJObject.Add("inv_vatAmount", inv_vatAmount);

                //    masterJObject.Add("inv_discountAmount", inv_discountAmount);
                //    inv_TotalAmount = inv_TotalAmountWithoutVat - inv_discountAmount + inv_vatAmount;


                //    masterJObject.Add("inv_TotalAmount", inv_TotalAmount);
                //}
                //else
                //{

                //}
                var data = new JObject
                {
                    {"tab_id", "TAB00188"},
                    {"tab_table", "inv_InvoiceAuthDetail"},
                    {"data", jArrayDetail}
                };
                var details = new JArray(data);
                masterJObject.Add("details", details);

                return masterJObject;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        private static JArray GetJArrayDetail(JObject data_api,out double inv_TotalAmount,out double inv_TotalAmountWithoutVat,out double inv_vatAmount,out double inv_discountAmount, Guid inv_InvoiceAuth_id)
        {
            inv_TotalAmount = 0.0;
            inv_TotalAmountWithoutVat = 0.0;
            inv_vatAmount = 0.0;
            inv_discountAmount = 0.0;
         
            var jArrayDetail = new JArray();

            var line_items = JArray.Parse(data_api.GetValue("line_items").ToString());

            //string whereVoucherDetail = $" WHERE SoPhieu = '{SoPhieu}' AND GianTiep=0 AND ButToanThueGTGT=0 ORDER BY STT";
            //string sqlSelectInvoiceDetail = CommonConstants.SqlSelectSaInvoiceDetail2017;
            //sqlSelectInvoiceDetail += whereVoucherDetail;

            //var dataTableInvoiceDetail = DataContext.GetDataTableTest(sqlConnection, sqlSelectInvoiceDetail);
            if (line_items.Count > 0)
            {
                int count = 0;
                foreach (var item in line_items)
                {
                    count++;
                    //inv_vatAmount1 = !string.IsNullOrEmpty(row["TienThue"].ToString()) ? double.Parse(row["TienThue"].ToString()) : 0;
                    var detailJObject = ConvertDetailJObjectSaInvoiceDetail(item, inv_InvoiceAuth_id,count);

                    inv_TotalAmount += (double)detailJObject["inv_TotalAmount"];
                    inv_TotalAmountWithoutVat += (double)detailJObject["inv_TotalAmountWithoutVat"];
                    inv_vatAmount += (double)detailJObject["inv_vatAmount"];
                    inv_discountAmount += (double)detailJObject["inv_discountAmount"];
                    jArrayDetail.Add(detailJObject);



                }


                //else
                //{
                //    foreach (DataRow dataRowInvoiceDetail in dataTableInvoiceDetail.Rows)
                //    {

                //        var detailJObject = ConvertDetailJObjectSaInvoiceDetail(dataRowInvoiceDetail, thue, inv_vatAmount1, suathue);
                //        inv_TotalAmount += (double)detailJObject["inv_TotalAmount"];
                //        inv_TotalAmountWithoutVat += (double)detailJObject["inv_TotalAmountWithoutVat"];
                //        inv_vatAmount += (double)detailJObject["inv_vatAmount"];
                //        inv_discountAmount += (double)detailJObject["inv_discountAmount"];
                //        jArrayDetail.Add(detailJObject);



                //    }
                //}
            }

            return jArrayDetail;
        }
    }
}