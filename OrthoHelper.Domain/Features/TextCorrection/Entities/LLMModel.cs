using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextCorrection.Entities
{
    public class LLMModel
    {
        public string ModelName { get; set; }

        public LLMModel(string modelName)
        {
            ModelName = modelName;
        }   

    }
}
