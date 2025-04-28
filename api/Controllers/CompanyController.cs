using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.CompanyDtos;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /// <summary>
    /// Controller for company operations
    /// </summary>
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<AppUser> _userManager;

        public CompanyController(ICompanyRepository companyRepository,
                                 UserManager<AppUser> userManager
                                )
        {
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates company data for the authorized user.
        /// Returns the created company data.
        /// </summary>
        /// <param name="createCompanyDto">Company data to be created</param>
        /// <returns>A response containing the created company data, or an error message if the request is invalid, user is not found, or company data already exists</returns>
        /// <response code="400">If the request is invalid, user is not found, or company data already exists</response>
        /// <response code="200">If the company data is created successfully</response>
        [HttpPost("create")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> CreateCompany(CreateCompanyDto createCompanyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }
            if (await _companyRepository.CompanyExistsByUserIdAsync(appUser.Id))
            {
                return BadRequest("Company data for this user already exists");
            }
            var company = createCompanyDto.CreateCompanyDtoToCompany();
            company.AppUserId = appUser.Id;

            var newCompany = await _companyRepository.CreateAsync(company);
            return Ok(newCompany.ToCompanyDto(appUser.UserName ?? "none"));
        }

        /// <summary>
        /// Updates company data for the authorized user.
        /// Determines target user by JWT token.
        /// </summary>
        /// <param name="editCompanyDto">The new company data (only those props you need)</param>
        /// <returns>A response containing the updated company data, or an error message if the user is not found or the company data does not exist</returns>
        /// <response code="400">If the user is not found or the company data does not exist</response>
        /// <response code="200">If the company data is updated successfully</response>

        [HttpPost("edit")]
        [Authorize]
        public async Task<IActionResult> EditCompany(EditCompanyDto editCompanyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return BadRequest("User not found");

            var company = await _companyRepository.GetCompanyByUserIdAsync(appUser.Id);
            if (company == null) return BadRequest("Company data does not exist");

            CompanyMapper.MapChangesToCompany(company, editCompanyDto);
            var editedCompany = await _companyRepository.UpdateAsync(company);

            return Ok(editedCompany.ToCompanyDto(appUser.UserName ?? "none"));
        }

        /// <summary>
        /// Retrieves company data for the specified user.
        /// </summary>
        /// <param name="userName">The username of the user whose company data is being retrieved.</param>
        /// <returns>An Ok response with company data if successful, BadRequest if the user does not exist, or if the company data does not exist.</returns>
        /// <response code="400">If the user is not found or the company data does not exist</response>
        /// <response code="200">If the company data is retrieved successfully</response>
        [HttpGet("getAllData/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetCompanyDataByUsername([FromRoute] string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null) return BadRequest("User not found");

            var role = _userManager.GetRolesAsync(appUser).Result.FirstOrDefault() ?? "none";
            var company = await _companyRepository.GetCompanyByUserIdAsync(appUser.Id);
            if (company == null) return BadRequest("Company data does not exist");

            return Ok(CompanyMapper.ToGetFullCompanyDataDto(company, appUser, role));
        }
    }
}