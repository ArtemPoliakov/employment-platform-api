using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.JobseekerDto.ElasticDtos
{
    public class JobseekerElasticDto
    {
        /// <summary>
        /// Matches the AppUserId
        /// </summary>
        public string Id { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public float Experience { get; set; } = 0;
    }
}