
using Xunit;
using FluentAssertions;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Tests.Features.Auth.Entities
{
    /// <summary>
    /// Contains unit tests for the User domain entity, focusing on creation,
    /// validation, and property assignment.
    /// </summary>
    public class UserTests
    {
        /// <summary>
        /// Function: User.Create (Factory)
        /// Scenario: When creating a user with valid arguments.
        /// Expected Result: Should assign properties correctly.
        /// </summary>
        [Fact]
        public void Create_WithValidArguments_ShouldAssignPropertiesCorrectly()
        {
            // ARRANGE
            // Define the valid inputs for the user.
            var username = "testuser";
            var passwordHash = "hashedpassword123";

            // ACT
            // Call the factory method to create the user instance.
            var user = User.Create(username, passwordHash);

            // ASSERT
            // Verify that the properties were assigned the correct values.
            user.Username.Should().Be(username);
            user.PasswordHash.Should().Be(passwordHash);
        }

        /// <summary>
        /// Function: User.Create (Factory)
        /// Scenario: When creating a user with an invalid username (either empty or too short).
        /// Expected Result: Should throw an InvalidUserNameException.
        /// </summary>
        [Theory]
        [InlineData("", "Username cannot be empty.")] // Test case for empty username
        [InlineData("te", "Username must contain at least 3 characters.")] // Test case for short username
        public void Create_WithInvalidUsername_ShouldThrowInvalidUserNameException(string invalidUsername, string expectedErrorMessage)
        {
            // ARRANGE
            // Define the action that attempts to create a user with an invalid username.
            Action act = () => User.Create(invalidUsername, "some_valid_hash");

            // ACT & ASSERT
            // Verify that the action throws the specific exception with the expected message.
            act.Should().Throw<InvalidUserNameException>()
               .WithMessage(expectedErrorMessage);
        }

        /// <summary>
        /// Function: User Properties (Setters)
        /// Scenario: When setting properties on a User instance.
        /// Expected Result: Should hold the assigned values correctly.
        /// </summary>
        [Theory]
        [InlineData(1, "user1", "hash1")]
        [InlineData(2, "user2", "hash2")]
        public void Properties_SetValues_ShouldHoldCorrectData(int id, string username, string passwordHash)
        {
            // ARRANGE
            // Create a new User instance using the default constructor.
            var user = new User
            {
                Id = id,
                Username = username,
                PasswordHash = passwordHash
            };

            // ACT & ASSERT
            // Verify that each property correctly holds the value it was assigned.
            user.Id.Should().Be(id);
            user.Username.Should().Be(username);
            user.PasswordHash.Should().Be(passwordHash);
        }
    }
}


