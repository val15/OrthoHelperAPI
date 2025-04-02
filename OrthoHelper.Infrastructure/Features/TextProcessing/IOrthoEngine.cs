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

        //Task InitializeUserSession(string username);
    }
}
