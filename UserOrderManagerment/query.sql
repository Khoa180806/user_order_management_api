SELECT *
FROM Users;

SELECT *
FROM Roles;

SELECT *
FROM UserRoles;

SELECT *
FROM Orders;

-- JOIN User + Role
SELECT u.Id,
       u.FullName,
       u.Email,
       r.Name AS RoleName
FROM Users u
         JOIN UserRoles ur ON u.Id = ur.UserId
         JOIN Roles r ON ur.RoleId = r.Id
WHERE u.IsDeleted = 0;

-- JOIN User + Orders
SELECT u.Id,
       u.FullName,
       o.Id AS OrderId,
       o.TotalAmount,
       o.CreatedAt
FROM Users u
         JOIN Orders o ON u.Id = o.UserId
WHERE u.IsDeleted = 0
  AND o.IsDeleted = 0;

-- Revenue report
SELECT CAST(o.CreatedAt AS date) AS OrderDate,
       SUM(o.TotalAmount)        AS Revenue,
       COUNT(*)                  AS OrderCount
FROM Orders o
WHERE o.IsDeleted = 0
  AND o.Status = 'Completed'
GROUP BY CAST(o.CreatedAt AS date)
ORDER BY OrderDate DESC;

-- Top customer
SELECT TOP 10 u.Id, u.FullName,
       u.Email,
       SUM(o.TotalAmount) AS TotalSpent
FROM Users u
         JOIN Orders o ON u.Id = o.UserId
WHERE u.IsDeleted = 0
  AND o.IsDeleted = 0
  AND o.Status = 'Completed'
GROUP BY u.Id, u.FullName, u.Email
ORDER BY TotalSpent DESC;

-- Search
SELECT *
FROM Users
WHERE IsDeleted = 0
  AND (
    FullName LIKE '%nguyen%'
        OR Email LIKE '%nguyen%'
    );

-- Pagination
SELECT *
FROM Users
WHERE IsDeleted = 0
ORDER BY Id
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;