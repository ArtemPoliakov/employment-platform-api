using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.OfferDtos;
using api.Enums;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /// <summary>
    /// Controller for offer operations
    /// </summary>
    [ApiController]
    [Route("api/offer")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly IVacancyRepository _vacancyRepository;
        private readonly UserManager<AppUser> _userManager;
        public OfferController(IOfferRepository offerRepository,
                                        IJobseekerRepository jobseekerRepository,
                                        IVacancyRepository vacancyRepository,
                                        UserManager<AppUser> userManager)
        {
            _offerRepository = offerRepository;
            _jobseekerRepository = jobseekerRepository;
            _vacancyRepository = vacancyRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new offer for a jobseeker and a specified vacancy.
        /// </summary>
        /// <param name="createOfferDto">The data transfer object containing offer information.</param>
        /// <returns>A success message if the offer is created successfully, or an error message if the request is invalid, the user is not found, or unauthorized.</returns>
        /// <response code="400">If the request is invalid, company user or jobseeker user is not found, or no jobseeker data is registered for the target user.</response>
        /// <response code="401">If the user is unauthorized to create the offer.</response>
        /// <response code="200">If the offer is created successfully.</response>
        [HttpPost("create")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> Create([FromBody] CreateOfferDto createOfferDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var companyAppUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (companyAppUser == null)
                return BadRequest("Company user not found");

            var vacancy = await _vacancyRepository.GetByIdAsync(createOfferDto.VacancyId, true);
            if (vacancy == null)
                return BadRequest("Vacancy not found");

            if (vacancy.Company.AppUserId != companyAppUser.Id)
                return Unauthorized("You are not authorized to create this offer");


            var targetJobseekerAppUser = await _userManager.FindByNameAsync(createOfferDto.JobseekerUsername);
            if (targetJobseekerAppUser == null)
                return BadRequest("Jobseeker user not found");


            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(targetJobseekerAppUser.Id);
            if (jobseeker == null)
                return BadRequest("No jobseeker data registered for the target user");


            var offer = new Offer
            {
                VacancyId = createOfferDto.VacancyId,
                JobseekerId = jobseeker.Id
            };
            await _offerRepository.CreateAsync(offer);
            return Ok("Created successfully");
        }

        /// <summary>
        /// Retrieves all offers made to a jobseeker.
        /// The request query should contain the jobseeker username.
        /// Only the username owner may get the data.
        /// Dto contains offer-specific data and vacancy data.
        /// </summary>
        /// <returns>A list of offer dtos, or an error message if the user is not found, jobseeker data does not exist, or the request is invalid.</returns>
        /// <response code="400">If the request is invalid, user is not found, or jobseeker data does not exist</response>
        /// <response code="401">If the user is unauthorized to get this data.</response>
        /// <response code="200">If the offers are retrieved successfully.</response>
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

            var offers = await _offerRepository.GetAllByJobseekerIdAsync(jobseeker.Id, true);
            return Ok(offers);
        }

        /// <summary>
        /// Retrieves all offers related to a specified vacancy.
        /// The request query should contain the vacancy id.
        /// Only the vacancy owner may get the data.
        /// Dto contains offer-specific data and jobseeker data.
        /// </summary>
        /// <returns>A list of offer dtos, or an error message if the user is not found, vacancy data does not exist, or the request is invalid.</returns>
        /// <response code="400">If the request is invalid, user is not found, or vacancy data does not exist</response>
        /// <response code="401">If the user is unauthorized to get this data.</response>
        /// <response code="200">If the offers are retrieved successfully.</response>
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

            var offers = await _offerRepository.GetAllByVacancyIdAsync(vacancyId);
            return Ok(offers);
        }

        /// <summary>
        /// Updates the jobseeker's reaction to a specific offer, including response message and status
        /// </summary>
        /// <param name="reactionDto">The data transfer object containing jobseeker's reaction information.</param>
        /// <returns>A success message if the reaction is updated successfully, or an error message if the request is invalid, the user is not found, unauthorized, or offer does not exist.</returns>
        /// <response code="400">If the request is invalid, user is not found, or offer does not exist</response>
        /// <response code="401">If the user is unauthorized to update the offer.</response>
        /// <response code="200">If the jobseeker's reaction is updated successfully.</response>
        [HttpPut("setJobseekerReaction")]
        [Authorize(Roles = "JOBSEEKER")]
        public async Task<IActionResult> SetJobseekerReaction([FromBody] SetJobseekerReactionDto reactionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }

            var jobseeker = await _jobseekerRepository.GetJobseekerByUserIdAsync(appUser.Id);
            if (jobseeker == null)
            {
                return BadRequest("No jobseeker data registered for the account");
            }

            if (jobseeker.Id != reactionDto.JobseekerId)
            {
                return Unauthorized("You are not authorized to update this offer");
            }

            var offer = await _offerRepository.GetByCompositeKeyAsync(reactionDto.JobseekerId, reactionDto.VacancyId);
            if (offer == null)
            {
                return BadRequest("Offer not found");
            }

            offer.Status = reactionDto.Status;
            offer.JobSeekerResponse = reactionDto.JobseekerResponse;
            await _offerRepository.UpdateAsync(offer);
            return Ok("Updated successfully");
        }

        /// <summary>
        /// Edits the company message for a specific offer.
        /// </summary>
        /// <param name="editCompanyMessageDto">The data transfer object containing the jobseeker ID, vacancy ID, and the new company message.</param>
        /// <returns>A success message if the company message is updated successfully, or an error message if the request is invalid, user is not found, unauthorized, or the offer does not exist.</returns>
        /// <response code="400">If the request is invalid, user is not found, vacancy is not found, or offer does not exist.</response>
        /// <response code="401">If the user is unauthorized to update the offer.</response>
        /// <response code="200">If the company message is updated successfully.</response>
        [HttpPut("editCompanyMessage")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> EditCompanyMessage([FromBody] EditCompanyMessageDto editCompanyMessageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }

            var vacancy = await _vacancyRepository.GetByIdAsync(editCompanyMessageDto.VacancyId, true);
            if (vacancy == null)
            {
                return BadRequest("Vacancy not found");
            }

            if (appUser.Id != vacancy.Company.AppUserId)
            {
                return Unauthorized("You are not authorized to update this offer");
            }

            var offer = await _offerRepository.GetByCompositeKeyAsync(editCompanyMessageDto.JobseekerId, editCompanyMessageDto.VacancyId);
            if (offer == null)
            {
                return BadRequest("Offer not found");
            }

            offer.CompanyMessage = editCompanyMessageDto.CompanyMessage;
            await _offerRepository.UpdateAsync(offer);
            return Ok("Updated successfully");
        }

        /// <summary>
        /// Deletes a specified offer related to a vacancy and jobseeker.
        /// Only the company which created the offer may delete it
        /// </summary>
        /// <param name="vacancyId">The id of the vacancy associated with the offer.</param>
        /// <param name="jobseekerId">The id of the jobseeker associated with the offer.</param>
        /// <returns>NoContent if the offer is deleted successfully, or an error message if the request is invalid, user is not found, unauthorized, or vacancy does not exist.</returns>
        /// <response code="400">If the request is invalid, user is not found, or vacancy does not exist</response>
        /// <response code="401">If the user is unauthorized to delete the offer.</response>
        /// <response code="204">If the offer is deleted successfully.</response>
        [HttpDelete("delete")]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> Delete([FromQuery] Guid vacancyId, Guid jobseekerId)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return BadRequest("User not found");
            }
            var vacancy = await _vacancyRepository.GetByIdAsync(vacancyId, true);
            if (vacancy == null)
            {
                return BadRequest("Vacancy not found");
            }

            if (appUser.Id != vacancy.Company.AppUserId)
            {
                return Unauthorized("You are not authorized to delete this offer");
            }

            await _offerRepository.DeleteAsync(jobseekerId, vacancyId);
            return NoContent();
        }
    }
}