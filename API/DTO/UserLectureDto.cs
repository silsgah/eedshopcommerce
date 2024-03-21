namespace API.DTO
{
    public class UserLectureDto
    {
        public Guid id { get; set; }
        public string CourseName { get; set; }
        public List<SectionDto> Sections { get; set; }
        public int CurrentLecture { get; set; }
    }
}
