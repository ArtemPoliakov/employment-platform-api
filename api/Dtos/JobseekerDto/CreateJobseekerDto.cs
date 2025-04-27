using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobseekerDto
{
    /// <summary>
    /// Dto for jobseeker creation
    /// </summary>
    public class CreateJobseekerDto
    {
        [MinLength(3)]
        [MaxLength(40)]
        [Required]
        public string Profession { get; set; } = string.Empty;

        [Range(0, 100)]
        [Required]
        public float Experience { get; set; } = 0;

        [Required]
        public DegreeTypes Education { get; set; } = DegreeTypes.NONE;

        [MinLength(3)]
        [MaxLength(60)]
        [Required]
        public string Location { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(100)]
        [Required]
        public string PreviousWorkplace { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(60)]
        [Required]
        public string PreviousPosition { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(200)]
        [Required]
        public string QuitReason { get; set; } = string.Empty;
        [MinLength(3)]
        [MaxLength(100)]
        [Required]
        public string FamilyConditions { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(200)]
        [Required]
        public string LivingConditions { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Preferences { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(600)]
        [Required]
        public string SelfDescription { get; set; } = string.Empty;

        [Required]
        public bool IsEmployed { get; set; }
    }
}