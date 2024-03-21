using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Lecture
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int SectionId { get; set; }

        public Section Section { get; set; }
    }
}
