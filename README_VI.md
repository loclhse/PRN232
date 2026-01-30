# HappyBox - Nền tảng E-Commerce Hộp Quà Tặng

Một nền tảng thương mại điện tử hiện đại để quản lý và bán hộp quà tặng có thể tùy chỉnh với tích hợp quản lý hàng tồn kho, xử lý đơn hàng và thanh toán.

## 📋 Tổng quan Dự án

HappyBox được xây dựng theo nguyên tắc **Clean Architecture** với sự tách biệt rõ ràng giữa ba lớp chính:
- **Domain Layer**: Các thực thể cốt lõi và quy tắc kinh doanh
- **Application Layer**: Logic kinh doanh, DTOs, mappings, và services
- **Infrastructure Layer**: Lưu trữ dữ liệu, cấu hình EF Core, migrations, và tích hợp bên ngoài

## 🛠️ Công nghệ Sử dụng

- **.NET 8** - Framework hỗ trợ dài hạn mới nhất
- **Entity Framework Core 8** - ORM để truy cập dữ liệu
- **SQL Server** - Cơ sở dữ liệu quan hệ
- **AutoMapper** - Mapping đối tượng-đối tượng
- **BCrypt.NET** - Mã hóa mật khẩu
- **Google.Apis.Auth** - Xác thực Google OAuth
- **JWT (JSON Web Tokens)** - Xác thực không trạng thái
- **Redis** - Cache phân tán cho refresh tokens

## 🏗️ Kiến trúc & Cấu trúc Dự án

```
PRN2322/
├── Domain/                          # Thực thể cốt lõi & interfaces
│   ├── Entities/                    # Các model miền
│   ├── Enums/                       # Enumerations
│   ├── Constants/                   # Hằng số (RoleIds, v.v.)
│   └── IUnitOfWork/                 # Interface UnitOfWork
│
├── Application/                     # Logic kinh doanh & DTOs
│   ├── DTOs/
│   │   ├── Request/                 # DTOs đầu vào
│   │   └── Response/                # DTOs đầu ra
│   ├── IService/                    # Interfaces dịch vụ
│   ├── Service/                     # Triển khai dịch vụ
│   ├── Mappings/                    # AutoMapper profiles
│   └── Application.csproj
│
├── Infrastructure/                  # Dữ liệu & dịch vụ bên ngoài
│   ├── Data/                        # DbContext
│   ├── Configurations/              # FluentAPI & Seeder
│   ├── Migrations/                  # EF Core migrations
│   ├── Repositories/                # Mẫu repository chung
│   ├── Services/                    # Triển khai dịch vụ bên ngoài
│   ├── UnitOfWork/                  # Triển khai UnitOfWork
│   └── Infrastructure.csproj
│
└── PRN2322/                         # Lớp API (Controllers)
    ├── Controllers/                 # Các endpoint REST API
    ├── Properties/                  # Cấu hình ứng dụng
    └── Program.cs                   # Cấu hình khởi động
```

## 📊 Sơ đồ Cơ sở Dữ liệu

### Các Thực thể Cốt lõi (14 bảng)

**Danh tính:**
- `Roles` - Vai trò người dùng (Admin, Staff, Customer, Guest)
- `Users` - Tài khoản người dùng với hỗ trợ B2B

**Sản phẩm & Danh mục:**
- `Categories` - Danh mục sản phẩm với hỗ trợ phân cấp
- `Products` - Danh mục sản phẩm
- `Images` - Hình ảnh sản phẩm/Hộp quà
- `GiftBoxes` - Hộp quà có thể tùy chỉnh
- `GiftBoxComponentConfig` - Mẫu thành phần hộp quà
- `BoxComponents` - Thành phần trong hộp quà (mối quan hệ N-N)

**Đơn hàng & Hàng tồn kho:**
- `Inventory` - Theo dõi hàng tồn kho sản phẩm
- `InventoryTransactions` - Ghi nhận chuyển động hàng tồn kho
- `Orders` - Đơn hàng của khách hàng
- `OrderDetails` - Các mục dòng trong đơn hàng
- `OrderHistories` - Theo dõi trạng thái đơn hàng

**Thanh toán & Giảm giá:**
- `Payments` - Bản ghi thanh toán
- `PaymentHistories` - Ghi nhận giao dịch thanh toán
- `Vouchers` - Phiếu giảm giá

## 🎯 Các Tính năng Chính

### 1. Xác thực & Phân quyền
- Đăng nhập Google OAuth với JWT tokens
- Tích hợp đăng nhập Facebook
- Đăng ký dựa trên email
- Kiểm soát truy cập dựa trên vai trò (RBAC)
- Đặt lại mật khẩu bằng xác thực OTP
- Quản lý refresh token qua Redis

### 2. Quản lý Sản phẩm
- Danh mục phân cấp (mối quan hệ cha-con)
- Danh mục sản phẩm với theo dõi SKU
- Quản lý hình ảnh (nhiều hình ảnh trên mỗi sản phẩm)
- Theo dõi hàng tồn kho sản phẩm

### 3. Cấu hình Hộp Quà
- Mẫu hộp quà được cấu hình trước (GiftBoxComponentConfig)
- Tạo hộp quà có thể tùy chỉnh
- Thành phần hộp với quản lý số lượng
- Mối quan hệ N-N giữa Sản phẩm và Hộp quà

### 4. Quản lý Đơn hàng
- Tạo và theo dõi đơn hàng
- Quản lý trạng thái đơn hàng (Đang chờ, Đang xử lý, Đã gửi, Đã giao, Đã hủy)
- Lịch sử đơn hàng với dấu thời gian
- Mục dòng cho Sản phẩm và Hộp quà

### 5. Hệ thống Quản lý Hàng tồn kho
- Theo dõi hàng tồn kho thời gian thực
- Giao dịch hàng tồn kho (Nhập, Bán, Trả lại, Hư hỏng, Chuyển)
- Cảnh báo mức hàng tồn kho thấp
- Độc lập chi nhánh (một kho hàng)

### 6. Xử lý Thanh toán
- Nhiều phương thức thanh toán (COD, MOMO, VN_PAY)
- Theo dõi trạng thái thanh toán (ĐẢ CHO PHÉP, HOÀN THÀNH, THẤT BẠI, HOÀN TIỀN)
- Ghi nhận lịch sử thanh toán
- Theo dõi tham chiếu giao dịch

### 7. Giảm giá & Khuyến mại
- Quản lý voucher/phiếu giảm giá
- Giảm giá theo phần trăm và số tiền cố định
- Yêu cầu giá trị đơn hàng tối thiểu
- Giới hạn giảm giá tối đa
- Giới hạn sử dụng và kích hoạt dựa trên ngày

## 🚀 Bắt đầu

### Yêu cầu
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 hoặc VS Code
- Git

### Cài đặt

1. **Clone kho lưu trữ**
   ```bash
   git clone https://github.com/loclhse/PRN232.git
   cd PRN2322
   ```

2. **Khôi phục phụ thuộc**
   ```bash
   dotnet restore
   ```

3. **Cấu hình kết nối cơ sở dữ liệu**
   - Chỉnh sửa `appsettings.json` trong dự án PRN2322
   - Cập nhật `ConnectionStrings:DefaultConnection`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=HappyBoxDb;Trusted_Connection=true;"
   }
   ```

4. **Áp dụng migrations**
   ```bash
   dotnet ef database update -p Infrastructure -s PRN2322
   ```

5. **Cấu hình dịch vụ bên ngoài** (trong `appsettings.json`)
   ```json
   {
     "Google": {
       "ClientId": "google-client-id-của-bạn"
     },
     "Facebook": {
       "AppId": "facebook-app-id-của-bạn",
       "AppSecret": "facebook-app-secret-của-bạn"
     },
     "EmailSettings": {
       "SmtpServer": "smtp.gmail.com",
       "Port": 587,
       "Username": "email-của-bạn",
       "Password": "mật-khẩu-của-bạn"
     }
   }
   ```

6. **Chạy ứng dụng**
   ```bash
   dotnet run --project PRN2322
   ```

   API sẽ có sẵn tại: `https://localhost:5001`

## 📚 Các Endpoint API (Tổng quan)

### Xác thực
- `POST /api/auth/register` - Đăng ký người dùng mới
- `POST /api/auth/login` - Đăng nhập bằng email/mật khẩu
- `POST /api/auth/google-login` - Đăng nhập Google OAuth
- `POST /api/auth/facebook-login` - Đăng nhập Facebook OAuth
- `POST /api/auth/refresh-token` - Làm mới JWT token
- `POST /api/auth/forgot-password` - Yêu cầu đặt lại mật khẩu
- `POST /api/auth/reset-password` - Đặt lại mật khẩu bằng OTP

### Sản phẩm
- `GET /api/products` - Liệt kê tất cả sản phẩm
- `GET /api/products/{id}` - Lấy chi tiết sản phẩm
- `POST /api/products` - Tạo sản phẩm (Admin)
- `PUT /api/products/{id}` - Cập nhật sản phẩm (Admin)
- `DELETE /api/products/{id}` - Xóa sản phẩm (Admin)

### Danh mục
- `GET /api/categories` - Liệt kê danh mục
- `GET /api/categories/{id}` - Lấy chi tiết danh mục
- `POST /api/categories` - Tạo danh mục (Admin)
- `PUT /api/categories/{id}` - Cập nhật danh mục (Admin)
- `DELETE /api/categories/{id}` - Xóa danh mục (Admin)

### Đơn hàng
- `GET /api/orders` - Liệt kê đơn hàng của người dùng
- `GET /api/orders/{id}` - Lấy chi tiết đơn hàng
- `POST /api/orders` - Tạo đơn hàng mới
- `PUT /api/orders/{id}/status` - Cập nhật trạng thái đơn hàng

### Hàng tồn kho
- `GET /api/inventory` - Lấy mức hàng tồn kho
- `POST /api/inventory/transactions` - Ghi nhận giao dịch hàng tồn kho

## 🔧 Migrations Cơ sở Dữ liệu

### Tạo migration mới
```bash
dotnet ef migrations add <TênMigration> -p Infrastructure -s PRN2322
```

### Áp dụng migrations
```bash
dotnet ef database update -p Infrastructure -s PRN2322
```

### Quay lại migration trước đó
```bash
dotnet ef database update <TênMigrationTrước> -p Infrastructure -s PRN2322
```

### Xóa cơ sở dữ liệu
```bash
dotnet ef database drop --force -p Infrastructure -s PRN2322
```

## 🔐 Các Tính năng Bảo mật

- Xác thực dựa trên JWT không có trạng thái
- Mã hóa mật khẩu bằng BCrypt
- Đặt lại mật khẩu dựa trên OTP
- Phân quyền dựa trên vai trò
- Thực thi HTTPS
- Cấu hình CORS
- Tích hợp OAuth 2.0 (Google, Facebook)
- Xoay vòng refresh token với Redis

## 📝 Cấu hình

### Cấu trúc appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=HappyBoxDb;..."
  },
  "Jwt": {
    "SecretKey": "khóa-bí-mật-của-bạn-tối-thiểu-32-ký-tự",
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

## 🧪 Kiểm tra

Chạy unit tests:
```bash
dotnet test
```

## 📖 Hướng dẫn Phát triển

### Kiểu Code
- Tuân theo các quy ước đặt tên C# (PascalCase cho các thành viên công khai)
- Sử dụng async/await cho các hoạt động I/O
- Giữ các phương pháp tập trung và nhỏ gọn
- Sử dụng tên biến có ý nghĩa

### Quy trình Git
1. Tạo nhánh tính năng: `git checkout -b feature/tên-tính-năng`
2. Commit thay đổi: `git commit -am 'Thêm tính năng'`
3. Đẩy đến nhánh: `git push origin feature/tên-tính-năng`
4. Tạo Pull Request

### Thay đổi Cơ sở Dữ liệu
- Luôn tạo migrations cho các thay đổi lược đồ
- Sử dụng tên migration có ý nghĩa
- Cập nhật seeder nếu cần thiết

## 🤝 Đóng góp

1. Fork kho lưu trữ
2. Tạo nhánh tính năng của bạn
3. Commit thay đổi của bạn
4. Đẩy đến nhánh
5. Tạo Pull Request

## 📄 Giấy phép

Dự án này được cấp phép theo Giấy phép MIT.

## 👨‍💻 Tác giả

**Loc** - Phát triển ban đầu

## 📞 Hỗ trợ

Để được hỗ trợ, vui lòng mở một issue trên GitHub hoặc liên hệ với nhóm phát triển.

---

**Cập nhật lần cuối:** 29 tháng 1 năm 2026
