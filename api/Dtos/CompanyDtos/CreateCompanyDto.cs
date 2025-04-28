using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.CompanyDtos
{
    /// <summary>
    /// Dto for company creation
    /// </summary>
    public class CreateCompanyDto
    {
        [MinLength(3)]
        [MaxLength(1000)]
        [Required]
        public string SelfDescription { get; set; } = string.Empty;

        [MinLength(3)]
        [MaxLength(60)]
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}