using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for token service. Creates JWT tockens.
    /// </summary>
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser);
    }
}