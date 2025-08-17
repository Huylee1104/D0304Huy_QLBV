using ClosedXML.Excel;
using M0304.Models.BangKeThu;
using M0304.Models.PhanTrang;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P0304.PDFDocument;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using S0304BangKeThu.Services;
using S0304FirstLoadFlat.Services;
using S0304HTTT.Services;
using S0304NhanVien.Services;
using S0304Report.Services;
using S0304ThongTinDoanhNghiep.Services;
using System.Data;
using System.Globalization;


namespace Huy_QLBV.Controllers.C0304
{
    [Route("C0304BangKeThu")]
    public class C0304BangKeThuController : Controller
    {
        //private string _maChucNang = "/c0304_bang_ke_thu";
        //private IMemoryCachingServices _memoryCache;

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
            IS0304ThongTinDoanhNghiepService thongTinDoanhNghiep, IS0304BangKeThuService bangKeThuService, IS0304ReportService reportService
            /*, IMemoryCachingServices memoryCache*/ )
        {
            _htttService = htttService;
            _nhanVienService = nhanVienService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _env = env;
            _firstLoadFlag = firstLoadFlag;
            _thongTinDoanhNghiep = thongTinDoanhNghiep;
            _bangKeThuService = bangKeThuService;
            _reportService = reportService;
            //_memoryCache = memoryCache;
        }
        [HttpGet("")]
        public async Task<IActionResult>()
        {
            //var quyenVaiTro = await _memoryCache.getQuyenVaiTro(_maChucNang);
            //if (quyenVaiTro == null)
            //{
            //    return RedirectToAction("NotFound", "Home");
            //}
            //ViewBag.quyenVaiTro = quyenVaiTro;
            //ViewData["Title"] = CommonServices.toEmptyData(quyenVaiTro);

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
                _firstLoadFlag.IsFirstLoad = false; 
            ViewBag.IsFirstLoadFromServer = firstLoad;

            var dsHTTT = _htttService.GetAllHTTT();
            System.Diagnostics.Debug.WriteLine("DSHTTT: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsHTTT));
            ViewBag.DSHTTT = dsHTTT;

            var dsNhanVien = _nhanVienService.GetAllNhanVien();
            System.Diagnostics.Debug.WriteLine("DSNhanVien: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsNhanVien));
            ViewBag.DSNhanVien = dsNhanVien;

            var model = new M0304PhanTrang
            {
                Data = new List<M0304BangKeThu>(),
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 15,
                TotalItems = 0
            };

            return View("~/Views/V0304/V0304BangKeThu.cshtml", model);
        }

        [HttpPost("index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>(string NgayBatDau, string NgayKetThuc, 
            long IDChiNhanh, long? IDHTTT, long? IDNhanVien, int page, int pageSize)
        {
            //var quyenVaiTro = await _memoryCache.getQuyenVaiTro(_maChucNang);
            //if (quyenVaiTro == null)
            //{
            //    return RedirectToAction("NotFound", "Home");
            //}
            //ViewBag.quyenVaiTro = quyenVaiTro;
            //ViewData["Title"] = CommonServices.toEmptyData(quyenVaiTro);

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
                _firstLoadFlag.IsFirstLoad = false; 
            ViewBag.IsFirstLoadFromServer = firstLoad;

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 15;

            bool hasNewFilter =
                !(string.IsNullOrWhiteSpace(NgayBatDau)
               && string.IsNullOrWhiteSpace(NgayKetThuc)
               && IDChiNhanh == 0
               && IDHTTT == null
               && IDNhanVien == null);

            if (!hasNewFilter)
            {
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
                HttpContext.Session.SetString("NgayBatDau", NgayBatDau ?? "");
                HttpContext.Session.SetString("NgayKetThuc", NgayKetThuc ?? "");
                HttpContext.Session.SetString("IDChiNhanh", IDChiNhanh.ToString());
                HttpContext.Session.SetString("IDHTTT", IDHTTT?.ToString() ?? "");
                HttpContext.Session.SetString("IDNhanVien", IDNhanVien?.ToString() ?? "");
            }

            var data = _bangKeThuService.S0304BangKeThu(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);
            var totalItems = data.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var dsHTTT = _htttService.GetAllHTTT();
            ViewBag.DSHTTT = dsHTTT;

            var dsNhanVien = _nhanVienService.GetAllNhanVien();
            ViewBag.DSNhanVien = dsNhanVien;

            var tenHTTT = dsHTTT.FirstOrDefault(x => x.id == IDHTTT)?.ten ?? "";

            var tenNhanVien = dsNhanVien.FirstOrDefault(x => x.Id == IDNhanVien)?.Ten ?? "";

            ViewBag.tenHTTT = tenHTTT;
            ViewBag.IDHTTT = IDHTTT;
            ViewBag.tenNhanVien = tenNhanVien;
            ViewBag.IDNhanVien = IDNhanVien;
            ViewBag.NgayBatDau = NgayBatDau;
            ViewBag.NgayKetThuc = NgayKetThuc;
            ViewBag.IDChiNhanh = IDChiNhanh;

            var model = new M0304PhanTrang
            {
                Data = pagedData,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems,
            };
            return PartialView("~/Views/V0304/V0304BangKeThuPartial.cshtml", model);
        }

        [HttpGet("ExportPdf")]
        public async Task<IActionResult> ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
            string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
            long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
            long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
            long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");

            var logoPath = Path.Combine(_env.WebRootPath, "dist", "img", "W0304", "logo.png");

            var reportData = await _reportService.S0304ReportData(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);

            if (reportData?.Data == null || !reportData.Data.Any())
            {
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "Không có dữ liệu để xuất PDF.";

                return RedirectToAction(""); 
            }

            var document = new P0304ReportTemplatePDF(
                reportData.Data,
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

            var pdfBytes = document.GeneratePdf();

            Response.Headers["Content-Disposition"] = "attachment; filename=Report.pdf";
            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
        string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
        string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
        long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
        long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
        long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");

        var reportData = await _reportService.S0304ReportData(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);
        if (reportData?.Data == null || !reportData.Data.Any())
        {
            TempData["ToastType"] = "warning";
            TempData["ToastMessage"] = "Không có dữ liệu để xuất Excel.";

            return RedirectToAction("");
        }

            var excelReport = new P0304ExcelReportTemplate(
            reportData.Data,       
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

        var excelBytes = excelReport.GenerateExcel();

        Response.Headers["Content-Disposition"] = "attachment; filename=Report.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
