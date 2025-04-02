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

    private readonly ICorrectionSessionRepository _repository;


    public CorrectTextUseCase(ITextProcessingEngine textProcessingEngine, ICurrentUserService currentUserService, ICorrectionSessionRepository repository)
    {
        _textProcessingEngine = textProcessingEngine;
        _currentUserService = currentUserService;
         _repository = repository;
    }

    public async Task<CorrectTextOutputDto> ExecuteAsync(CorrectTextInputDto input)
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();
        // Validation applicative
        if (string.IsNullOrWhiteSpace(input.Text))
            throw new InvalidTextException("Le texte à corriger ne peut pas être vide.");

        //if (string.IsNullOrWhiteSpace(input.UserName))
        //    throw new UserNotFoundException("Le nom d'utilisateur est requis.");

        var username = _currentUserService.UserName
            ?? throw new UserNotFoundException("Nom d'utilisateur introuvable");


        // Création de l'entité Domain
        var correctionSession = CorrectionSession.Create(input.Text);

        try
        {

            //Appel de l'initailaisation pour lires les données dans la base
           // await _textProcessingEngine.InitializeUserSession(username);
            // Appel au moteur externe via le port
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

    

            _repository.AddAsync(correctionSession);
            return result;

          
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