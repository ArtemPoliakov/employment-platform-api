using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.CustomException.companyExceptions;
using api.CustomException.VacancyExceptions;
using api.Data;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IVacancyElasticService _vacancyElasticService;
        public CompanyRepository(ApplicationDbContext dbContext, IVacancyElasticService vacancyElasticService)
        {
            _dbContext = dbContext;
            _vacancyElasticService = vacancyElasticService;
        }


        /// <summary>
        /// Checks whether a company with given user id exists.
        /// </summary>
        /// <param name="userId">User id to be checked</param>
        /// <returns>True if the company exists, false otherwise</returns>
        public async Task<bool> CompanyExistsByUserIdAsync(string userId)
        {
            return await _dbContext.Companies.AnyAsync(c => c.AppUserId.Equals(userId));
        }

        public async Task<Company> CreateAsync(Company company)
        {
            if (company == null) throw new NullCompanyException("Created company can't be null");

            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }


        /// <summary>
        /// Adds a list of companies to the database in bulk and puts their associated vacancies in ElasticSearch.
        /// </summary>
        /// <param name="companies">The list of companies to be added.</param>
        /// <returns>The number of companies successfully added to the database.</returns>
        /// <exception cref="VacancyElasticException">Thrown when updating vacancies in ElasticSearch fails.</exception>
        public async Task<long> CreateBulkAsync(List<Company> companies)
        {
            await _dbContext.Companies.AddRangeAsync(companies);
            int writtenEntriesCount = await _dbContext.SaveChangesAsync();
            foreach (var company in companies)
            {
                var result = await _vacancyElasticService
                .AddOrUpdateVacancyBulkAsync(company.Vacancies.Select(v => v.ToVacancyElasticDto()));
                if (!result)
                {
                    throw new VacancyElasticException("Failed to add vacancies to elastic");
                }

            }
            return writtenEntriesCount;
        }

        /// <summary>
        /// Creates a default company for a user.
        /// The default company has description and location set to "none".
        /// </summary>
        /// <param name="userId">The user id to associate with the default company.</param>
        /// <returns>The created default company.</returns>
        public async Task<Company> CreateDefaultCompanyAsync(string userId)
        {
            var defaultCompany = new Company
            {
                AppUserId = userId,
                SelfDescription = "none",
                Location = "none",
            };
            return await CreateAsync(defaultCompany);
        }


        /// <summary>
        /// Retrieves a company by its user id from the database.
        /// </summary>
        /// <param name="userId">The user id of the company to be retrieved</param>
        /// <param name="includeVacancies">Whether to include related vacancies in the result</param>
        /// <returns>The retrieved company, or null if none is found</returns>
        public async Task<Company?> GetCompanyByUserIdAsync(string userId, bool includeVacancies = false)
        {
            var companyQuery = _dbContext.Companies.AsQueryable();
            if (includeVacancies)
            {
                companyQuery = companyQuery.Include(c => c.Vacancies);
            }
            return await companyQuery.FirstOrDefaultAsync(c => c.AppUserId.Equals(userId));
        }

        /// <summary>
        /// Updates a company in the database.
        /// </summary>
        /// <param name="company">The company to be updated</param>
        /// <returns>The updated company</returns>
        /// <exception cref="NullCompanyException">If the company is null</exception>
        public async Task<Company> UpdateAsync(Company company)
        {
            if (company == null) throw new NullCompanyException("Updated company can't be null");

            _dbContext.Companies.Update(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }
    }
}