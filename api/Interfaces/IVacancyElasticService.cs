using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos.ElasticDtos;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    /// <summary>
    /// Facilitates ElasticSearch operations for vacancies
    /// </summary>
    public interface IVacancyElasticService
    {
        Task CreateIndexIfNotExistsAsync();

        Task<bool> AddOrUpdateVacancyAsync(VacancyElasticDto vacancy);

        Task<bool> AddOrUpdateVacancyBulkAsync(IEnumerable<VacancyElasticDto> vacancies);

        Task<VacancyElasticDto?> GetVacancyAsync(string key);

        Task<List<VacancyElasticDto>?> GetAllVacanciesAsync();

        Task<bool> DeleteVacancyByIdAsync(string key);

        Task<long?> RemoveAllAsync();

        Task<List<VacancyElasticDto>?> SearchVacanciesByQueryAsync(VacancyQueryDto query);
    }
}