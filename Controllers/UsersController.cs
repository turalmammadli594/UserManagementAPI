using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Dtos;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null
            ? NotFound(new { message = $"User with id {id} was not found." })
            : Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] UserCreateDto dto)
    {
        // DataAnnotations invalid olsa [ApiController] avtomatik 400 qaytarır.

        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            return Conflict(new { message = "A user with this email already exists." });

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            IsActive = dto.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var existing = await _db.Users.FindAsync(id);
        if (existing is null)
            return NotFound(new { message = $"User with id {id} was not found." });

        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
        if (emailExists)
            return Conflict(new { message = "A user with this email already exists." });

        existing.FirstName = dto.FirstName.Trim();
        existing.LastName = dto.LastName.Trim();
        existing.Email = dto.Email.Trim();
        existing.Department = dto.Department.Trim();
        existing.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with id {id} was not found." });

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Optional: middleware test üçün qəsdən exception (istəsən sil)
    [HttpGet("throw")]
    public IActionResult Throw() => throw new Exception("Test exception");
}
