

namespace Entity.Specifications
{
    public class CoursesWithCategoriesSpecification : BaseSpecification<Course>
    {
        public CoursesWithCategoriesSpecification(CourseParams courseParams) : base(x =>
            (string.IsNullOrEmpty(courseParams.Search) || x.Title.ToLower().Contains(courseParams.Search)) &&
            (!courseParams.CategoryId.HasValue || x.CategoryId == courseParams.CategoryId) && (x.Published == true)
        )
        {
            IncludeMethod(c => c.Category);
            IncludeMethod(c => c.Requirements);
            IncludeMethod(c => c.Learnings);
            SortMethod(c => c.Title);
            ApplyPagination(courseParams.PageSize * (courseParams.PageIndex - 1), courseParams.PageSize);

            if (!string.IsNullOrEmpty(courseParams.Sort))
            {
                switch (courseParams.Sort)
                {
                    case "priceAscending":
                        SortMethod(c => c.Price);
                        break;
                    case "priceDescending":
                        SortByDescendingMethod(c => c.Price);
                        break;
                    default:
                        SortMethod(c => c.Title);
                        break;
                }
            }
        }

        public CoursesWithCategoriesSpecification(Guid id) : base(x => x.Id == id)
        {
            IncludeMethod(c => c.Requirements);
            IncludeMethod(c => c.Learnings);
            IncludeMethod(c => c.Category);
            SortMethod(c => c.Id);
        }

    }
}
