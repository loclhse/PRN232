using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public static class FluentApiConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // =========================================================
            // IDENTITY
            // =========================================================
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .HasConversion<string>()
                .HasMaxLength(50);

            // =========================================================
            // IDENTITY
            // =========================================================

            // User - Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // GIFT BOX COMPONENT CONFIG
            // =========================================================

            modelBuilder.Entity<GiftBoxComponentConfig>()
                .Property(gc => gc.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GiftBoxComponentConfig>()
                .HasIndex(gc => gc.Name)
                .IsUnique();

            // =========================================================
            // CORE PRODUCTS
            // =========================================================

            // Category - Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category Self-Referencing
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category - GiftBox
            modelBuilder.Entity<GiftBox>()
                .HasOne(gb => gb.Category)
                .WithMany(c => c.GiftBoxes)
                .HasForeignKey(gb => gb.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // GiftBox - ComponentConfig (1-1)
            modelBuilder.Entity<GiftBox>()
                .HasOne(gb => gb.ComponentConfig)
                .WithOne()
                .HasForeignKey<GiftBox>(gb => gb.GiftBoxComponentConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            // Image - Product (nullable)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Image - GiftBox (nullable)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.GiftBox)
                .WithMany(gb => gb.Images)
                .HasForeignKey(i => i.GiftBoxId)
                .OnDelete(DeleteBehavior.Restrict);

            // BoxComponent - GiftBox
            modelBuilder.Entity<BoxComponent>()
                .HasOne(bc => bc.GiftBox)
                .WithMany(gb => gb.BoxComponents)
                .HasForeignKey(bc => bc.GiftBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            // BoxComponent - Product
            modelBuilder.Entity<BoxComponent>()
                .HasOne(bc => bc.Product)
                .WithMany(p => p.BoxComponents)
                .HasForeignKey(bc => bc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // INVENTORY
            // =========================================================

            // Inventory - Product
            modelBuilder.Entity<Inventory>()
                .HasOne(inv => inv.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(inv => inv.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // InventoryTransaction - Inventory
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Inventory)
                .WithMany(inv => inv.Transactions)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // SALES
            // =========================================================

            // Order Relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precision for Decimal types
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // OrderHistory - Order
            modelBuilder.Entity<OrderHistory>()
                .HasOne(oh => oh.Order)
                .WithMany(o => o.OrderHistories)
                .HasForeignKey(oh => oh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment - Order
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentHistory - Payment
            modelBuilder.Entity<PaymentHistory>()
                .HasOne(ph => ph.Payment)
                .WithMany(p => p.PaymentHistories)
                .HasForeignKey(ph => ph.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Order - Voucher (optional)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Voucher)
                .WithMany(v => v.Orders)
                .HasForeignKey(o => o.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

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
