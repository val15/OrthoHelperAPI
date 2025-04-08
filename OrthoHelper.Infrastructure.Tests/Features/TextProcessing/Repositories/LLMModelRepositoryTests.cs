using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class LLMModelRepositoryTests
    {
        private readonly ILLMModelRepository _llmModelRepository;
        public LLMModelRepositoryTests()
        {
            // Setup DbContext
            _llmModelRepository = new LLMModelRepository();
        }

        [Fact]
        public async Task GetUserByUsername_Should_Return_Correct_User()
        {
            // Arrange
            //var expectedUser = new User { Username = "testuser", PasswordHash = "hash" };
            //_context.Users.Add(expectedUser);
            //await _context.SaveChangesAsync();

            // Act
            var result = await _llmModelRepository.GetAvailableLLMModelsAsync();

            // Assert
            //result.Should().BeEquivalentTo(expectedUser);
            result.Should().NotHaveCount(0);
        }
    }
}
