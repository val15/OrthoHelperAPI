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
        Task<string> ProcessTextAsync(string inputText);
        void SetModelName(string modelName);
        string GetModelName();

        //Task InitializeUserSession(string username);
    }
}
