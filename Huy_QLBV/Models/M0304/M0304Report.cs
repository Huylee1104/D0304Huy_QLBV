using M0304NhanVien.Models;
using System.Collections.Generic;
using Huy_QLBV.Models.M0304; // namespace chứa M0304Huy_Mau4

namespace M0304Report.Models
{
    public class M0304ReportData
    {
        public List<M0304Huy_Mau4> Data { get; set; }  // ✅ Đổi từ object
        public List<M0304ThongTinDoanhNghiep> DataDN { get; set; } // nếu DataDN cũng là list
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
        public string TenNhanVien { get; set; }
        public string TenHTTT { get; set; }
        public string LogoPath { get; set; }
        public decimal TongHuy { get; set; }
        public decimal TongHoan { get; set; }
        public decimal TongSoTien { get; set; }
        public decimal TongChenhLech { get; set; }
        public List<M0304NhanVienModel> DanhSachNhanVien { get; set; }
        public List<M0304TongTheoNhanVien> TongTheoNhanVien { get; set; }
    }
}
