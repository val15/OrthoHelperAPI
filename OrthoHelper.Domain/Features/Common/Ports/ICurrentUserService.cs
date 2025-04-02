using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.Common.Ports
{
    public interface ICurrentUserService
    {
        int UserId { get; }

        public string UserName { get; }
        bool IsAuthenticated { get; }
    }
}
