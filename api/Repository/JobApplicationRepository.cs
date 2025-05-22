using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.JobApplicationDtos;
using api.Dtos.JobseekerDto;
using api.Dtos.VacancyDtos;
using api.Enums;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    /// <summary>
    /// Repository for job application operations
    /// </summary>
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JobApplicationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new job application
        /// </summary>
        /// <param name="jobApplication">The job application to be created</param>
        /// <returns>The created job application</returns>
        public async Task<JobApplication> CreateAsync(JobApplication jobApplication)
        {
            await _dbContext.JobApplications.AddAsync(jobApplication);
            await _dbContext.SaveChangesAsync();
            return jobApplication;
        }

        /// <summary>
        /// Deletes the job application from the database using a composite key consisting of a jobseeker ID and a vacancy ID.
        /// </summary>
        /// <param name="jobseekerId">The ID of the jobseeker associated with the job application.</param>
        /// <param name="vacancyId">The ID of the vacancy associated with the job application.</param>
        /// <returns>The number of rows deleted from the database.</returns>
        public async Task<int> DeleteAsync(Guid jobseekerId, Guid vacancyId)
        {
            int deletedRows = await _dbContext.JobApplications
                            .Where(a => a.JobseekerId == jobseekerId && a.VacancyId == vacancyId)
                            .ExecuteDeleteAsync();
            return deletedRows;
        }


        /// <summary>
        /// Retrieves the pageSize number of job applications associated with a specific jobseeker.
        /// </summary>
        /// <param name="id">The unique identifier of the jobseeker.</param>
        /// <param name="page">Page number for pagination, defaults to 1.</param>
        /// <param name="pageSize">Number of job applications per page, defaults to 10.</param>
        /// <returns>A list of job application DTOs containing jobseeker-specific data for the specified jobseeker.</returns>
        public async Task<List<JobApplicationWithVacancyDto>> GetAllByJobseekerIdAsync(Guid id, int page, int pageSize)
        {
            return await _dbContext.JobApplications
                        .Where(a => a.JobseekerId == id)
                        .OrderByDescending(a => a.CreationDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(
                            a => new JobApplicationWithVacancyDto
                            {
                                VacancyCompactDto = new VacancyCompactDto
                                {
                                    CompanyUserName = a.Vacancy.Company.AppUser.UserName ?? "none",
                                    Title = a.Vacancy.Title,
                                    Description = a.Vacancy.Description,
                                    Position = a.Vacancy.Position,
                                    SalaryMin = a.Vacancy.SalaryMin,
                                    SalaryMax = a.Vacancy.SalaryMax,
                                    WorkMode = a.Vacancy.WorkMode.ToString(),
                                    Id = a.Vacancy.Id
                                },
                                CompanyResponse = a.CompanyResponse,
                                Status = a.Status.ToString(),
                                VacancyId = a.VacancyId,
                                JobseekerId = a.JobseekerId,
                                CreationDate = a.CreationDate
                            }
                        ).ToListAsync();
        }


        /// <summary>
        /// Retrieves all job applications for a specific vacancy.
        /// </summary>
        /// <param name="id">The unique identifier of the vacancy.</param>
        /// <param name="getOnlyNonRejected">A flag indicating whether to exclude rejected applications.</param>
        /// <returns>A list of job application DTOs containing jobseeker-specific data for the specified vacancy.</returns>
        public async Task<List<JobApplicationWithJobseekerDto>> GetAllByVacancyIdAsync(Guid id, bool getOnlyNonRejected = false)
        {
            var query = _dbContext.JobApplications.AsQueryable();
            if (getOnlyNonRejected)
            {
                query = query.Where(a => a.Status != JobApplicationStatuses.REJECTED);
            }
            return await query.Where(a => a.VacancyId == id)
            .Select(
                a => new JobApplicationWithJobseekerDto
                {
                    JobseekerCompactDto = new JobseekerCompactSearchResultDto
                    {
                        UserName = a.Jobseeker.AppUser.UserName ?? string.Empty,
                        Profession = a.Jobseeker.Profession ?? string.Empty,
                        Experience = a.Jobseeker.Experience,
                        Education = a.Jobseeker.Education.ToString(),
                        Location = a.Jobseeker.Location ?? string.Empty,
                        IsEmployed = a.Jobseeker.IsEmployed,
                        AppUserId = a.Jobseeker.AppUserId ?? string.Empty
                    },
                    CompanyResponse = a.CompanyResponse,
                    Status = a.Status.ToString(),
                    VacancyId = a.VacancyId,
                    JobseekerId = a.JobseekerId,
                    CreationDate = a.CreationDate
                }
            ).ToListAsync();
        }

        /// <summary>
        /// Retrieves a job application by a composite key consisting of a jobseeker ID and a vacancy ID.
        /// </summary>
        /// <param name="jobseekerId">The ID of the jobseeker associated with the job application.</param>
        /// <param name="vacancyId">The ID of the vacancy associated with the job application.</param>
        /// <returns>The job application if the composite key is valid; otherwise, null.</returns>
        public async Task<JobApplication?> GetByCompositeKeyAsync(Guid jobseekerId, Guid vacancyId)
        {
            return await _dbContext.JobApplications
            .FirstOrDefaultAsync(a => a.JobseekerId == jobseekerId && a.VacancyId == vacancyId);
        }

        /// <summary>
        /// Updates an existing job application.
        /// </summary>
        /// <param name="jobApplication">The job application to be updated.</param>
        /// <returns>The updated job application.</returns>
        public async Task<JobApplication> UpdateAsync(JobApplication jobApplication)
        {
            _dbContext.Update(jobApplication);
            await _dbContext.SaveChangesAsync();
            return jobApplication;
        }
    }
}