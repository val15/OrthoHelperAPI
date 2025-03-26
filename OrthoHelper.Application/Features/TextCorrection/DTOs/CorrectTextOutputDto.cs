namespace OrthoHelper.Application.Features.TextCorrection.DTOs
{

    public class CorrectTextOutputDto
    {
        public string InputText { get; set; } = string.Empty;
        public string OutputText { get; set; } = string.Empty;
        public string Diff { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
