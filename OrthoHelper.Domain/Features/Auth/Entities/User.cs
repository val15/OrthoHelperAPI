using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Features.Auth.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }



        public static User Create(string userName,string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidUserNameException("Username cannot be empty.");

            if (userName.Length<3)
                throw new InvalidUserNameException("Username must contain at least 3 characters.");


            return new User
            {
                Username = userName,
                PasswordHash = passwordHash
            };

        }

    }
}
