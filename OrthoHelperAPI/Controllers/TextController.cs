using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrthoHelperAPI.Model;
using OrthoHelperAPI.Repositories;
using OrthoHelperAPI.Services.Interfaces;
using System.Security.Claims;

namespace OrthoHelperAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TextController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ITextProcessingService _textProcessingService;

        public TextController(
            IMessageRepository messageRepository,
            ITextProcessingService textProcessingService)
        {
            _messageRepository = messageRepository;
            _textProcessingService = textProcessingService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessText([FromBody] TextProcessingRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var messages = await _messageRepository.GetUserMessagesAsync(userId);

                var startDate = DateTime.Now;
                //var result = await _textProcessingService.ProcessTextAsync(request.Text, messages);
                var result = await _textProcessingService.ProcessTextAsync(request.Text);
                var processingTime = DateTime.Now - startDate;

                Console.WriteLine($"REFLECTION TIME :{processingTime}");

                var message = new Message
                {
                    InputText = request.Text,
                    OutputText = result.correctedText,
                    Diff = result.diffText,
                    CreatedAt = DateTime.UtcNow,
                    ProcessingTime = processingTime,
                    UserId = userId
                };

                await _messageRepository.AddMessageAsync(message);

                return Ok(new
                {
                    inputText = message.InputText,
                    outputText = message.OutputText,
                    diff = message.Diff,
                    processingTime = message.ProcessingTime,
                    timestamp = message.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue : {ex.Message}");
            }
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetUserMessages()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var messages = await _messageRepository.GetUserMessagesAsync(userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue : {ex.Message}");
            }
        }

        [HttpDelete("messages/all")]
        public async Task<IActionResult> DeleteAllMessages()
        {
            try
            {
                var count = await _messageRepository.DeleteAllMessagesAsync();
                return Ok(new { deletedCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue : {ex.Message}");
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID not found in token");
            }
            return int.Parse(userIdClaim.Value);
        }
    }
}