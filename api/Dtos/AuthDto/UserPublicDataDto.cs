using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    /// <summary>
    /// Dto for user public data retrieval
    /// </summary>
    public class AppUserPublicDataDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
    }
}