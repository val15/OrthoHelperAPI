using Microsoft.EntityFrameworkCore;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using OrthoHelper.Infrastructure.Features.TextProcessing.Mappings;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class CorrectionSessionRepository : ISessionRepository
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
        public async Task AddAsync(Session correctionSession)
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

            if(correctionSession.OutputText == null)
                correctionSession.OutputText = string.Empty; // Assurez-vous que OutputText n'est jamais null

            // Mapper vers l'entité Message
            var message = new Message
            {
                InputText = correctionSession.InputText,
                OutputText = correctionSession.OutputText,
                ProcessingTime = correctionSession.ProcessingTime,
                Diff = correctionSession.Diff,
                UserId = user.Id, // Utilisation directe de l'ID
               ModelName = correctionSession.ModelName,
               Type = correctionSession.Type,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAllUserCorrectionSessionsAsync()
        {

            try
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



                var userMessages = _context.Messages.Where(x => x.UserId == user.Id);

                _context.Messages.RemoveRange(userMessages);

                await _context.SaveChangesAsync();
                return userMessages.Count();
            }
            catch (Exception)
            {

                return -1;
            }
            
        }

        public async Task<Session?> GetByIdAsync(int id)
        {
            MessageType messageType = MessageType.Corrector;
            var session = await _context.Messages.FirstOrDefaultAsync(x => x.Id == id && x.Type == messageType);
            return session is null ? null : CorrectionSessionAdapter.ToDomain(session);
        }

        public async Task<Session?> GetSessionAsync(string inputText, MessageType messageType, string modelName)
        {
            var session  = await _context.Messages.FirstOrDefaultAsync(x => x.InputText == inputText && x.Type == messageType && x.ModelName == modelName);
            return session is null ? null : CorrectionSessionAdapter.ToDomain(session);

        }


        public async Task<IEnumerable<Session>> GetSessionsAsync()
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

        public Task UpdateAsync(Session correctionSession)
        {
            throw new NotImplementedException();
        }
    }
}
