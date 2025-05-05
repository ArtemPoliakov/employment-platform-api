using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobApplicationDtos;
using api.Models;

namespace api.Mappers
{
    public static class JobApplicationMapper
    {
        public static void UpdateChangesToJobApplication(this JobApplication jobApplication, UpdateJobApplicationDto updateJobApplicationDto)
        {
            jobApplication.CompanyResponse = updateJobApplicationDto.CompanyResponse ?? jobApplication.CompanyResponse;
            jobApplication.Status = updateJobApplicationDto.Status ?? jobApplication.Status;
        }
    }
}