using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Application.DTOs;
using INMS.Identity.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace INMS.Identity.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IUserAreaAssignmentRepository _areaAssignmentRepository;

    public UserService(IUserRepository repository, IUserAreaAssignmentRepository areaAssignmentRepository)
    {
        _repository = repository;
        _areaAssignmentRepository = areaAssignmentRepository;
    }

    public async Task<List<UserResponseDto>> GetAll()
    {
        var users = await _repository.GetAll();

        var result = new List<UserResponseDto>();
        foreach (var user in users)
        {
            var assignments = await _areaAssignmentRepository.GetAllByUserId(user.UserId);
            var region = assignments.FirstOrDefault(a => a.AreaType == "Region");
            var province = assignments.FirstOrDefault(a => a.AreaType == "Province");
            var lea = assignments.FirstOrDefault(a => a.AreaType == "LEA");

            result.Add(new UserResponseDto(
                user.UserId,
                user.Username,
                user.FullName,
                user.RoleId,
                user.Role?.RoleName,
                user.ServiceId,
                user.Email,
                region?.RegionCode,
                province?.ProvinceCode,
                lea?.LEACode,
                region?.RegionCode,
                province?.ProvinceCode,
                lea?.LEACode));
        }

        return result;
    }

    public async Task<User?> GetById(int id)
    {
        return (await _repository.GetById(id));
    }

    public async Task Create(string username, string password, int roleId)
    {
        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            RoleId = roleId
        };

        await _repository.Create(user);
    }

    public async Task CreateFromDto(CreateUserDto dto)
    {
        var fullName = $"{dto.FirstName} {dto.LastName}".Trim();
        var generatedUsername = GenerateUniqueUsername(dto.FirstName, dto.LastName);

        var user = new User
        {
            Username = generatedUsername,
            PasswordHash = HashPassword("DefaultPassword123!"),
            FullName = fullName,
            RoleId = dto.RoleId,
            ServiceId = dto.ServiceId,
            Email = dto.Email
        };

        await _repository.Create(user);

        if (dto.RegionId.HasValue)
        {
            await _areaAssignmentRepository.AssignArea(new UserAreaAssignment { UserId = user.UserId, AreaType = "Region", AreaId = dto.RegionId.Value, RegionCode = dto.RegionCode });
        }

        if (dto.ProvinceId.HasValue)
        {
            await _areaAssignmentRepository.AssignArea(new UserAreaAssignment { UserId = user.UserId, AreaType = "Province", AreaId = dto.ProvinceId.Value, ProvinceCode = dto.ProvinceCode });
        }

        if (dto.LEAId.HasValue)
        {
            await _areaAssignmentRepository.AssignArea(new UserAreaAssignment { UserId = user.UserId, AreaType = "LEA", AreaId = dto.LEAId.Value, LEACode = dto.LEACode });
        }
    }

    public async Task Update(int id, string username, int roleId)
    {
        var user = await _repository.GetById(id);
        if (user == null) return;

        user.Username = username;
        user.RoleId = roleId;

        await _repository.Update(user);
    }

    public async Task Delete(int id)
    {
        await _areaAssignmentRepository.RemoveAssignmentsByUserId(id);
        await _repository.Delete(id);
    }

    private string HashPassword(string password)
    {
        using SHA256 sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private string GenerateUniqueUsername(string firstName, string lastName)
    {
        var baseName = $"{firstName}.{lastName}".ToLower().Replace(" ", "");
        var username = baseName;
        int counter = 1;

        while (UsernameExists(username).Result)
        {
            username = $"{baseName}{counter}";
            counter++;
        }

        return username;
    }

    private async Task<bool> UsernameExists(string username)
    {
        var allUsers = await _repository.GetAll();
        return allUsers.Any(u => u.Username.ToLower() == username.ToLower());
    }
}
