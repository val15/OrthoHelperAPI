
using Xunit;
using FluentAssertions;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Tests.Features.Auth.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_Created_Should_Have_Correct_Properties()
        {
            // Arrange
            var user = User.Create("testuser", "hashedpassword123");

            // Act & Assert
            user.Username.Should().Be("testuser");
            user.PasswordHash.Should().Be("hashedpassword123");
        }

        [Fact]
        public void Create_WithEmptyUserName_ShouldThrowArgumentException()
        {

            // Act & Assert
            Action act = () => User.Create("", "hashedpassword123");
            act.Should().Throw<InvalideUserNameException>()
                .WithMessage("userName ne peut pas être vide.");
        }

        [Fact]
        public void Create_WithInvalideUserName_ShouldThrowArgumentException()
        {

            // Act & Assert
            Action act = () => User.Create("te", "hashedpassword123");
            act.Should().Throw<InvalideUserNameException>()
                .WithMessage("userName doit au moin contenir 3 carracaires.");
        }

        [Fact]
        public void User_Should_Have_Correct_Properties()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "hashedpassword123"
            };

            // Act & Assert
            user.Id.Should().Be(1);
            user.Username.Should().Be("testuser");
            user.PasswordHash.Should().Be("hashedpassword123");
        }

        [Theory]
        [InlineData(1, "user1", "hash1")]
        [InlineData(2, "user2", "hash2")]
        [InlineData(3, "user3", "hash3")]
        public void User_Should_Handle_Different_Values_Correctly(
            int id, string username, string passwordHash)
        {
            // Arrange
            var user = new User
            {
                Id = id,
                Username = username,
                PasswordHash = passwordHash
            };

            // Act & Assert
            user.Id.Should().Be(id);
            user.Username.Should().Be(username);
            user.PasswordHash.Should().Be(passwordHash);
        }

        [Fact]
        public void User_Properties_Should_Be_Modifiable()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "olduser",
                PasswordHash = "oldhash"
            };

            // Act
            user.Id = 2;
            user.Username = "newuser";
            user.PasswordHash = "newhash";

            // Assert
            user.Id.Should().Be(2);
            user.Username.Should().Be("newuser");
            user.PasswordHash.Should().Be("newhash");
        }
    }
}


