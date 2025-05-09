using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos
{
    /// <summary>
    /// Dto for user registration
    /// </summary>
    public class RegisterDto
    {
        [Required]
        public AllowedSafeRoles SafeRole { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in international format like +123456789012")]
        public string PhoneNumber { get; set; }
    }
}