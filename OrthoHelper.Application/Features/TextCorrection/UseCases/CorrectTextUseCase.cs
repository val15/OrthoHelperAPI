using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using System.Diagnostics;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Features.TextCorrection.UseCases;

public class CorrectTextUseCase : ICorrectTextUseCase
{
    private readonly ITextProcessingEngine _textProcessingEngine;
    // private readonly ICorrectionSessionRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    private readonly ICorrectionSessionRepository _correctionSessionRepository;
    private readonly ILLMModelRepository _llmModelRepository;


    public CorrectTextUseCase(ITextProcessingEngine textProcessingEngine, ICurrentUserService currentUserService, ICorrectionSessionRepository correctionSessionRepository, ILLMModelRepository llmModelRepository)
    {
        _textProcessingEngine = textProcessingEngine;
        _currentUserService = currentUserService;
        _correctionSessionRepository = correctionSessionRepository;
        _llmModelRepository = llmModelRepository;
    }

    public async Task<CorrectTextOutputDto> ExecuteAsync(CorrectTextInputDto input)
    {
        //input ajout de MODEL_NAME
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        // Validation applicative
        if (string.IsNullOrWhiteSpace(input.Text))
            throw new InvalidTextException("Le texte à corriger ne peut pas être vide.");


        if (string.IsNullOrWhiteSpace(input.ModelName))
            throw new InvalidModelNameException("Le nom du model ne peut pas être vide.");

        //if (input.ModelName== "string")
        //{

        //}

        //TODO 

        // if not in models list
        if (string.IsNullOrWhiteSpace(input.ModelName))
            throw new InvalidModelNameException("Le nom du model n' est pas dans la list  des models disponible.");


        //if (string.IsNullOrWhiteSpace(input.UserName))
        //    throw new UserNotFoundException("Le nom d'utilisateur est requis.");

        var username = _currentUserService.UserName
            ?? throw new UserNotFoundException("Nom d'utilisateur introuvable");


        // Création de l'entité Domain
        var correctionSession = CorrectionSession.Create(input.Text);

        try
        {
            var availableModels = await _llmModelRepository.GetAvailableLLMModelsAsync();

            //Appel de l'initailaisation pour lires les données dans la base
            // await _textProcessingEngine.InitializeUserSession(username);
            // Appel au moteur externe via le port
            correctionSession.SetModelName(input.ModelName,_textProcessingEngine, availableModels);
            var correctedText = await _textProcessingEngine.CorrectTextAsync(input.Text);

            // Mise à jour de l'entité Domain
            correctionSession.ApplyCorrection(correctedText);

            // correctedText.Diff = input.Text, correctedText);
            stopwatch.Stop();
            // Mapping vers le DTO de sortie

            var result = new CorrectTextOutputDto
            {
                InputText = input.Text,
                OutputText = correctedText,
                Diff = correctionSession.Diff,
                ProcessingTime = stopwatch.Elapsed,
                CreatedAt = startTime
            };



            _correctionSessionRepository.AddAsync(correctionSession);
            return result;


        }
        catch (InvalidModelNameException ex)
        {
            throw new InvalidModelNameException("Le modèle spécifié n'est pas valide.");
        }
        catch (InvalidTextException ex)
        {
            throw new InvalidTextException("Le texte spécifié n'est pas valide.");
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