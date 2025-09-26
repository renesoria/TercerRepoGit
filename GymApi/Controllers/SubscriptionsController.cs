using Microsoft.AspNetCore.Mvc;
using System.Reflection;

[ApiController]
[Route("api/v1/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private static readonly List<Subscription> _subscriptions = new()
    {
        new Subscription { Name = "Plan Básico", Duration = 6, SubscriptionDate = DateTime.UtcNow.AddDays(-20) },
        new Subscription { Name = "Plan Premium", Duration = 12, SubscriptionDate = DateTime.UtcNow.AddDays(-5) },
        new Subscription { Name = "Plan Pro", Duration = 24, SubscriptionDate = DateTime.UtcNow.AddDays(-1) }
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
        [FromQuery] string? order,
        [FromQuery] string? q,
        [FromQuery] int? minDuration,
        [FromQuery] int? maxDuration,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate
    )
    {
        var (p, l) = NormalizePage(page, limit);

        IEnumerable<Subscription> query = _subscriptions;

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(s => s.Name.Contains(q, StringComparison.OrdinalIgnoreCase));

        if (minDuration.HasValue) query = query.Where(s => s.Duration >= minDuration.Value);
        if (maxDuration.HasValue) query = query.Where(s => s.Duration <= maxDuration.Value);

        if (fromDate.HasValue) query = query.Where(s => s.SubscriptionDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(s => s.SubscriptionDate <= toDate.Value);

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
                filters = new
                {
                    minDuration,
                    maxDuration,
                    fromDate,
                    toDate
                }
            }
        });
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Subscription> GetOne(Guid id)
        => _subscriptions.FirstOrDefault(s => s.Id == id) is { } sub
           ? Ok(sub)
           : NotFound(new { error = "Subscription not found" });

    [HttpPost]
    public ActionResult<Subscription> Create([FromBody] CreateSubscriptionDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            SubscriptionDate = dto.SubscriptionDate,
            Duration = dto.Duration,
            Name = dto.Name.Trim()
        };

        _subscriptions.Add(sub);
        return CreatedAtAction(nameof(GetOne), new { id = sub.Id }, sub);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateSubscriptionDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var index = _subscriptions.FindIndex(s => s.Id == id);
        if (index == -1) return NotFound(new { error = "Subscription not found" });

        _subscriptions[index] = new Subscription
        {
            Id = id,
            SubscriptionDate = dto.SubscriptionDate,
            Duration = dto.Duration,
            Name = dto.Name.Trim()
        };

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var removed = _subscriptions.RemoveAll(s => s.Id == id);
        return removed == 0 ? NotFound(new { error = "Subscription not found" }) : NoContent();
    }
}