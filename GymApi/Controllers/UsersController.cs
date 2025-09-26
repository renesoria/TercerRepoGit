using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace newCRUD.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> _users = new()
        {
            new User { Id = Guid.NewGuid(), Name = "Luna",    Email = "LunaIsHere.@gmail.com",  Password = "MyBirthday",   Age = 63 },
            new User { Id = Guid.NewGuid(), Name = "Mishel",  Email = "Mishigato123.@gmail.com",Password = "Password1234", Age = 28 },
            new User { Id = Guid.NewGuid(), Name = "Gaylord", Email = "Gaylord.@gmail.com",     Password = "Hello-World",  Age = 33 },
            new User { Id = Guid.NewGuid(), Name = "David",   Email = "David.@gmail.com",       Password = "Holakhease",   Age = 62 },
            new User { Id = Guid.NewGuid(), Name = "Goliath", Email = "Goliath.@gmail.com",     Password = "12345678",     Age = 31 },
            new User { Id = Guid.NewGuid(), Name = "True",    Email = "True.@gmail.com",        Password = "Noquiero",     Age = 32 }
        };

        private static (int page, int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }

        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src, string? sort, string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return src;
            return string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase)
                ? src.OrderByDescending(x => prop.GetValue(x))
                : src.OrderBy(x => prop.GetValue(x));
        }

        // GET /api/v1/users
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] string? order,    // asc | desc
            [FromQuery] string? q,
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge,
            [FromQuery] string? email
        )
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<User> query = _users;

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(u => u.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                         u.Email.Contains(q, StringComparison.OrdinalIgnoreCase));

            if (minAge.HasValue) query = query.Where(u => u.Age >= minAge.Value);
            if (maxAge.HasValue) query = query.Where(u => u.Age <= maxAge.Value);
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

            query = OrderByProp(query, sort, order);

            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();

            return Ok(new
            {
                data,
                meta = new
                {
                    page = p,
                    limit = l,
                    total,
                    sort = string.IsNullOrWhiteSpace(sort) ? null : sort,
                    order = string.IsNullOrWhiteSpace(order) ? "asc" : order.ToLower(),
                    q,
                    filters = new { minAge, maxAge, email }
                }
            });
        }

        // GET /api/v1/users/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<User> GetOne(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return user is null
                ? NotFound(new { error = "User not found", status = 404 })
                : Ok(user);
        }

        // POST /api/v1/users
        [HttpPost]
        public ActionResult<User> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password.Trim(),
                Age = dto.Age
            };

            _users.Add(user);
            return CreatedAtAction(nameof(GetOne), new { id = user.Id }, user);
        }

        // PUT /api/v1/users/{id}
        [HttpPut("{id:guid}")]
        public ActionResult<User> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var index = _users.FindIndex(u => u.Id == id);
            if (index == -1) return NotFound(new { error = "User not found", status = 404 });

            var updated = new User
            {
                Id = id,
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password.Trim(),
                Age = dto.Age
            };

            _users[index] = updated;
            return Ok(updated);
        }

        // DELETE /api/v1/users/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _users.RemoveAll(u => u.Id == id);
            return removed == 0 ? NotFound(new { error = "User not found", status = 404 }) : NoContent();
        }
    }
}


