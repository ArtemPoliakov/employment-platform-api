using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/jobseeker")]
    [ApiController]
    public class JobseekerController : ControllerBase
    {
        public JobseekerController() { }

        [HttpGet("test_endpoint")]
        public async Task<IActionResult> TestEndpoint()
        {
            return Ok("Jobseeker API is working!");
        }
    }
}