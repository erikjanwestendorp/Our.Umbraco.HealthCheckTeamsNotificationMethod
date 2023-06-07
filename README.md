# HealthCheckTeamsNotificationMethod


## appSettings.json 

```json
  "Umbraco": {
    "CMS": {
      "HealthChecks": {
        "Notification": {
          "Enabled": true,
          "NotificationMethods": {
            "teams": {
              "Enabled": true,
              "Verbosity": "Detailed",
              "Settings": {
                "webHookUrl": "your_webhook_here"
              }
            }
          }
        }
      }
    }
  }
```
