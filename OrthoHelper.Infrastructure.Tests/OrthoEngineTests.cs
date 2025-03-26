using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OrthoHelper.Infrastructure.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Tests
{
    public class OrthoEngineTests
    {
        private readonly HttpClient _client;

        public OrthoEngineTests()
        {
            _client =   new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };
        }


        [Theory]
        [InlineData("Je veut un café", "Je veux un café.")]
        [InlineData("sa va", "Ça va.")]
        public void ProcessTextAsync_WithCommonErrors_ReturnsCorrectedText(string input, string expected)
        {
            // Arrange
            var engine = new OrthoEngine(_client);

            // Act
            var result = engine.ProcessTextAsync(input).Result; // .Result car la méthode est async

            // Assert
            Assert.True(result.Contains(expected));
        }
    }
}
