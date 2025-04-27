using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class JobseekerRepository : IJobseekerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JobseekerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Jobseeker> CreateAsync(Jobseeker jobseeker)
        {
            try
            {
                await _dbContext.Jobseekers.AddAsync(jobseeker);
                await _dbContext.SaveChangesAsync();
                return jobseeker;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"EF SaveChanges failed: {innerMessage}", ex);
            }
        }

        public async Task<List<Jobseeker>> GetAllJobseekersAsync()
        {
            var jobseekers = await _dbContext.Jobseekers.ToListAsync();
            return jobseekers;
        }

        public Task<Jobseeker?> GetJobseekerByUserId(string userId) //!
        {
            return _dbContext.Jobseekers.FirstOrDefaultAsync(js => js.AppUserId.ToString().Equals(userId));
        }

        public async Task<List<Jobseeker>> GetJobseekersByUserIdsAsync(List<string> userIds)
        {
            return await _dbContext
                        .Jobseekers
                        .Where(js => userIds.Contains(js.AppUserId))
                        .Include(js => js.AppUser)
                        .ToListAsync();
        }

        public async Task<bool> JobseekerExistsByUserId(string userId)
        {
            return await _dbContext.Jobseekers.AnyAsync(js => js.AppUserId.ToString().Equals(userId));
        }

        public async Task<Jobseeker> UpdateAsync(Jobseeker jobseeker)
        {
            _dbContext.Update(jobseeker);
            await _dbContext.SaveChangesAsync();
            return jobseeker;
        }
    }
}