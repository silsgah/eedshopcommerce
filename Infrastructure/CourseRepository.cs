using Entity;
using Entity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CourseRepository : ICourseRepository
    {
        private readonly StoreContext context;

        public CourseRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<Course> GetCourseByIdAsync(Guid id)
        {
            return await context.Courses.Include(c => c.Category)
          .Include(c => c.Requirements)
          .Include(c => c.Learnings)
          .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Course>> GetCoursesAsync()
        {
            return await context.Courses.Include(c => c.Category).ToListAsync();
        }
    }
}
