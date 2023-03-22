using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.HealthChecks.NotificationMethods;

namespace UmbracoHealthCheckTeamsNotifications.HealthCheck.Notifications;

[HealthCheckNotificationMethod("teams")]
public class TeamsNotificationMethod : NotificationMethodBase
{
    public string? WebHookUrl { get; }

    private const string WebHookUrlKey = "webHookUrl";

    public TeamsNotificationMethod(IOptionsMonitor<HealthChecksSettings> healthCheckSettings) : base(healthCheckSettings)
    {
        if (Settings != null && Settings.ContainsKey(WebHookUrlKey))
        {
            WebHookUrl = Settings[WebHookUrlKey];
        }

        if (string.IsNullOrWhiteSpace(WebHookUrl))
        {
            Enabled = false;
        }
    }

    public override Task SendAsync(HealthCheckResults results)
    {
        throw new NotImplementedException();
    }
}
