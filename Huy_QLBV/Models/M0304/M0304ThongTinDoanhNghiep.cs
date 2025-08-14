
namespace Huy_QLBV.Models.M0304
{

	public partial class M0304ThongTinDoanhNghiep
	{
		public long ID { get; set; }
		public string MaCSKCB { get; set; }
		public string TenCSKCB { get; set; }
		public string TenCoQuanChuyenMon { get; set; }
		public string DiaChi { get; set; }
		public Nullable<long> IDTinh { get; set; }
		public Nullable<long> IDQuan { get; set; }
		public string DienThoai { get; set; }
		public string Email { get; set; }
		public string HangBenhVien { get; set; }
		public string TuyenCMKT { get; set; }
		public string LoaiBenhVien { get; set; }
		public string UserNameBHXH { get; set; }
		public string PasswordBHXH { get; set; }
		public Nullable<bool> Active { get; set; }
		public Nullable<int> MaHang { get; set; }
		public string SoTk { get; set; }
		public string ChuTK { get; set; }
		public string NganHang { get; set; }
		public string MaSoThue { get; set; }
		public string Hotline { get; set; }
		public string Website { get; set; }
		public string Fanpage { get; set; }
		public Nullable<long> IDThuTruongDonVi { get; set; }
		public Nullable<long> IDKeToanTruong { get; set; }
		public string MaLienThongTTCSKCB { get; set; }
		public string MatKhauLienThongTTCSKCB { get; set; }
		public string APICheckIn { get; set; }
		public string APIGiamDinh { get; set; }
		public string APINhanBenh { get; set; }
		public string APIToken { get; set; }
		public string APITokenDaoTao { get; set; }
		public string PasswordDaoTao { get; set; }
		public Nullable<bool> CongBHXH { get; set; }
		public string CapBenhVien { get; set; }
		public string MaLTDuocQuocGia { get; set; }
		public string TaiKhoanLTDuocQuocGia { get; set; }
		public string MatKhauLTDuocQuocGia { get; set; }
		public string TenPhieuInVaoVien { get; set; }
		public string TenPhieuInHangHoa { get; set; }
		public string APICongVan { get; set; }
		public string MauIn { get; set; }
		public string TenDangNhap { get; set; }
		public long IDChiNhanh { get; set; }
	}
}
