using OrthoHelper.Domain.Entities;
using OrthoHelper.Domain.Ports;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Exceptions;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

namespace OrthoHelper.Application.Features.TextCorrection.UseCases;

public class CorrectTextUseCase : ICorrectTextUseCase
{
    private readonly ITextProcessingEngine _textProcessingEngine;

    public CorrectTextUseCase(ITextProcessingEngine textProcessingEngine)
    {
        _textProcessingEngine = textProcessingEngine;
    }

    public async Task<CorrectTextOutputDto> ExecuteAsync(CorrectTextInputDto input)
    {
        // Validation applicative
        if (string.IsNullOrWhiteSpace(input.Text))
            throw new InvalidTextException("Le texte à corriger ne peut pas être vide.");

        // Création de l'entité Domain
        var correctionSession = CorrectionSession.Create(input.Text);

        try
        {
            // Appel au moteur externe via le port
            var correctedText = await _textProcessingEngine.CorrectTextAsync(input.Text);

            // Mise à jour de l'entité Domain
            correctionSession.ApplyCorrection(correctedText);

            // Mapping vers le DTO de sortie
            return new CorrectTextOutputDto(
                correctionSession.OriginalText,
                correctionSession.CorrectedText,
                DateTime.UtcNow
            );
        }
        catch (Exception ex)
        {
            throw new TextCorrectionFailedException("Échec de la correction du texte.", ex);
        }
    }
}

// Exception applicative
public class TextCorrectionFailedException : Exception
{
    public TextCorrectionFailedException(string message, Exception inner)
        : base(message, inner) { }
}