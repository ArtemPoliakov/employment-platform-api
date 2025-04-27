using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    /// <summary>
    /// Dto for jobseeker search query. Contains both search params and pagination params
    /// </summary>
    public class JobseekerQueryDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        public string Profession { get; set; } = string.Empty;

        public string Education { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Experience must be between 0 and 100")]
        public float ExperienceMin { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "Experience must be between 0 and 100")]
        public float ExperienceMax { get; set; } = 100;
    }
}