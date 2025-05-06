using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Assignment2
{
    public class Topic:IEntity<Guid>
    {
        [Column("TopicId")]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Session> Sessions { get; set; }

        public Guid CourseId { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n\tTitle: {Title}\n\tDescription: {Description}\n\tSessions: {Sessions?.Count ?? 0}";
        }
    }
}
