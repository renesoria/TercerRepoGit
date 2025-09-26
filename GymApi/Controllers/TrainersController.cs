using Microsoft.AspNetCore.Mvc;
using System.Reflection;
namespace GymApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller")]
    public class TrainersController: ControllerBase
    {
        private static readonly List<Trainer> _trainers = new()
        {
            new Trainer{Id= Guid.NewGuid(), FullName="Dante Soria", Specialty="cardio", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="David Andazola", Specialty="funcional", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="Denar Yamil", Specialty="cardio", Certified=true},
            new Trainer{Id= Guid.NewGuid(), FullName="Denis Azpilicueta", Specialty="yoga", Certified=false},
            new Trainer{Id= Guid.NewGuid(), FullName="Giovani Gandarillas", Specialty="yoga", Certified=false},
        };
        private static (int page, int limit) NormalizePage(int? page, int ? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1;if(l> 100) l = 100;
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
    }
}
