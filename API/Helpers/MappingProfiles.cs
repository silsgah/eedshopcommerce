using API.DTO;
using AutoMapper;
using Entity;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Course, CourseDTO>()
            .ForMember(c => c.Category, o => o.MapFrom(s => s.Category.Name));
            CreateMap<Learning, LearningDTO>();
            CreateMap<Requirement, RequirementDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<Category, CategoriesDTO>();
            CreateMap<Basket, BasketDto>();
            CreateMap<BasketItem, BasketItemDto>()
            .ForMember(b => b.CourseId, o => o.MapFrom(c => c.Course.Id))
            .ForMember(b => b.Title, o => o.MapFrom(c => c.Course.Title))
            .ForMember(b => b.Price, o => o.MapFrom(c => c.Course.Price))
            .ForMember(b => b.Image, o => o.MapFrom(c => c.Course.Image))
            .ForMember(b => b.Instructor, o => o.MapFrom(c => c.Course.Instructor));
            CreateMap<Section, SectionDto>()
            .ForMember(s => s.SectionName, o => o.MapFrom(c => c.Name));
            CreateMap<Lecture, LectureDto>();
        }
    }
}
