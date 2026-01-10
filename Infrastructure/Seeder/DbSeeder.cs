using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public static class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Data (Roles)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin", Description = "System Administrator" },
                new Role { Id = 2, RoleName = "Staff", Description = "Staff/Employee" },
                new Role { Id = 3, RoleName = "Customer", Description = "Registered Customer" },
                new Role { Id = 4, RoleName = "Guest", Description = "Guest User" }
            );

            // Seed Data (Branch) - Để Staff có chỗ làm việc
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = 1, BranchName = "HappyBox HQ", Address = "Ho Chi Minh City", Phone = "0909000111", Region = "HCM" }
            );

            // Seed Data (Users)
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName = "System Admin", 
                    Email = "admin@happybox.vn", 
                    RoleId = 1, // Admin
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User 
                { 
                    Id = 2, 
                    Username = "staff", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName = "Nguyen Van Staff", 
                    Email = "staff@happybox.vn", 
                    RoleId = 2, // Staff
                    BranchId = 1, // HappyBox HQ
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
