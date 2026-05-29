using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace INMS.Identity.Infrastructure.Repositories;

public class AccountRequestRepository : IAccountRequestRepository
{
    private readonly IdentityDbContext _context;

    public AccountRequestRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountRequest>> GetAll()
    {
        return await _context.AccountRequests.Include(a => a.Role).ToListAsync();
    }

    public async Task<AccountRequest?> GetById(int id)
    {
        return await _context.AccountRequests.Include(a => a.Role).FirstOrDefaultAsync(x => x.RequestId == id);
    }

    public async Task Create(AccountRequest request)
    {
        _context.AccountRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task Update(AccountRequest request)
    {
        _context.AccountRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var ar = await _context.AccountRequests.FindAsync(id);
        if (ar != null)
        {
            _context.AccountRequests.Remove(ar);
            await _context.SaveChangesAsync();
        }
    }
}
