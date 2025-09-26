using FirstExam.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FirstExam.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MembershipsController : ControllerBase
    {
        // Datos en memoria (demo)
        private static readonly List<Memberships> _memberships = new()
        {
            new Memberships {
                Id        = Guid.NewGuid(),
                MemberId  = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Plan      = "pro",
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate   = DateTime.UtcNow.AddMonths(11),
                Status    = "active"
            },
            new Memberships {
                Id        = Guid.NewGuid(),
                MemberId  = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Plan      = "basic",
                StartDate = DateTime.UtcNow.AddMonths(-3),
                EndDate   = DateTime.UtcNow.AddDays(-1),
                Status    = "expired"
            }
        };

        // Helpers: paginación y orden
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

        // GET /api/v1/memberships?sort=&order=&page=&limit=
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] string? order
        )
        {
            var (p, l) = NormalizePage(page, limit);

            IEnumerable<Memberships> query = _memberships;
            query = OrderByProp(query, sort, order);

            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();

            return Ok(new { data, meta = new { page = p, limit = l, total } });
        }

        // GET /api/v1/memberships/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<Memberships> GetOne(Guid id)
        {
            var item = _memberships.FirstOrDefault(m => m.Id == id);
            return item is null
                ? NotFound(new { error = "Membership not found", status = 404 })
                : Ok(item);
        }

        // POST /api/v1/memberships
        [HttpPost]
        public ActionResult<Memberships> Create([FromBody] CreateMembershipsDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            if (dto.EndDate <= dto.StartDate)
                return BadRequest(new { error = "EndDate must be after StartDate", status = 400 });

            var status = string.IsNullOrWhiteSpace(dto.Status) ? "active" : dto.Status!.Trim();

            var entity = new Memberships
            {
                Id = Guid.NewGuid(),
                MemberId = dto.MemberId,
                Plan = dto.Plan.Trim(),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = status
            };

            _memberships.Add(entity);
            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, entity); // 201
        }

        // PUT /api/v1/memberships/{id}
        [HttpPut("{id:guid}")]
        public ActionResult<Memberships> Update(Guid id, [FromBody] UpdateMembershipsDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            if (dto.EndDate <= dto.StartDate)
                return BadRequest(new { error = "EndDate must be after StartDate", status = 400 });

            var index = _memberships.FindIndex(m => m.Id == id);
            if (index == -1)
                return NotFound(new { error = "Membership not found", status = 404 });

            var updated = new Memberships
            {
                Id = id,
                MemberId = dto.MemberId,
                Plan = dto.Plan.Trim(),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status.Trim()
            };

            _memberships[index] = updated;
            return Ok(updated); // 200
        }

        // DELETE /api/v1/memberships/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _memberships.RemoveAll(m => m.Id == id);
            return removed == 0
                ? NotFound(new { error = "Membership not found", status = 404 })
                : NoContent(); // 204
        }
    }
}
