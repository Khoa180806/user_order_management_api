using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserOrderManagerment.Data;
using UserOrderManagerment.DTOs;

namespace UserOrderManagerment.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<List<RevenueReportDto>>> RevenueReport()
    {
        var report = await _db.Orders
            .AsNoTracking()
            .Where(o => o.Status == "Completed")
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new RevenueReportDto
            {
                Date = g.Key,
                Revenue = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return Ok(report);
    }

    [HttpGet("top-customers")]
    public async Task<ActionResult<List<TopCustomerDto>>> TopCustomers()
    {
        var customers = await _db.Orders
            .AsNoTracking()
            .Where(o => o.Status == "Completed")
            .GroupBy(o => new
            {
                o.UserId,
                o.User.FullName,
                o.User.Email
            })
            .Select(g => new TopCustomerDto
            {
                UserId = g.Key.UserId,
                FullName = g.Key.FullName,
                Email = g.Key.Email,
                TotalSpent = g.Sum(x => x.TotalAmount)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(10)
            .ToListAsync();

        return Ok(customers);
    }
}