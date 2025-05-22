using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobApplicationDtos;
using api.Models;

namespace api.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication> CreateAsync(JobApplication jobApplication);
        Task<JobApplication> UpdateAsync(JobApplication jobApplication);
        Task<List<JobApplicationWithVacancyDto>> GetAllByJobseekerIdAsync(Guid id, int page, int pageSize);
        Task<List<JobApplicationWithJobseekerDto>> GetAllByVacancyIdAsync(Guid id, bool getOnlyNonRejected);
        Task<JobApplication?> GetByCompositeKeyAsync(Guid jobseekerId, Guid vacancyId);
        Task<int> DeleteAsync(Guid jobseekerId, Guid vacancyId);
    }
}