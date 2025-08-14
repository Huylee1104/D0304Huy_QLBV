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

        public C0304BangKeThuController(S0304HTTTService htttService, S0304NhanVienService nhanVienService,
            IConfiguration configuration, IWebHostEnvironment env)
        {
            _htttService = htttService;
            _nhanVienService = nhanVienService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _env = env;
        }

        public IActionResult Index()
        {
            // Lấy danh sách HTTT
            var dsHTTT = _htttService.GetAllHTTT();
            System.Diagnostics.Debug.WriteLine("DSHTTT: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsHTTT));
            ViewBag.DSHTTT = dsHTTT;

            // Lấy danh sách Nhân viên
            var dsNhanVien = _nhanVienService.GetAllNhanVien();
            System.Diagnostics.Debug.WriteLine("DSNhanVien: " + Newtonsoft.Json.JsonConvert.SerializeObject(dsNhanVien));
            ViewBag.DSNhanVien = dsNhanVien;

            //var initial = new { NgayBatDau = (string)null, NgayKetThuc = (string)null };
            //ViewBag.InitialRange = JsonConvert.SerializeObject(initial);
            return View("~/Views/V0304/V0304BangKeThu.cshtml");
        }

        [HttpPost]
        public IActionResult Index(string NgayBatDau, string NgayKetThuc, long IDChiNhanh, long? IDHTTT, long? IDNhanVien)
        {
            // 1. Gọi Stored Procedure với tham số NgayBatDau, NgayKetThuc
            var data = CallStoredProcedure(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);
            ViewBag.Data = data;

            // Lưu tham số vào Session
            HttpContext.Session.SetString("NgayBatDau", NgayBatDau);
            HttpContext.Session.SetString("NgayKetThuc", NgayKetThuc);
            HttpContext.Session.SetString("IDChiNhanh", IDChiNhanh.ToString());
            HttpContext.Session.SetString("IDHTTT", IDHTTT?.ToString() ?? "");
            HttpContext.Session.SetString("IDNhanVien", IDNhanVien?.ToString() ?? "");

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

            return View("~/Views/V0304/V0304BangKeThu.cshtml");
        }

        public List<M0304Huy_Mau4> CallStoredProcedure(
            string NgayBatDau,
            string NgayKetThuc,
            long idCN,
            long? idHTTT = null,
            long? idNhanVien = null)
        {
            var result = new List<M0304Huy_Mau4>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("Huy_BKTTNT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TuNgay", NgayBatDau);
                cmd.Parameters.AddWithValue("@DenNgay", NgayKetThuc);
                cmd.Parameters.AddWithValue("@IDCN", idCN);
                cmd.Parameters.AddWithValue("@IDHTTT", (object?)idHTTT ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IDNhanVien", (object?)idNhanVien ?? DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new M0304Huy_Mau4
                        {
                            STT = reader["STT"] != DBNull.Value ? Convert.ToInt32(reader["STT"]) : 0,
                            MaYTe = reader["MaYTe"] as string,
                            HoVaTen = reader["HoVaTen"] as string,
                            QuyenSo = reader["QuyenSo"] as string,
                            SoBienLai = reader["SoBienLai"] as string,
                            Loai = reader["Loai"] as string,
                            NgayThu = reader["NgayThu"] != DBNull.Value ? (DateTime?)reader["NgayThu"] : null,
                            Huy = reader["Huy"] != DBNull.Value ? (decimal?)reader["Huy"] : null,
                            Hoan = reader["Hoan"] != DBNull.Value ? (decimal?)reader["Hoan"] : null,
                            SoTien = reader["SoTien"] != DBNull.Value ? (decimal?)reader["SoTien"] : null,
                            IDCN = reader["IDCN"] != DBNull.Value ? (long?)reader["IDCN"] : null,
                            IDHTTT = reader["IDHTTT"] != DBNull.Value ? (long?)reader["IDHTTT"] : null,
                            IDNhanVien = reader["IDNhanVien"] != DBNull.Value ? (long?)reader["IDNhanVien"] : null
                        };
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private List<M0304ThongTinDoanhNghiep> CallStoredDN(long idCN)
        {
            var DN = new List<M0304ThongTinDoanhNghiep>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("Huy_TTDN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Truyền thẳng chuỗi hoặc DBNull nếu null hoặc rỗng
                    cmd.Parameters.AddWithValue("@id", idCN);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new M0304ThongTinDoanhNghiep
                            {
                                TenCSKCB = reader["TenCSKCB"] as string,
                                TenCoQuanChuyenMon = reader["TenCoQuanChuyenMon"] as string,
                                DiaChi = reader["DiaChi"] as string,
                                DienThoai = reader["DienThoai"] as string
                            };
                            DN.Add(item);
                        }
                    }
                }
            }
            return DN;
        }

        public IActionResult ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Lấy lại tham số từ Session
            string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
            string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
            long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
            long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
            long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");

            var logoPath = Path.Combine(_env.WebRootPath, "dist", "img", "W0304", "logo.png");

            // Lấy dữ liệu từ service
            var nvService = new S0304NhanVienService(_env);
            var htttService = new S0304HTTTService(_env);
            // Lấy dữ liệu từ stored procedure
            var data = CallStoredProcedure(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);

            string tenHTTT = htttService.GetAllHTTT()
                .FirstOrDefault(ht => ht.id == IDHTTT)?.ten ?? "Tất cả";

            List<M0304NhanVienModel> danhSachNhanVien = null;
            string tenNhanVien = "Tất cả nhân viên";
            if (IDNhanVien != 0)
            {
                tenNhanVien = nvService.GetAllNhanVien()
                    .FirstOrDefault(nv => nv.Id == IDNhanVien)?.Ten ?? "Không rõ";
            }
            else
            {
                // Lấy danh sách ID nhân viên thực tế có trong _data
                var idNhanVienCoDuLieu = data
                    .Select(d => d.IDNhanVien)
                    .Distinct()
                    .ToList();

                // Lấy danh sách nhân viên từ service nhưng chỉ giữ lại những ID này
                danhSachNhanVien = nvService.GetAllNhanVien()
                    .Where(nv => idNhanVienCoDuLieu.Contains(nv.Id))
                    .ToList();
            }

            if (data == null || !data.Any())
                return Content("Không có dữ liệu để xuất PDF.");

            // Tính tổng các giá trị
            decimal tongHuy = data.Sum(r => r.Huy ?? 0m);
            decimal tongHoan = data.Sum(r => r.Hoan ?? 0m);
            decimal tongSoTien = data.Sum(r => r.SoTien ?? 0m);
            decimal tongChenhLech = data.Sum(r => (r.SoTien ?? 0m) - ((r.Huy ?? 0m) + (r.Hoan ?? 0m)));
            // Tính toán theo nhân viên
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

            var dataDN = CallStoredDN(IDChiNhanh); // Lấy thông tin doanh nghiệp
            // Tạo template
            var document = new P0304ReportTemplatePDF(data, dataDN, NgayBatDau,
                NgayKetThuc, tenNhanVien, tenHTTT, logoPath, tongHuy, tongHoan, tongSoTien,
                tongChenhLech, danhSachNhanVien, tongTheoNhanVien);
            var pdfBytes = document.GeneratePdf();

            Response.Headers["Content-Disposition"] = "attachment; filename=Report.pdf";

            return File(pdfBytes, "application/pdf");
        }

        public IActionResult ExportExcel()
        {
            // Lấy lại tham số từ Session
            string NgayBatDau = HttpContext.Session.GetString("NgayBatDau");
            string NgayKetThuc = HttpContext.Session.GetString("NgayKetThuc");
            long IDChiNhanh = Convert.ToInt64(HttpContext.Session.GetString("IDChiNhanh") ?? "0");
            long IDHTTT = Convert.ToInt64(HttpContext.Session.GetString("IDHTTT") ?? "0");
            long IDNhanVien = Convert.ToInt64(HttpContext.Session.GetString("IDNhanVien") ?? "0");
            var logoPath = Path.Combine(_env.WebRootPath, "dist", "img", "W0304", "logo.png");

            // Lấy dữ liệu từ service
            var nvService = new S0304NhanVienService(_env);
            var htttService = new S0304HTTTService(_env);

            // Lấy dữ liệu từ stored procedure
            var data = CallStoredProcedure(NgayBatDau, NgayKetThuc, IDChiNhanh, IDHTTT, IDNhanVien);

            string tenHTTT = htttService.GetAllHTTT()
                .FirstOrDefault(ht => ht.id == IDHTTT)?.ten ?? "Tất cả";
            List<M0304NhanVienModel> danhSachNhanVien = null;
            string tenNhanVien = "Tất cả nhân viên";
            if (IDNhanVien != 0)
            {
                tenNhanVien = nvService.GetAllNhanVien()
                    .FirstOrDefault(nv => nv.Id == IDNhanVien)?.Ten ?? "Không rõ";
            }
            else
            {
                // Lấy danh sách ID nhân viên thực tế có trong _data
                var idNhanVienCoDuLieu = data
                    .Select(d => d.IDNhanVien)
                    .Distinct()
                    .ToList();

                // Lấy danh sách nhân viên từ service nhưng chỉ giữ lại những ID này
                danhSachNhanVien = nvService.GetAllNhanVien()
                    .Where(nv => idNhanVienCoDuLieu.Contains(nv.Id))
                    .ToList();
            }

            if (data == null || !data.Any())
                return Content("Không có dữ liệu để xuất Excel.");

            // Tính tổng các giá trị
            decimal tongHuy = data.Sum(r => r.Huy ?? 0m);
            decimal tongHoan = data.Sum(r => r.Hoan ?? 0m);
            decimal tongSoTien = data.Sum(r => r.SoTien ?? 0m);
            decimal tongChenhLech = data.Sum(r => (r.SoTien ?? 0m) - ((r.Huy ?? 0m) + (r.Hoan ?? 0m)));
            // Tính toán theo nhân viên
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

            var dataDN = CallStoredDN(IDChiNhanh); // Lấy thông tin doanh nghiệp

            var excelReport = new P0304ExcelReportTemplate(data, dataDN, NgayBatDau, NgayKetThuc,
                tenNhanVien, tenHTTT, logoPath, tongHuy, tongHoan, tongSoTien,
                tongChenhLech, danhSachNhanVien, tongTheoNhanVien);

            var excelBytes = excelReport.GenerateExcel();

            Response.Headers["Content-Disposition"] = "attachment; filename=Report.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
