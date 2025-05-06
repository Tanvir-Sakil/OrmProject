using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class ForeignKeyConstraint
    {
        public string ConstraintName { get; set; }
        public string ChildTable { get; set; }
        public string ChildColumn { get; set; }
        public string ParentTable { get; set; }
        public string ParentColumn { get; set; }
    }
}
