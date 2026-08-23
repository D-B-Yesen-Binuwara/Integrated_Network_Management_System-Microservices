namespace INMS.Identity.Application.DTOs;

public record CreateAccountRequestDto(
    string FullName,
    string Email,
    string ServiceId,
    int RoleId,
    Guid RegionId,
    Guid? ProvinceId = null,
    Guid? LEAId = null,
    string? RegionCode = null,
    string? ProvinceCode = null,
    string? LEACode = null);

public record UpdateAccountRequestStatusDto(string Status);
