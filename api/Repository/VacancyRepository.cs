using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.CustomException.VacancyExceptions;
using api.Data;
using api.Dtos.VacancyDtos;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Service
{
    public class VacancyRepository : IVacancyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IVacancyElasticService _vacancyElasticService;
        public VacancyRepository(ApplicationDbContext dbContext,
                                 IVacancyElasticService vacancyElasticService)
        {
            _dbContext = dbContext;
            _vacancyElasticService = vacancyElasticService;
        }

        public async Task<Vacancy> CreateAsync(Vacancy vacancy)
        {
            await _dbContext.Vacancies.AddAsync(vacancy);
            await _dbContext.SaveChangesAsync();
            bool result = await _vacancyElasticService.AddOrUpdateVacancyAsync(vacancy.ToVacancyElasticDto());
            if (!result)
            {
                throw new VacancyElasticException("Failed to add vacancy to elastic");
            }
            return vacancy;
        }

        public async Task<bool> DeleteAsync(Vacancy vacancy)
        {
            _dbContext.Vacancies.Remove(vacancy);
            await _dbContext.SaveChangesAsync();
            bool result = await _vacancyElasticService.DeleteVacancyByIdAsync(vacancy.Id.ToString());
            if (!result)
            {
                throw new VacancyElasticException("Failed to delete vacancy from elastic");
            }
            return true;
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await _dbContext.Vacancies.AnyAsync(v => v.Id == id);
        }

        public async Task<List<Vacancy>> GetAllByCompanyIdAsync(Guid id)
        {
            var vacancies = await _dbContext.Vacancies.Where(v => v.Company.Id == id).ToListAsync();
            return vacancies;
        }

        public async Task<List<Vacancy>> GetAllVacanciesAsync()
        {
            var vacancies = await _dbContext.Vacancies.ToListAsync();
            return vacancies;
        }

        public async Task<List<Vacancy>> GetAllVacanciesByIdsAsync(List<Guid> ids)
        {
            return await _dbContext
            .Vacancies
            .Where(v => ids.Contains(v.Id))
            .Include(v => v.Company)
            .ThenInclude(c => c.AppUser)
            .ToListAsync();
        }

        public async Task<List<VacancyDto>> GetAllVacancyDtosByIdsForRequesterAsync(List<Guid> ids, Guid requesterId)
        {
            return await _dbContext
            .Vacancies
            .Where(v => ids.Contains(v.Id))
            .Select(GetVacancyDtoSelector(requesterId))
            .ToListAsync();
        }


        public async Task<Vacancy?> GetByIdAsync(Guid id, bool includeCompany = false)
        {
            var vacancyQuery = _dbContext.Vacancies.AsQueryable();
            if (includeCompany)
            {
                vacancyQuery = vacancyQuery.Include(v => v.Company);
            }
            return await vacancyQuery.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<VacancyDto>> GetRecentVacanciesAsync(int page, int pageSize, Guid requesterId)
        {
            return await _dbContext
                .Vacancies
                .OrderByDescending(v => v.PublishDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(GetVacancyDtoSelector(requesterId))
                .ToListAsync();
        }

        public async Task<List<VacancyDto>> SearchByQueryAsync(VacancyQueryDto query, Guid requesterId)
        {
            var searchResults = await _vacancyElasticService.SearchVacanciesByQueryAsync(query);

            var idDictionary = new Dictionary<Guid, int>();
            for (int i = 0; i < searchResults?.Count; i++)
            {
                idDictionary.Add(searchResults[i].Id, i);
            }

            var vacancies = await GetAllVacancyDtosByIdsForRequesterAsync(idDictionary.Keys.ToList(), requesterId);
            vacancies = vacancies.OrderBy(v => idDictionary[v.Id]).ToList();

            return vacancies;
        }

        public async Task<Vacancy> UpdateAsync(Vacancy vacancy)
        {
            _dbContext.Update(vacancy);
            await _dbContext.SaveChangesAsync();
            var result = await _vacancyElasticService.AddOrUpdateVacancyAsync(vacancy.ToVacancyElasticDto());
            if (!result)
            {
                throw new VacancyElasticException("Failed to update vacancy in elastic");
            }
            return vacancy;
        }

        public async Task<VacancyDto?> GetVacancyDtoByIdAsync(Guid vacancyId, Guid requesterId)
        {
            var vacancyQuery = _dbContext.Vacancies.AsQueryable();

            return await vacancyQuery
            .Where(v => v.Id == vacancyId)
            .Select(GetVacancyDtoSelector(requesterId))
            .FirstOrDefaultAsync();
        }

        public static Expression<Func<Vacancy, VacancyDto>> GetVacancyDtoSelector(Guid requesterId)
        {
            return v => new VacancyDto
            {
                Id = v.Id,
                CompanyUserName = v.Company.AppUser.UserName ?? "NONE",
                Title = v.Title,
                Description = v.Description,
                CandidateDescription = v.CandidateDescription,
                Position = v.Position,
                SalaryMin = v.SalaryMin,
                SalaryMax = v.SalaryMax,
                WorkMode = v.WorkMode.ToString(),
                LivingConditions = v.LivingConditions,
                EditDate = v.EditDate,
                PublishDate = v.PublishDate,
                ApplicationStatus = v.JobApplications
                    .Where(a => a.JobseekerId == requesterId)
                    .Select(a => a.Status.ToString())
                    .FirstOrDefault() ?? "NONE",
                OfferStatus = v.Offers
                    .Where(o => o.JobseekerId == requesterId)
                    .Select(o => o.Status.ToString())
                    .FirstOrDefault() ?? "NONE",
            };
        }

    }
}