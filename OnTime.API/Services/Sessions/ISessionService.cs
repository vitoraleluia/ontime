using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;
using OnTime.API.Models.Results;

namespace OnTime.API.Services.Sessions;

interface ISessionService
{
    Task<Result<SessionResponse>> Get(int id);
    Task<Result<int>> Create(CreateSessionRequest request);
    Task<Result> Update(int id, UpdateSessionRequest request);
    Task<Result> Delete(int id);
}