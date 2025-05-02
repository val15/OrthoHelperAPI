using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;

namespace OrthoHelper.Application.Features.TextCorrection.UseCases;

public class CorrectTextUseCase : ICorrectTextUseCase
{
    private readonly ICorrectionOrchestrator _correctionOrchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICorrectionSessionRepository _correctionSessionRepository;

    public CorrectTextUseCase(
        ICorrectionOrchestrator correctionOrchestrator,
        ICurrentUserService currentUserService,
        ICorrectionSessionRepository correctionSessionRepository)
    {
        _correctionOrchestrator = correctionOrchestrator;
        _currentUserService = currentUserService;
        _correctionSessionRepository = correctionSessionRepository;
    }

    public async Task<CorrectTextOutputDto> ExecuteAsync(CorrectTextInputDto input)
    {
        // Validation des données d'entrée
        var textToCorrect = new TextToCorrect(input.Text);
        var modelName = new ModelName(input.ModelName);

        var username = _currentUserService.UserName
            ?? throw new UserNotFoundException("Nom d'utilisateur introuvable");

        // Utilisation de l'orchestrateur pour traiter la correction
        var correctionSession = await _correctionOrchestrator.ProcessCorrectionAsync(textToCorrect, modelName, username);

        // Sauvegarde de la session
        await _correctionSessionRepository.AddAsync(correctionSession);

        // Mapping vers le DTO de sortie
        return new CorrectTextOutputDto
        {
            InputText = correctionSession.OriginalText,
            OutputText = correctionSession.CorrectedText,
            Diff = correctionSession.Diff,
            ProcessingTime = correctionSession.ProcessingTime,
            CreatedAt = correctionSession.CreatedAt
        };
    }
}

// Exception applicative
public class TextCorrectionFailedException : Exception
{
    public TextCorrectionFailedException(string message, Exception inner)
        : base(message, inner) { }
}