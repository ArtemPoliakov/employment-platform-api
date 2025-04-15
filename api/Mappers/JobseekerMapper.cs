using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobseekerDto;
using api.Dtos.JobseekerDto.ElasticDtos;
using api.Models;

namespace api.Mappers
{
    public static class JobseekerMapper
    {
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

        public static JobseekerDto ToJobseekerDto(this Jobseeker jobseeker)
        {
            return new JobseekerDto
            {
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
                IsEmployed = jobseeker.IsEmployed
            };
        }

        public static GetFullJobseekerDataDto ToGetFullJobseekerDataDto(this Jobseeker? jobseeker, AppUser appUser, string role)
        {
            return new GetFullJobseekerDataDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                Phone = appUser.PhoneNumber,
                Role = role,
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
                IsEmployed = jobseeker.IsEmployed
            };
        }
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
    }
}