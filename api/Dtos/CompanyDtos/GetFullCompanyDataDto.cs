using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.CompanyDtos
{
    /// <summary>
    /// Dto for getting full company data including public account details
    /// </summary>
    public class GetFullCompanyDataDto
    {
        public AppUserPublicDataDto AppUserPublicData { get; set; }
        public CompanyDto CompanyData { get; set; }
    }
}