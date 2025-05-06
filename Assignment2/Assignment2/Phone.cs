using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Phone:IEntity<Guid>
    {
        [Column("PhoneId")]
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public string CountryCode { get; set; }

        public Guid InstructorId { get; set; }

        public override string ToString()
        {
            return $"Phone Number: {CountryCode}-{Number},{Extension}";
        }
    }
}
