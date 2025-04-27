using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobseekerDto;
using api.Dtos.JobseekerDto.ElasticDtos;
using api.Models;

namespace api.Mappers
{
    /// <summary>
    /// Mapper for transfering Jobseeker to Jobseeker-related Dtos and vice versa
    /// </summary>
    public static class JobseekerMapper
    {
        /// <summary>
        /// Converts CreateJobseekerDto to Jobseeker
        /// </summary>
        /// <param name="createJobseekerDto">Jobseeker data to be created</param>
        /// <returns>Jobseeker model</returns>
        public static Jobseeker ToJobseeker(this CreateJobseekerDto createJobseekerDto)
        {
            return new Jobseeker
            {
                Profession = createJobseekerDto.Profession,
                Experience = createJobseekerDto.Experience,
                Education = createJobseekerDto.Education,
                Location = createJobseekerDto.Location,
                PreviousWorkplace = createJobseekerDto.PreviousWorkplace,
                PreviousPosition = createJobseekerDto.PreviousPosition,
                QuitReason = createJobseekerDto.QuitReason,
                FamilyConditions = createJobseekerDto.FamilyConditions,
                LivingConditions = createJobseekerDto.LivingConditions,
                Preferences = createJobseekerDto.Preferences,
                SelfDescription = createJobseekerDto.SelfDescription,
                IsEmployed = createJobseekerDto.IsEmployed
            };
        }

        /// <summary>
        /// Converts Jobseeker to JobseekerDto (info to give away)
        /// </summary>
        /// <param name="jobseeker">Jobseeker model</param>
        /// <param name="userName">Related user name</param>
        /// <returns>JobseekerDto</returns>
        public static JobseekerDto ToJobseekerDto(this Jobseeker jobseeker, string userName)
        {
            return new JobseekerDto
            {
                UserName = userName,
                Profession = jobseeker.Profession,
                Experience = jobseeker.Experience,
                Education = jobseeker.Education.ToString(),
                Location = jobseeker.Location,
                PreviousWorkplace = jobseeker.PreviousWorkplace,
                PreviousPosition = jobseeker.PreviousPosition,
                QuitReason = jobseeker.QuitReason,
                FamilyConditions = jobseeker.FamilyConditions,
                LivingConditions = jobseeker.LivingConditions,
                Preferences = jobseeker.Preferences,
                SelfDescription = jobseeker.SelfDescription,
                IsEmployed = jobseeker.IsEmployed,
                RegisterDate = jobseeker.RegisterDate
            };
        }

        /// <summary>
        /// Converts Jobseeker and AppUser to GetFullJobseekerDataDto (full jobseeker data, including public account data)
        /// </summary>
        /// <param name="jobseeker">Jobseeker model</param>
        /// <param name="appUser">Related AppUser model</param>
        /// <param name="role">Role of the user (e.g. "Jobseeker" or "Company")</param>
        /// <returns>GetFullJobseekerDataDto</returns>
        public static GetFullJobseekerDataDto ToGetFullJobseekerDataDto(this Jobseeker jobseeker, AppUser appUser, string role)
        {
            return new GetFullJobseekerDataDto
            {
                AppUserPublicData = appUser.ToAppUserPublicDataDto(role),
                JobseekerData = jobseeker.ToJobseekerDto(appUser.UserName),
            };
        }
        /// <summary>
        /// Maps data from EditJobseekerDto to Jobseeker. Null values in EditJobseekerDto are ignored.
        /// </summary>
        /// <param name="jobseeker">Jobseeker model to be updated</param>
        /// <param name="editJobseekerDto">Jobseeker data to be updated</param>
        public static void MapChangesToJobseeker(Jobseeker jobseeker, EditJobseekerDto editJobseekerDto)
        {
            jobseeker.Profession = editJobseekerDto.Profession ?? jobseeker.Profession;
            jobseeker.Experience = editJobseekerDto.Experience ?? jobseeker.Experience;
            jobseeker.Education = editJobseekerDto.Education ?? jobseeker.Education;
            jobseeker.Location = editJobseekerDto.Location ?? jobseeker.Location;
            jobseeker.PreviousWorkplace = editJobseekerDto.PreviousWorkplace ?? jobseeker.PreviousWorkplace;
            jobseeker.PreviousPosition = editJobseekerDto.PreviousPosition ?? jobseeker.PreviousPosition;
            jobseeker.QuitReason = editJobseekerDto.QuitReason ?? jobseeker.QuitReason;
            jobseeker.FamilyConditions = editJobseekerDto.FamilyConditions ?? jobseeker.FamilyConditions;
            jobseeker.LivingConditions = editJobseekerDto.LivingConditions ?? jobseeker.LivingConditions;
            jobseeker.Preferences = editJobseekerDto.Preferences ?? jobseeker.Preferences;
            jobseeker.SelfDescription = editJobseekerDto.SelfDescription ?? jobseeker.SelfDescription;
            jobseeker.IsEmployed = editJobseekerDto.IsEmployed ?? jobseeker.IsEmployed;
        }

        /// <summary>
        /// Converts Jobseeker to JobseekerElasticDto (dto for ElasticSearch document)
        /// </summary>
        /// <param name="jobseeker">Jobseeker model to be converted</param>
        /// <returns>JobseekerElasticDto</returns>
        public static JobseekerElasticDto ToJobseekerElasticDto(this Jobseeker jobseeker)
        {
            return new JobseekerElasticDto
            {
                Id = jobseeker.AppUserId,
                Profession = jobseeker.Profession,
                Education = jobseeker.Education.ToString().ToLower(),
                Location = jobseeker.Location,
                Experience = jobseeker.Experience
            };
        }

        /// <summary>
        /// Converts Jobseeker to JobseekerCompactSearchResultDto (compact jobseeker data for search results)
        /// </summary>
        /// <param name="jobseeker">Jobseeker model to be converted</param>
        /// <param name="userName">Related user name</param>
        /// <returns>JobseekerCompactSearchResultDto</returns>
        public static JobseekerCompactSearchResultDto ToJobseekerCompactSearchResultDto(this Jobseeker jobseeker, string userName)
        {
            return new JobseekerCompactSearchResultDto
            {
                UserName = userName,
                Profession = jobseeker.Profession,
                Experience = jobseeker.Experience,
                Education = jobseeker.Education,
                Location = jobseeker.Location,
                IsEmployed = jobseeker.IsEmployed,
                AppUserId = jobseeker.AppUserId
            };
        }
    }
}