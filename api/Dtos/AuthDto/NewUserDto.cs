using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    /// <summary>
    /// Dto for new user
    /// </summary>
    public class NewUserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public Guid AccountDataId { get; set; }
        public string Token { get; set; }
    }
}