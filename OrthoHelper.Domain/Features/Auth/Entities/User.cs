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
                throw new InvalideUserNameException("userName ne peut pas être vide.");

            if (userName.Length<3)
                throw new InvalideUserNameException("userName doit au moin contenir 3 carracaires.");


            return new User
            {
                Username = userName,
                PasswordHash = passwordHash
            };

        }

    }
}
