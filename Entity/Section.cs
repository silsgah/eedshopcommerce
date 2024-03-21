using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Section
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Lecture> Lectures { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }
    }
}
