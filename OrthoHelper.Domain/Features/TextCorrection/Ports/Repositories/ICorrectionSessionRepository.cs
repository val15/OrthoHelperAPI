using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories
{
    public interface ICorrectionSessionRepository
    {

        Task<CorrectionSession?> GetByIdAsync(Guid id);
        Task<IEnumerable<CorrectionSession>> GetCorrectionSessionsAsync();
        Task AddAsync(CorrectionSession correctionSession);
        Task UpdateAsync(CorrectionSession correctionSession);
        Task DeleteAsync(Guid id);
    }
}

