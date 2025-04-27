using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    /// <summary>
    /// Dto for updating user account data
    /// </summary>
    public class UpdateAccountDataDto
    {
        [Required]
        public string Id { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [MinLength(3)]
        [MaxLength(30)]
        public string? UserName { get; set; }
        [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in international format like +123456789012")]
        public string? PhoneNumber { get; set; }
    }
}