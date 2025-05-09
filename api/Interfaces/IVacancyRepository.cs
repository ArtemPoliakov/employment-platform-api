using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    /// <summary>
    /// Facilitated db operations for vacancies
    /// </summary>
    public interface IVacancyRepository
    {
        Task<Vacancy> CreateAsync(Vacancy vacancy);
        Task<Vacancy> UpdateAsync(Vacancy vacancy);
        Task<Vacancy?> GetByIdAsync(Guid id, bool includeCompany = false);
        Task<List<Vacancy>> GetAllByCompanyIdAsync(Guid id);
        Task<List<Vacancy>> GetAllVacanciesByIdsAsync(List<Guid> ids);
        Task<List<Vacancy>> SearchByQueryAsync(VacancyQueryDto query);
        Task<bool> DeleteAsync(Vacancy vacancy);
        Task<List<Vacancy>> GetAllVacanciesAsync();
        Task<bool> ExistsByIdAsync(Guid id);
        Task<List<Vacancy>> GetRecentVacanciesAsync(int page, int pageSize);
    }
}