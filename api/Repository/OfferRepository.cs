using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.JobseekerDto;
using api.Dtos.OfferDtos;
using api.Dtos.VacancyDtos;
using api.Enums;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    /// <summary>
    /// Repository for offer operations
    /// </summary>
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public OfferRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Asynchronously creates a new offer in the database.
        /// </summary>
        /// <param name="offer">The offer to be created.</param>
        /// <returns>The created offer.</returns>
        public async Task<Offer> CreateAsync(Offer offer)
        {
            await _dbContext.Offers.AddAsync(offer);
            await _dbContext.SaveChangesAsync();
            return offer;
        }

        /// <summary>
        /// Deletes an offer from the database using a composite key consisting of a jobseeker ID and a vacancy ID.
        /// </summary>
        /// <param name="jobseekerId">The ID of the jobseeker associated with the offer.</param>
        /// <param name="vacancyId">The ID of the vacancy associated with the offer.</param>
        /// <returns>The number of rows deleted from the database.</returns>
        public async Task<int> DeleteAsync(Guid jobseekerId, Guid vacancyId)
        {
            int deletedRows = await _dbContext.Offers
                            .Where(o => o.JobseekerId == jobseekerId && o.VacancyId == vacancyId)
                            .ExecuteDeleteAsync();
            return deletedRows;
        }


        /// <summary>
        /// Retrieves all offers made to a jobseeker.
        /// The request query should contain the jobseeker id.
        /// The request query should contain a flag indicating whether to exclude rejected offers.
        /// The request query should contain the page number and page size for pagination.
        /// The response contains offer-specific data and associated vacancy data.
        /// </summary>
        /// <param name="id">The id of the jobseeker.</param>
        /// <param name="getOnlyNonRejected">A flag indicating whether to exclude rejected offers.</param>
        /// <param name="page">Page number for pagination, defaults to 1.</param>
        /// <param name="pageSize">Number of job applications per page, defaults to 10.</param>
        /// <returns>A list of offer dtos containing vacancy data, or an error message if the user is not found, jobseeker data does not exist, or the request is invalid.</returns>
        /// <response code="400">If the request is invalid, user is not found, or jobseeker data does not exist</response>
        /// <response code="401">If the user is unauthorized to get this data.</response>
        /// <response code="200">If the offers are retrieved successfully.</response>
        public async Task<List<OfferWithVacancyDto>> GetAllByJobseekerIdAsync(Guid id, bool getOnlyNonRejected, int page, int pageSize)
        {
            var query = _dbContext.Offers.AsQueryable();
            if (getOnlyNonRejected)
            {
                query = query.Where(o => o.Status != JobApplicationStatuses.REJECTED);
            }
            return await query
                         .Where(o => o.JobseekerId == id)
                         .OrderByDescending(a => a.CreationDate)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .Select(
                             o => new OfferWithVacancyDto
                             {
                                 VacancyCompactDto = new VacancyCompactDto
                                 {
                                     CompanyUserName = o.Vacancy.Company.AppUser.UserName ?? "none",
                                     Title = o.Vacancy.Title,
                                     Description = o.Vacancy.Description,
                                     Position = o.Vacancy.Position,
                                     SalaryMin = o.Vacancy.SalaryMin,
                                     SalaryMax = o.Vacancy.SalaryMax,
                                     WorkMode = o.Vacancy.WorkMode.ToString(),
                                     Id = o.Vacancy.Id
                                 },
                                 CompanyMessage = o.CompanyMessage,
                                 JobseekerResponse = o.JobSeekerResponse,
                                 Status = o.Status.ToString(),
                                 VacancyId = o.VacancyId,
                                 JobseekerId = o.JobseekerId,
                                 CreationDate = o.CreationDate
                             }
                         ).ToListAsync();
        }

        /// <summary>
        /// Retrieves all offers related to a specified vacancy.
        /// </summary>
        /// <param name="id">The unique identifier of the vacancy.</param>
        /// <returns>A list of offers with associated jobseeker details for the specified vacancy.</returns>
        public async Task<List<OfferWithJobseekerDto>> GetAllByVacancyIdAsync(Guid id)
        {
            return await _dbContext
            .Offers
            .Where(o => o.VacancyId == id)
            .Select(
                o => new OfferWithJobseekerDto
                {
                    JobseekerCompactDto = new JobseekerCompactSearchResultDto
                    {
                        UserName = o.Jobseeker.AppUser.UserName ?? string.Empty,
                        Profession = o.Jobseeker.Profession ?? string.Empty,
                        Experience = o.Jobseeker.Experience,
                        Education = o.Jobseeker.Education.ToString(),
                        Location = o.Jobseeker.Location ?? string.Empty,
                        IsEmployed = o.Jobseeker.IsEmployed,
                        AppUserId = o.Jobseeker.AppUserId ?? string.Empty
                    },
                    CompanyMessage = o.CompanyMessage,
                    JobseekerResponse = o.JobSeekerResponse,
                    Status = o.Status.ToString(),
                    VacancyId = o.VacancyId,
                    JobseekerId = o.JobseekerId,
                    CreationDate = o.CreationDate
                }
            ).ToListAsync();
        }

        /// <summary>
        /// Retrieves an offer by a composite key consisting of a jobseeker ID and a vacancy ID.
        /// </summary>
        /// <param name="jobseekerId">The ID of the jobseeker.</param>
        /// <param name="vacancyId">The ID of the vacancy.</param>
        /// <returns>The offer if the composite key is valid; otherwise, null.</returns>
        public async Task<Offer?> GetByCompositeKeyAsync(Guid jobseekerId, Guid vacancyId)
        {
            return await _dbContext.Offers
            .FirstOrDefaultAsync(o => o.JobseekerId == jobseekerId && o.VacancyId == vacancyId);
        }

        /// <summary>
        /// Updates the offer in the database.
        /// </summary>
        /// <param name="offer">The offer to update.</param>
        /// <returns>The updated offer.</returns>
        public async Task<Offer> UpdateAsync(Offer offer)
        {
            _dbContext.Update(offer);
            await _dbContext.SaveChangesAsync();
            return offer;
        }
    }
}