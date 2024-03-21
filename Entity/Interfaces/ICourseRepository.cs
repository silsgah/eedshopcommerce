using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetCourseByIdAsync(Guid id);

        Task<IReadOnlyList<Course>> GetCoursesAsync();
    }
}
