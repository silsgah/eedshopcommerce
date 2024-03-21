﻿namespace Entity
{
    public class Requirement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }
    }
}