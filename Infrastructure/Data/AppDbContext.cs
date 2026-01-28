using Domain.Entities;
using Infrastructure.Configurations;
using Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #region DbSets

        // Identity
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        // Core Products
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }

        // Inventory
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        // Sales
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }

        // Finance
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }

        // Marketing
        public DbSet<Voucher> Vouchers { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Fluent API Configurations
            FluentApiConfiguration.Configure(modelBuilder);

            // Seeding Data
            DbSeeder.Seed(modelBuilder);
        }
    }
}
