using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobseekerDto.ElasticDtos;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for jobseeker ElasticSearch service.
    /// Used for flexible search.
    /// </summary>
    public interface IJobseekerElasticService
    {
        Task CreateIndexIfNotExistsAsync();

        Task<bool> AddOrUpdateJobseekerAsync(JobseekerElasticDto jobseeker);

        Task<bool> AddOrUpdateJobseekerBulkAsync(IEnumerable<JobseekerElasticDto> jobseekers);

        Task<JobseekerElasticDto?> GetJobseekerAsync(string key);

        Task<List<JobseekerElasticDto>?> GetAllJobseekersAsync();

        Task<bool> DeleteJobseekerAsync(string key);

        Task<long?> RemoveAllAsync();

        Task<List<JobseekerElasticDto>?> SearchJobseekersByQueryAsync(JobseekerQueryDto query);
    }
}