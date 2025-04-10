namespace OrthoHelper.Infrastructure.Features.TextProcessing.Entities
{

    //Json object from Ollama API
    public class OllamaModelsResponse
    {

        public ModelInfo[] models { get; set; }
    }

    public class ModelInfo
    {
        public string name { get; set; }
        public string model { get; set; }
        public DateTime modified_at { get; set; }
        public long size { get; set; }
        public string digest { get; set; }
        public Details details { get; set; }
    }

    public class Details
    {
        public string parent_model { get; set; }
        public string format { get; set; }
        public string family { get; set; }
        public string[] families { get; set; }
        public string parameter_size { get; set; }
        public string quantization_level { get; set; }
    }
}
