using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Du_Toan_Xay_Dung.Models
{
    public class HaoPhi_User_ViewModel
    {
        public HaoPhi_User_ViewModel() { }

        public HaoPhi_User_ViewModel(ThanhPhanHaoPhi obj)
        {
            MaHP = obj.MaHP;
            MaHieuCV_User = obj.MaHieuCV_User;
            Ten = obj.Ten;
            DonVi = obj.DonVi;
            Gia = obj.Gia;
        }
        public string MaHP { get; set; }
        public string MaHieuCV_User { get; set; }
        public string Ten { get; set; }
        public string DonVi { get; set; }
        public decimal Gia { get; set; }
    }
}