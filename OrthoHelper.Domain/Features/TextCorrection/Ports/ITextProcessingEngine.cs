using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports
{
    // OrthoHelper.Domain/Ports/ITextProcessingEngine.cs
    public interface ITextProcessingEngine
    {
        Task<string> CorrectTextAsync(string inputText);

        //Task InitializeUserSession(string username);
    }
}
