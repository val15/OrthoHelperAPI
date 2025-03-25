namespace OrthoHelper.Application.Features.TextCorrection.DTOs
{

    public record CorrectTextOutputDto(
    string OriginalText,
    string CorrectedText,
    DateTime ProcessedAt
);
}
