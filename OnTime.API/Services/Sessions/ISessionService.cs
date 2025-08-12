using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;

namespace OnTime.API.Services.Sessions;

public interface ISessionService
{
    Task<SessionResponse?> Get(int id);
    Task<int> Create(CreateSessionRequest request);
    Task<bool> Update(int id, UpdateSessionRequest request);
    Task<bool> Delete(int id);
}