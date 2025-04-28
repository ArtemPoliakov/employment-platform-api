using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for jobseeker repository (Database operations).
    /// </summary>
    public interface IJobseekerRepository
    {
        Task<Jobseeker> CreateAsync(Jobseeker jobseeker);
        Task<Jobseeker> UpdateAsync(Jobseeker jobseeker);
        Task<bool> JobseekerExistsByUserIdAsync(string userId);
        Task<Jobseeker?> GetJobseekerByUserIdAsync(string userId);
        Task<List<Jobseeker>> GetAllJobseekersAsync();
        Task<List<Jobseeker>> GetJobseekersByUserIdsAsync(List<string> userIds);
    }
}