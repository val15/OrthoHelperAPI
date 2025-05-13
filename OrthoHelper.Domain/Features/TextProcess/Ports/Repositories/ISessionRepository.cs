using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories
{
    public interface ISessionRepository
    {

        Task<Session?> GetByIdAsync(Guid id);
        Task<IEnumerable<Session>> GetCorrectionSessionsAsync();
        Task AddAsync(Session correctionSession);
        Task UpdateAsync(Session correctionSession);
        Task<int> DeleteAllUserCorrectionSessionsAsync();
    }
}

