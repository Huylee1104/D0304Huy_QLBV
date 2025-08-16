using ClosedXML.Excel;
using Huy.Helpers;
using M0304NhanVien.Models;
using M0304.Models.BangKeThu;
using M0304.Models.ThongTinDOanhNghiep;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class P0304ExcelReportTemplate
{
    private List<M0304BangKeThu> _data;
    private List<M0304ThongTinDoanhNghiep> _dataDN;
    private string _ngayBatDau;
    private string _ngayKetThuc;
    private string _tenNhanVien;
    private string _tenHTTT;
    private string _logoPath;
    private decimal _tongHuy;
    private decimal _tongHoan;
    private decimal _tongSoTien;
    private decimal _tongChenhLech;
    private List<M0304NhanVienModel> _danhSachNhanVien;
    private List<M0304TongTheoNhanVien> _tongTheoNhanVien;

    public P0304ExcelReportTemplate(List<M0304BangKeThu> data, List<M0304ThongTinDoanhNghiep> dataDN, string ngayBatDau, string ngayKetThuc,
            string tenNhanVien, string tenHTTT, string logoPath, decimal tongHuy, decimal tongHoan, decimal tongSoTien, 
            decimal tongChenhLech, List<M0304NhanVienModel> danhSachNhanVien, List<M0304TongTheoNhanVien> tongTheoNhanVien)
    {
        _data = data;
        _dataDN = dataDN;
        _ngayBatDau = ngayBatDau;
        _ngayKetThuc = ngayKetThuc;
        _tenNhanVien = tenNhanVien;
        _tenHTTT = tenHTTT;
        _logoPath = logoPath;
        _tongHuy = tongHuy;
        _tongHoan = tongHoan;
        _tongSoTien = tongSoTien;
        _tongChenhLech = tongChenhLech;
        _danhSachNhanVien = danhSachNhanVien;
        _tongTheoNhanVien = tongTheoNhanVien;
    }

    public byte[] GenerateExcel()
    {
        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("Báo cáo");

            int currentRow = 1;

            if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
            {
                ws.Range(1, 1, 2, 2).Merge();
                var img = ws.AddPicture(_logoPath)
                    .MoveTo(ws.Cell(1, 1))
                    .Scale(0.2);
                ws.Row(1).Height = 0;
            }

            foreach (var dn in _dataDN)
            {
                ws.Range(1, 3, 1, 10).Merge();
                ws.Cell(1, 3).Value = dn.TenCSKCB ?? "";
                ws.Cell(1, 3).Style.Font.FontSize = 9;

                ws.Range(2, 3, 2, 10).Merge();
                ws.Cell(2, 3).Value = dn.TenCoQuanChuyenMon ?? "";
                ws.Cell(2, 3).Style.Font.FontSize = 9;

                ws.Range(3, 3, 3, 10).Merge();
                ws.Cell(3, 3).Value = dn.DiaChi ?? "";
                ws.Cell(3, 3).Style.Font.FontSize = 9;

                ws.Range(4, 3, 4, 10).Merge();
                ws.Cell(4, 3).Value = dn.DienThoai ?? "";
                ws.Cell(4, 3).Style.Font.FontSize = 9;
            }

            currentRow += 5;

            ws.Range(currentRow, 1, currentRow, 10).Merge();
            ws.Cell(currentRow, 1).Value = "BẢNG KÊ THU TIỀN NGOẠI TRÚ THEO BL/HĐ";
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            ws.Cell(currentRow, 1).Style.Font.FontSize = 14;
            ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow++;

            ws.Range(currentRow, 1, currentRow, 10).Merge();
            DateTime dtStart, dtEnd;
            if (DateTime.TryParse(_ngayBatDau, out dtStart) && DateTime.TryParse(_ngayKetThuc, out dtEnd))
            {
                ws.Cell(currentRow, 1).Value = $"Từ ngày {dtStart:dd/MM/yyyy} đến ngày {dtEnd:dd/MM/yyyy}";
            }
            else
            {
                ws.Cell(currentRow, 1).Value = $"Từ ngày {_ngayBatDau} đến ngày {_ngayKetThuc}";
            }
            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
            ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow++;

            ws.Range(currentRow, 1, currentRow, 10).Merge();
            ws.Cell(currentRow, 1).Value = _tenHTTT;
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
            ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow += 2;

            string[] headers = new string[]
            {
            "STT", "Mã Y Tế", "Họ và Tên", "Quyển Sổ", "Số Biên Lai",
            "Loại", "Ngày Thu", "Hủy", "Hoàn", "Số Tiền"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(currentRow, i + 1).Value = headers[i];
                ws.Cell(currentRow, i + 1).Style.Font.Bold = true;
                ws.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(currentRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            currentRow++;
            if (_danhSachNhanVien != null && _danhSachNhanVien.Any())
            {
                int stt = 1; 
                foreach (var nv in _danhSachNhanVien)
                {
                    var dataNV = _data.Where(d => d.IDNhanVien == nv.Id).ToList();
                    if (!dataNV.Any()) continue;

                    var tongNV = _tongTheoNhanVien.FirstOrDefault(t => t.IDNhanVien == nv.Id);

                    ws.Range(currentRow, 1, currentRow, 7).Merge();
                    ws.Cell(currentRow, 1).Value = $"Tên nhân viên: {nv.Ten}";
                    ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell(currentRow, 1).Style.Font.Bold = true;
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 10;

                    ws.Cell(currentRow, 8).Value = tongNV?.TongHuy;
                    ws.Cell(currentRow, 9).Value = tongNV?.TongHoan;
                    ws.Cell(currentRow, 10).Value = tongNV?.TongSoTien;
                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0";
                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0";
                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0";

                    ws.Range(currentRow, 1, currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(currentRow, 1, currentRow, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    currentRow++;

                    foreach (var item in dataNV)
                    {
                        ws.Cell(currentRow, 1).Value = stt++;
                        ws.Cell(currentRow, 2).Value = item.MaYTe ?? "";
                        ws.Cell(currentRow, 3).Value = item.HoVaTen ?? "";
                        ws.Cell(currentRow, 4).Value = item.QuyenSo ?? "";
                        ws.Cell(currentRow, 5).Value = item.SoBienLai ?? "";
                        ws.Cell(currentRow, 6).Value = item.Loai ?? "";
                        ws.Cell(currentRow, 7).Value = item.NgayThu?.ToString("dd/MM/yyyy") ?? "";
                        ws.Cell(currentRow, 8).Value = item.Huy ?? (decimal?)null;
                        ws.Cell(currentRow, 9).Value = item.Hoan ?? (decimal?)null;
                        ws.Cell(currentRow, 10).Value = item.SoTien ?? (decimal?)null;

                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0";
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0";
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0";

                        for (int col = 1; col <= headers.Length; col++)
                            ws.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        currentRow++;
                    }
                }
            }
            else
            {
                ws.Range(currentRow, 1, currentRow, 7).Merge();
                ws.Cell(currentRow, 1).Value = $"Tên nhân viên: {_tenNhanVien}";
                ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                ws.Cell(currentRow, 1).Style.Font.Bold = true;
                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;

                ws.Cell(currentRow, 8).Value = _tongHuy;
                ws.Cell(currentRow, 9).Value = _tongHoan;
                ws.Cell(currentRow, 10).Value = _tongSoTien;
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0";

                ws.Range(currentRow, 1, currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(currentRow, 1, currentRow, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                currentRow++;

                if (!_data.Any())
                {
                    ws.Range(currentRow, 1, currentRow, headers.Length).Merge();
                    ws.Cell(currentRow, 1).Value = "Không có dữ liệu";
                    ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(currentRow, 1).Style.Font.Italic = true;
                    currentRow++;
                }
                else
                {
                    int stt = 1;
                    foreach (var item in _data)
                    {
                        ws.Cell(currentRow, 1).Value = stt++;
                        ws.Cell(currentRow, 2).Value = item.MaYTe ?? "";
                        ws.Cell(currentRow, 3).Value = item.HoVaTen ?? "";
                        ws.Cell(currentRow, 4).Value = item.QuyenSo ?? "";
                        ws.Cell(currentRow, 5).Value = item.SoBienLai ?? "";
                        ws.Cell(currentRow, 6).Value = item.Loai ?? "";
                        ws.Cell(currentRow, 7).Value = item.NgayThu?.ToString("dd/MM/yyyy") ?? "";
                        ws.Cell(currentRow, 8).Value = item.Huy ?? (decimal?)null;
                        ws.Cell(currentRow, 9).Value = item.Hoan ?? (decimal?)null;
                        ws.Cell(currentRow, 10).Value = item.SoTien ?? (decimal?)null;

                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0";
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0";
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0";

                        for (int col = 1; col <= headers.Length; col++)
                            ws.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        currentRow++;
                    }
                }
            }

            var totalRange = ws.Range(currentRow, 1, currentRow, 7);
            totalRange.Merge();
            totalRange.Value = "Tổng cộng";
            totalRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            totalRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(currentRow, 8).Value = _tongHuy;
            ws.Cell(currentRow, 9).Value = _tongHoan;
            ws.Cell(currentRow, 10).Value = _tongSoTien;

            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0";
            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0";
            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0";

            for (int col = 8; col <= 10; col++)
            {
                ws.Cell(currentRow, col).Style.Font.Bold = true;
                ws.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            currentRow += 2;

            ws.Range(currentRow, 1, currentRow, 6).Merge(); 
            ws.Cell(currentRow, 1).Value = $"Số tiền phải nộp: {_tongChenhLech:N0}";
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            ws.Range(currentRow, 1, currentRow, 6).Merge(); 
            ws.Cell(currentRow, 1).Value = $"Bằng chữ: {H0304NumberToTextHelper.ConvertSoThanhChu(_tongChenhLech)}";
            ws.Cell(currentRow, 1).Style.Font.Italic = true;
            currentRow += 2;

            ws.Range(currentRow, 7, currentRow + 5, 10).Merge();
            ws.Cell(currentRow, 7).Value =
                $"Ngày {DateTime.Now:dd} Tháng {DateTime.Now:MM} Năm {DateTime.Now:yyyy}\n" +
                "Người lập bảng\n\n\n" +
                "Trần Thanh Thảo";
            ws.Cell(currentRow, 7).Style.Font.Bold = true;
            ws.Cell(currentRow, 7).Style.Alignment.WrapText = true;
            ws.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(currentRow, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Columns().AdjustToContents();


            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }
}
