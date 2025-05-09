using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobApplicationDtos;
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
    /// Controller for job application operations
    /// </summary>
    [Route("api/jobApplication")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IVacancyRepository _vacancyRepository;
        private readonly UserManager<AppUser> _userManager;
        public JobApplicationController(IJobApplicationRepository jobApplicationRepository,
                                        IJobseekerRepository jobseekerRepository,
                                        IVacancyRepository vacancyRepository,
                                        UserManager<AppUser> userManager)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobseekerRepository = jobseekerRepository;
            _vacancyRepository = vacancyRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new job application.
        /// The request body should contain a <see cref="CreateJobApplicationDto"/>.
        /// </summary>
        /// <returns>The created job application, or an error message if the request is invalid, user is not found, jobseeker data does not exist, or vacancy data does not exist.</returns>
        /// <response code="400">If the request is invalid, user is not found, jobseeker data does not exist, or vacancy data does not exist</response>
        /// <response code="401">If the user is unauthorized to create the job application.</response>
        /// <response code="200">If the job application is created successfully.</response>
        [HttpPost("create")]
        [Authorize(Roles = "JOBSEEKER")]
        public async Task<IActionResult> Create([FromBody] CreateJobApplicationDto createJobApplicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return BadRequest("User not found");
            if (appUser.UserName != createJobApplicationDto.JobseekerUsername)
            {
                return Unauthorized("You are not authorized to create this job application");
            }
            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(appUser.Id);
            if (jobseeker == null)
            {
                return BadRequest("No jobseeker data registered for the account");
            }
            if (!await _vacancyRepository.ExistsByIdAsync(createJobApplicationDto.VacancyId))
            {
                return BadRequest("Vacancy not found");
            }

            var jobApplication = new JobApplication
            {
                VacancyId = createJobApplicationDto.VacancyId,
                JobseekerId = jobseeker.Id
            };
            await _jobApplicationRepository.CreateAsync(jobApplication);
            return Ok("Created successfully");
        }

        /// <summary>
        /// Retrieves all job applications made by a jobseeker.
        /// The request query should contain the jobseeker username.
        /// Only the username owner may get the data.
        /// Dto contains application-specific data and vacancy data.
        /// </summary>
        /// <returns>A list of job application dtos, or an error message if the user is not found, jobseeker data does not exist, or the request is invalid.</returns>
        /// <response code="400">If the request is invalid, user is not found, or jobseeker data does not exist</response>
        /// <response code="401">If the user is unauthorized to get this data.</response>
        /// <response code="200">If the job applications are retrieved successfully.</response>
        [HttpGet("getAllByJobseekerUserName")]
        [Authorize(Roles = "JOBSEEKER")]
        public async Task<IActionResult> GetAllByJobseekerUserName([FromQuery] string jobseekerUsername)
        {
            if (jobseekerUsername != User.GetUsername())
                return Unauthorized("You are not authorized to get this data");

            var appUser = await _userManager.FindByNameAsync(jobseekerUsername);
            if (appUser == null)
                return BadRequest("User not found");
            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(appUser.Id);
            if (jobseeker == null)
                return BadRequest("No jobseeker data registered for the account");

            var jobApplicationDtos = await _jobApplicationRepository.GetAllByJobseekerIdAsync(jobseeker.Id);
            return Ok(jobApplicationDtos);
        }

        /// <summary>
        /// Retrieves all job applications made to a vacancy.
        /// The request query should contain the vacancy id.
        /// Only the vacancy owner may get the data.
        /// Dto contains application-specific data and jobseeker data.
        /// </summary>
        /// <returns>A list of job application dtos, or an error message if the request is invalid, user is not found, or vacancy data does not exist</returns>
        /// <response code="400">If the request is invalid, user is not found, or vacancy data does not exist</response>
        /// <response code="401">If the user is unauthorized to get this data.</response>
        /// <response code="200">If the job applications are retrieved successfully.</response>
        [HttpGet("getAllByVacancyId")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> GetAllByVacancyId([FromQuery] Guid vacancyId)
        {
            var vacancy = await _vacancyRepository.GetByIdAsync(vacancyId, true);
            if (vacancy == null)
                return BadRequest("Vacancy not found");

            var vacancyOwner = await _userManager.FindByIdAsync(vacancy.Company.AppUserId);
            if (vacancyOwner == null)
                return BadRequest("Vacancy owner not found");

            if (vacancyOwner.UserName != User.GetUsername())
                return Unauthorized("You are not authorized to get this data");

            var jobApplicationDtos = await _jobApplicationRepository.GetAllByVacancyIdAsync(vacancyId, true);
            return Ok(jobApplicationDtos);
        }

        /// <summary>
        /// Updates the job application with the given id.
        /// Company-owner of the vacancy may set status and repsonse to the application.
        /// </summary>
        /// <param name="updateJobApplicationDto">The new data for the job application with the id of the application to be updated.</param>
        /// <returns>A response containing the updated job application, or an error message if the request is invalid, user is not found, or job application data does not exist.</returns>
        /// <response code="400">If the request is invalid, user is not found, or job application data does not exist</response>
        /// <response code="401">If the user is unauthorized to update this job application.</response>
        /// <response code="200">If the job application is updated successfully.</response>
        [HttpPut("edit")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> Edit([FromBody] UpdateJobApplicationDto updateJobApplicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return BadRequest("User not found");
            var vacancy = await _vacancyRepository.GetByIdAsync(updateJobApplicationDto.VacancyId, true);
            if (vacancy == null)
            {
                return BadRequest("Vacancy not found");
            }
            if (vacancy.Company.AppUserId != appUser.Id)
            {
                return Unauthorized("You are not authorized to update this job application");
            }

            var jobApplication = await _jobApplicationRepository
            .GetByCompositeKeyAsync(updateJobApplicationDto.JobseekerId,
                                    updateJobApplicationDto.VacancyId);

            if (jobApplication == null)
            {
                return BadRequest("Job application not found");
            }

            JobApplicationMapper.UpdateChangesToJobApplication(jobApplication, updateJobApplicationDto);
            await _jobApplicationRepository.UpdateAsync(jobApplication);
            return Ok("Updated successfully");
        }

        /// <summary>
        /// Deletes the specified job application.
        /// Only the jobseeker who created the application may delete it
        /// </summary>
        /// <param name="jobseekerId">The id of the jobseeker.</param>
        /// <param name="vacancyId">The id of the vacancy.</param>
        /// <returns>NoContent if the operation was successful</returns>
        /// <response code="400">If the request is invalid, user is not found, jobseeker data does not exist or job application does not exist</response>
        /// <response code="204">If the job application is deleted successfully</response>
        [HttpDelete("delete")]
        [Authorize(Roles = "JOBSEEKER")]
        public async Task<IActionResult> Delete([FromQuery] Guid jobseekerId, Guid vacancyId)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return BadRequest("User not found");
            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(appUser.Id);
            if (jobseeker == null)
            {
                return BadRequest("No jobseeker data registered for the account");
            }
            var jobApplication = await _jobApplicationRepository.GetByCompositeKeyAsync(jobseekerId, vacancyId);
            if (jobApplication == null)
            {
                return BadRequest("Job application not found");
            }
            await _jobApplicationRepository.DeleteAsync(jobseekerId, vacancyId);
            return NoContent();
        }
    }
}