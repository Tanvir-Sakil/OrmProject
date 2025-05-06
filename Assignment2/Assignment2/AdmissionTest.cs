using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{

    public class AdmissionTest:IEntity<Guid>
    {
        [Column("AdmissionTestId")]
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double TestFees { get; set; }
        public Guid CourseId { get; set; }
        // public Course Course { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n\tStart From: {StartDateTime}\n\tEnd To: {EndDateTime}\n\tFee: {TestFees}";
        }

    }


}
