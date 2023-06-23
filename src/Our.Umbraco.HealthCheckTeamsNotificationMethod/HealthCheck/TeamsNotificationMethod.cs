using Microsoft.Extensions.Options;
using Teams.WebHooks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.HealthChecks.NotificationMethods;
using Umbraco.Cms.Core.HealthChecks;

namespace Our.Umbraco.HealthCheckTeamsNotificationMethod.HealthCheck;

[HealthCheckNotificationMethod(Constants.NotificationMethods.Teams)]
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

        var card = new Message
        {
            Summary = "Something went wrong",
            Type = "MessageCard"
        };
        

        foreach (var healthCheckResult in healthCheckResults)
        {
            if (healthCheckResult.Value.Any())
            {

                AddSections(card, healthCheckResult);
                
            }

        }

        cards.Add(card);

        foreach (var message in cards)
        {
            await MessageClient.SendAsync(WebHookUrl, message);
        }
    }

    private static void AddSections(Message card, KeyValuePair<string, IEnumerable<HealthCheckStatus>> healthCheckStatus)
    {
        foreach (var val in healthCheckStatus.Value)
        {
            card.Sections.Add(new Section
            {
                ActivityTitle = healthCheckStatus.Key,
                Markdown = true,
                Facts = new List<Fact>
                {
                    new()
                    {
                        Name = Constants.Facts.Message,
                        Value = val.Message
                    },
                    new ()
                    {
                        Name = Constants.Facts.ReadMore,
                        Value = val.ReadMoreLink
                    },
                    new()
                    {
                        Name = Constants.Facts.ResultType,
                        Value = val.ResultType.ToString()
                    }
                }
            });
        }
    }
}