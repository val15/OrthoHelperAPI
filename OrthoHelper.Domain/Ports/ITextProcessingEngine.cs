using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Ports
{
    // OrthoHelper.Domain/Ports/ITextProcessingEngine.cs
    public interface ITextProcessingEngine
    {
        Task<string> CorrectTextAsync(string inputText);
    }
}
