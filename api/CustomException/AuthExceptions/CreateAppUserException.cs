using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException.AuthExceptions
{
    public class CreateAppUserException : Exception
    {
        public CreateAppUserException() { }
        public CreateAppUserException(string message) : base(message) { }
    }
}