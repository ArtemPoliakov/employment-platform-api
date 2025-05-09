using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> CreateAsync(Company company);
        Task<long> CreateBulkAsync(List<Company> companies);
        Task<Company> UpdateAsync(Company company);
        Task<bool> CompanyExistsByUserIdAsync(string userId);
        Task<Company?> GetCompanyByUserIdAsync(string userId, bool includeVacancies = false);
    }
}