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
