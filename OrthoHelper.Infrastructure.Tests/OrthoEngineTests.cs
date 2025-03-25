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
        [Theory]
        [InlineData("Je veut un café", "Je veux un café.")]
        [InlineData("sa va", "Ça va.")]
        public void ProcessTextAsync_WithCommonErrors_ReturnsCorrectedText(string input, string expected)
        {
            // Arrange
            var engine = new OrthoEngine();

            // Act
            var result = engine.ProcessTextAsync(input).Result; // .Result car la méthode est async

            // Assert
            Assert.True(result.Contains(expected));
        }
    }
}
