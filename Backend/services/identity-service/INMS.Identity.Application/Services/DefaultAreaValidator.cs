using INMS.Identity.Application.Interfaces;

namespace INMS.Identity.Application.Services;

public class DefaultAreaValidator : IAreaValidator
{
    public Task<bool> AreaExists(string areaType, Guid areaId)
    {
        // Minimal validation: ensure GUID is not empty.
        // Replace with cross-service checks if topology/inventory endpoints are available.
        return Task.FromResult(areaId != Guid.Empty);
    }
}
