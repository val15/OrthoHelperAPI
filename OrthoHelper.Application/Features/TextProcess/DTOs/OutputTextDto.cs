using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Application.Features.TextCorrection.DTOs
{

    public class OutputTextDto
    {
        public string InputText { get; set; } = string.Empty;
        public string OutputText { get; set; } = string.Empty;
        public string Diff { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageType Type { get; set; }
        public string ModelName { get; set; } 
    }

}
