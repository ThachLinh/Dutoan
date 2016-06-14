using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Du_Toan_Xay_Dung.Models;
using Du_Toan_Xay_Dung.Handlers;

namespace Du_Toan_Xay_Dung.Controllers
{
    public class HangMucController : Controller
    {

        public DataDTXDDataContext _db = new DataDTXDDataContext();
        //
        // GET: /TT_D_KLCongViec/

        public ActionResult Index(string ID)
        {

            if (ID != null)
            {
                int d = ID.IndexOf(',');
                if (d == -1)
                {
                    ViewData["CongTrinh"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(ID)).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
                }
                else
                {
                    var Arr_ID = ID.Split(',');
                    ViewData["CongTrinh"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(Arr_ID[0])).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
                    ViewData["HangMuc_ChiTiet"] = _db.CongViecs.Where(i => i.MaHM.Equals(Arr_ID[1])).Select(i => new CongViec_User_ViewModel(i)).ToList();
                    ViewData["HangMuc"] = _db.HangMucs.Where(i => i.MaHM.Equals(Arr_ID[1])).Select(i => new HangMucViewModel(i)).FirstOrDefault();
                }
            }

            if (SessionHandler.User != null)
            {
                ViewData["DSCongTrinh"] = _db.CongTrinhs.Where(i => i.Email.Equals(SessionHandler.User.Email)).Select(i => new CongTrinhViewModel(i)).ToList();
            }


            ViewData["DSDinhMuc"] = _db.DinhMucs.Select(i => new DinhMucViewModel(i)).ToList();

            return View();
        }

        public ActionResult Index_2(string ID)
        {
            if (ID != null)
            {
                int d = ID.IndexOf(',');
                if (d == -1)
                {
                    ViewData["CongTrinh"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(ID)).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
                }
                else
                {
                    var Arr_ID = ID.Split(',');
                    ViewData["CongTrinh"] = _db.CongTrinhs.Where(i => i.MaCT.Equals(Arr_ID[0])).Select(i => new CongTrinhViewModel(i)).FirstOrDefault();
                    ViewData["HangMuc_ChiTiet"] = _db.CongViecs.Where(i => i.MaHM.Equals(Arr_ID[1])).Select(i => new CongViec_User_ViewModel(i)).ToList();
                    ViewData["HangMuc"] = _db.HangMucs.Where(i => i.MaHM.Equals(Arr_ID[1])).Select(i => new HangMucViewModel(i)).FirstOrDefault();
                }
            }

            if (SessionHandler.User != null)
            {
                ViewData["DSCongTrinh"] = _db.CongTrinhs.Where(i => i.Email.Equals(SessionHandler.User.Email)).Select(i => new CongTrinhViewModel(i)).ToList();
            }


            ViewData["DSDinhMuc"] = _db.DinhMucs.Select(i => new DinhMucViewModel(i)).ToList();

            return View();
        }


        [HttpPost]
        public JsonResult getAllPrice(string idDinhMuc)
        {
            decimal totalGiaVL = 0;
            decimal totalGiaNC = 0;
            decimal totalGiaMay = 0;

            var giaVL = _db.ChiTiet_DinhMucs
                .Where(i => i.MaHieuCV_DM.Equals(idDinhMuc) && i.MaVL_NC_MTC.Contains("V")).ToList();
            foreach (var item in giaVL)
            {
                var _temGia = _db.DonGias.Where(i => i.MaVL_NC_MTC.Equals(item.MaVL_NC_MTC)).FirstOrDefault();
                totalGiaVL += _temGia.Gia * item.SoLuong;
            }
            var giaNC = _db.ChiTiet_DinhMucs.Where(i => i.MaHieuCV_DM.Equals(idDinhMuc) && i.MaVL_NC_MTC.Contains("N")).ToList();
            foreach (var item in giaNC)
            {
                var _temGia = _db.DonGias.Where(i => i.MaVL_NC_MTC.Equals(item.MaVL_NC_MTC)).FirstOrDefault();
                totalGiaNC += _temGia.Gia * item.SoLuong;
            }

            var giaMay = _db.ChiTiet_DinhMucs.Where(i => i.MaHieuCV_DM.Equals(idDinhMuc) && i.MaVL_NC_MTC.Contains("M")).ToList();
            foreach (var item in giaMay)
            {
                var _temGia = _db.DonGias.Where(i => i.MaVL_NC_MTC.Equals(item.MaVL_NC_MTC)).FirstOrDefault();
                totalGiaMay += _temGia.Gia * item.SoLuong;
            }
            return Json(new { totalGiaVL = totalGiaVL, totalGiaNC = totalGiaNC, totalGiaMay = totalGiaMay, idDinhMuc });
        }



        public ActionResult ChiTiet_VL_NC_MTC(string ID)
        {
            if (ID != null)
            {
                string temp = ID.Substring(0, 3);
                if (temp != "CV_")
                {
                    ViewData["List_VL"] = (from ct in _db.ChiTiet_DinhMucs
                                           join dg in _db.DonGias on ct.MaVL_NC_MTC equals
                                               dg.MaVL_NC_MTC
                                           where ct.MaHieuCV_DM.Equals(ID) && ct.MaVL_NC_MTC.Contains("V")
                                           select new HaoPhi_DM_ViewModel()
                                           {
                                               Ma = ct.MaVL_NC_MTC,
                                               Ten = dg.Ten,
                                               DonVi = dg.DonVi,
                                               Gia = dg.Gia * ct.SoLuong
                                           }).ToList();

                    ViewData["List_NC"] = (from ct in _db.ChiTiet_DinhMucs
                                           join dg in _db.DonGias on ct.MaVL_NC_MTC equals
                                               dg.MaVL_NC_MTC
                                           where ct.MaHieuCV_DM.Equals(ID) && ct.MaVL_NC_MTC.Contains("N")
                                           select new HaoPhi_DM_ViewModel()
                                           {
                                               Ma = ct.MaVL_NC_MTC,
                                               Ten = dg.Ten,
                                               DonVi = dg.DonVi,
                                               Gia = dg.Gia * ct.SoLuong
                                           }).ToList();

                    ViewData["List_MTC"] = (from ct in _db.ChiTiet_DinhMucs
                                            join dg in _db.DonGias on ct.MaVL_NC_MTC equals
                                                dg.MaVL_NC_MTC
                                            where ct.MaHieuCV_DM.Equals(ID) && ct.MaVL_NC_MTC.Contains("M")
                                            select new HaoPhi_DM_ViewModel()
                                            {
                                                Ma = ct.MaVL_NC_MTC,
                                                Ten = dg.Ten,
                                                DonVi = dg.DonVi,
                                                Gia = dg.Gia * ct.SoLuong
                                            }).ToList();
                }
                else
                {
                    ViewData["List_VL"] = _db.ThanhPhanHaoPhis.Where(i => i.MaHieuCV_User.Equals(ID)).Where(i => i.MaHP.Contains("V_HP")).Select(i => new HaoPhi_User_ViewModel(i)).ToList();

                    ViewData["List_NC"] = _db.ThanhPhanHaoPhis.Where(i => i.MaHieuCV_User.Equals(ID)).Where(i => i.MaHP.Contains("N_HP")).Select(i => new HaoPhi_User_ViewModel(i)).ToList();

                    ViewData["List_MTC"] = _db.ThanhPhanHaoPhis.Where(i => i.MaHieuCV_User.Equals(ID)).Where(i => i.MaHP.Contains("M_HP")).Select(i => new HaoPhi_User_ViewModel(i)).ToList();
                }

                ViewData["ID_MaHieuCV"] = ID;

            }
            return View();
        }


        [HttpPost]
        public ActionResult FormSubmit(FormCollection form)
        {

            if (SessionHandler.User == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var txtmacongtrinh = form["txt_ma_congtrinh"];
            var txtmahangmuc = form["txt_ma_hangmuc"];
            var txttenhangmuc = form["txt_ten_hangmuc"];
            var txtmahieucv_dmArr = form["txtmahieucv_dm[]"].Split(',');
            var txttencvArr = form["txttencv[]"].Split(',');
            var txtdonviArr = form["txtdonvi[]"].Split(',');
            var txtkhoiluongArr = form["txtkhoiluong[]"].Split(',');
            var txtgiavlArr = form["txtgiavl[]"].Split(',');
            var txtgiancArr = form["txtgianc[]"].Split(',');
            var txtgiamtcArr = form["txtgiamtc[]"].Split(',');
            var txtthanhtiencArr = form["txtthanhtien[]"].Split(',');
            var txttongtien = form["txt_tongtien"];


            if (txtmacongtrinh != "")
            {
                if (txtmahangmuc == "")                                     //tao hang muc, bao gom cong viec moi
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
                    hm.MaCT = txtmacongtrinh;
                    hm.TenHM = txttenhangmuc;
                    hm.MoTa = "";
                    hm.Gia = Convert.ToDecimal(txttongtien);

                    _db.HangMucs.InsertOnSubmit(hm);

                    //lay dong cuoi cung bang congviec
                    var macv_last = _db.CongViecs.OrderByDescending(i => i.MaHieuCV_User).Select(i => i.MaHieuCV_User).FirstOrDefault();
                    var macv_now = (dynamic)null;
                    int d_cv = 0;

                    if (macv_last != null)
                    {
                        macv_now = macv_last.ToString().Substring(3, macv_last.ToString().Length - 3);
                        d_cv = Convert.ToInt32(macv_now.ToString());
                    }
                    else
                    {
                        d_cv = 0;
                    }

                    //lay dong cuoi cung bang thanhphanhaophi
                    var mahp_last = _db.ThanhPhanHaoPhis.OrderByDescending(o => o.MaHP).Select(o => o.MaHP).FirstOrDefault();
                    var mahp_now = (dynamic)null;
                    int d_hp = 0;
                    if (mahp_last != null)
                    {
                        mahp_now = mahp_last.ToString().Substring(2, mahp_last.ToString().Length - 2);
                        d_hp = Convert.ToInt32(mahp_now.ToString());
                    }
                    else
                    {
                        d_hp = 0;
                    }

                    for (int i = 0; i < txtmahieucv_dmArr.Length; i++)
                    {
                        macv_now = "CV_" + Convert.ToString(d_cv + 1);

                        CongViec cv_user = new CongViec();
                        cv_user.MaHM = mahm_now;
                        cv_user.MaHieuCV_User = macv_now;
                        cv_user.MaHieuCV_DM = txtmahieucv_dmArr[i];
                        cv_user.TenCongViec = txttencvArr[i];
                        cv_user.DonVi = txtdonviArr[i];
                        cv_user.KhoiLuong = Convert.ToDecimal(txtkhoiluongArr[i]);
                        cv_user.GiaVL = Convert.ToDecimal(txtgiavlArr[i]);
                        cv_user.GiaNC = Convert.ToDecimal(txtgiancArr[i]);
                        cv_user.GiaMTC = Convert.ToDecimal(txtgiamtcArr[i]);
                        cv_user.ThanhTien = Convert.ToDecimal(txtthanhtiencArr[i]);

                        //insert thanh phan hao phi
                        var list_haophi = (from ct in _db.ChiTiet_DinhMucs
                                           join dg in _db.DonGias on ct.MaVL_NC_MTC equals
                                               dg.MaVL_NC_MTC
                                           where ct.MaHieuCV_DM.Equals(cv_user.MaHieuCV_DM)
                                           select new HaoPhi_DM_ViewModel()
                                           {
                                               Ma = ct.MaVL_NC_MTC,
                                               Ten = dg.Ten,
                                               DonVi = dg.DonVi,
                                               Gia = dg.Gia * ct.SoLuong
                                           }).ToList();

                        foreach (var k in list_haophi)
                        {
                            mahp_now = "HP" + Convert.ToString(d_hp + 1);

                            ThanhPhanHaoPhi hp = new ThanhPhanHaoPhi();
                            hp.MaHP = mahp_now;
                            hp.MaHieuCV_User = macv_now;
                            hp.Ten = k.Ten;
                            hp.DonVi = k.DonVi;
                            hp.Gia = k.Gia;

                            d_hp = d_hp + 1;

                            _db.ThanhPhanHaoPhis.InsertOnSubmit(hp);
                        }

                        d_cv = d_cv + 1;
                        _db.CongViecs.InsertOnSubmit(cv_user);
                    }
                }
            }
            _db.SubmitChanges();

            return RedirectToAction("Index", "CongTrinh");
        }

    }
}
