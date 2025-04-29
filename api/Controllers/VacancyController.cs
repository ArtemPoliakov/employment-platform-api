using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /// <summary>
    /// Controller for vacancy operations
    /// </summary>
    [Route("api/vacancy")]
    [ApiController]
    public class VacancyController : ControllerBase
    {
        private readonly IVacancyRepository _vacancyRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<AppUser> _userManager;
        public VacancyController(IVacancyRepository vacancyRepository,
                                 ICompanyRepository companyRepository,
                                 UserManager<AppUser> userManager)
        {
            _vacancyRepository = vacancyRepository;
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new vacancy.
        /// The request body should contain a <see cref="CreateVacancyDto"/>.
        /// </summary>
        /// <returns>The created vacancy, or an error message if the request is invalid, user is not found, or company data does not exist</returns>
        /// <response code="400">If the request is invalid, user is not found, or company data does not exist</response>
        /// <response code="200">If the vacancy is created successfully</response>
        [HttpPost("create")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyDto createVacancyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return BadRequest("User not found");

            var company = await _companyRepository.GetCompanyByUserIdAsync(appUser.Id);
            if (company == null) return BadRequest("Company data does not exist");

            var vacancy = createVacancyDto.ToVacancy();
            vacancy.CompanyId = company.Id;

            var createdVacancy = await _vacancyRepository.CreateAsync(vacancy);

            return Ok(createdVacancy.ToVacancyDto(appUser.UserName ?? "none"));
        }

        /// <summary>
        /// Retrieves a vacancy by its id.
        /// </summary>
        /// <param name="vacancyId">The id of the vacancy to retrieve.</param>
        /// <returns>The retrieved vacancy, or an error message if the request is invalid, user is not found, or company data does not exist</returns>
        /// <response code="400">If the request is invalid, user is not found, or company data does not exist</response>
        /// <response code="200">If the vacancy is retrieved successfully</response>
        [HttpGet("get/{vacancyId}")]
        [Authorize]
        public async Task<IActionResult> GetVacancyById([FromRoute] Guid vacancyId)
        {
            var vacancy = await _vacancyRepository.GetByIdAsync(vacancyId, true);
            if (vacancy == null) return BadRequest("Vacancy not found");

            var companyAppUser = await _userManager.FindByIdAsync(vacancy.Company.AppUserId);
            if (companyAppUser == null) return BadRequest("Company user not found");

            return Ok(vacancy.ToVacancyDto(companyAppUser.UserName ?? "none"));
        }

        /// <summary>
        /// Retrieves all vacancies for a given company name.
        /// </summary>
        /// <param name="companyName">The name of the company whose vacancies are being retrieved.</param>
        /// <returns>A list of the retrieved vacancies, or an error message if the request is invalid, user is not found, or company data does not exist</returns>
        /// <response code="400">If the request is invalid, user is not found, or company data does not exist</response>
        /// <response code="200">If the vacancies are retrieved successfully</response>
        [HttpGet("getAllByCompanyName/{companyName}")]
        [Authorize]
        public async Task<IActionResult> GetAllVacanciesForCompany([FromRoute] string companyName)
        {
            var companyAppUser = await _userManager.FindByNameAsync(companyName);
            if (companyAppUser == null) return BadRequest("Company user not found");

            var company = await _companyRepository.GetCompanyByUserIdAsync(companyAppUser.Id, true);
            if (company == null) return BadRequest("Company data does not exist");

            return Ok(company.Vacancies
                   .Select(v => v.ToVacancyDto(companyAppUser.UserName ?? "none"))
                   .ToList());
        }

        [HttpDelete("delete/{vacancyId}")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> DeleteVacancy([FromRoute] Guid vacancyId)
        {
            var companyAppUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (companyAppUser == null) return BadRequest("Company user not found");

            var vacancy = await _vacancyRepository.GetByIdAsync(vacancyId, true);
            if (vacancy == null) return BadRequest("Vacancy not found");

            if (vacancy.Company.AppUserId != companyAppUser.Id)
            {
                return Unauthorized("You are not authorized to delete this vacancy");
            }

            await _vacancyRepository.DeleteAsync(vacancy);
            return NoContent();
        }

        [HttpPut("edit/{vacancyId}")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> EditVacancy([FromRoute] Guid vacancyId,
                                                     [FromBody] EditVacancyDto editVacancyDto)
        {
            var companyAppUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (companyAppUser == null) return BadRequest("Company user not found");

            var vacancy = await _vacancyRepository.GetByIdAsync(vacancyId, true);
            if (vacancy == null) return BadRequest("Vacancy not found");

            if (vacancy.Company.AppUserId != companyAppUser.Id)
            {
                return Unauthorized("You are not authorized to edit this vacancy");
            }

            VacancyMapper.MapChangesToVacancy(vacancy, editVacancyDto);
            var updatedVacancy = await _vacancyRepository.UpdateAsync(vacancy);
            return Ok(updatedVacancy.ToVacancyDto(companyAppUser.UserName ?? "none"));
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchByQuery([FromQuery] VacancyQueryDto searchVacancyDto)
        {
            var vacancies = await _vacancyRepository.SearchByQueryAsync(searchVacancyDto);
            return Ok(
            vacancies
            .Select(v => v.ToVacancyCompactDto(v.Company.AppUser.UserName ?? "none"))
            .ToList());
        }
    }
}