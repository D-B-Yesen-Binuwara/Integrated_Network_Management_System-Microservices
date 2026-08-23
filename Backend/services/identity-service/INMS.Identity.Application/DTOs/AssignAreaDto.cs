namespace INMS.Identity.Application.DTOs;

public record AssignAreaDto(string AreaType, Guid AreaId, string? RegionCode = null, string? ProvinceCode = null, string? LEACode = null);
