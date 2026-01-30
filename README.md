# HappyBox - Gift Box E-Commerce Platform

A modern e-commerce platform for managing and selling customizable gift boxes with integrated inventory management, order processing, and payment handling.

## 📋 Project Overview

HappyBox is built using **Clean Architecture** principles with clear separation of concerns across three main layers:
- **Domain Layer**: Core business entities and rules
- **Application Layer**: Business logic, DTOs, mappings, and services
- **Infrastructure Layer**: Data persistence, EF Core configurations, migrations, and external integrations

## 🛠️ Technology Stack

- **.NET 8** - Latest long-term support framework
- **Entity Framework Core 8** - ORM for data access
- **SQL Server** - Relational database
- **AutoMapper** - Object-object mapping
- **BCrypt.NET** - Password hashing
- **Google.Apis.Auth** - Google OAuth authentication
- **JWT (JSON Web Tokens)** - Stateless authentication
- **Redis** - Distributed cache for refresh tokens

## 🏗️ Architecture & Project Structure

```
PRN2322/
├── Domain/                          # Core entities & interfaces
│   ├── Entities/                    # Domain models
│   ├── Enums/                       # Enumerations
│   ├── Constants/                   # Constants (RoleIds, etc.)
│   └── IUnitOfWork/                 # UnitOfWork interface
│
├── Application/                     # Business logic & DTOs
│   ├── DTOs/
│   │   ├── Request/                 # Input DTOs
│   │   └── Response/                # Output DTOs
│   ├── IService/                    # Service interfaces
│   ├── Service/                     # Service implementations
│   ├── Mappings/                    # AutoMapper profiles
│   └── Application.csproj
│
├── Infrastructure/                  # Data & external services
│   ├── Data/                        # DbContext
│   ├── Configurations/              # FluentAPI & Seeder
│   ├── Migrations/                  # EF Core migrations
│   ├── Repositories/                # Generic repository pattern
│   ├── Services/                    # External service implementations
│   ├── UnitOfWork/                  # UnitOfWork implementation
│   └── Infrastructure.csproj
│
└── PRN2322/                         # API layer (Controllers)
    ├── Controllers/                 # REST API endpoints
    ├── Properties/                  # App settings
    └── Program.cs                   # Startup configuration
```

## 📊 Database Schema

### Core Entities (14 tables)

**Identity:**
- `Roles` - User roles (Admin, Staff, Customer, Guest)
- `Users` - User accounts with B2B support

**Products & Categories:**
- `Categories` - Product categories with hierarchical support
- `Products` - Product catalog
- `Images` - Product/GiftBox images
- `GiftBoxes` - Customizable gift boxes
- `GiftBoxComponentConfig` - Gift box component templates
- `BoxComponents` - Components in a gift box (N-N relationship)

**Orders & Inventory:**
- `Inventory` - Product stock tracking
- `InventoryTransactions` - Inventory movement logs
- `Orders` - Customer orders
- `OrderDetails` - Line items in orders
- `OrderHistories` - Order status tracking

**Payment & Discounts:**
- `Payments` - Payment records
- `PaymentHistories` - Payment transaction logs
- `Vouchers` - Discount coupons

## 🎯 Key Features

### 1. Authentication & Authorization
- Google OAuth login with JWT tokens
- Facebook login integration
- Email-based registration
- Role-based access control (RBAC)
- Password reset with OTP verification
- Refresh token management via Redis

### 2. Product Management
- Category hierarchy (parent-child relationships)
- Product catalog with SKU tracking
- Image management (multiple images per product)
- Product inventory tracking

### 3. Gift Box Configuration
- Pre-configured gift box templates (GiftBoxComponentConfig)
- Customizable gift box creation
- Box components with quantity management
- N-N relationship between Products and GiftBoxes

### 4. Order Management
- Order creation and tracking
- Order status management (Pending, Processing, Shipped, Delivered, Cancelled)
- Order history with timestamps
- Line items for Products and GiftBoxes

### 5. Inventory System
- Real-time stock tracking
- Inventory transactions (Import, Sale, Return, Damage, Transfer)
- Low stock level alerts
- Branch-independent (single warehouse)

### 6. Payment Processing
- Multiple payment methods (COD, MOMO, VN_PAY)
- Payment status tracking (PENDING, COMPLETED, FAILED, REFUNDED)
- Payment history logs
- Transaction reference tracking

### 7. Discount & Promotions
- Voucher/coupon management
- Percentage and fixed amount discounts
- Minimum order value requirements
- Maximum discount caps
- Usage limits and date-based activation

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/loclhse/PRN232.git
   cd PRN2322
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   - Edit `appsettings.json` in the PRN2322 project
   - Update `ConnectionStrings:DefaultConnection`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=HappyBoxDb;Trusted_Connection=true;"
   }
   ```

4. **Apply migrations**
   ```bash
   dotnet ef database update -p Infrastructure -s PRN2322
   ```

5. **Configure external services** (in `appsettings.json`)
   ```json
   {
     "Google": {
       "ClientId": "your-google-client-id"
     },
     "Facebook": {
       "AppId": "your-facebook-app-id",
       "AppSecret": "your-facebook-app-secret"
     },
     "EmailSettings": {
       "SmtpServer": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email",
       "Password": "your-password"
     }
   }
   ```

6. **Run the application**
   ```bash
   dotnet run --project PRN2322
   ```

   API will be available at: `https://localhost:5001`

## 📚 API Endpoints (Overview)

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login with email/password
- `POST /api/auth/google-login` - Google OAuth login
- `POST /api/auth/facebook-login` - Facebook OAuth login
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with OTP

### Products
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product details
- `POST /api/products` - Create product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Categories
- `GET /api/categories` - List categories
- `GET /api/categories/{id}` - Get category details
- `POST /api/categories` - Create category (Admin)
- `PUT /api/categories/{id}` - Update category (Admin)
- `DELETE /api/categories/{id}` - Delete category (Admin)

### Orders
- `GET /api/orders` - List user's orders
- `GET /api/orders/{id}` - Get order details
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/status` - Update order status

### Inventory
- `GET /api/inventory` - Get inventory levels
- `POST /api/inventory/transactions` - Log inventory transaction

## 🔧 Database Migrations

### Create a new migration
```bash
dotnet ef migrations add <MigrationName> -p Infrastructure -s PRN2322
```

### Apply migrations
```bash
dotnet ef database update -p Infrastructure -s PRN2322
```

### Revert to previous migration
```bash
dotnet ef database update <PreviousMigrationName> -p Infrastructure -s PRN2322
```

### Drop database
```bash
dotnet ef database drop --force -p Infrastructure -s PRN2322
```

## 🔐 Security Features

- JWT-based stateless authentication
- Password hashing with BCrypt
- OTP-based password reset
- Role-based authorization
- HTTPS enforcement
- CORS configuration
- OAuth 2.0 integration (Google, Facebook)
- Refresh token rotation with Redis

## 📝 Configuration

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=HappyBoxDb;..."
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "HappyBox",
    "Audience": "HappyBoxUsers",
    "ExpirationMinutes": 30
  },
  "Google": {
    "ClientId": "..."
  },
  "Facebook": {
    "AppId": "..."
  },
  "EmailSettings": {
    "SmtpServer": "...",
    "Port": 587,
    "Username": "...",
    "Password": "..."
  }
}
```

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

## 📖 Development Guidelines

### Code Style
- Follow C# naming conventions (PascalCase for public members)
- Use async/await for I/O operations
- Keep methods focused and small
- Use meaningful variable names

### Git Workflow
1. Create feature branch: `git checkout -b feature/feature-name`
2. Commit changes: `git commit -am 'Add feature'`
3. Push to branch: `git push origin feature/feature-name`
4. Create Pull Request

### Database Changes
- Always create migrations for schema changes
- Use meaningful migration names
- Include seeder updates if necessary

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

**Loc** - Initial development

## 📞 Support

For support, open an issue on GitHub or contact the development team.

---

**Last Updated:** January 29, 2026
