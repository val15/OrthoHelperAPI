using MediatR;
using OrthoHelper.Domain.Features.Auth.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.Auth.Queries.LoginUser
{
    public record LoginUserQuery(string Username, string Password) : IRequest<LoginUserResponse>;
    public record LoginUserResponse(string Token);
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginUserQueryHandler(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResponse?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsername(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var token = _tokenService.GenerateToken(claims);
            return new LoginUserResponse(token);
        }

    }
}




