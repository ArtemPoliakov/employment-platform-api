using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using api.CustomException;
using api.Data;
using api.Dtos.JobseekerDto;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/jobseeker")]
    [ApiController]
    public class JobseekerController : ControllerBase
    {
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IJobseekerElasticService _jobseekerElasticService;
        private readonly UserManager<AppUser> _userManager;
        public JobseekerController(IJobseekerRepository jobseekerRepository,
                                   UserManager<AppUser> userManager,
                                   IJobseekerElasticService jobseekerElasticService)
        {
            _jobseekerRepository = jobseekerRepository;
            _jobseekerElasticService = jobseekerElasticService;
            _userManager = userManager;
        }


        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateJobseeker(CreateJobseekerDto createJobseekerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }
            if (await _jobseekerRepository.JobseekerExistsByUserId(appUser.Id))
            {
                return BadRequest("Jobseeker data for this user already exists");
            }

            var jobseeker = createJobseekerDto.ToJobseeker();
            jobseeker.AppUserId = appUser.Id;
            var createdJobseeker = await _jobseekerRepository.CreateAsync(jobseeker);

            bool isElasticSuccess = await _jobseekerElasticService
                                    .AddOrUpdateJobseekerAsync(createdJobseeker.ToJobseekerElasticDto());
            if (!isElasticSuccess)
            {
                throw new JobseekerElasticException("Failed to add jobseeker to elastic");
            }
            return Ok(createdJobseeker.ToJobseekerDto());
        }

        [HttpGet("getAllData/{username}")]
        [Authorize]
        public async Task<IActionResult> GetAllDataByUsername([FromRoute] string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }
            var role = _userManager.GetRolesAsync(appUser).Result.First();
            var jobseeker = await _jobseekerRepository.GetJobseekerByUserId(appUser.Id);

            return Ok(JobseekerMapper.ToGetFullJobseekerDataDto(jobseeker, appUser, role));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditJobseeker([FromBody] EditJobseekerDto updateJobseekerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return BadRequest("User not found");

            var jobseeker = await _jobseekerRepository.GetJobseekerByUserId(appUser.Id);
            if (jobseeker == null) return BadRequest("Jobseeker data does not exist");

            JobseekerMapper.MapChangesToJobseeker(jobseeker, updateJobseekerDto);
            var editedJobseeker = await _jobseekerRepository.UpdateAsync(jobseeker);

            bool isElasticSuccess = await _jobseekerElasticService
                                    .AddOrUpdateJobseekerAsync(editedJobseeker.ToJobseekerElasticDto());
            if (!isElasticSuccess)
            {
                throw new JobseekerElasticException("Failed to update jobseeker in elastic");
            }
            return Ok(editedJobseeker.ToJobseekerDto());
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchJobseekers([FromQuery] JobseekerQueryDto query)
        {
            var jobseekersDocs = await _jobseekerElasticService.SearchJobseekersByQueryAsync(query);
            var idDictionary = new Dictionary<string, int>();
            for (int i = 0; i < jobseekersDocs.Count; i++)
            {
                idDictionary.Add(jobseekersDocs[i].Id, i);
            }

            var jobseekers = await _jobseekerRepository.GetJobseekersByUserIdsAsync(idDictionary.Keys.ToList());
            jobseekers = jobseekers.OrderBy(js => idDictionary[js.AppUserId]).ToList();

            return Ok(jobseekers.Select(js => js.ToJobseekerDto()).ToList());  // CHANGE DTO TYPE!!!!
        }
    }
}