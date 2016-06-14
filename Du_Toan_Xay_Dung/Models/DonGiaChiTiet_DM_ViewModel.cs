using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Du_Toan_Xay_Dung.Models
{
    public class DonGiaChiTiet_DM_ViewModel
    {
        public DonGiaChiTiet_DM_ViewModel() { }
        public string MaHieuCV { get; set; }
        public decimal SoLuong { get; set; }
        public string DonViCV { get; set; }
        public string TenCT { get; set; }
        public string DonViCT { get; set; }
        public decimal ?Gia { get; set; }
    }
}