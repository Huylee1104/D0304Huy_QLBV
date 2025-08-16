using DocumentFormat.OpenXml.InkML;
using M0304.Models.ThongTinDOanhNghiep;
using M0304.Models.BangKeThu;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Huy_QLBV.Models.M0304
{
    public class M0304Context : DbContext
    {
        public M0304Context(DbContextOptions<M0304Context> options)
            : base(options)
        {
        }

        public DbSet<M0304BangKeThu> M0304Huy_Mau4 { get; set; }
        public DbSet<M0304ThongTinDoanhNghiep> M0304ThongTinDoanhNghiep { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<M0304BangKeThu>().ToTable("Huy_Mau4");
            modelBuilder.Entity<M0304ThongTinDoanhNghiep>().ToTable("ThongTinDoanhNghiep");
        }
    }
}
