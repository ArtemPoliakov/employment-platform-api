using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    /// <summary>
    /// User authentication model
    /// </summary>
    public class AppUser : IdentityUser<Guid>
    {

    }
}