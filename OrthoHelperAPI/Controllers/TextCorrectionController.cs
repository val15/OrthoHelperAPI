using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TextCorrectionController : ControllerBase
{
    private readonly ICorrectTextUseCase _correctTextUseCase;
    private readonly IMediator _mediator;
    public TextCorrectionController(ICorrectTextUseCase correctTextUseCase, IMediator mediator)
    {
        _correctTextUseCase = correctTextUseCase;
        _mediator = mediator;
        _mediator = mediator;
    }

    [HttpPost("correct")]
    public async Task<IActionResult> CorrectText([FromBody] CorrectTextInputDto input)
    {
        try
        {
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
    [HttpGet("Messages")]
    public async Task<IActionResult> BrowseCorrectionSessions()
    {
        var query = new BrowseCorrectionSessionQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("DeleteUserMessages")]
    public async Task<IActionResult> DeleteAllMessages()
    {
        try
        {
            var query = new DeleteAllUserCorrectionSessionQuery();
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