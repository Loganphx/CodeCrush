using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services;

public interface IMessageRepository
{
    public void AddMessage(Message message);
    public void DeleteMessage(Message message);
    public Task<Message> GetMessage(int id);
    public Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    public Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
    public void AddGroup(Group group);
    public void RemoveConnection(Connection connection);
    public Task<Connection> GetConnection(string connectionId);
    public Task<Group> GetMessageGroup(string groupName);
    public Task<Group> GetGroupForConnection(string connectionId);
}