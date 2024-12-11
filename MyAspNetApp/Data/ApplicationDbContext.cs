using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Models;

namespace MyAspNetApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Xe> Xe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đảm bảo tên bảng trong cơ sở dữ liệu Oracle khớp với tên bảng trong DbContext
            modelBuilder.Entity<Xe>().ToTable("XE");
        }
    }
}
