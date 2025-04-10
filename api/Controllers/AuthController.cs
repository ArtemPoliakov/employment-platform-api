using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dtos;
using api.Enums;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITokenService _tokenService;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = registerDto.toAppUser();
            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, registerDto.SafeRole.ToString());
                if (roleResult.Succeeded)
                {
                    return
                    Ok(
                        new NewUserDto
                        {
                            Email = appUser.Email,
                            UserName = appUser.UserName,
                            PhoneNumber = appUser.PhoneNumber,
                            Role = registerDto.SafeRole.ToString().ToUpper(),
                            Token = await _tokenService.CreateToken(appUser)
                        }
                    );
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
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (appUser == null)
                return NotFound("User not found");

            var result = await _signinManager.CheckPasswordSignInAsync(appUser, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid username and/or password");
            var roles = await _userManager.GetRolesAsync(appUser);
            return Ok(
                new NewUserDto
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,
                    PhoneNumber = appUser.PhoneNumber,
                    Role = roles.First(),
                    Token = await _tokenService.CreateToken(appUser)
                }
            );
        }

        [HttpGet("data")]
        [Authorize]
        public async Task<IActionResult> GetAccountData(string UserName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == UserName.ToLower());
            if (appUser == null)
                return NotFound("User not found");
            var roles = await _userManager.GetRolesAsync(appUser);
            return Ok(appUser.ToAppUserPublicDataDto(roles.First()));
        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditAccountData([FromBody] UpdateAccountDataDto updateAccountDataDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != updateAccountDataDto.Id.ToString())
                return Unauthorized("You are not authorized to update this user");

            var appUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == updateAccountDataDto.Id.ToString());
            if (appUser == null)
                return NotFound("User not found");
            var roles = await _userManager.GetRolesAsync(appUser);

            if (updateAccountDataDto.UserName != null) appUser.UserName = updateAccountDataDto.UserName;
            if (updateAccountDataDto.Email != null) appUser.Email = updateAccountDataDto.Email;
            if (updateAccountDataDto.PhoneNumber != null) appUser.PhoneNumber = updateAccountDataDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
                return Ok(appUser.ToAppUserPublicDataDto(roles.First()));

            return StatusCode(500, result.Errors);
        }

        [HttpPut("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = await _userManager.FindByNameAsync(changePasswordDto.UserName);
            if (appUser == null)
                return NotFound("User not found");

            var result = await _userManager
            .ChangePasswordAsync(appUser, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                return Ok(
                        new NewUserDto
                        {
                            Email = appUser.Email,
                            UserName = appUser.UserName,
                            PhoneNumber = appUser.PhoneNumber,
                            Role = roles.First(),
                            Token = await _tokenService.CreateToken(appUser)
                        }
                    );
            }
            return BadRequest("Wrong pasword!");
        }
    }
}