using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public static class FluentApiConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // =========================================================
            // CORE PRODUCTS
            // =========================================================

            // BoxComponent Composite Key (Many-to-Many between GiftBox and Product)
            modelBuilder.Entity<BoxComponent>()
                .HasKey(bc => new { bc.GiftBoxId, bc.ProductId });

            modelBuilder.Entity<BoxComponent>()
                .HasOne(bc => bc.GiftBox)
                .WithMany(gb => gb.BoxComponents)
                .HasForeignKey(bc => bc.GiftBoxId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BoxComponent>()
                .HasOne(bc => bc.Product)
                .WithMany(p => p.BoxComponents)
                .HasForeignKey(bc => bc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category Self-Referencing
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // INVENTORY
            // =========================================================

            // Inventory should be unique per Branch + Product
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => new { i.BranchId, i.ProductId })
                .IsUnique();

            // =========================================================
            // SALES
            // =========================================================

            // Order Relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precision for Decimal types
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FinalAmount)
                .HasPrecision(18, 2);

            // =========================================================
            // FINANCE
            // =========================================================
            
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                 .Property(v => v.Value)
                 .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                 .Property(v => v.MinOrderValue)
                 .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                 .Property(v => v.MaxDiscountAmount)
                 .HasPrecision(18, 2);
        }
    }
}
