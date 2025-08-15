using Huy_QLBV.Models.M0304;
using Microsoft.Data.SqlClient;
using System.Data;

namespace S0304BangKeThu.Services
{
    public interface IS0304BangKeThuService
    {
        List<M0304Huy_Mau4> S0304BangKeThu(string ngayBatDau, string ngayKetThuc, long idCN, long? idHTTT = null, long? idNhanVien = null);
    }
    public class S0304ReportRepository : IS0304BangKeThuService
    {
        private readonly string _connectionString;

        public S0304ReportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<M0304Huy_Mau4> S0304BangKeThu(string ngayBatDau, string ngayKetThuc, long idCN, long? idHTTT = null, long? idNhanVien = null)
        {
            var result = new List<M0304Huy_Mau4>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Huy_BKTTNT", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TuNgay", ngayBatDau);
            cmd.Parameters.AddWithValue("@DenNgay", ngayKetThuc);
            cmd.Parameters.AddWithValue("@IDCN", idCN);
            cmd.Parameters.AddWithValue("@IDHTTT", (object?)idHTTT ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IDNhanVien", (object?)idNhanVien ?? DBNull.Value);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new M0304Huy_Mau4
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
                });
            }

            return result;
        }
    }

}