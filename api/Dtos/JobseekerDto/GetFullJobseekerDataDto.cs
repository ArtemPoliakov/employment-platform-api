using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobseekerDto
{
    /// <summary>
    /// Dto for getting full jobseeker data including public account details
    /// </summary>
    public class GetFullJobseekerDataDto
    {
        public AppUserPublicDataDto AppUserPublicData { get; set; }
        public JobseekerDto JobseekerData { get; set; }
    }
}