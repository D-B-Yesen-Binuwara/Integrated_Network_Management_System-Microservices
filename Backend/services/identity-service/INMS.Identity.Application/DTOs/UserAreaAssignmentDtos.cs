namespace INMS.Identity.Application.DTOs;

public record UserAreaAssignmentDto(int AssignmentId, string AreaType, Guid AreaId, string? RegionCode = null, string? ProvinceCode = null, string? LEACode = null);
