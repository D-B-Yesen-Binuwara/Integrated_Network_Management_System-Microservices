using INMS.Identity.Application.DTOs;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace INMS.Identity.Application.Services;

public class AccountRequestService : IAccountRequestService
{
    private readonly IAccountRequestRepository _repo;
    private readonly IUserRepository _userRepository;

    public AccountRequestService(IAccountRequestRepository repo, IUserRepository userRepository)
    {
        _repo = repo;
        _userRepository = userRepository;
    }

    public async Task Submit(CreateAccountRequestDto dto)
    {
        var request = new AccountRequest
        {
            FullName = dto.FullName,
            Email = dto.Email,
            ServiceId = dto.ServiceId,
            RoleId = dto.RoleId,
            RegionId = dto.RegionId,
            ProvinceId = dto.ProvinceId,
            LEAId = dto.LEAId
        };

        await _repo.Create(request);
    }

    public async Task<List<object>> GetAll()
    {
        var requests = await _repo.GetAll();
        return requests.Select(r => (object)r).ToList();
    }

    public async Task<bool> Approve(int requestId)
    {
        var request = await _repo.GetById(requestId);
        if (request == null || request.Status != "PENDING") return false;

        // check by email
        var existing = (await _userRepository.GetAll()).FirstOrDefault(u => u.Email == request.Email);
        if (existing != null) throw new Exception("User already exists");

        var user = new User
        {
            Username = request.Email,
            PasswordHash = HashPassword(request.ServiceId),
            FullName = request.FullName,
            Email = request.Email,
            ServiceId = request.ServiceId,
            RoleId = request.RoleId
        };

        await _userRepository.Create(user);
        request.Status = "APPROVED";
        await _repo.Update(request);
        return true;
    }

    public async Task<bool> Reject(int requestId)
    {
        var request = await _repo.GetById(requestId);
        if (request == null || request.Status != "PENDING") return false;
        request.Status = "REJECTED";
        await _repo.Update(request);
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
