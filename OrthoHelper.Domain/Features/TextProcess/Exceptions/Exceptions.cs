using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextCorrection.Exceptions
{
    public class InvalidTextException : Exception
    {
        public InvalidTextException(string message) : base(message) { }

    }
    public class InvalidModelNameException : Exception
    {
        public InvalidModelNameException(string message) : base(message) { }

    }

    public class UserNotFoundException : Exception
    {


        public UserNotFoundException(string message) : base(message) { }
    }
}
