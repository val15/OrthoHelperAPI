using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

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
    public async Task<IActionResult> BrowseAvailableLots()
    {
        var query = new BrowseCorrectionSessionQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}