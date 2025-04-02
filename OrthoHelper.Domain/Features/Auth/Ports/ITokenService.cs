using System.Security.Claims;

namespace OrthoHelper.Domain.Features.Auth.Ports
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
