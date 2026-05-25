# User Order Management System

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-Web_API-0A66C2?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core Web API" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-8.0-6DB33F?style=for-the-badge" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/SQL_Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
</p>

## Giới thiệu

**User Order Management System** là một mini project tự học về backend với **.NET 8**, **ASP.NET Core Web API**, **Entity Framework Core** và **SQL Server**.

Dự án tập trung vào các kiến thức nền tảng nhưng rất thực tế trong phát triển backend: thiết kế database quan hệ, mapping EF Core, CRUD API, phân trang, tìm kiếm, soft delete, seed data, viết SQL query báo cáo và tối ưu hiệu năng query.

Project này phù hợp để luyện tập các kỹ năng backend quan trọng trước khi bước sang các kiến trúc lớn hơn như Clean Architecture, CQRS, DDD hoặc ABP Framework.

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Tính năng chính](#tính-năng-chính)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Thiết kế database](#thiết-kế-database)
- [Cài đặt và chạy project](#cài-đặt-và-chạy-project)
- [API endpoints](#api-endpoints)
- [Ví dụ request](#ví-dụ-request)
- [SQL query mẫu](#sql-query-mẫu)
- [Ghi chú về EF Core](#ghi-chú-về-ef-core)
- [Tối ưu hiệu năng](#tối-ưu-hiệu-năng)
- [Mục tiêu học tập](#mục-tiêu-học-tập)
- [License](#license)

## Tính năng chính

- Quản lý người dùng với API danh sách, tìm kiếm, phân trang, tạo mới và soft delete
- Quản lý vai trò người dùng thông qua bảng trung gian `UserRoles`
- Quản lý dữ liệu đơn hàng theo từng người dùng
- Báo cáo doanh thu theo ngày
- Báo cáo top khách hàng có tổng chi tiêu cao nhất
- Áp dụng các trường audit theo phong cách ABP:
  - `CreationTime`
  - `CreatorUserId`
  - `LastModificationTime`
  - `IsDeleted`
- Ràng buộc database:
  - Email duy nhất
  - Foreign key
  - Check constraint `Age > 0`
- Index database:
  - `Users.Email`
  - `Orders.UserId`
  - `Orders.CreatedAt`
- Soft delete bằng EF Core global query filter
- Seed data khi database chưa có dữ liệu
- Swagger UI để test API
- File `query.sql` chứa các câu SQL phục vụ học tập và kiểm tra dữ liệu

## Công nghệ sử dụng

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- Swagger / Swashbuckle
- JetBrains Rider
- DataGrip

## Cấu trúc thư mục

```text
UserOrderManagerment/
├── Controllers/
│   ├── ReportsController.cs
│   └── UsersController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── DTOs/
│   ├── PagedResult.cs
│   ├── RevenueReportDto.cs
│   ├── TopCustomerDto.cs
│   └── UserDto.cs
├── Entities/
│   ├── AuditedEntity.cs
│   ├── Order.cs
│   ├── Role.cs
│   ├── User.cs
│   └── UserRole.cs
├── Migrations/
├── Program.cs
├── appsettings.json
└── query.sql
```

## Thiết kế database

### Users

Bảng `Users` lưu thông tin người dùng.

Các cột chính:

- `Id`
- `FullName`
- `Email`
- `Age`
- `CreationTime`
- `CreatorUserId`
- `LastModificationTime`
- `IsDeleted`

Ràng buộc:

- `Email` là duy nhất
- `Age` phải lớn hơn `0`
- Dữ liệu soft delete được lọc mặc định bằng global query filter

### Roles

Bảng `Roles` lưu danh sách vai trò, ví dụ:

- `Admin`
- `Customer`

### UserRoles

Bảng `UserRoles` là bảng trung gian thể hiện quan hệ nhiều-nhiều giữa `Users` và `Roles`.

Khóa chính gồm:

- `UserId`
- `RoleId`

### Orders

Bảng `Orders` lưu thông tin đơn hàng của người dùng.

Các cột chính:

- `Id`
- `UserId`
- `TotalAmount`
- `CreatedAt`
- `Status`
- `CreationTime`
- `CreatorUserId`
- `LastModificationTime`
- `IsDeleted`

Index:

- `UserId`
- `CreatedAt`

## Cài đặt và chạy project

### Yêu cầu môi trường

Cài đặt các công cụ sau:

- .NET 8 SDK
- SQL Server
- JetBrains Rider
- DataGrip

### Clone project

```bash
git clone <repository-url>
cd UserOrderManagerment
```

### Cấu hình connection string

Mở file `UserOrderManagerment/appsettings.json` và chỉnh connection string:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=UserOrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Nếu dùng SQL Server Authentication:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=UserOrderManagementDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Restore package

```bash
dotnet restore
```

### Tạo database bằng migration

```bash
cd UserOrderManagerment
dotnet ef database update
```

### Chạy ứng dụng

```bash
dotnet run
```

Swagger UI:

```text
https://localhost:7221/swagger
http://localhost:5220/swagger
```

Khi chạy ứng dụng, project sẽ tự seed dữ liệu mẫu nếu database đang trống.

## API endpoints

### Users

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| GET | `/api/users` | Lấy danh sách người dùng đang hoạt động |
| GET | `/api/users/search?keyword=a&page=1&pageSize=10` | Tìm kiếm người dùng có phân trang |
| POST | `/api/users` | Tạo người dùng mới |
| DELETE | `/api/users/{id}` | Soft delete người dùng |

### Reports

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| GET | `/api/reports/revenue` | Báo cáo doanh thu theo ngày |
| GET | `/api/reports/top-customers` | Top 10 khách hàng theo tổng giá trị đơn hàng hoàn thành |

## Ví dụ request

Tạo người dùng mới:

```json
{
  "fullName": "Le Van C",
  "email": "c@example.com",
  "age": 28
}
```

Tìm kiếm người dùng:

```text
GET /api/users/search?keyword=nguyen&page=1&pageSize=10
```

## SQL query mẫu

Project có file `UserOrderManagerment/query.sql` chứa các câu SQL để luyện tập và kiểm tra dữ liệu.

### Join User và Role

```sql
SELECT u.Id,
       u.FullName,
       u.Email,
       r.Name AS RoleName
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id
WHERE u.IsDeleted = 0;
```

### Join User và Orders

```sql
SELECT u.Id,
       u.FullName,
       o.Id AS OrderId,
       o.TotalAmount,
       o.CreatedAt
FROM Users u
JOIN Orders o ON u.Id = o.UserId
WHERE u.IsDeleted = 0
  AND o.IsDeleted = 0;
```

### Báo cáo doanh thu

```sql
SELECT CAST(o.CreatedAt AS date) AS OrderDate,
       SUM(o.TotalAmount)        AS Revenue,
       COUNT(*)                  AS OrderCount
FROM Orders o
WHERE o.IsDeleted = 0
  AND o.Status = 'Completed'
GROUP BY CAST(o.CreatedAt AS date)
ORDER BY OrderDate DESC;
```

### Top khách hàng

```sql
SELECT TOP 10 u.Id,
       u.FullName,
       u.Email,
       SUM(o.TotalAmount) AS TotalSpent
FROM Users u
JOIN Orders o ON u.Id = o.UserId
WHERE u.IsDeleted = 0
  AND o.IsDeleted = 0
  AND o.Status = 'Completed'
GROUP BY u.Id, u.FullName, u.Email
ORDER BY TotalSpent DESC;
```

### Tìm kiếm

```sql
SELECT *
FROM Users
WHERE IsDeleted = 0
  AND (
      FullName LIKE '%nguyen%'
      OR Email LIKE '%nguyen%'
  );
```

### Phân trang

```sql
SELECT *
FROM Users
WHERE IsDeleted = 0
ORDER BY Id
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
```

## Ghi chú về EF Core

### DbContext

`AppDbContext` quản lý các `DbSet` chính:

- `Users`
- `Roles`
- `UserRoles`
- `Orders`

Đồng thời cấu hình:

- Tên bảng
- Độ dài field
- Required field
- Unique index
- Foreign key
- Composite key
- Check constraint
- Decimal precision
- Global query filter

### Soft delete

Các entity kế thừa `AuditedEntity` đều có trường `IsDeleted`.

Global query filter giúp tự động ẩn dữ liệu đã bị soft delete:

```csharp
entity.HasQueryFilter(x => !x.IsDeleted);
```

Khi xóa người dùng, API chỉ cập nhật `IsDeleted = true`:

```csharp
user.IsDeleted = true;
await _db.SaveChangesAsync();
```

### Tracking và NoTracking

Các query chỉ đọc dùng `AsNoTracking()` để giảm chi phí tracking của EF Core:

```csharp
var users = await _db.Users
    .AsNoTracking()
    .Select(u => new UserDto
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Age = u.Age
    })
    .ToListAsync();
```

Các luồng cập nhật hoặc xóa dùng tracking để EF Core theo dõi thay đổi và lưu vào database.

## Tối ưu hiệu năng

Dự án áp dụng một số kỹ thuật tối ưu cơ bản:

- Tạo index cho các cột thường dùng để tìm kiếm, join và báo cáo
- Dùng DTO projection thay vì trả về toàn bộ entity graph
- Dùng `AsNoTracking()` cho query chỉ đọc
- Phân trang dữ liệu bằng `Skip` và `Take`
- Tránh N+1 query bằng cách projection dữ liệu liên quan trong cùng một query
- Thực hiện báo cáo ở database thông qua `GroupBy`, `Sum`, `Count` và `Take`

## Mục tiêu học tập

Mini project này được xây dựng để luyện tập:

- Thiết kế database quan hệ
- Mapping entity với Entity Framework Core
- Quan hệ một-nhiều và nhiều-nhiều
- CRUD API trong ASP.NET Core
- SQL join, search, pagination và report
- Soft delete
- Migration và seed data
- Tracking và no-tracking query
- Tối ưu query backend cơ bản
- Quy trình phát triển backend với Rider và DataGrip

## Ghi chú

Đây là dự án mini project tự học về .NET backend. Mục tiêu chính là hiểu rõ cách xây dựng API, thiết kế database, thao tác với EF Core và tối ưu query ở mức nền tảng.

## License

Dự án này được sử dụng cho mục đích học tập, thực hành và xây dựng portfolio cá nhân.

Bạn có thể tham khảo, chỉnh sửa và phát triển thêm theo nhu cầu học tập của mình.
