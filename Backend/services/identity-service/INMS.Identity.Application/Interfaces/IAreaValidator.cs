namespace INMS.Identity.Application.Interfaces;

public interface IAreaValidator
{
    Task<bool> AreaExists(string areaType, Guid areaId);
}
