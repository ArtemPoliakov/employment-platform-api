using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos;
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
        Task<List<VacancyDto>> GetAllVacancyDtosByIdsForRequesterAsync(List<Guid> ids, Guid requesterId);
        Task<List<VacancyDto>> SearchByQueryAsync(VacancyQueryDto query, Guid requesterId);
        Task<bool> DeleteAsync(Vacancy vacancy);
        Task<List<Vacancy>> GetAllVacanciesAsync();
        Task<bool> ExistsByIdAsync(Guid id);
        Task<List<VacancyDto>> GetRecentVacanciesAsync(int page, int pageSize, Guid requesterId);
        Task<VacancyDto?> GetVacancyDtoByIdAsync(Guid vacancyId, Guid requesterId);
    }
}