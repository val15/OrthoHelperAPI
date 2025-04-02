using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;
using OrthoHelper.Infrastructure.Features.Common.Services.OrthoHelper.Infrastructure.Features.Common.Services;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using OrthoHelper.Infrastructure.Features.TextProcessing.Mappings;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class CorrectionSessionRepository : ICorrectionSessionRepository
    {
        private readonly ApiDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;


        public CorrectionSessionRepository(ApiDbContext context, IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _context = context;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }
        public async Task AddAsync(CorrectionSession correctionSession)
        {
            var userName = "";
            // Récupérer l'utilisateur
            if (_currentUserService.IsAuthenticated)
            {
                //historyText = GetTextUserSessionHistory(_currentUserService.UserName).Result;

                userName = _currentUserService.UserName;


            }


            var user = await _userRepository.GetUserByUsername(userName);
            if (user is null) throw new Exception("Utilisateur non trouvé");

            // Mapper vers l'entité Message
            var message = new Message
            {
                InputText = correctionSession.OriginalText,
                OutputText = correctionSession.CorrectedText,
                ProcessingTime = correctionSession.ProcessingTime,
                Diff = correctionSession.Diff,
                UserId = user.Id, // Utilisation directe de l'ID
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CorrectionSession?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CorrectionSession>> GetCorrectionSessionsAsync()
        {

            var userName = "";
            // Récupérer l'utilisateur
            if (_currentUserService.IsAuthenticated)
            {

                userName = _currentUserService.UserName;


            }


            var user = await _userRepository.GetUserByUsername(userName);
            if (user is null)
                throw new Exception($"User {userName} not found");

            var messages = await _context.Messages
                .Where(m => m.UserId == user.Id)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return messages.Select(CorrectionSessionAdapter.ToDomain); // Utilisation de l'Adapter ici

        }

        public Task UpdateAsync(CorrectionSession correctionSession)
        {
            throw new NotImplementedException();
        }
    }
}
