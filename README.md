# User Order Management System

A .NET 8 Web API for managing users, roles, user-role assignments, and customer orders. The project demonstrates a practical backend implementation using Entity Framework Core, SQL Server, ABP-style audit fields, soft delete, relational mapping, reporting queries, pagination, and common performance optimizations.

## Features

- User management with list, search, pagination, create, and soft delete APIs
- Role-based relationship model using a many-to-many `UserRoles` table
- Order management data model with customer-order relationships
- Revenue report endpoint grouped by order date
- Top customer report based on completed order value
- ABP-style audit columns:
  - `CreationTime`
  - `CreatorUserId`
  - `LastModificationTime`
  - `IsDeleted`
- Database constraints:
  - Unique email
  - Foreign keys
  - Check constraint for `Age > 0`
- Database indexes:
  - `Users.Email`
  - `Orders.UserId`
  - `Orders.CreatedAt`
- EF Core global query filters for soft delete
- Seed data for local development
- SQL examples for joins, reporting, search, and pagination
- Swagger UI for API testing

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- Swagger / Swashbuckle
- JetBrains Rider
- DataGrip

## Project Structure

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

## Database Model

### Users

Stores customer/user profile information.

Key fields:

- `Id`
- `FullName`
- `Email`
- `Age`
- `CreationTime`
- `CreatorUserId`
- `LastModificationTime`
- `IsDeleted`

Rules:

- `Email` is unique
- `Age` must be greater than `0`
- Soft-deleted users are filtered by default

### Roles

Stores application roles such as `Admin` and `Customer`.

### UserRoles

Join table for the many-to-many relationship between users and roles.

Composite primary key:

- `UserId`
- `RoleId`

### Orders

Stores customer order data.

Key fields:

- `Id`
- `UserId`
- `TotalAmount`
- `CreatedAt`
- `Status`
- `CreationTime`
- `CreatorUserId`
- `LastModificationTime`
- `IsDeleted`

Indexes:

- `UserId`
- `CreatedAt`

## Getting Started

### Prerequisites

Install the following tools:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server
- JetBrains Rider
- DataGrip

### Clone the Repository

```bash
git clone <your-repository-url>
cd UserOrderManagerment
```

### Configure Database Connection

Update the connection string in `UserOrderManagerment/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=UserOrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

For SQL Server authentication:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=UserOrderManagementDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Restore Packages

```bash
dotnet restore
```

### Apply Database Migration

```bash
cd UserOrderManagerment
dotnet ef database update
```

### Run the API

```bash
dotnet run
```

Swagger UI:

```text
https://localhost:7221/swagger
http://localhost:5220/swagger
```

The application seeds initial users, roles, user-role assignments, and orders on startup when the database is empty.

## API Endpoints

### Users

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/users` | Get all active users |
| GET | `/api/users/search?keyword=a&page=1&pageSize=10` | Search users with pagination |
| POST | `/api/users` | Create a new user |
| DELETE | `/api/users/{id}` | Soft delete a user |

### Reports

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/reports/revenue` | Get daily revenue report |
| GET | `/api/reports/top-customers` | Get top 10 customers by total completed order value |

## Sample User Create Request

```json
{
  "fullName": "Le Van C",
  "email": "c@example.com",
  "age": 28
}
```

## SQL Examples

The project includes `UserOrderManagerment/query.sql` with useful SQL queries for:

- Joining users and roles
- Joining users and orders
- Revenue reporting
- Top customer reporting
- Search
- Pagination

Example revenue report:

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

Example pagination:

```sql
SELECT *
FROM Users
WHERE IsDeleted = 0
ORDER BY Id
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
```

## EF Core Implementation Notes

### DbContext

`AppDbContext` defines the main entity sets:

- `Users`
- `Roles`
- `UserRoles`
- `Orders`

It also configures:

- Table names
- Property lengths and required fields
- Unique indexes
- Foreign key relationships
- Composite keys
- Query filters
- Decimal precision
- Check constraints

### Soft Delete

Entities inheriting from `AuditedEntity` include `IsDeleted`.

Global query filters hide soft-deleted records:

```csharp
entity.HasQueryFilter(x => !x.IsDeleted);
```

The delete API marks a user as deleted instead of removing the row:

```csharp
user.IsDeleted = true;
await _db.SaveChangesAsync();
```

### Tracking and No Tracking

Read-heavy endpoints use `AsNoTracking()` to reduce EF Core change-tracking overhead:

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

Update and delete flows use tracking so EF Core can persist entity changes.

## Performance Notes

This project applies several common backend performance practices:

- Uses indexes on frequent lookup and reporting columns
- Uses DTO projection instead of returning full entity graphs
- Uses `AsNoTracking()` for read-only queries
- Uses pagination for user search
- Avoids N+1 query patterns by projecting related data in a single query
- Keeps reporting queries server-side with `GroupBy`, `Sum`, `Count`, and `Take`

## Development Workflow

Recommended workflow:

1. Open the solution in JetBrains Rider.
2. Update the SQL Server connection string in `appsettings.json`.
3. Run EF Core migrations.
4. Start the API with the `https` launch profile.
5. Test endpoints in Swagger.
6. Use DataGrip to inspect tables, constraints, indexes, and SQL query results.

## Useful Commands

```bash
dotnet restore
dotnet build
dotnet run --project UserOrderManagerment
dotnet ef migrations add <MigrationName> --project UserOrderManagerment
dotnet ef database update --project UserOrderManagerment
```

## Learning Goals

This project is designed to practice:

- Relational database design
- Entity Framework Core mapping
- Many-to-many relationships
- SQL joins and reports
- REST API development
- Soft delete implementation
- Pagination
- Query performance optimization
- Practical backend development with .NET 8

## License

This project is intended for learning and portfolio use.
