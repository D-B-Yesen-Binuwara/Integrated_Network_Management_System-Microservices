using System.Text.Json;
using alarm_service.Correlation.Models;

namespace alarm_service.Correlation.Engine;

public class RuleLoader
{
    private readonly IReadOnlyList<CorrelationRule> _slbnRules;
    private readonly IReadOnlyList<CorrelationRule> _ceanRules;
    private readonly IReadOnlyList<CorrelationRule> _msanRules;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RuleLoader(IWebHostEnvironment env)
    {
        var rulesPath = Path.Combine(env.ContentRootPath, "Correlation", "Rules");
        _slbnRules = LoadRules(Path.Combine(rulesPath, "slbn-rules.json"));
        _ceanRules = LoadRules(Path.Combine(rulesPath, "cean-rules.json"));
        _msanRules = LoadRules(Path.Combine(rulesPath, "msan-rules.json"));
    }

    public IReadOnlyList<CorrelationRule> SlbnRules => _slbnRules;
    public IReadOnlyList<CorrelationRule> CeanRules => _ceanRules;
    public IReadOnlyList<CorrelationRule> MsanRules => _msanRules;

    public IEnumerable<CorrelationRule> GetAllRules() =>
        _slbnRules.Concat(_ceanRules).Concat(_msanRules)
                  .Where(r => r.Enabled)
                  .OrderBy(r => r.Priority)
                  .ThenBy(r => r.RuleName, StringComparer.OrdinalIgnoreCase);

    public CorrelationRule? FindMatchingRule(CorrelationContext context) =>
        GetAllRules().FirstOrDefault(rule =>
            string.Equals(rule.SourceAlarmType, context.AlarmType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(rule.SourceDeviceType, context.DeviceType, StringComparison.OrdinalIgnoreCase));

    private static IReadOnlyList<CorrelationRule> LoadRules(string path)
    {
        if (!File.Exists(path))
            return [];

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<CorrelationRule>>(json, _jsonOptions) ?? [];
        }
        catch (Exception) when (File.Exists(path))
        {
            // A malformed or unreadable rule file must not take down the alarm API.
            return [];
        }
    }
}
