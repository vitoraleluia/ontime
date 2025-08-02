using OnTime.API.Models.Requests;

namespace OnTime.API.Services.Session;

interface ISessionService
{
    void Get();
    int Create(CreateSessionRequest request);
    void Update();
    bool Delete();
}