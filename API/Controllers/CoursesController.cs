using API.DTO;
using API.ErrorResponse;
using API.Helpers;
using AutoMapper;
using Entity;
using Entity.Interfaces;
using Entity.Specifications;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly IGenericRepository<Course> repository;
        private readonly IMapper mapper;
        private readonly StoreContext _context;
        public CoursesController(IGenericRepository<Course> repository, IMapper mapper, StoreContext context)
        {
            this.repository = repository;
            this.mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<CourseDTO>>> GetCourses([FromQuery] CourseParams courseParams)
        {
            var spec = new CoursesWithCategoriesSpecification(courseParams);

            var countSpec = new CoursesFiltersCountSpecification(courseParams);

            var total = await repository.CountResultAsync(countSpec);

            var courses = await repository.ListWithSpec(spec);

            if (courses == null) return NotFound(new ApiResponse(404));

            var data = mapper.Map<IReadOnlyList<Course>, IReadOnlyList<CourseDTO>>(courses);

            return Ok(new Pagination<CourseDTO>(courseParams.PageIndex, courseParams.PageSize, total, data));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse(Guid id)
        {
            var spec = new CoursesWithCategoriesSpecification(id);

            var course = await repository.GetEntityWithSpec(spec);

            if (course == null) return NotFound(new ApiResponse(404));

            return mapper.Map<Course, CourseDTO>(course);

        }
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<string>> CreateCourse([FromBody] Course course)
        {
            course.Instructor = User.Identity.Name;

            _context.Courses.Add(course);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return "Course Created Successfully";

            return BadRequest(new ApiResponse(400, "Problem creating Course"));
        }
        ///
        [Authorize(Roles = "Instructor")]
        [HttpPost("publish/{courseId}")]

        public async Task<ActionResult<string>> PublishCourse(Guid courseId)
        {

            var course = await _context.Courses.FindAsync(courseId);

            if (course == null) return NotFound(new ApiResponse(404));

            course.Published = true;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return "Course Published Successfully";

            return BadRequest(new ApiResponse(400, "Problem publishing the Course"));

        }
    }
}
