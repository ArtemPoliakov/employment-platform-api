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
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JobApplicationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobApplication> CreateAsync(JobApplication jobApplication)
        {
            await _dbContext.JobApplications.AddAsync(jobApplication);
            await _dbContext.SaveChangesAsync();
            return jobApplication;
        }

        public async Task<int> DeleteAsync(Guid jobseekerId, Guid vacancyId)
        {
            int deletedRows = await _dbContext.JobApplications
                            .Where(a => a.JobseekerId == jobseekerId && a.VacancyId == vacancyId)
                            .ExecuteDeleteAsync();
            return deletedRows;
        }

        public async Task<List<JobApplicationWithVacancyDto>> GetAllByJobseekerIdAsync(Guid id)
        {
            return await _dbContext.JobApplications
                        .Where(a => a.JobseekerId == id)
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

        public async Task<JobApplication?> GetByCompositeKeyAsync(Guid jobseekerId, Guid vacancyId)
        {
            return await _dbContext.JobApplications
            .FirstOrDefaultAsync(a => a.JobseekerId == jobseekerId && a.VacancyId == vacancyId);
        }

        public async Task<JobApplication> UpdateAsync(JobApplication jobApplication)
        {
            _dbContext.Update(jobApplication);
            await _dbContext.SaveChangesAsync();
            return jobApplication;
        }
    }
}