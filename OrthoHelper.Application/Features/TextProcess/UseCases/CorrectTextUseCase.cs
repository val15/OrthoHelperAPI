using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OrthoHelper.Application.Features.TextProcess.UseCases;

namespace OrthoHelper.Application.Features.TextCorrection.UseCases;

public class CorrectTextUseCase : ICorrectTextUseCase
{
    private readonly ICorrectorOrchestrator _orchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISessionRepository _correctionSessionRepository;

    public CorrectTextUseCase(
        ICorrectorOrchestrator correctionOrchestrator,
        ICurrentUserService currentUserService,
        ISessionRepository correctionSessionRepository)
    {
        _orchestrator = correctionOrchestrator;
        _currentUserService = currentUserService;
        _correctionSessionRepository = correctionSessionRepository;
    }

    public async Task<OutputTextDto> ExecuteAsync(InputTextDto input)
    {
        // Validation des données d'entrée
        var textToCorrect = new TextToCorrect(input.Text);
        var modelName = new ModelName(input.ModelName);

        var username = _currentUserService.UserName
            ?? throw new UserNotFoundException("Nom d'utilisateur introuvable");

        // Utilisation de l'orchestrateur pour traiter la correction
        var correctionSession = await _orchestrator.ProcessAsync(textToCorrect, modelName, username);

        // Sauvegarde de la session
        await _correctionSessionRepository.AddAsync(correctionSession);

        // Mapping vers le DTO de sortie
        return new OutputTextDto
        {
            InputText = correctionSession.InputText,
            OutputText = correctionSession.OutputText,
            Diff = correctionSession.Diff,
            ProcessingTime = correctionSession.ProcessingTime,
            ModelName = correctionSession.ModelName,
            Type = correctionSession.Type,
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