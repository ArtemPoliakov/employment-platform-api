using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobseekerDto
{
    public class GetFullJobseekerDataDto
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Profession { get; set; } = string.Empty;

        public float Experience { get; set; } = 0;

        public string Education { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string PreviousWorkplace { get; set; } = string.Empty;

        public string PreviousPosition { get; set; } = string.Empty;

        public string QuitReason { get; set; } = string.Empty;

        public string FamilyConditions { get; set; } = string.Empty;

        public string LivingConditions { get; set; } = string.Empty;

        public string Preferences { get; set; } = string.Empty;

        public string SelfDescription { get; set; } = string.Empty;

        public bool IsEmployed { get; set; }
    }
}