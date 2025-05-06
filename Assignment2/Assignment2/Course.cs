using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Course :IEntity<Guid>
    {
        [Column("CourseId")]
        public Guid Id { get; set; }
        public string Title { get; set; }

        [NotMapped]
        public Instructor Teacher { get; set; }
        public List<Topic> Topics { get; set; }
        public double Fees { get; set; }
        public List<AdmissionTest> Tests{ get; set; }

        public Guid? TeacherId { get; set; } 
    }

}
