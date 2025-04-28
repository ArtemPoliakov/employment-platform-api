using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.CompanyDtos
{
    /// <summary>
    /// Dto for company editing. All props are optional.
    /// </summary>
    public class EditCompanyDto
    {
        [MinLength(3)]
        [MaxLength(1000)]
        public string? SelfDescription { get; set; }

        [MinLength(3)]
        [MaxLength(60)]
        public string? Location { get; set; }
    }
}