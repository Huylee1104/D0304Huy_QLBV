using Huy.Helpers;
using M0304NhanVien.Models;
using M0304.Models.BangKeThu;
using M0304.Models.ThongTinDOanhNghiep;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace P0304.PDFDocument
{
    public class P0304ReportTemplatePDF : IDocument
    {
        private readonly List<M0304BangKeThu> _data;
        private readonly List<M0304ThongTinDoanhNghiep> _dataDN;
        private string _ngayBatDau;
        private string _ngayKetThuc;
        private readonly string _logoPath;
        private string _tenNhanVien;
        private string _tenHTTT;
        private decimal _tongHuy;
        private decimal _tongHoan;
        private decimal _tongSoTien;
        private decimal _tongChenhLech;
        private List<M0304NhanVienModel> _danhSachNhanVien;
        private List<M0304TongTheoNhanVien> _tongTheoNhanVien;


        public P0304ReportTemplatePDF(List<M0304BangKeThu> data, List<M0304ThongTinDoanhNghiep> dataDN, string ngayBatDau, string ngayKetThuc, 
            string tenNhanVien, string tenHTTT, string logoPath, decimal tongHuy, decimal tongHoan, decimal tongSoTien, 
            decimal tongChenhLech, List <M0304NhanVienModel> danhSachNhanVien, List<M0304TongTheoNhanVien> tongTheoNhanVien)
        {
            _data = data ?? new List<M0304BangKeThu>();
            _dataDN = dataDN ?? new List<M0304ThongTinDoanhNghiep>();
            _ngayBatDau = ngayBatDau;
            _ngayKetThuc = ngayKetThuc;
            _logoPath = logoPath;
            _tenNhanVien = tenNhanVien;
            _tenHTTT = tenHTTT;
            _tongHuy = tongHuy;
            _tongHoan = tongHoan;
            _tongSoTien = tongSoTien;
            _tongChenhLech = tongChenhLech;
            _danhSachNhanVien = danhSachNhanVien ?? new List<M0304NhanVienModel>();
            _tongTheoNhanVien = tongTheoNhanVien ?? new List<M0304TongTheoNhanVien>();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(15);
                page.DefaultTextStyle(x => x.FontSize(10)); 

                page.Header().ShowOnce().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(3).Row(left =>
                        {
                            left.ConstantItem(40).AlignMiddle().Image(_logoPath);

                            left.RelativeItem().Column(info =>
                            {
                                foreach (var dn in _dataDN)
                                {
                                    info.Item().Text(dn.TenCSKCB ?? "").FontSize(9);
                                    info.Item().Text(dn.TenCoQuanChuyenMon ?? "").FontSize(9);
                                    info.Item().Text(dn.DiaChi ?? "").FontSize(9);
                                    info.Item().Text(dn.DienThoai ?? "").FontSize(9);
                                }
                            });
                        });
                    });

                    col.Item().AlignCenter().Column(center =>
                    {
                        center.Item()
                            .Text("BẢNG KÊ THU TIỀN NGOẠI TRÚ THEO BL/HĐ")
                            .Bold()
                            .FontSize(12);

                        center.Item()
                            .Width(250)   
                            .AlignCenter()
                            .Text($"Từ ngày {_ngayBatDau:dd/MM/yyyy} đến ngày {_ngayKetThuc:dd/MM/yyyy}")
                            .FontSize(9);

                        center.Item()
                            .Width(250)   
                            .AlignCenter()
                            .Text($"{_tenHTTT}")
                            .Bold()
                            .FontSize(9);
                    });
                });

                page.Content().PaddingVertical(6).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);   
                            columns.ConstantColumn(70);   
                            columns.RelativeColumn(2);    
                            columns.ConstantColumn(60);   
                            columns.ConstantColumn(60);   
                            columns.ConstantColumn(40);   
                            columns.ConstantColumn(60);   
                            columns.ConstantColumn(40);   
                            columns.ConstantColumn(40);   
                            columns.ConstantColumn(50);   
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("STT");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Mã y tế");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Họ và tên");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Quyển sổ");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Số biên lai");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Loại");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Ngày thu");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Hủy");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Hoàn");
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Số tiền");
                        });

                        int stt = 1;

                        if (_danhSachNhanVien != null && _danhSachNhanVien.Any())
                        {
                            foreach (var nv in _danhSachNhanVien)
                            {
                                var tongNV = _tongTheoNhanVien.FirstOrDefault(t => t.IDNhanVien == nv.Id);

                                table.Cell().ColumnSpan(7)
                                    .Element(CellStyle)
                                    .AlignLeft()
                                    .Text($"Tên nhân viên: {nv.Ten}").Bold();

                                table.Cell().Element(CellStyle).AlignRight()
                                    .Text(tongNV?.TongHuy.ToString("N0") ?? "0");
                                table.Cell().Element(CellStyle).AlignRight()
                                    .Text(tongNV?.TongHoan.ToString("N0") ?? "0");
                                table.Cell().Element(CellStyle).AlignRight()
                                    .Text(tongNV?.TongSoTien.ToString("N0") ?? "0");

                                var dataNV = _data.Where(d => d.IDNhanVien == nv.Id).ToList(); 
                                foreach (var item in dataNV)
                                {
                                    table.Cell().Element(CellStyle).AlignCenter().Text(stt++.ToString());
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.MaYTe ?? "");
                                    table.Cell().Element(CellStyle).AlignLeft().Text(item.HoVaTen ?? "");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.QuyenSo ?? "");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.SoBienLai ?? "");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Loai ?? "");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(item.NgayThu?.ToString("dd/MM/yyyy") ?? "");
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Huy?.ToString("N0") ?? "");
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Hoan?.ToString("N0") ?? "");
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.SoTien?.ToString("N0") ?? "");
                                }
                            }
                        }
                        else
                        {
                            table.Cell().ColumnSpan(7)
                                .Element(CellStyle)
                                .AlignLeft()
                                .Text($"Tên nhân viên: {_tenNhanVien}").Bold();
                            table.Cell().Element(CellStyle).AlignRight().Text(_tongHuy.ToString("N0"));
                            table.Cell().Element(CellStyle).AlignRight().Text(_tongHoan.ToString("N0"));
                            table.Cell().Element(CellStyle).AlignRight().Text(_tongSoTien.ToString("N0"));

                            foreach (var item in _data)
                            {
                                table.Cell().Element(CellStyle).AlignCenter().Text(stt++.ToString());
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.MaYTe ?? "");
                                table.Cell().Element(CellStyle).AlignLeft().Text(item.HoVaTen ?? "");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.QuyenSo ?? "");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.SoBienLai ?? "");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.Loai ?? "");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.NgayThu?.ToString("dd/MM/yyyy") ?? "");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Huy?.ToString("N0") ?? "");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Hoan?.ToString("N0") ?? "");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.SoTien?.ToString("N0") ?? "");
                            }
                        }

                        table.Cell().ColumnSpan(7)
                        .Element(CellStyle)
                        .AlignCenter()
                        .Text("Tổng cộng").Bold();

                        table.Cell().Element(CellStyle).AlignRight().Text(_tongHuy.ToString("N0"));
                        table.Cell().Element(CellStyle).AlignRight().Text(_tongHoan.ToString("N0"));
                        table.Cell().Element(CellStyle).AlignRight().Text(_tongSoTien.ToString("N0"));

                        col.Item().Height(10);

                        col.Item().EnsureSpace() 
                            .Column(cuoi =>
                            {
                                cuoi.Spacing(5);

                                cuoi.Item().Text($"Số tiền phải nộp: {_tongChenhLech:N0}")
                                    .Bold();

                                cuoi.Item().Text($"Bằng chữ: {H0304NumberToTextHelper.ConvertSoThanhChu(_tongChenhLech)}")
                                    .Italic();

                                cuoi.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(""); 
                                    row.ConstantItem(200).Column(right =>
                                    {
                                        right.Item().Text($"Ngày {DateTime.Now:dd} Tháng {DateTime.Now:MM} Năm {DateTime.Now:yyyy}");
                                        right.Item().Text("Người lập bảng").Bold();
                                        right.Item().Height(40); 
                                        right.Item().Text("Trần Thị Hồng Châu");
                                    });
                                });
                            });

                    });
                });

                page.Footer()
                    .AlignRight()
                    .Text(txt =>
                    {
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
            });
        }

        static IContainer CellStyleHeader(IContainer container) =>
            container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(4)
                .AlignMiddle()
                .DefaultTextStyle(x => x.SemiBold().FontSize(10));

        static IContainer CellStyle(IContainer container) =>
            container
                .Border(1)
                .Padding(4)
                .AlignMiddle()
                .DefaultTextStyle(x => x.FontSize(9));
    }
}
