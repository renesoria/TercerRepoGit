using Microsoft.AspNetCore.Mvc;
using System.Reflection;
namespace GymApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TrainersController : ControllerBase
    {
        private static readonly List<Trainer> _trainers = new()
        {
            new Trainer{Id= Guid.NewGuid(), FullName="Dante Soria", Specialty="cardio", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="David Andazola", Specialty="funcional", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="Denar Yamil", Specialty="cardio", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="Denis Azpilicueta", Specialty="yoga", Certified=false},
            new Trainer{Id= Guid.NewGuid(), FullName="Giovani Gandarillas", Specialty="yoga", Certified=false},
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

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] string? order
            )
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Trainer> query = _trainers;
            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta = new { page = p, limit = l, total }
            });
        }
    
    [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var trainer = _trainers.FirstOrDefault(t => t.Id == id);
            return trainer is null
             ? NotFound(new { error = "Trainer not found", status = 404 })
                : Ok(trainer);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateTrainerDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var trainer = new Trainer
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Specialty = dto.Specialty.Trim(),
            };

            _trainers.Add(trainer);
            return CreatedAtAction(nameof(GetOne), new { id = trainer.Id }, trainer);
        }

        // PUT /api/v1/members/{id}
        [HttpPut("{id:guid}")]
        public ActionResult<Trainer> Update(Guid id, [FromBody] UpdateTrainerDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var index = _trainers.FindIndex(m => m.Id == id);
            if (index == -1)
                return NotFound(new { error = "Trainer not found", status = 404 });

            var updated = new Trainer
            {
                Id = id,
                FullName = dto.FullName.Trim(),
                Specialty = dto.Specialty.Trim(),
            };

            _trainers[index] = updated;
            return Ok(updated);
        }

        // DELETE /api/v1/members/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _trainers.RemoveAll(t => t.Id == id);
            return removed == 0
                ? NotFound(new { error = "Trainer not found", status = 404 })
                : NoContent();
        }
    }
}
