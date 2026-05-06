using SSIS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class CoursePrerequesite :BaseEntity
    {
        public Guid Courseid { get; set; }
        public Guid PrerequesiteCourseid { get; set; }
        public virtual Course Course { get; set; } = null!;
        public virtual Course PrerequesiteCourse  { get; set; }   = null!;

    }
}
