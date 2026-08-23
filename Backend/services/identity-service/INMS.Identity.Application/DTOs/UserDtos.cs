namespace INMS.Identity.Application.DTOs;

public record CreateUserDto(
    string FirstName,
    string LastName,
    int RoleId,
    string? ServiceId = null,
    string? Email = null,
    Guid? RegionId = null,
    Guid? ProvinceId = null,
    Guid? LEAId = null,
    string? RegionCode = null,
    string? ProvinceCode = null,
    string? LEACode = null
);

public record UpdateUserDto(string Username, string FullName, int RoleId, string? ServiceId = null, string? Email = null);

public record UserResponseDto(
    int UserId,
    string Username,
    string FullName,
    int RoleId,
    string? RoleName,
    string? ServiceId,
    string? Email,
    string? Region,
    string? Province,
    string? LEA,
    string? RegionCode = null,
    string? ProvinceCode = null,
    string? LEACode = null);
