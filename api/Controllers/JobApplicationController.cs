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

        [HttpGet("getAllByJobseekerUserName")]
        [Authorize]
        public async Task<IActionResult> GetAllByJobseekerUserName([FromQuery] string jobseekerUsername)
        {
            var appUser = await _userManager.FindByNameAsync(jobseekerUsername);
            if (appUser == null)
                return BadRequest("User not found");
            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(appUser.Id);
            if (jobseeker == null)
                return BadRequest("No jobseeker data registered for the account");

            var jobApplicationDtos = await _jobApplicationRepository.GetAllByJobseekerIdAsync(jobseeker.Id);
            return Ok(jobApplicationDtos);
        }

        [HttpGet("getAllByVacancyId")]
        [Authorize]
        public async Task<IActionResult> GetAllByVacancyId([FromQuery] Guid vacancyId)
        {
            var jobApplicationDtos = await _jobApplicationRepository.GetAllByVacancyIdAsync(vacancyId, true);
            return Ok(jobApplicationDtos);
        }

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