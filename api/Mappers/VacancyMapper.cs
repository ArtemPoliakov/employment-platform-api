using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos;
using api.Dtos.VacancyDtos.ElasticDtos;
using api.Models;

namespace api.Mappers
{
    /// <summary>
    /// Mapper for transfering Vacancy to Vacancy-related Dtos and vice versa
    /// </summary>
    public static class VacancyMapper
    {
        /// <summary>
        /// Converts the given <see cref="Vacancy"/> to a <see cref="VacancyElasticDto"/>.
        /// </summary>
        /// <param name="vacancy">The <see cref="Vacancy"/> to convert.</param>
        /// <returns>The converted <see cref="VacancyElasticDto"/>.</returns>
        public static VacancyElasticDto ToVacancyElasticDto(this Vacancy vacancy)
        {
            return new VacancyElasticDto
            {
                Id = vacancy.Id,
                Position = vacancy.Position,
                MinSalary = vacancy.SalaryMin,
                MaxSalary = vacancy.SalaryMax,
                WorkMode = vacancy.WorkMode.ToString(),
                Title = vacancy.Title,
                Description = vacancy.Description
            };
        }

        /// <summary>
        /// Converts a <see cref="Vacancy"/> to a <see cref="VacancyDto"/>,
        /// adding the company user name.
        /// </summary>
        /// <param name="vacancy">The <see cref="Vacancy"/> to convert.</param>
        /// <param name="companyUserName">The user name of the company.</param>
        /// <returns>The converted <see cref="VacancyDto"/>.</returns>
        public static VacancyDto ToVacancyDto(this Vacancy vacancy, string companyUserName)
        {
            return new VacancyDto
            {
                CompanyUserName = companyUserName,
                Id = vacancy.Id,
                Title = vacancy.Title,
                Description = vacancy.Description,
                CandidateDescription = vacancy.CandidateDescription,
                Position = vacancy.Position,
                SalaryMin = vacancy.SalaryMin,
                SalaryMax = vacancy.SalaryMax,
                WorkMode = vacancy.WorkMode.ToString(),
                LivingConditions = vacancy.LivingConditions,
                EditDate = vacancy.EditDate,
                PublishDate = vacancy.PublishDate
            };
        }


        /// <summary>
        /// Converts a <see cref="CreateVacancyDto"/> to a <see cref="Vacancy"/>.
        /// </summary>
        /// <param name="dto">The <see cref="CreateVacancyDto"/> to convert.</param>
        /// <returns>The converted <see cref="Vacancy"/>.</returns>
        public static Vacancy ToVacancy(this CreateVacancyDto dto)
        {
            return new Vacancy
            {
                Title = dto.Title,
                Description = dto.Description,
                CandidateDescription = dto.CandidateDescription,
                Position = dto.Position,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax,
                WorkMode = dto.WorkMode,
                LivingConditions = dto.LivingConditions
            };
        }

        /// <summary>
        /// Maps data from EditVacancyDto to Vacancy. Null values in EditVacancyDto are ignored.
        /// </summary>
        /// <param name="vacancy">The Vacancy model to be updated.</param>
        /// <param name="editVacancyDto">The Dto with new data.</param>
        public static void MapChangesToVacancy(Vacancy vacancy, EditVacancyDto editVacancyDto)
        {
            vacancy.Title = editVacancyDto.Title ?? vacancy.Title;
            vacancy.Description = editVacancyDto.Description ?? vacancy.Description;
            vacancy.CandidateDescription = editVacancyDto.CandidateDescription ?? vacancy.CandidateDescription;
            vacancy.Position = editVacancyDto.Position ?? vacancy.Position;
            vacancy.SalaryMin = editVacancyDto.SalaryMin ?? vacancy.SalaryMin;
            vacancy.SalaryMax = editVacancyDto.SalaryMax ?? vacancy.SalaryMax;
            vacancy.WorkMode = editVacancyDto.WorkMode ?? vacancy.WorkMode;
            vacancy.LivingConditions = editVacancyDto.LivingConditions ?? vacancy.LivingConditions;
            vacancy.EditDate = DateTime.Now;
        }


        /// <summary>
        /// Maps data from Vacancy to VacancyCompactDto.
        /// </summary>
        /// <param name="vacancy">The Vacancy model to be mapped.</param>
        /// <param name="companyUserName">The user name of the company.</param>
        /// <returns>The mapped VacancyCompactDto.</returns>
        public static VacancyCompactDto ToVacancyCompactDto(this Vacancy vacancy, string companyUserName)
        {
            return new VacancyCompactDto
            {
                CompanyUserName = companyUserName,
                Title = vacancy.Title,
                Description = vacancy.Description,
                Position = vacancy.Position,
                SalaryMin = vacancy.SalaryMin,
                SalaryMax = vacancy.SalaryMax,
                WorkMode = vacancy.WorkMode.ToString(),
                Id = vacancy.Id
            };
        }
    }
}