using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    using System.ComponentModel.DataAnnotations;
    using api.Enums;

    public class EditJobseekerDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Profession { get; set; } = string.Empty;

        [Required]
        public float Experience { get; set; } = 0;

        [Required]
        public DegreeTypes Education { get; set; } = DegreeTypes.NONE;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string PreviousWorkplace { get; set; } = string.Empty;

        [Required]
        public string PreviousPosition { get; set; } = string.Empty;

        [Required]
        public string QuitReason { get; set; } = string.Empty;

        [Required]
        public string FamilyConditions { get; set; } = string.Empty;

        [Required]
        public string LivingConditions { get; set; } = string.Empty;

        [Required]
        public string Preferences { get; set; } = string.Empty;

        [Required]
        public string SelfDescription { get; set; } = string.Empty;

        [Required]
        public bool IsEmployed { get; set; }

        [Required]
        public DateTime MyProperty { get; } = DateTime.Now.Date;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}