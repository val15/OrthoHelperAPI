using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories
{
    public interface ISessionRepository
    {

        Task<Session?> GetByIdAsync(int id);
        Task<IEnumerable<Session>> GetSessionsAsync();
        Task<Session> GetSessionAsync(string inputText,MessageType messageType,string modelName);
        Task AddAsync(Session correctionSession);
        Task UpdateAsync(Session correctionSession);
        Task<int> DeleteAllUserCorrectionSessionsAsync();
    }
}

