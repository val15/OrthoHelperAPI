using Microsoft.EntityFrameworkCore;
using OrthoHelperAPI.Data;
using OrthoHelperAPI.Model;

namespace OrthoHelperAPI.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApiDbContext _context;

        public MessageRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetUserMessagesAsync(int userId)
        {
            return await _context.Messages
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> DeleteAllMessagesAsync()
        {
            return await _context.Messages.ExecuteDeleteAsync();
        }
    }
}