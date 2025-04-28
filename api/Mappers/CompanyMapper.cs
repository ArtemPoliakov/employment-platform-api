using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.CompanyDtos;
using api.Models;

namespace api.Mappers
{
    /// <summary>
    /// Mapper for transfering Company to Company-related Dtos and vice versa
    /// </summary>
    public static class CompanyMapper
    {

        /// <summary>
        /// Converts CreateCompanyDto to Company
        /// </summary>
        /// <param name="createCompanyDto">Company data to be created</param>
        /// <returns>Company model</returns>
        public static Company CreateCompanyDtoToCompany(this CreateCompanyDto createCompanyDto)
        {
            return new Company
            {
                SelfDescription = createCompanyDto.SelfDescription,
                Location = createCompanyDto.Location
            };
        }


        /// <summary>
        /// Converts a Company model to a CompanyDto.
        /// </summary>
        /// <param name="company">The Company model to convert.</param>
        /// <param name="userName">The user name associated with the Company.</param>
        /// <returns>A CompanyDto containing the converted data.</returns>
        public static CompanyDto ToCompanyDto(this Company company, string userName)
        {
            return new CompanyDto
            {
                UserName = userName,
                AppUserId = company.AppUserId,
                SelfDescription = company.SelfDescription,
                Location = company.Location,
                RegisterDate = company.RegisterDate
            };
        }

        /// <summary>
        /// Maps data from EditCompanyDto to Company. Null values in EditCompanyDto are ignored.
        /// </summary>
        /// <param name="company">Company model to be updated</param>
        /// <param name="editCompanyDto">Dto with new data</param>
        public static void MapChangesToCompany(Company company, EditCompanyDto editCompanyDto)
        {
            company.SelfDescription = editCompanyDto.SelfDescription ?? company.SelfDescription;
            company.Location = editCompanyDto.Location ?? company.Location;
        }

        /// <summary>
        /// Converts Company and AppUser to GetFullCompanyDataDto (full company data, including public account data)
        /// </summary>
        /// <param name="company">Company model</param>
        /// <param name="appUser">Related AppUser model</param>
        /// <param name="role">Role of the user (e.g. "Jobseeker" or "Company")</param>
        /// <returns>GetFullCompanyDataDto</returns>
        public static GetFullCompanyDataDto ToGetFullCompanyDataDto(this Company company, AppUser appUser, string role)
        {
            return new GetFullCompanyDataDto
            {
                AppUserPublicData = appUser.ToAppUserPublicDataDto(role),
                CompanyData = company.ToCompanyDto(appUser.UserName ?? "none")
            };
        }
    }
}