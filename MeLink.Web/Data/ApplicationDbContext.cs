using MeLink.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeLink.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Patient>();
            builder.Entity<Pharmacy>();
            builder.Entity<DistributionCompany>();
            builder.Entity<MedicineWarehouse>();
            builder.Entity<Manufacturer>();
            builder.Entity<UserRelation>();
            // فهرس مركّب يمنع تكرار نفس الدواء لنفس المستخدم في المخزون
            builder.Entity<Inventory>()
                   .HasIndex(i => new { i.UserId, i.MedicineId })
                   .IsUnique();

            builder.Entity<Order>()
                   .HasOne(o => o.FromUser)
                   .WithMany()
                   .HasForeignKey(o => o.FromUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                   .HasOne(o => o.ToUser)
                   .WithMany()
                   .HasForeignKey(o => o.ToUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderDetail>()
                   .HasOne(d => d.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);


     
        }
    }
}
