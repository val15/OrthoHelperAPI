using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Exceptions
{
    public class InvalidTextException : Exception
    {
        public InvalidTextException(string message) : base(message) { }
    }
}
