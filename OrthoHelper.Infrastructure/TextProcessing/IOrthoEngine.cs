using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.TextProcessing
{
    // IOrthoEngine.cs
    public interface IOrthoEngine
    {
        Task<string> ProcessTextAsync(string inputText);
    }
}
