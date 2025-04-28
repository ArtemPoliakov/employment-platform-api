using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Models;

namespace api.Mappers
{
    /// <summary>
    /// Mapper for transfering AppUser to Auth-related Dtos and vice versa
    /// </summary>
    public static class AuthMapper
    {
        /// <summary>
        /// Converts RegisterDto to AppUser
        /// </summary>
        /// <param name="registerDto">Data for registering</param>
        /// <returns>AppUser</returns>
        public static AppUser ToAppUser(this RegisterDto registerDto)
        {

            return new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
            };
        }

        /// <summary>
        /// Converts AppUser to UserPublicDataDto (public data about user, including role)
        /// </summary>
        /// <param name="appUser">AppUser model to be converted</param>
        /// <param name="role">Role of the user (e.g. "Jobseeker" or "Company")</param>
        /// <returns>UserPublicDataDto</returns>
        public static AppUserPublicDataDto ToAppUserPublicDataDto(this AppUser appUser, string role)
        {
            return new AppUserPublicDataDto
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Role = role
            };
        }
    }
}