using FluentAssertions;
using Moq;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Infrastructure.Features.TextProcessing;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
{

    // OrthoHelper.Infrastructure.Tests/OrthoEngineAdapterTests.cs
    /// <summary>
    /// Contains unit tests for the OrthoEngineAdapter.
    /// The purpose of these tests is to ensure that the adapter correctly
    /// delegates calls to the IOrthoEngine instance it wraps.
    /// </summary>
    public class OrthoEngineAdapterTests
    {
        /// <summary>
        /// Function: ProcessTextAsync
        /// Scenario: When the adapter's method is called.
        /// Expected Result: It should call the corresponding method on the wrapped engine and return its result.
        /// </summary>
        [Fact]
        public async Task ProcessTextAsync_WhenCalled_ShouldReturnResultFromWrappedEngine()
        {
            // ARRANGE
            // 1. Create a mock of the underlying engine that the adapter will wrap.
            var mockEngine = new Mock<IOrthoEngine>();

            // 2. Configure the mock engine to return a specific output ("TEST")
            //    when its ProcessTextAsync method is called with a specific input ("test").
            mockEngine.Setup(e => e.ProcessTextAsync("test")).ReturnsAsync("TEST");

            // 3. Create an instance of the adapter, injecting the mock engine.
            var adapter = new OrthoEngineAdapter(mockEngine.Object);

            // ACT
            // Call the method on the adapter.
            var result = await adapter.ProcessTextAsync("test");

            // ASSERT
            // 1. Verify that the result returned by the adapter is the same as the one
            //    configured in the mock engine.
            result.Should().Be("TEST");

            // 2. Verify that the adapter actually called the ProcessTextAsync method on the
            //    wrapped engine exactly one time with the correct input.
            //    This is crucial for testing the adapter pattern.
            mockEngine.Verify(e => e.ProcessTextAsync("test"), Times.Once);
        }
    }
}