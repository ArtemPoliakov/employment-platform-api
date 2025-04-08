using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITockenService _tockenService;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, ITockenService tockenService)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tockenService = tockenService;
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
                            Tocken = await _tockenService.CreateTocken(appUser)
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
                return StatusCode(500, createdUser.Errors);
            }
        }
    }
}