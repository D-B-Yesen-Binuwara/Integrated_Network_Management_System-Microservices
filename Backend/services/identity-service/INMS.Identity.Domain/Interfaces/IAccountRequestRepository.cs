using INMS.Identity.Domain.Entities;

namespace INMS.Identity.Domain.Interfaces;

public interface IAccountRequestRepository
{
    Task<List<AccountRequest>> GetAll();
    Task<AccountRequest?> GetById(int id);
    Task Create(AccountRequest request);
    Task Update(AccountRequest request);
    Task Delete(int id);
}
