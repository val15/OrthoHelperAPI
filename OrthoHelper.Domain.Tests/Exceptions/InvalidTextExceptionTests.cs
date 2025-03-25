﻿using OrthoHelper.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Tests.Exceptions
{
    // OrthoHelper.Domain.Tests/Exceptions/InvalidTextExceptionTests.cs
    public class InvalidTextExceptionTests
    {
        [Fact]
        public void Throws_WithCustomMessage()
        {
            var exception = new InvalidTextException("Test message");
            Assert.Equal("Test message", exception.Message);
        }
    }
}
