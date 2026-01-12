using Domain.Entities;
using Domain.Enums;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public static class DbSeeder
    {
        private static readonly Guid HQBranchId = Guid.Parse("7c8d9e0f-1a2b-3c4d-5e6f-7a8b9c0d1e2f");
        
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Data (Roles)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = RoleIds.Admin, RoleName = UserRole.Admin, Description = "System Administrator" },
                new Role { Id = RoleIds.Staff, RoleName = UserRole.Staff, Description = "Staff/Employee" },
                new Role { Id = RoleIds.Customer, RoleName = UserRole.Customer, Description = "Registered Customer" },
                new Role { Id = RoleIds.Guest, RoleName = UserRole.Guest, Description = "Guest User" }
            );

            // Seed Data (Branch) - Để Staff có chỗ làm việc
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = HQBranchId, BranchName = "HappyBox HQ", Address = "Ho Chi Minh City", Phone = "0909000111", Region = "HCM" }
            );

            // Seed Data (Users)
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = Guid.Parse("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"), 
                    Username = "admin", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName = "System Admin", 
                    Email = "admin@happybox.vn", 
                    RoleId = RoleIds.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User 
                { 
                    Id = Guid.Parse("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"), 
                    Username = "staff", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName = "Nguyen Van Staff", 
                    Email = "staff@happybox.vn", 
                    RoleId = RoleIds.Staff,
                    BranchId = HQBranchId,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
