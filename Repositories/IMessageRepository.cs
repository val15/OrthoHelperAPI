using OrthoHelperAPI.Model;

namespace OrthoHelperAPI.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> AddMessageAsync(Message message);
        Task<List<Message>> GetUserMessagesAsync(int userId);
        Task<int> DeleteAllMessagesAsync();
    }
}