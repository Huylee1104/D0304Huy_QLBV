using M0304NhanVien.Models;
using M0304Report.Models;
using S0304HTTT.Services;
using S0304NhanVien.Services;
using S0304BangKeThu.Services;
using S0304ThongTinDoanhNghiep.Services;

namespace S0304Report.Services
{
    public interface IS0304ReportService
    {
        Task<M0304ReportData> S0304ReportData(string ngayBatDau, string ngayKetThuc, long idChiNhanh, long idHTTT, long idNhanVien);
    }

    public class S0304ReportService : IS0304ReportService
    {
        private readonly IWebHostEnvironment _env;
        private readonly S0304NhanVienService _nvService;
        private readonly S0304HTTTService _htttService;
        private readonly IS0304BangKeThuService _bangKeThuRepository;
        private readonly IS0304ThongTinDoanhNghiepService _doanhNghiepService;

        public S0304ReportService(IWebHostEnvironment env, S0304NhanVienService nvService, S0304HTTTService htttService,
            IS0304BangKeThuService bangKeThuRepository, IS0304ThongTinDoanhNghiepService doanhNghiepService)
        {
            _env = env;
            _nvService = nvService;
            _htttService = htttService;
            _bangKeThuRepository = bangKeThuRepository;
            _doanhNghiepService = doanhNghiepService;
        }

        public async Task<M0304ReportData> S0304ReportData(string ngayBatDau, string ngayKetThuc, long idChiNhanh, long idHTTT, long idNhanVien)
        {
            var data = _bangKeThuRepository.S0304BangKeThu(ngayBatDau, ngayKetThuc, idChiNhanh, idHTTT, idNhanVien);

            if (data == null || !data.Any())
                return null;

            var tenHTTT = _htttService.GetAllHTTT().FirstOrDefault(ht => ht.id == idHTTT)?.ten ?? "Tất cả";

            List<M0304NhanVienModel> danhSachNhanVien = null;
            string tenNhanVien = "Tất cả nhân viên";
            if (idNhanVien != 0)
                tenNhanVien = _nvService.GetAllNhanVien().FirstOrDefault(nv => nv.Id == idNhanVien)?.Ten ?? "Không rõ";
            else
            {
                var ids = data.Select(d => d.IDNhanVien).Distinct().ToList();
                danhSachNhanVien = _nvService.GetAllNhanVien().Where(nv => ids.Contains(nv.Id)).ToList();
            }

            decimal tongHuy = data.Sum(r => r.Huy ?? 0m);
            decimal tongHoan = data.Sum(r => r.Hoan ?? 0m);
            decimal tongSoTien = data.Sum(r => r.SoTien ?? 0m);
            decimal tongChenhLech = data.Sum(r => (r.SoTien ?? 0m) - ((r.Huy ?? 0m) + (r.Hoan ?? 0m)));

            var tongTheoNhanVien = data
                .GroupBy(r => r.IDNhanVien)
                .Select(g => new M0304TongTheoNhanVien
                {
                    IDNhanVien = g.Key ?? 0,
                    TongHuy = g.Sum(x => x.Huy ?? 0m),
                    TongHoan = g.Sum(x => x.Hoan ?? 0m),
                    TongSoTien = g.Sum(x => x.SoTien ?? 0m),
                    TongChenhLech = g.Sum(x => (x.SoTien ?? 0m) - ((x.Huy ?? 0m) + (x.Hoan ?? 0m)))
                }).ToList();

            var dataDN = _doanhNghiepService.S0304DoanhNghiepById(idChiNhanh);

            var logoPath = Path.Combine(_env.WebRootPath, "dist", "img", "W0304", "logo.png");

            return new M0304ReportData
            {
                Data = data,
                DataDN = dataDN,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayKetThuc,
                TenNhanVien = tenNhanVien,
                TenHTTT = tenHTTT,
                LogoPath = logoPath,
                TongHuy = tongHuy,
                TongHoan = tongHoan,
                TongSoTien = tongSoTien,
                TongChenhLech = tongChenhLech,
                DanhSachNhanVien = danhSachNhanVien,
                TongTheoNhanVien = tongTheoNhanVien
            };
        }
    }

}