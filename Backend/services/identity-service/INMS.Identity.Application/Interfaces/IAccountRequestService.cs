using INMS.Identity.Application.DTOs;

namespace INMS.Identity.Application.Interfaces;

public interface IAccountRequestService
{
    Task Submit(CreateAccountRequestDto dto);
    Task<List<object>> GetAll();
    Task<bool> Approve(int requestId);
    Task<bool> Reject(int requestId);
}
