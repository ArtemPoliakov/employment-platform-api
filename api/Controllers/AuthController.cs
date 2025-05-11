using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.CustomException;
using api.CustomException.AuthExceptions;
using api.Data;
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
    /// <summary>
    /// Controller for auth operations
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly IJobseekerRepository _jobseekerRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ApplicationDbContext _dbContext;

        public AuthController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signinManager,
                              ITokenService tokenService,
                              IJobseekerRepository jobseekerRepository,
                              ICompanyRepository companyRepository,
                              ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            _jobseekerRepository = jobseekerRepository;
            _companyRepository = companyRepository;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Registers a new user and creates default data for jobseeker or company based on the role.
        /// </summary>
        /// <param name="registerDto">Data for registering the user</param>
        /// <returns>Ok with user data and token on success, BadRequest on invalid model state,
        /// StatusCode 400 with errors on user creation failure, StatusCode 500 with errors on role assignment failure</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var appUser = registerDto.ToAppUser();
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, registerDto.SafeRole.ToString());
                    if (roleResult.Succeeded)
                    {
                        if (registerDto.SafeRole == AllowedSafeRoles.Jobseeker)
                        {
                            await _jobseekerRepository.CreateDefaultJobseekerAsync(appUser.Id);
                        }
                        else if (registerDto.SafeRole == AllowedSafeRoles.Company)
                        {
                            await _companyRepository.CreateDefaultCompanyAsync(appUser.Id);
                        }

                        await transaction.CommitAsync();

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
                        throw new AssignToRoleException(string.Join(", ", roleResult.Errors.Select(x => x.Description)));
                    }
                }
                else
                {
                    throw new CreateAppUserException(string.Join(", ", createdUser.Errors.Select(x => x.Description)));
                }
            }
            catch (CreateAppUserException e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="loginDto">Data for logging in</param>
        /// <returns>Ok with user data and token on success, BadRequest on invalid model state,
        /// NotFound if user doesn't exist, Unauthorized if password is invalid</returns>
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
                    Role = roles.FirstOrDefault() ?? "none",
                    Token = await _tokenService.CreateToken(appUser)
                }
            );
        }

        /// <summary>
        /// Retrieves account data for the specified user.
        /// </summary>
        /// <param name="UserName">The username of the user whose account data is being retrieved.</param>
        /// <returns>An Ok response with user public data if successful, BadRequest if the model state is invalid, or NotFound if the user does not exist.</returns>
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

        /// <summary>
        /// Updates user account data.
        /// </summary>
        /// <param name="updateAccountDataDto">Data for updating the user account.</param>
        /// <returns>Ok with updated user public data on success, BadRequest if the model state is invalid,
        /// Unauthorized if the user is not authorized to update the user, NotFound if the user doesn't exist,
        /// or StatusCode 500 if the update fails</returns>
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

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="changePasswordDto">Data containing the username, old password, and new password.</param>
        /// <returns>An Ok response with updated user data and token if successful, BadRequest if the model state is invalid
        /// or the password change fails, NotFound if the user does not exist.</returns>

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