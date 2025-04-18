using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Admin;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IJobseekerElasticService _jobseekerElasticService;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(IJobseekerRepository jobseekerRepository,
                               IJobseekerElasticService jobseekerElasticService,
                               UserManager<AppUser> userManager)
        {
            _jobseekerRepository = jobseekerRepository;
            _jobseekerElasticService = jobseekerElasticService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("reindex")]
        public async Task<IActionResult> ReindexElastic()
        {
            await _jobseekerElasticService.CreateIndexIfNotExistsAsync();
            var jobseekers = await _jobseekerRepository.GetAllJobseekersAsync();
            var jobseekerElasticDtos = jobseekers.Select(js => js.ToJobseekerElasticDto()).ToList();
            var result = await _jobseekerElasticService.AddOrUpdateJobseekerBulkAsync(jobseekerElasticDtos);
            return Ok($"Jobseekers reindexed: {result}");
        }

        [HttpPost("bulkJobseekers")]
        public async Task<IActionResult> BulkJobseekers([FromBody] List<ImmediateJobseekerCreateDto> jobseekerDtos)
        {
            List<string> resultMsgs = [];
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var dto in jobseekerDtos)
            {
                string resultMsg = "";
                var appUserModel = dto.RegisterDto.toAppUser();
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
                if (await _jobseekerRepository.JobseekerExistsByUserId(appUser.Id))
                {
                    return BadRequest("Jobseeker data for this user already exists");
                }

                var jobseeker = dto.CreateJobseekerDto.ToJobseeker();
                jobseeker.AppUserId = appUser.Id;
                var createdJobseeker = await _jobseekerRepository.CreateAsync(jobseeker);
                resultMsg += "\nJobseeker data added successfully!";

                resultMsgs.Add(resultMsg);
            }
            return Ok(resultMsgs);
        }
    }
}