using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Session : IEntity<Guid>
    {
        [Column("SessionId")]
        public Guid Id { get; set; }
        public int DurationInHour { get; set; }
        public string LearningObjective { get; set; }

        public Guid TopicId { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}\n\tDuration: {DurationInHour}h\n\tLearning Objective: {LearningObjective}";
        }

    }

}
