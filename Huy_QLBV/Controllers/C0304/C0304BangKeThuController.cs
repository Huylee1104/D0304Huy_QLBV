using ClosedXML.Excel;
using M0304HTTT.Models;
using S0304HTTT.Services;
using Huy_QLBV.Models.M0304;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using M0304NhanVien.Models;
using S0304NhanVien.Services;
using S0304FirstLoadFlat.Services;
using S0304ThongTinDoanhNghiep.Services;
using S0304BangKeThu.Services;
using S0304Report.Services;
using QLBV.Reports;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Data;
using System.Globalization;


namespace Huy_QLBV.Controllers.C0304
{
    public class C0304BangKeThuController : Controller
    {
        private readonly S0304HTTTService _htttService;
        private readonly S0304NhanVienService _nhanVienService;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;
        private readonly S0304FirstLoadFlagService _firstLoadFlag;
        private readonly IS0304ThongTinDoanhNghiepService _thongTinDoanhNghiep;
        private readonly IS0304BangKeThuService _bangKeThuService;
        private readonly IS0304ReportService _reportService;

        public C0304BangKeThuController(S0304HTTTService htttService, S0304NhanVienService nhanVienService,
            IConfiguration configuration, IWebHostEnvironment env, S0304FirstLoadFlagService firstLoadFlag, 
            IS0304ThongTinDoanhNghiepService thongTinDoanhNghiep, IS0304BangKeThuService bangKeThuService, IS0304ReportService reportService)
        {
            _htttService = htttService;
            _nhanVienService = nhanVienService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _env = env;
            _firstLoadFlag = firstLoadFlag;
            _thongTinDoanhNghiep = thongTinDoanhNghiep;
            _bangKeThuService = bangKeThuService;
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            ViewBag.quyenVaiTro = new
            {
                Them = true,
                Sua = true,
                Xoa = true,
                Xuat = true,
                CaNhan = true,
                Xem = true,
            };

            bool firstLoad = _firstLoadFlag.IsFirstLoad;

            if (firstLoad)
                _firstLoadFlag.IsFirstLoad = false; // chỉ dùng 1 lần sau restart server

            ViewBag.IsFirstLoadFromServer = firstLoad;

            // Lấy danh sách HTTT
            var dsHTTT = _htttService.GetAllHTTT();
            System.Diagnostics.Debug.WriteLine("DSHTTT: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsHTTT));
            ViewBag.DSHTTT = dsHTTT;

            // Lấy danh sách Nhân viên
            var dsNhanVien = _nhanVienService.GetAllNhanVien();
            System.Diagnostics.Debug.WriteLine("DSNhanVien: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsNhanVien));
            ViewBag.DSNhanVien = dsNhanVien;

            return View("~/Views/V0304/V0304BangKeThu.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string NgayBatDau, string NgayKetThuc, 
            long IDChiNhanh, long? IDHTTT, long? IDNhanVien, int page, int pageSize)
        {
            ViewBag.quyenVaiTro = new
            {
                Them = true,
                Sua = true,
                Xoa = true,
                Xuat = true,
                CaNhan = true,
                Xem = true,
            };

            bool firstLoad = _firstLoadFlag.IsFirstLoad;

            if (firstLoad)
                _firstLoadFlag.IsFirstLoad = false; // chỉ dùng 1 lần sau restart server

            ViewBag.IsFirstLoadFromServer = firstLoad;

            // ===== Chuẩn hoá page/pageSize =====
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 15;

            // ===== Xác định có filter mới từ form hay không =====
            bool hasNewFilter =
                !(string.IsNullOrWhiteSpace(NgayBatDau)
               && string.IsNullOrWhiteSpace(NgayKetThuc)
               && IDChiNhanh == 0
               && IDHTTT == null
               && IDNhanVien == null);

            if (!hasNewFilter)
            {
                // Không có filter mới -> đọc từ Session
                NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
                NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");

                var sChiNhanh = HttpContext.Session.GetString("IDChiNhanh");
                IDChiNhanh = long.TryParse(sChiNhanh, out var _idcn) ? _idcn : 0;

                var sHTTT = HttpContext.Session.GetString("IDHTTT");
                IDHTTT = long.TryParse(sHTTT, out var _idht) ? _idht : (long?)null;

                var sNV = HttpContext.Session.GetString("IDNhanVien");
                IDNhanVien = long.TryParse(sNV, out var _idnv) ? _idnv : (long?)null;
            }
            else
            {
                // Có filter mới -> lưu vào Session
                HttpContext.Session.SetString("NgayBatDau", NgayBatDau ?? "");
                HttpContext.Session.SetString("NgayKetThuc", NgayKetThuc ?? "");
                HttpContext.Session.SetString("IDChiNhanh", IDChiNhanh.ToString());
                HttpContext.Session.SetString("IDHTTT", IDHTTT?.ToString() ?? "");
                HttpContext.Session.SetString("IDNhanVien", IDNhanVien?.ToString() ?? "");
            }

            // 1. Gọi Stored Procedure với tham số NgayBatDau, NgayKetThuc
            var data = _bangKeThuService.S0304BangKeThu(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);
            var totalItems = data.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Data = pagedData;

            // 2. Load thêm dữ liệu khác (nếu cần)
            var dsHTTT = _htttService.GetAllHTTT();
            ViewBag.DSHTTT = dsHTTT;

            var dsNhanVien = _nhanVienService.GetAllNhanVien();
            ViewBag.DSNhanVien = dsNhanVien;

            // Lấy tên từ ID
            var tenHTTT = dsHTTT.FirstOrDefault(x => x.id == IDHTTT)?.ten ?? "";

            var tenNhanVien = dsNhanVien.FirstOrDefault(x => x.Id == IDNhanVien)?.Ten ?? "";

            // Truyền cho view
            ViewBag.tenHTTT = tenHTTT;
            ViewBag.IDHTTT = IDHTTT;
            ViewBag.tenNhanVien = tenNhanVien;
            ViewBag.IDNhanVien = IDNhanVien;
            ViewBag.NgayBatDau = NgayBatDau;
            ViewBag.NgayKetThuc = NgayKetThuc;
            ViewBag.IDChiNhanh = IDChiNhanh;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View("~/Views/V0304/V0304BangKeThu.cshtml");
        }

        public async Task<IActionResult> ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Lấy tham số từ Session
            string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
            string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
            long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
            long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
            long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");

            var logoPath = Path.Combine(_env.WebRootPath, "dist", "img", "W0304", "logo.png");

            // Lấy dữ liệu từ service
            var reportData = await _reportService.S0304ReportData(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);

            if (reportData?.Data == null || !reportData.Data.Any())
                return Content("Không có dữ liệu để xuất PDF.");

            // Tạo template PDF
            var document = new P0304ReportTemplatePDF(
                reportData.Data,             // List<M0304Huy_Mau4>
                reportData.DataDN,
                reportData.NgayBatDau,
                reportData.NgayKetThuc,
                reportData.TenNhanVien,
                reportData.TenHTTT,
                reportData.LogoPath,
                reportData.TongHuy,
                reportData.TongHoan,
                reportData.TongSoTien,
                reportData.TongChenhLech,
                reportData.DanhSachNhanVien,
                reportData.TongTheoNhanVien
            );

            // Sinh file PDF
            var pdfBytes = document.GeneratePdf();

            // Trả file về client
            Response.Headers["Content-Disposition"] = "attachment; filename=Report.pdf";
            return File(pdfBytes, "application/pdf");
        }

        public async Task<IActionResult> ExportExcel()
        {
        // Lấy tham số từ Session
        string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
        string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
        long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
        long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
        long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");

        // Lấy dữ liệu từ service
        var reportData = await _reportService.S0304ReportData(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);

        if (reportData?.Data == null || !reportData.Data.Any())
            return Content("Không có dữ liệu để xuất Excel.");

        // Tạo Excel report
        var excelReport = new P0304ExcelReportTemplate(
            reportData.Data,               // ✅ List<M0304Huy_Mau4>
            reportData.DataDN,
            reportData.NgayBatDau,
            reportData.NgayKetThuc,
            reportData.TenNhanVien,
            reportData.TenHTTT,
            reportData.LogoPath,
            reportData.TongHuy,
            reportData.TongHoan,
            reportData.TongSoTien,
            reportData.TongChenhLech,
            reportData.DanhSachNhanVien,
            reportData.TongTheoNhanVien
        );

        // Sinh file Excel
        var excelBytes = excelReport.GenerateExcel();

        // Trả file về client
        Response.Headers["Content-Disposition"] = "attachment; filename=Report.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
