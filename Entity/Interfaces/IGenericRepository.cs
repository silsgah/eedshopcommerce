﻿using Entity.Specifications;

namespace Entity.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IReadOnlyList<T>> ListAllAsync();

        Task<T> GetByIdAsync(dynamic id);

        Task<T> GetEntityWithSpec(ISpecification<T> spec);

        Task<IReadOnlyList<T>> ListWithSpec(ISpecification<T> spec);
        Task<int> CountResultAsync(ISpecification<T> spec);
    }
}
