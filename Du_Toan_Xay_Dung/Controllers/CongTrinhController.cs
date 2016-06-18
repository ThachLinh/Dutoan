using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Du_Toan_Xay_Dung.Models;
using Du_Toan_Xay_Dung.Handlers;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
//using CRUDDeom.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Du_Toan_Xay_Dung.Controllers
{
    public class CongTrinhController : Controller
    {
        DataDTXDDataContext _db = new DataDTXDDataContext();
        //
        // GET: /CongTrinh/

        public ActionResult Index()
        {
            if (SessionHandler.User != null)
            {
                var user = SessionHandler.User;

                 

                ViewData["List_CongTrinh"] = (from ct in _db.CongTrinhs
                                              where ct.Email.Equals(user.Email)
                                              select new CongTrinhViewModel()
                                              {
                                                  MaCT = ct.MaCT,
                                                  TenCT = ct.TenCT,
                                                  MoTa = ct.MoTa,
                                                  Gia = ct.Gia
                                              }).ToList();

                ViewData["List_HangMuc"] = (from hm in _db.HangMucs
                                            where hm.MaCT.Equals(hm.MaCT)
                                            select new HangMucViewModel()
                                            {
                                                MaCT = hm.MaCT,
                                                MaHM = hm.MaHM,
                                                TenHM = hm.TenHM,
                                                MoTa = hm.MoTa,
                                                Gia = hm.Gia
                                            }).ToList();

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


        public ActionResult ChiTiet_CongTrinh(string id)
        {

            ViewData["CongTrinh"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(id)).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();

            ViewData["ChiTiet_CongTrinh"] = (from hm in _db.HangMucs
                                    where hm.MaCT.Equals(id)
                                    select new HangMucViewModel()
                                    {
                                        MaHM = hm.MaHM,
                                        TenHM = hm.TenHM,
                                        MoTa = hm.MoTa,
                                        Gia = hm.Gia
                                    }).ToList();

            return View();
        }



        public ActionResult UpdateCongTrinh(string ID)
        {
            if (SessionHandler.User == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (ID != null)
                {
                    ViewData["CongTrinh_Update"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(ID)).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCongTrinh(FormCollection form)
        {
            if (SessionHandler.User == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string ID = form["Ma"];
                if (ID != null)
                {
                    var congtrinh = _db.CongTrinhs.Single(i => i.MaCT.Equals(ID));
                    if (congtrinh != null)
                    {
                        congtrinh.TenCT = form["TenCT"];
                        congtrinh.MoTa = form["MoTa"];
                        congtrinh.Gia = Convert.ToDecimal(form["Gia"]);
                        
                        //hinh anh

                        _db.SubmitChanges();
                        return RedirectToAction("Index", "CongTrinh");
                    }
                }
            }
            return View();
        }

        public ActionResult UpdateHangMuc(string ID)
        {

            if (ID != null)
            {
                ViewData["HangMuc_Update"] = _db.HangMucs.Where(i => i.MaHM.Equals(ID)).Select(i => new HangMucViewModel(i)).FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateHangMuc(FormCollection form)
        {
            string ID = form["txtma"];
            if (ID != null)
            {
                var hangmuc = _db.HangMucs.Single(i => i.MaHM.Equals(ID));
                if (hangmuc != null)
                {
                    hangmuc.MaCT = form["txtmact"];
                    hangmuc.TenHM = form["txttenhm"];
                    hangmuc.MoTa = form["txtmota"];
                    hangmuc.Gia = Convert.ToDecimal(form["txtgia"]);

                    //hinh anh

                    _db.SubmitChanges();
                }
            }
            ViewData["HangMuc_Update"] = _db.HangMucs.Where(i => i.MaHM.Equals(ID)).Select(i => new HangMucViewModel(i)).FirstOrDefault();
            return View();
        }


        public ActionResult ThemCongTrinh()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemCongTrinh(CongTrinhViewModel obj)
        {
            if (SessionHandler.User == null)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var index = _db.CongTrinhs.Count() + 1;         //Phai chinh sua
                var congtrinh = new CongTrinh();
                congtrinh.MaCT = "CT" + index;
                congtrinh.Email = SessionHandler.User.Email;
                congtrinh.TenCT = obj.TenCT;
                congtrinh.MoTa = obj.MoTa;
                congtrinh.Gia = 0;

                _db.CongTrinhs.InsertOnSubmit(congtrinh);
                _db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("ThemCongTrinh");
            }
        }

        public ActionResult ThemHangMuc(string id)
        {
            ViewData["MaCT_ThemHangMuc"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult ThemHangMuc(FormCollection form)
        {
            string ID = form["txtmact"];
            if (ID != null)
            {
                //lay dong cuoi cung bang hangmuc
                var mahm_last = _db.HangMucs.OrderByDescending(i => i.MaHM).Select(i => i.MaHM).FirstOrDefault();
                var mahm_now = (dynamic)null;
                int d_hm = 0;
                if (mahm_last != null)
                {
                    mahm_now = mahm_last.ToString().Substring(2, mahm_last.ToString().Length - 2);
                    d_hm = Convert.ToInt32(mahm_now.ToString());
                    mahm_now = "HM" + Convert.ToString(d_hm + 1);
                }
                else
                {
                    mahm_now = "HM1";
                }

                HangMuc hm = new HangMuc();
                hm.MaHM = mahm_now;
                hm.MaCT = ID;
                hm.TenHM = form["txttenhm"];
                hm.MoTa = form["txtmota"];
                hm.Gia = 0;

                _db.HangMucs.InsertOnSubmit(hm);
                _db.SubmitChanges();

            }

            return RedirectToAction("ChiTiet_CongTrinh", "CongTrinh", new { Id = ID });
        }

        public ActionResult ExportToExcel(string ID)
        {
            if(ID!=null)
            {

                var congtrinh = _db.CongTrinhs.Where(i => i.MaCT.Equals(ID)).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
               
                var hangmuc = _db.HangMucs.Where(i => i.MaCT.Equals(ID)).Select(i => new HangMucViewModel(i)).ToList();
                var mahangmucs = _db.HangMucs.Where(i => i.MaCT.Equals(ID)).Select(i => i.MaHM).ToList();
                var hangmuccout=hangmuc.Count;
                var congviec = _db.CongViecs.Where(i => mahangmucs.Contains(i.MaHM)).Select(i => new CongViec_User_ViewModel(i)).ToList();
                var macongviecs = _db.CongViecs.Where(i => mahangmucs.Contains(i.MaHM)).Select(i => i.MaHieuCV_User).ToList();
                var congvieccout=congviec.Count;
                var haophi = _db.ThanhPhanHaoPhis.Where(i => macongviecs.Contains(i.MaHieuCV_User)).Select(i => new HaoPhi_User_ViewModel(i)).ToList();
                var haophicout=haophi.Count;

                using (ExcelPackage pck = new ExcelPackage())
                {
                   ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Hạng mục");
                   ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add("Công việc");
                   ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("Thành phần hao phí");
                   
                    // ADD dữ liệu cho sheet hạng mục
                    ws.Cells["I5"].Value= "BẢNG DỰ TOÁN HẠNG MỤC CÔNG TRÌNH";
                    ws.Cells["J7"].Value=  "Tên công trình;"+"   "+congtrinh.TenCT;
    
     // table  
     ws.Cells["B8"].Value= "Mã hạng mục";
     ws.Cells["B8:D8"].Merge = true;

     ws.Cells["E8"].Value= "Mã công trình";
     ws.Cells["E8:G8"].Merge = true;

     ws.Cells["H8"].Value= "Tên hạng mục";
     ws.Cells["H8:M8"].Merge = true;

     ws.Cells["N8"].Value= "Mô tả"; 
     ws.Cells["N8:Q8"].Merge = true;

     ws.Cells["R8"].Value= "Đơn giá";
     ws.Cells["R8:T8"].Merge = true;
     var dong=(hangmuccout+1).ToString();
     var dong1="H"+dong;
    var dong2="I"+dong;
    
     ws.Cells[dong1].Value="Tổng tiền công trình";

     //
     var r = 9;
     foreach(var row in hangmuc ){
         
         var A = "A" + r;
         var B = "B" + r;
         var C = "C" + r;
         var D = "D" + r;
         var E = "E" + r;
         var F = "F" + r;
         var G = "G" + r;
         var H = "H" + r;
         var N = "N" + r;
         var R = "R" + r;
         var U = "U" + r;
         
         //  content
         
         ws.Cells[B].Value= row.MaHM; 
         ws.Cells[E].Value= row.MaCT;
         ws.Cells[H].Value= row.TenHM; 
         ws.Cells[N].Value= row.MoTa;
         ws.Cells[R].Style.Numberformat.Format = "+SUM(Công việc!U9:U" + (congvieccout + 1) + ")";
         ws.Cells[dong2].Style.Numberformat.Format = "+SUM(R9:R" + (hangmuccout + 1) + ")";
        r++;
        }
                
              
    var A2 = "A" + (r+1);
    var B2 = "B" + (r+1);
    var C2 = "A" + (r+2);
    var D2 = "B" + (r+2);
    // add dữ liệu cho sheet công việc
    ws1.Cells["I6"].Value = "DANH SÁCH CÔNG VIỆC THUỘC HẠNG MỤC";
    ws1.Cells["A8"].Value = "Mã công việc- người dùng";
    ws1.Cells["A8:C8"].Merge = true;

    ws1.Cells["D8"].Value = "Mã hạng mục";
    ws1.Cells["D8:E8"].Merge = true;

    ws1.Cells["F8"].Value = "Mã công việc- định mức";
    ws1.Cells["F8:H8"].Merge = true;

    ws1.Cells["I8"].Value = "Tên công việc";
    ws1.Cells["I8:J8"].Merge = true;

    ws1.Cells["K8"].Value = "Đơn vị";
    ws1.Cells["K8:L8"].Merge = true;

    ws1.Cells["M8"].Value = "Khối lượng";
    ws1.Cells["M8:N8"].Merge = true;

    ws1.Cells["O8"].Value = "Giá vật liệu";
    ws1.Cells["O8:P8"].Merge = true;

    ws1.Cells["Q8"].Value = "Giá nhân công";
    ws1.Cells["Q8:R8"].Merge = true;

    ws1.Cells["S8"].Value = "Giá máy thi công";
    ws1.Cells["S8:T8"].Merge = true;
    

    ws1.Cells["U8"].Value = "Thành tiền";
    ws1.Cells["U8:V8"].Merge = true;
    var r1 = 9;
    foreach (var row in congviec)
    {

        var A = "A" + r1;
        var B = "B" + r1;
        var C = "C" + r1;
        var D = "D" + r1;
        var E = "E" + r1;
        var F = "F" + r1;
        var G = "G" + r1;
        var H = "H" + r1;
        var I = "I" + r1;
        var K=  "K" +  r1;
        var L=  "L" +  r1;
        var N="O"+r1;
        var M="M"+r1;
        var O="O"+ r1;
        var P="P"+r1;
        var Q="Q"+r1;
        var R="R"+r1;
        var S="S"+r1;
        var T="T"+r1;
        var U="U"+r1;
        var V="V"+r1;



        //  content

        ws1.Cells[A].Value = row.MaHieuCV_User;
        ws1.Cells[D].Value = row.MaHM;
        ws1.Cells[F].Value = row.MaHieuCV_DM;
        ws1.Cells[I].Value = row.TenCongViec;
        ws1.Cells[K].Value = row.DonVi;
        ws1.Cells[M].Style.Numberformat.Format = (row.KhoiLuong).ToString();
        ws1.Cells[O].Style.Numberformat.Format = "SUMIF(Thành phần hao phí!H9:H" + (haophicout + 1) + ",\"VL*\",R9:R"+(haophicout+1)+")";
        ws1.Cells[Q].Style.Numberformat.Format = "SUMIF(Thành phần hao phí!H9:H" + (haophicout + 1) + ",\"NC*\",R9:R" + (haophicout + 1) + ")"; ;
        ws1.Cells[S].Style.Numberformat.Format = "SUMIF(Thành phần hao phí!H9:H" + (haophicout + 1) + ",\"MTC*\",R9:R" + (haophicout + 1) + ")"; ;
        ws1.Cells[U].Style.Numberformat.Format = "+PRODUCT(SUM(O9:S9),M9)";
        r1++;
    }
    //add dữ liệu cho sheet thành phần hao phí
    // table 
    ws2.Cells["I5"].Value = "DANH MỤC THÀNH PHẦN HAO PHÍ";
    ws2.Cells["B8"].Value = "Mã hạng mục";
    ws2.Cells["B8:D8"].Merge = true;

    ws2.Cells["E8"].Value = "Mã hiệu công việc- user";
    ws2.Cells["E8:G8"].Merge = true;

    ws2.Cells["H8"].Value = "Tên thành phần";
    ws2.Cells["H8:M8"].Merge = true;

    ws2.Cells["N8"].Value = "Đơn vị";
    ws2.Cells["N8:Q8"].Merge = true;

    ws2.Cells["R8"].Value = "Đơn giá";
    ws2.Cells["R8:T8"].Merge = true;

    //
    var r2 = 9;
    foreach (var row in haophi)
    {

        var A = "A" + r2;
        var B = "B" + r2;
        var C = "C" + r2;
        var D = "D" + r2;
        var E = "E" + r2;
        var F = "F" + r2;
        var G = "G" + r2;
        var H = "H" + r2;
        var N = "N" + r2;
        var R = "R" + r2;

        //  content

        ws2.Cells[B].Value = row.MaHP;
        ws2.Cells[E].Value = row.MaHieuCV_User;
        ws2.Cells[H].Value = row.Ten;
        ws2.Cells[N].Value = row.DonVi;
        ws2.Cells[R].Style.Numberformat.Format = (row.Gia).ToString();
        r++;
    }
                

    Byte[] fileBytes = pck.GetAsByteArray();
    Response.Clear();
    Response.Buffer = true;
    Response.AddHeader("content-disposition", "attachment;filename=DuToanXayDung.xlsx");
    Response.Charset = "";
    Response.ContentType = "application/vnd.ms-excel";
    StringWriter sw = new StringWriter();
    Response.BinaryWrite(fileBytes);
    Response.End();
                }

                return RedirectToAction("Index");

            }



            return View();
        }
    }
}