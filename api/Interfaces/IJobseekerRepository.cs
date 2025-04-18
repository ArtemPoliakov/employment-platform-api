using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IJobseekerRepository
    {
        Task<Jobseeker> CreateAsync(Jobseeker jobseeker);
        Task<Jobseeker> UpdateAsync(Jobseeker jobseeker);
        Task<bool> JobseekerExistsByUserId(string userId);
        Task<Jobseeker?> GetJobseekerByUserId(string userId);
        Task<List<Jobseeker>> GetAllJobseekersAsync();
        Task<List<Jobseeker>> GetJobseekersByUserIdsAsync(List<string> userIds);
    }
}