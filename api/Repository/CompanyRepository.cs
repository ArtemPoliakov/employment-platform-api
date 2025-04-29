using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.CustomException.companyExceptions;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CompanyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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