namespace API.DTO
{
    public class CourseDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Language { get; set; }

        public string Level { get; set; }

        public string SubTitle { get; set; }

        public int Students { get; set; }

        public string Description { get; set; }
        public float Price { get; set; }

        public string Instructor { get; set; }

        public string Image { get; set; }

        public string Category { get; set; }

        public decimal Rating { get; set; }
        public DateTime LastUpdated { get; set; }

        public ICollection<RequirementDTO> Requirements { get; set; }

        public ICollection<LearningDTO> Learnings { get; set; }
    }
}
