using ModernMewgibot.Models;
using ModernMewgibot.Services;
using ModernMewgibot.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace ModernMewgibot.ViewModels
{
    class BotViewModel : INotifyPropertyChanged
    {
        public static TwitchClient client = null;

        private List<string> _permittedUsers;

        private List<Caster> _thankedCasters;
        public List<Caster> ThankedCasters
        {
            get { return _thankedCasters; }
            set
            {
                _thankedCasters = value;
                OnPropertyChanged();
            }
        }

        private List<BotCommand> _commands;
        public List<BotCommand> Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                OnPropertyChanged();
                SaveCommands();
            }
        }

        private List<Quote> _quotes;
        public List<Quote> Quotes
        {
            get { return _quotes; }
            set
            {
                _quotes = value;
                OnPropertyChanged();
                SaveQuotes();
            }
        }

        private string messageToSend = "";
        public string MessageToSend
        {
            get { return messageToSend; }
            set
            {
                messageToSend = value;
                OnPropertyChanged();
            }
        }

        private string _consoleText = "";
        public string ConsoleText
        {
            get { return _consoleText; }
            set
            {
                _consoleText = value;
                _log = value;
                OnPropertyChanged();
            }
        }

        private string _log = "";
        public string Log {
            get { return _log; }
            set { _log = value; }
        }

        public BotViewModel()
        {
            Task.Factory.StartNew(async () => await LoadAllData());
            //Task.Factory.StartNew(async () => { await Task.Delay(TimeSpan.FromSeconds(20)); ConsoleText = "20 seconds passed. Update RTB Text."; });

            client = new TwitchClient(new ConnectionCredentials(SettingsService.Username, SettingsService.OAuth));
            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(5, TimeSpan.FromSeconds(10));
            client.AddChatCommandIdentifier('!');

            client.OnConnected += Client_OnConnected;
            client.OnClientLeftChannel += Client_OnClientLeftChannel;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnMessageSent += Client_OnMessageSent;
            client.OnHostingStarted += Client_OnHostingStarted;
            

            Task.Factory.StartNew(async () => { ConsoleText = "Connecting...\r\n";  await Task.Delay(5); client.Connect(); });
            TwitchApi.SetClientId(SettingsService.ClientID);
        }

        private void CreateCommand(ChatCommand cmd)
        {
            List<string> args = cmd.ArgumentsAsList;

            string trigger = null;
            string response = null;
            BotCommand.ChatLevel accessLevel = BotCommand.ChatLevel.None;

            switch (args.Count)
            {
                case 2:
                    trigger = args[0].ToLower();
                    response = args[1];
                    break;
                case 3:
                    if (args[3].ToLower() == "mod" || args[3].ToLower() == "moderator")
                        accessLevel = BotCommand.ChatLevel.Moderator;
                    else if (args[3].ToLower() == "caster" || args[3].ToLower() == "broadcaster")
                        accessLevel = BotCommand.ChatLevel.Broadcaster;
                    goto case 2;
                default:
                    client.SendMessage("Invalid syntax. Please try again.");
                    break;
            }

            // Break out of method if invalid addcmd syntax
            if (String.IsNullOrEmpty(trigger))
            {
                return;
            }

            // Format trigger
            if (trigger.StartsWith("!"))
                trigger = trigger.Substring(1);

            // List of built-in commands
            List<string> botCmds = new List<string>()
            {
                "addcmd",
                "delcmd",
                "addquote",
                "delquote",
                "quote",
                "permit"
            };

            // Verify command doesn't already exist
            BotCommand c = _commands.Find(command => command.Trigger == trigger);
            if (c != null || botCmds.Contains(trigger))
            {
                client.SendMessage($"!{trigger} already exists.");
                return;
            }

            // TODO: Format response?

            // Create command
            c = new BotCommand
            {
                Trigger = trigger,
                Response = response,
                AccessLevel = accessLevel
            };

            // Add command to commands list
            _commands.Add(c);

            // Save commands
            SaveCommands();

            client.SendMessage($"!{trigger} created.");
        }

        private void DeleteCommand(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage($"Invalid syntax.");
                return;
            }

            string trigger = cmd.ArgumentsAsList[0].ToLower();

            if (trigger.StartsWith("!"))
                trigger = trigger.Substring(1);

            BotCommand command = _commands.Find(c => c.Trigger == trigger);
            if (command != null)
            {
                _commands.Remove(command);
                SaveCommands();
                client.SendMessage($"Command !{trigger} removed.");
            }
            else
                client.SendMessage($"Command !{trigger} not found.");
        }

        private void ExecuteCommand(ChatCommand cmd)
        {
            BotCommand command = _commands.Find(c => c.Trigger == cmd.Command);

            // Verify command exists
            if (command == null)
            {
                client.SendMessage("Invalid command.");
                return;
            }

            // Verify user has permissions to execute command
            if (command.AccessLevel == BotCommand.ChatLevel.Moderator)
            {
                if (!cmd.ChatMessage.IsModerator && !cmd.ChatMessage.IsBroadcaster)
                {
                    client.SendMessage($"Only moderators can use command !{cmd.Command}");
                    return;
                }
            }
            else if (command.AccessLevel == BotCommand.ChatLevel.Broadcaster)
            {
                if (!cmd.ChatMessage.IsBroadcaster)
                {
                    client.SendMessage($"Only the broadcaster can use command !{cmd.Command}");
                    return;
                }
            }

            // Retrieve response from command
            string message = command.Response;

            // TODO: Replace variables in message
            message = message.Replace("%u%", cmd.ChatMessage.DisplayName);
            message = message.Replace("%U%", cmd.ChatMessage.DisplayName);

            if (cmd.ArgumentsAsList.Count > 0)
            {
                message = message.Replace("%t%", cmd.ArgumentsAsList[0]);
                message = message.Replace("%T%", cmd.ArgumentsAsList[0]);
            }
            else
            {
                message = message.Replace("%t%", cmd.ChatMessage.DisplayName);
                message = message.Replace("%T%", cmd.ChatMessage.DisplayName);
            }

            // Execute command
            client.SendMessage(message);
        }

        private void CreateQuote(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid quote syntax. Try surrounding the quote in quotation marks (\")");
                return;
            }

            Quote quote = _quotes.Find(q => q.Message.ToLower().Trim() == cmd.ArgumentsAsList[0].ToLower().Trim());
            if (quote != null)
            {
                client.SendMessage($"Quote already exists.");
                return;
            }

            int id = _quotes.Count + 1;
            string msg = cmd.ArgumentsAsList[0];

            quote = new Quote
            {
                QuoteID = id,
                Message = msg,
                TimeStamp = DateTimeOffset.Now
            };

            _quotes.Add(quote);

            SaveQuotes();

            client.SendMessage($"Quote added with ID {quote.QuoteID}");
        }

        private void DeleteQuote(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid syntax.");
                return;
            }

            int id;
            Quote quote;

            if (int.TryParse(cmd.ArgumentsAsList[0], out id))
            {
                quote = _quotes.Find(q => q.QuoteID == id);
                if (quote != null)
                {
                    _quotes.Remove(quote);
                    FixQuoteIDs();
                    SaveQuotes();
                    client.SendMessage($"Quote with ID {id} removed. IDs of remaining quotes may be altered to remain unique.");
                }
                else
                    client.SendMessage($"Quote with ID {id} does not exist.");
            }
            else
                client.SendMessage($"{cmd.ArgumentsAsList[0]} is not a valid Quote ID.");
        }

        private void SendQuote(ChatCommand cmd)
        {
            if (_quotes.Count == 0)
            {
                client.SendMessage("No quotes exist.");
                return;
            }

            if (cmd.ArgumentsAsList.Count == 0)
            {
                Random rand = new Random();
                Quote quote = _quotes[rand.Next(0, _quotes.Count)];

                client.SendMessage($"Quote {quote.QuoteID}: \"{quote.Message}\" - { quote.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss") }");
            }
            else if (cmd.ArgumentsAsList.Count == 1)
            {
                int id;
                if (int.TryParse(cmd.ArgumentsAsList[0], out id))
                {
                    Quote quote = _quotes.Find(q => q.QuoteID == id);

                    if (quote == null)
                        client.SendMessage($"Quote with ID {id} does not exist.");
                    else
                        client.SendMessage($"Quote {quote.QuoteID}: \"{quote.Message}\" - { quote.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss") }");
                }
                else
                    client.SendMessage($"{cmd.ArgumentsAsList[0]} is not a valid Quote ID.");
            }
            else
                client.SendMessage("Invalid syntax.");
        }

        private void FixQuoteIDs()
        {
            for (int i = 0; i < _quotes.Count; i++)
                _quotes[i].QuoteID = i + 1;
        }

        private async void ThankCaster(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid syntax.");
                return;
            }
            if (await IsValidChannel(cmd.ArgumentsAsList[0]))
            {
                Caster thanks = new Caster { Username = cmd.ArgumentsAsList[0], Channel = $"https://www.twitch.tv/{cmd.ArgumentsAsList[0]}", Game = await GetGame(cmd.ArgumentsAsList[0]) };

                _thankedCasters.Add(thanks);

                client.SendMessage($"Follow { thanks.Username } over at { thanks.Channel } ! Last seen playing { thanks.Game }!");
                SaveThanks();
            }
            else
                client.SendMessage($"{ cmd.ArgumentsAsList[0] } is not a valid Twitch user.");
        }

        private async Task<bool> IsValidChannel(string caster)
        {
            try
            {
                TwitchLib.TwitchAPIClasses.Channel channel = await TwitchApi.GetTwitchChannel(caster);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetGame(string caster)
        {
            try
            {
                TwitchLib.TwitchAPIClasses.Channel channel = await TwitchApi.GetTwitchChannel(caster);

                return channel.Game;
            }
            catch
            {
                return "Unknown";
            }
        }

        private void SaveCommands()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFile(_commands, @".\Data\commands.json"));
        }

        private void SaveQuotes()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFile(_quotes, @".\Data\quotes.json"));
        }

        private void SaveThanks()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFile(_thankedCasters, @".\Logs\thanks.json"));
        }

        public void SaveLog()
        {
            Log += "End of MewgiBot session.\r\n";
            string date = DateTimeOffset.Now.ToString("MMddyyyy");
            string filename = @".\Logs\log-" + date + ".txt";

            File.AppendAllText(filename, Log);
        }

        public static void SwitchChannels(string channelToJoin)
        {
            if (client.JoinedChannels.Count > 0)
            {
                foreach (var channel in client.JoinedChannels)
                {
                    client.LeaveChannel(channel);
                }
            }

            client.JoinChannel(channelToJoin);
        }

        private void Client_OnConnected(object sender, TwitchClient.OnConnectedArgs e)
        {
            ConsoleText += $"Connected to Twitch chat servers.\r\nJoining { SettingsService.Channel } as { e.Username }\r\n";
            client.JoinChannel(SettingsService.Channel);
        }

        private void Client_OnMessageSent(object sender, TwitchClient.OnMessageSentArgs e)
        {
            ConsoleText += "> " + e.Message + "\r\n";
        }

        private void Client_OnMessageReceived(object sender, TwitchClient.OnMessageReceivedArgs e)
        {
            HandleLinks(e.ChatMessage);

            ConsoleText += $"{e.ChatMessage.DisplayName}: {e.ChatMessage.Message}\r\n";
        }

        private void Client_OnChatCommandReceived(object sender, TwitchClient.OnChatCommandReceivedArgs e)
        {
            if (e.Command.ChatMessage.IsBroadcaster || e.Command.ChatMessage.IsModerator)
            {
                if (e.Command.Command == "addcmd")
                {
                    CreateCommand(e.Command);
                    return;
                }
                else if (e.Command.Command == "delcmd")
                {
                    DeleteCommand(e.Command);
                    return;
                }
                else if (e.Command.Command == "permit")
                {
                    PermitUser(e.Command);
                    return;
                }
                else if (e.Command.Command == "delquote")
                {
                    DeleteQuote(e.Command);
                    return;
                }
                else if (e.Command.Command == "caster")
                {
                    ThankCaster(e.Command);
                    return;
                }
                else if (e.Command.Command == "!purge")
                {
                    PurgeUser(e.Command);
                }
            }

            if (e.Command.Command == "addquote")
            {
                CreateQuote(e.Command);
                return;
            }
            else if (e.Command.Command == "quote")
            {
                SendQuote(e.Command);
                return;
            }
            else if (e.Command.Command == "song")
            {
                DisplaySong(e.Command);
            }

            ExecuteCommand(e.Command);
        }

        private void Client_OnJoinedChannel(object sender, TwitchClient.OnJoinedChannelArgs e)
        {
            ConsoleText += $"Joined { e.Channel }\r\n";
        }

        private void Client_OnClientLeftChannel(object sender, TwitchClient.OnClientLeftChannelArgs e)
        {
            ConsoleText += $"Left { e.Channel }\r\n";
        }

        private async void Client_OnHostingStarted(object sender, TwitchClient.OnHostingStartedArgs e)
        {
            ConsoleText += $"HOST INFO - { e.TargetChannel } is being hosted by { e.HostingChannel } for { e.Viewers } viewers.\r\n";

            if (e.TargetChannel == client.JoinedChannels[0].Channel)
            {
                Caster thanks = new Caster { Username = e.HostingChannel, Channel = $"https://www.twitch.tv/{e.HostingChannel}", Game = await GetGame(e.HostingChannel) };
                _thankedCasters.Add(thanks);

                client.SendMessage($"Follow { thanks.Username } over at { thanks.Channel } ! Last seen playing { thanks.Game }!");
            }
        }

        private void Client_OnChatCleared(object sender, TwitchClient.OnChatClearedArgs e)
        {
            ConsoleText += $"Chat in { e.Channel } was cleared.\r\n";
        }

        private void Client_OnViewerBanned(object sender, TwitchClient.OnViewerBannedArgs e)
        {
            string reason = (e.BanReason == "") ? "No reasoning provided." : e.BanReason;
            ConsoleText += $"{ e.Viewer } was banned in { e.Channel } with reasoning: { reason }\r\n";
        }

        private void Client_OnViewerTimedout(object sender, TwitchClient.OnViewerTimedoutArgs e)
        {
            string reason = (e.TimeoutReason == "") ? "No reasoning provided." : e.TimeoutReason;
            ConsoleText += $"{ e.Viewer } was banned in { e.Channel }  for { e.TimeoutDuration } seconds with reasoning: { reason }\r\n";
        }

        private void ChatThrottler_OnClientThrottled(object sender, TwitchLib.Services.MessageThrottler.OnClientThrottledArgs e)
        {
            ConsoleText += $"Outgoing message '{ e.Message }' was blocked by the message throttler.\r\n";
        }

        private void PermitUser(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count == 0)
                client.SendMessage($"No user specified. Please try again.");
            else
            {
                _permittedUsers.Add(cmd.ArgumentsAsList[0].ToLower());
                client.SendMessage($"{cmd.ArgumentsAsList[0]} is permitted to post a single link.");
            }
        }

        private void HandleLinks(ChatMessage message)
        {
            if (SettingsService.LinkModEnabled)
            {
                string linkRegex = @"(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]";
                Regex reg = new Regex(linkRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (reg.IsMatch(message.Message))
                {
                    if (_permittedUsers.Contains(message.DisplayName.ToLower()))
                    {
                        _permittedUsers.Remove(message.DisplayName.ToLower());
                        return;
                    }
                    else if (message.IsBroadcaster || message.IsModerator || message.IsMe)
                    {
                        return;
                    }
                    else
                    {
                        client.TimeoutUser(message.Username, TimeSpan.FromSeconds(10), $"{message.DisplayName} has not been allowed to post a link.");
                    }
                }
            }
        }

        private void PurgeUser(ChatCommand cmd)
        {
            if (!SettingsService.PurgeEnabled)
                return;

            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid syntax.");
                return;
            }

            client.TimeoutUser(cmd.ArgumentsAsList[0], TimeSpan.FromSeconds(1));
            return;
        }

        private void DisplaySong(ChatCommand command)
        {
            if (!SettingsService.SongEnabled)
                return;

            if (SettingsService.SongFile == "Not Configured")
            {
                client.SendMessage("!song has not been configured.");
                return;
            }

            if ((!File.Exists(SettingsService.SongFile)) || String.IsNullOrEmpty(File.ReadAllText(SettingsService.SongFile)))
            {
                client.SendMessage("Unknown Song.");
                return;
            }

            client.SendMessage($"Current song: {File.ReadAllText(SettingsService.SongFile)}");
        }

        private async Task LoadAllData()
        {
            _commands = await JsonFileService.LoadFromFile<List<BotCommand>>(@".\Data\commands.json");
            _quotes = await JsonFileService.LoadFromFile<List<Quote>>(@".\Data\quotes.json");

            if (_commands == null)
                _commands = new List<BotCommand>();
            if (_quotes == null)
                _quotes = new List<Quote>();

            _permittedUsers = new List<string>();
            _thankedCasters = new List<Caster>();
        }

        private void SaveAllData()
        {
            SaveCommands();
            SaveQuotes();
            SaveThanks();
            SaveLog();
        }

        ICommand sendMessage;
        public ICommand SendMessageCommand =>
            sendMessage ?? (sendMessage = new Command(() => SendMessage()));
        private void SendMessage()
        {
            Console.WriteLine(MessageToSend);
            client.SendMessage(MessageToSend);
            MessageToSend = "";
        }

        ICommand windowClosing;
        public ICommand WindowClosing =>
            windowClosing ?? (windowClosing = new Command(() => HandleClosing()));
        private void HandleClosing()
        {
            SaveAllData();

            if (_thankedCasters.Count > 0)
            {
                Process.Start("notepad.exe", @".\Logs\thanks.json");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
