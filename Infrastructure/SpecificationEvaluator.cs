using Entity.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); // c => c.Price < 10
            }

            if (spec.Sort != null)
            {
                query = query.OrderBy(spec.Sort);
            }

            if (spec.SortByDescending != null)
            {
                query = query.OrderByDescending(spec.SortByDescending);
            }

            if (spec.IsPaging)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Include.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
