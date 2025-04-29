using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.CustomException;
using api.CustomException.AuthExceptions;
using api.Data;
using api.Dtos.Admin;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /// <summary>
    /// Controller for admin actions
    /// </summary>
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IJobseekerElasticService _jobseekerElasticService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IVacancyRepository _vacancyRepository;
        private readonly IVacancyElasticService _vacancyElasticService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public AdminController(IJobseekerRepository jobseekerRepository,
                               IJobseekerElasticService jobseekerElasticService,
                               ICompanyRepository companyRepository,
                               IVacancyRepository vacancyRepository,
                               IVacancyElasticService vacancyElasticService,
                               UserManager<AppUser> userManager,
                               ApplicationDbContext applicationDbContext)
        {
            _jobseekerRepository = jobseekerRepository;
            _jobseekerElasticService = jobseekerElasticService;
            _companyRepository = companyRepository;
            _vacancyRepository = vacancyRepository;
            _vacancyElasticService = vacancyElasticService;
            _userManager = userManager;
            _dbContext = applicationDbContext;
        }

        /// <summary>
        /// Reindexes all jobseekers in ElasticSearch.
        /// </summary>
        /// <returns>A message indicating the number of jobseekers reindexed</returns>
        /// <response code="200">If the reindexing is successful</response>
        [HttpPost]
        [Route("reindex")]
        public async Task<IActionResult> ReindexElastic()
        {
            await _jobseekerElasticService.CreateIndexIfNotExistsAsync();
            var jobseekers = await _jobseekerRepository.GetAllJobseekersAsync();
            var jobseekerElasticDtos = jobseekers.Select(js => js.ToJobseekerElasticDto()).ToList();
            var jobseekerResult = await _jobseekerElasticService.AddOrUpdateJobseekerBulkAsync(jobseekerElasticDtos);

            await _vacancyElasticService.CreateIndexIfNotExistsAsync();
            var vacancies = await _vacancyRepository.GetAllVacanciesAsync();
            var vacancyElasticDtos = vacancies.Select(v => v.ToVacancyElasticDto()).ToList();
            var vacancyResult = await _vacancyElasticService.AddOrUpdateVacancyBulkAsync(vacancyElasticDtos);

            return Ok($"""
                       Jobseekers reindexed: {jobseekerResult}
                       Vacancies reindexed: {vacancyResult}
                       """);
        }

        /// <summary>
        /// Creates jobseeker data for multiple users.
        /// Returns a list of success messages, one for each jobseeker.
        /// </summary>
        /// <param name="jobseekerDtos">List of jobseeker data to be created</param>
        /// <returns>A list of success messages, one for each jobseeker</returns>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If the registration or adding to role fails</response>
        /// <response code="200">If the jobseekers are created successfully</response>
        [HttpPost("bulkJobseekers")]
        public async Task<IActionResult> BulkJobseekers([FromBody] List<ImmediateJobseekerCreateDto> jobseekerDtos)
        {
            List<string> resultMsgs = [];
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var dto in jobseekerDtos)
            {
                string resultMsg = "";
                var appUserModel = dto.RegisterDto.ToAppUser();
                var createdUser = await _userManager.CreateAsync(appUserModel, dto.RegisterDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUserModel, dto.RegisterDto.SafeRole.ToString());
                    if (roleResult.Succeeded)
                    {
                        resultMsg += $"User {dto.RegisterDto.UserName} registered successfully! ";
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(400, createdUser.Errors);
                }


                var username = dto.RegisterDto.UserName;
                var appUser = await _userManager.FindByNameAsync(username);
                if (appUser == null)
                {
                    return BadRequest("User not found");
                }
                if (await _jobseekerRepository.JobseekerExistsByUserIdAsync(appUser.Id))
                {
                    return BadRequest("Jobseeker data for this user already exists");
                }

                var jobseeker = dto.CreateJobseekerDto.ToJobseeker();
                jobseeker.AppUserId = appUser.Id;
                var createdJobseeker = await _jobseekerRepository.CreateAsync(jobseeker);
                resultMsg += "Jobseeker data added successfully!";

                resultMsgs.Add(resultMsg);
                bool isElsaticOperationSuccess = await _jobseekerElasticService
                .AddOrUpdateJobseekerAsync(createdJobseeker.ToJobseekerElasticDto());

                if (!isElsaticOperationSuccess)
                {
                    throw new JobseekerElasticException("Failed to add jobseeker to elastic");
                }
            }

            return Ok(resultMsgs);
        }

        /// <summary>
        /// Clears all jobseekers from ElasticSearch.
        /// </summary>
        /// <returns>A message indicating the number of jobseekers deleted from ElasticSearch</returns>
        /// <response code="200">If the deletion is successful</response>
        [HttpDelete("clearAllJobseekersFromElastic")]
        public async Task<IActionResult> ClearJobseekersFromElastic()
        {
            var deletedCount = await _jobseekerElasticService.RemoveAllAsync();
            return Ok($"{deletedCount} jobseekers deleted from elastic");
        }

        /// <summary>
        /// Adds a list of companies to the database in bulk.
        /// </summary>
        /// <param name="companyBulkDtos">The list of companies to be added.</param>
        /// <returns>The number of companies successfully added to the database.</returns>
        /// <response code="400">If the request is invalid</response>
        /// <response code="200">If the companies are added successfully</response>
        [HttpPost("bulkCompanies")]
        public async Task<IActionResult> BulkCompanies([FromBody] List<CompanyBulkDto> companyBulkDtos)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                List<string> newUserIds = [];

                foreach (var bulkDto in companyBulkDtos)
                {
                    var newAppUser = bulkDto.RegisterDto.ToAppUser();
                    var result = await _userManager.CreateAsync(newAppUser, bulkDto.RegisterDto.Password);
                    if (result.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(newAppUser, bulkDto.RegisterDto.SafeRole.ToString());
                        if (!roleResult.Succeeded) throw new AssignToRoleException($"Failed to add user to role: {roleResult.Errors}");
                    }
                    else
                    {
                        var errorMessages = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList();
                        throw new CreateAppUserException("Failed to create user:\n" + string.Join("\n", errorMessages));
                    }
                    newUserIds.Add(newAppUser.Id);
                }

                var companyModels = companyBulkDtos
                                    .Select(dto => dto.CompanyBulkDtoToCompany())
                                    .ToList();
                if (companyModels.Count != newUserIds.Count)
                    throw new InvalidOperationException("Mismatch between user IDs and company models.");

                for (int i = 0; i < newUserIds.Count; i++)
                {
                    companyModels[i].AppUserId = newUserIds[i];
                }
                var addedCompaniesCount = await _companyRepository.CreateBulkAsync(companyModels);

                await transaction.CommitAsync();
                return Ok(addedCompaniesCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("clearAllVacanciesFromElastic")]
        public async Task<IActionResult> DeleteAllVacanciesFromElastic()
        {
            var result = await _vacancyElasticService.RemoveAllAsync();
            return Ok($"Vacancies deleted from elastic: {result}");
        }
    }
}