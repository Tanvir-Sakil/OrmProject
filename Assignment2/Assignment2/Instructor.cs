using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Instructor:IEntity<Guid>
    {
        [Column("InstructorId")]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Guid? PresentAddressId { get; set; }

        [NotMapped]
        public Address PresentAddress { get; set; }
        public Guid? PermanentAddressId { get; set; }
        
        [NotMapped]
        public Address PermanentAddress { get; set; }
        public List<Phone> PhoneNumbers { get; set; }

        public Guid CourseId { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}\n\tEmail: {Email}";
        }

    }


}
