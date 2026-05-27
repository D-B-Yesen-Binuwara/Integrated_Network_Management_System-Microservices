namespace INMS.Identity.Application.DTOs;

public record CreateAccountRequestDto(string FullName, string Email, string ServiceId, int RoleId, Guid RegionId, Guid? ProvinceId = null, Guid? LEAId = null);

public record UpdateAccountRequestStatusDto(string Status);
