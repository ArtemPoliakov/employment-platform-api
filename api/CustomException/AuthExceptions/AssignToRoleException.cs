using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException.AuthExceptions
{
    public class AssignToRoleException : Exception
    {
        public AssignToRoleException() { }
        public AssignToRoleException(string message) : base(message) { }
    }
}