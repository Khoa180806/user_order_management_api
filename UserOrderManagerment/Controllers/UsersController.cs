using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserOrderManagerment.Data;
using UserOrderManagerment.DTOs;
using UserOrderManagerment.Entities;

namespace UserOrderManagerment.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _db.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Age = u.Age,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<UserDto>>> Search(
        string? keyword,
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _db.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u =>
                u.FullName.Contains(keyword) ||
                u.Email.Contains(keyword));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Age = u.Age,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(new PagedResult<UserDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        });
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user.Id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        user.IsDeleted = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}