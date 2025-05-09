using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.CustomException;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class JobseekerRepository : IJobseekerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJobseekerElasticService _jobseekerElasticService;

        public JobseekerRepository(ApplicationDbContext dbContext,
                                   IJobseekerElasticService jobseekerElasticService)
        {
            _dbContext = dbContext;
            _jobseekerElasticService = jobseekerElasticService;
        }

        /// <summary>
        /// Adds a new jobseeker to the database and ElasticSearch.
        /// </summary>
        /// <param name="jobseeker">The jobseeker entity to be created.</param>
        /// <returns>The created jobseeker entity.</returns>
        /// <exception cref="JobseekerElasticException">
        /// Thrown when the jobseeker fails to be added to ElasticSearch.
        /// </exception>
        public async Task<Jobseeker> CreateAsync(Jobseeker jobseeker)
        {
            await _dbContext.Jobseekers.AddAsync(jobseeker);
            await _dbContext.SaveChangesAsync();

            bool isElasticSuccess = await _jobseekerElasticService
                                .AddOrUpdateJobseekerAsync(jobseeker.ToJobseekerElasticDto());
            if (!isElasticSuccess)
            {
                throw new JobseekerElasticException("Failed to add jobseeker to elastic");
            }

            return jobseeker;
        }

        /// <summary>
        /// Retrieves all jobseekers from the database.
        /// </summary>
        /// <returns>A list of all jobseekers.</returns>
        public async Task<List<Jobseeker>> GetAllJobseekersAsync()
        {
            var jobseekers = await _dbContext.Jobseekers.ToListAsync();
            return jobseekers;
        }

        /// <summary>
        /// Retrieves a jobseeker by a related user ID.
        /// </summary>
        /// <param name="userId">The user ID to retrieve the jobseeker for.</param>
        /// <returns>The jobseeker, including related AppUser, or null if no jobseeker is found.</returns>
        public async Task<Jobseeker?> GetJobseekerByUserIdAsync(string userId)
        {
            return await _dbContext.Jobseekers.FirstOrDefaultAsync(js => js.AppUserId.ToString().Equals(userId));
        }

        /// <summary>
        /// Retrieves a list of jobseekers by a list of user IDs.
        /// </summary>
        /// <param name="userIds">The list of user IDs to retrieve jobseekers for.</param>
        /// <returns>A list of jobseekers, including their AppUser, that match the user IDs.</returns>
        public async Task<List<Jobseeker>> GetJobseekersByUserIdsAsync(List<string> userIds)
        {
            return await _dbContext
                        .Jobseekers
                        .Where(js => userIds.Contains(js.AppUserId))
                        .Include(js => js.AppUser)
                        .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of the most recently registered jobseekers.
        /// </summary>
        /// <param name="page">Page number for pagination, defaults to 1</param>
        /// <param name="pageSize">Number of jobseekers per page, defaults to 10</param>
        /// <returns>A list of recent jobseekers, including their AppUser and RegisterDate</returns>
        public async Task<List<Jobseeker>> GetRecentJobseekersAsync(int page, int pageSize)
        {
            return await _dbContext.Jobseekers
                        .OrderByDescending(js => js.RegisterDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Include(js => js.AppUser)
                        .ToListAsync();
        }

        /// <summary>
        /// Checks if a jobseeker exists with the specified user ID.
        /// </summary>
        /// <param name="userId">The user ID to check for existence.</param>
        /// <returns>A task representing the asynchronous operation, containing true if the jobseeker exists, otherwise false.</returns>
        public async Task<bool> JobseekerExistsByUserIdAsync(string userId)
        {
            return await _dbContext.Jobseekers.AnyAsync(js => js.AppUserId.ToString().Equals(userId));
        }

        /// <summary>
        /// Searches for jobseekers in ElasticSearch and retrieves their AppUser using the AppUserId.
        /// </summary>
        /// <param name="jobseekerQueryDto">Dto containing search query parameters and pagination parameters</param>
        /// <returns>A list of jobseekers matching the query parameters</returns>
        public async Task<List<Jobseeker>> SearchByQueryAsync(JobseekerQueryDto jobseekerQueryDto)
        {
            var jobseekersDocs = await _jobseekerElasticService.SearchJobseekersByQueryAsync(jobseekerQueryDto);
            var idDictionary = new Dictionary<string, int>();
            for (int i = 0; i < jobseekersDocs.Count; i++)
            {
                idDictionary.Add(jobseekersDocs[i].Id, i);
            }

            var jobseekers = await GetJobseekersByUserIdsAsync(idDictionary.Keys.ToList());
            jobseekers = jobseekers.OrderBy(js => idDictionary[js.AppUserId]).ToList();

            return jobseekers;
        }

        /// <summary>
        /// Updates an existing jobseeker in the database and ElasticSearch.
        /// </summary>
        /// <param name="jobseeker">The jobseeker entity to be updated.</param>
        /// <returns>The updated jobseeker entity.</returns>
        /// <exception cref="JobseekerElasticException">
        /// Thrown when the jobseeker fails to be updated in ElasticSearch.
        /// </exception>
        public async Task<Jobseeker> UpdateAsync(Jobseeker jobseeker)
        {
            _dbContext.Update(jobseeker);
            await _dbContext.SaveChangesAsync();

            bool isElasticSuccess = await _jobseekerElasticService
                                    .AddOrUpdateJobseekerAsync(jobseeker.ToJobseekerElasticDto());
            if (!isElasticSuccess)
            {
                throw new JobseekerElasticException("Failed to update jobseeker in elastic");
            }

            return jobseeker;
        }
    }
}