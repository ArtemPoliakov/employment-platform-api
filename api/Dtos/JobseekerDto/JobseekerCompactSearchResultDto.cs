using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobseekerDto
{
    public class JobseekerCompactSearchResultDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public float Experience { get; set; } = 0;
        public string Education { get; set; } = DegreeTypes.NONE.ToString();
        public string Location { get; set; } = string.Empty;
        public bool IsEmployed { get; set; } = false;
        public string AppUserId { get; set; } = string.Empty;
    }
}