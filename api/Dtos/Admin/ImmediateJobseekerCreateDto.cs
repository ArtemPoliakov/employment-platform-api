using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobseekerDto;

namespace api.Dtos.Admin
{
    public class ImmediateJobseekerCreateDto
    {
        [Required]
        public RegisterDto RegisterDto { get; set; }
        [Required]
        public CreateJobseekerDto CreateJobseekerDto { get; set; }
    }
}