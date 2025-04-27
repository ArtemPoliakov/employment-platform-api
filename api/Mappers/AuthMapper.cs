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
        public static AppUser toAppUser(this RegisterDto registerDto)
        {

            return new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
            };
        }

        public static UserPublicDataDto ToAppUserPublicDataDto(this AppUser appUser, string role)
        {
            return new UserPublicDataDto
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