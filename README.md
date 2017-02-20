# modern-mewgibot
Modern MewgiBot is a Twitch chat bot used for storing customized commands, quotes, etc. It makes use of the Open Source projects [TwitchLib](https://github.com/swiftyspiffy/TwitchLib) and [ModernUI](https://github.com/firstfloorsoftware/mui).
It also makes use of the [jService.io](http://jservice.io) web API, and [SwiftySpiffy's](https://github.com/swiftyspiffy) [Twitch Token Generator](https://twitchtokengenerator.com).

# Getting Started
To get starting with customizing Modern Mewgibot, do the following:
1. Clone the repo
2. Restore the NuGet packages
3. Set the value of defaultUsername and defaultOAuth in BotViewModel.cs to values corresponding to whatever account you would like the bot to use by default.
4. Set the value of ClientID in Bot.settings to the client ID you generate in your [Twitch Connections](https://www.twitch.tv/settings/connections).