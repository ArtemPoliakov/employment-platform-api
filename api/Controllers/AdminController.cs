using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IJobseekerElasticService _jobseekerElasticService;
        public AdminController(IJobseekerRepository jobseekerRepository,
                               IJobseekerElasticService jobseekerElasticService)
        {
            _jobseekerRepository = jobseekerRepository;
            _jobseekerElasticService = jobseekerElasticService;
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
    }
}