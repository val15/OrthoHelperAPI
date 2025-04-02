using MediatR;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;

namespace OrthoHelper.Application.Features.Auth.Commands.RegisterUser
{
   
        public record RegisterUserCommand(string Username, string Password) : IRequest<RegisterUserResponse>;
        public record RegisterUserResponse(int Id, string Username);


    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, RegisterUserResponse> // <-- Signature complète
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Ajouter le CancellationToken dans les paramètres
        public async Task<RegisterUserResponse> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken) // <-- Paramètre manquant
        {
            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var createdUser = await _userRepository.CreateUser(user);
            return new RegisterUserResponse(createdUser.Id, createdUser.Username);
        }
    }
}
