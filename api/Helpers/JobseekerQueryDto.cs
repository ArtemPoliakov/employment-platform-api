using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class JobseekerQueryDto
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string Profession { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public float ExperienceMin { get; set; } = 0;
        public float ExperienceMax { get; set; } = 100;
    }
}