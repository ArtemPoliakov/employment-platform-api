using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobseekerDto
{
    /// <summary>
    /// Dto for jobseeker editing
    /// </summary>
    public class EditJobseekerDto
    {
        [MinLength(3)]
        [MaxLength(40)]
        public string? Profession { get; set; }

        [Range(0, 100)]
        public float? Experience { get; set; }

        public DegreeTypes? Education { get; set; }

        [MinLength(3)]
        [MaxLength(60)]
        public string? Location { get; set; }

        [MinLength(3)]
        [MaxLength(100)]
        public string? PreviousWorkplace { get; set; }

        [MinLength(3)]
        [MaxLength(60)]
        public string? PreviousPosition { get; set; }

        [MinLength(3)]
        [MaxLength(200)]
        public string? QuitReason { get; set; }

        [MinLength(3)]
        [MaxLength(100)]
        public string? FamilyConditions { get; set; }

        [MinLength(3)]
        [MaxLength(200)]
        public string? LivingConditions { get; set; }

        [MinLength(3)]
        [MaxLength(300)]
        public string? Preferences { get; set; }

        [MinLength(3)]
        [MaxLength(600)]
        public string? SelfDescription { get; set; }

        public bool? IsEmployed { get; set; }
    }
}