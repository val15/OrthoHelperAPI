using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    // IOrthoEngine.cs
    public interface IOrthoEngine
    {
        Task<string> ProcessTextAsync(string inputText);

        void SetModelName(string modelName);
        //Task InitializeUserSession(string username);
    }
}
