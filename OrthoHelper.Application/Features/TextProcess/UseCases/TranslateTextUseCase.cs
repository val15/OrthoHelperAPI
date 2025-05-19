using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OrthoHelper.Application.Features.TextProcess.UseCases;

namespace OrthoHelper.Application.Features.TextCorrection.UseCases;

public class TranslateTextUseCase : ITranslateTextUseCase
{
    private readonly ITranslatorOrchestrator _orchestrator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISessionRepository _sessionRepository;

    public TranslateTextUseCase(
        ITranslatorOrchestrator orchestrator,
        ICurrentUserService currentUserService,
        ISessionRepository sessionRepository)
    {
        _orchestrator = orchestrator;
        _currentUserService = currentUserService;
        _sessionRepository = sessionRepository;
    }

    public async Task<OutputTextDto> ExecuteAsync(InputTextDto input)
    {
        // Validation des données d'entrée
        var textToCorrect = new TextToCorrect(input.Text);
        var modelName = new ModelName(input.ModelName);

        var username = _currentUserService.UserName
            ?? throw new UserNotFoundException("Nom d'utilisateur introuvable");

        // Utilisation de l'orchestrateur pour traiter la correction
        var session = await _orchestrator.ProcessAsync(textToCorrect, modelName, username);

        // Sauvegarde de la session
        await _sessionRepository.AddAsync(session);

        // Mapping vers le DTO de sortie
        return new OutputTextDto
        {
            InputText = session.InputText,
            OutputText = session.OutputText,
            Diff = session.Diff,
            ProcessingTime = session.ProcessingTime,
            ModelName = session.ModelName,
            Type = session.Type,
            CreatedAt = session.CreatedAt
        };
    }
}

// Exception applicative
public class TextTranslateFailedException : Exception
{
    public TextTranslateFailedException(string message, Exception inner)
        : base(message, inner) { }
}