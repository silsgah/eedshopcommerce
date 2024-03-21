using System.Linq.Expressions;

namespace Entity.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }

        List<Expression<Func<T, object>>> Include { get; }
        Expression<Func<T, object>> Sort { get; }
        Expression<Func<T, object>> SortByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPaging { get; }
    }
}
