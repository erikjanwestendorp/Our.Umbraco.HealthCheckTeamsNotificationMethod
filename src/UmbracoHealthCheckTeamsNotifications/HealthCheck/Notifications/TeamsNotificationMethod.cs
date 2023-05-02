using Microsoft.Extensions.Options;
using Teams.WebHooks;
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

    public override async Task SendAsync(HealthCheckResults results)
    {
        var healthCheckResults = results.GetResultsForStatus(StatusResultType.Error);

        var cards = new List<Message>();

        if (healthCheckResults == null)
        {
            return;
        }

        foreach (var healthCheckResult in healthCheckResults)
        {
            if (healthCheckResult.Value.Any())
            {
                var card = new Message { Summary = healthCheckResult.Key };
                

                foreach (var val in healthCheckResult.Value)
                {
                    card.Sections.Add(new Section
                    {
                        ActivityTitle = val.Message,
                        
                        ActivityImage = "https://umbraco.com/media/ziikdjap/umbraco_social_og.png",
                        Facts = new List<Fact>
                        {
                            new()
                            {
                                Name = "message",
                                Value = val.Message
                            }
                        }

                    });
                }
                cards.Add(card);
            }
            
        }

        foreach (var message in cards)
        {
            await MessageClient.SendAsync(WebHookUrl, message);
        }
    }
}
