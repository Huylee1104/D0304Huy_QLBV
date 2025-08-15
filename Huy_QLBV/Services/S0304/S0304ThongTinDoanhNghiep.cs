using Huy_QLBV.Models.M0304;
using M0304NhanVien.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace S0304ThongTinDoanhNghiep.Services
{
    public interface IS0304ThongTinDoanhNghiepService
    {
        List<M0304ThongTinDoanhNghiep> S0304DoanhNghiepById(long idCN);
    }

    public class S0304ThongTinDoanhNghiepRepository : IS0304ThongTinDoanhNghiepService
    {
        private readonly string _connectionString;

        public S0304ThongTinDoanhNghiepRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<M0304ThongTinDoanhNghiep> S0304DoanhNghiepById(long idCN)
        {
            var DN = new List<M0304ThongTinDoanhNghiep>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("Huy_TTDN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
    }
}
