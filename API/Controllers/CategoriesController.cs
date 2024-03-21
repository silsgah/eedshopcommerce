using API.DTO;
using AutoMapper;
using Entity;
using Entity.Interfaces;
using Entity.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IGenericRepository<Category> repository;
        private readonly IMapper mapper;

        public CategoriesController(IGenericRepository<Category> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoriesDTO>>> GetCategories()
        {
            var categories = await repository.ListAllAsync();
            return Ok(mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoriesDTO>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var spec = new CategoriesWithCoursesSpecification(id);
            var category = await repository.GetEntityWithSpec(spec);
            return mapper.Map<Category, CategoryDTO>(category);
        }
    }
}
