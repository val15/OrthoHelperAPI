using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Application.Features.TextProcess.UseCases;
using OrthoHelper.Application.Features.TextTranslation;
using OrthoHelper.Application.Features.TextTranslation.DTOs;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

namespace OrthoHelper.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TextController : ControllerBase
{
    private readonly ICorrectTextUseCase _correctTextUseCase;
    private readonly ITranslateTextUseCase _translateTextUseCase;
    private readonly ITranslateHtmlFileUseCase _translateHtmlFileUseCase;
    private readonly IMediator _mediator;
    public TextController(ICorrectTextUseCase correctTextUseCase,
        ITranslateTextUseCase translatTextUseCase,
        ITranslateHtmlFileUseCase translateHtmlFileUseCase,
        IMediator mediator)
    {
        _correctTextUseCase = correctTextUseCase;
        _translateTextUseCase = translatTextUseCase;
        _translateHtmlFileUseCase = translateHtmlFileUseCase;
        _mediator = mediator;
    }

    [HttpPost("correct")]
    public async Task<IActionResult> CorrectText([FromBody] InputTextDto input)
    {
        try
        {
            ///api/text/correct
            var result = await _correctTextUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (TextCorrectionFailedException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("translate")]
    public async Task<IActionResult> TranslateText([FromBody] InputTextDto input)
    {
        try
        {
            var result = await _translateTextUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (TextCorrectionFailedException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("translate-html-file")]
    public async Task<IActionResult> TranslateHtmlFile([FromBody] HtmlTranslationInputDto input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.HtmlFilePath))
            return BadRequest("Le chemin du fichier HTML est requis.");

        try
        {
            var result = await _translateHtmlFileUseCase.ExecuteAsync(input);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur lors de la traduction du fichier HTML : {ex.Message}");
        }
    }

    [HttpGet("Messages")]
    public async Task<IActionResult> BrowseCorrectionSessions()
    {
        var query = new BrowseSessionQuery();
        var result = await _mediator.Send(query);
        return Ok(result);

    }

    [HttpDelete("DeleteUserMessages")]
    public async Task<IActionResult> DeleteAllMessages()
    {
        try
        {
            var query = new DeleteAllUserSessionQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Une erreur est survenue : {ex.Message}");
        }
    }

    [HttpGet("Models")]
    public async Task<IActionResult> BrowseAvailableModels()
    {
        var query = new BrowseLLMModelsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}