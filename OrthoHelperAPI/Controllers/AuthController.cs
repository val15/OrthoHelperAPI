using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrthoHelper.Application.Features.Auth.Commands.RegisterUser;
using OrthoHelper.Application.Features.Auth.DTOs;
using OrthoHelper.Application.Features.Auth.Queries.LoginUser;

namespace OrthoHelperAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {

            _mediator = mediator;
        }


        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        //{
        //    var user = new User
        //    {
        //        Username = dto.Username,
        //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        //    };
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return Ok("Utilisateur créé");
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto dto)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        //    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //        return Unauthorized();

        //    var token = GenerateJwtToken(user);
        //    return Ok(new { token });
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var command = new RegisterUserCommand(dto.Username, dto.Password);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var query = new LoginUserQuery(dto.Username, dto.Password);
            var result = await _mediator.Send(query);
            return result is not null ? Ok(result) : NotFound();
        }

        //private string GenerateJwtToken(User user)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name, user.Username)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(1),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }



  
}
