# Examples

| Example | Description |
| - | - |
| [Tutorial](Tutorial/) | Display a basic static image |

# FAQ

## How do I change the local port my app runs on?

Update `launchsettings.json`: change `"applicationUrl": "http://localhost:5500"` to the port you want to use.

## When I run my server, it opens the web browser

Update `launchsettings.json`: delete `"launchBrowser": true` from `launchSettings.json`

## I see `Unable to configure browser refresh script injection on the response.` in the logs

Update `launchsettings.json`: add `"hotReloadEnabled": false` into `launchSettings.json`

## I don't see any image in the Warpcast Frame Developer Validator

If you're using `ngrok`, visit your ngrok site e.g. `https://1234-123-456-78-900.ngrok-free.app` and click the "Visit Site" button. Go back to Warpcast and click the refresh button (🔃) and your fapp should now work.

## Something's wrong, how can I see ASP .NET debug log messages?

Update `appsettings.Development.json` as follows:

```json
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Debug"
    }
  }
```