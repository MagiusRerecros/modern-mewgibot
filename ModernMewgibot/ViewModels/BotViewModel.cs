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
using TwitchLib.Extensions.Client;
using TwitchLib.Events.Client;
using TwitchLib.Events.Services.MessageThrottler;
using TwitchLib.Models.API;
using TwitchLib.Models.Client;
using ModernMewgibot.Dialogs;
using FirstFloor.ModernUI.Windows.Controls;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ModernMewgibot.ViewModels
{
    class BotViewModel : INotifyPropertyChanged
    {
        public static TwitchClient client = null;

        private static System.Timers.Timer hostTimer;
        private static System.Timers.Timer triviaTimer;

        private string defaultUsername = "DefaultUsername";
        private string defaultOAuth = "OAuthForDefaultUser";

        private string _channelAccessToken = SettingsService.ChannelAccessToken;
        public string ChannelAccessToken
        {
            get { return _channelAccessToken; }
            set
            {
                _channelAccessToken = value;
                SettingsService.ChannelAccessToken = _channelAccessToken;
                OnPropertyChanged();
            }
        }

        private string _channelTitle;
        public string ChannelTitle
        {
            get { return _channelTitle; }
            set
            {
                _channelTitle = value;
                OnPropertyChanged();
            }
        }

        private string _channelGame;
        public string ChannelGame
        {
            get { return _channelGame; }
            set
            {
                _channelGame = value;
                OnPropertyChanged();
            }
        }

        private List<string> _permittedUsers;
        public List<string> PermittedUsers
        {
            get { return _permittedUsers; }
            set
            {
                _permittedUsers = value;
                OnPropertyChanged();
            }
        }

        private string _queueJoinCommand = "!joinqueue";
        public string QueueJoinCommand
        {
            get { return _queueJoinCommand; }
            set
            {
                _queueJoinCommand = value;
                OnPropertyChanged();
            }
        }

        private List<string> _queueAllUsers;
        public List<string> QueueAllUsers
        {
            get { return _queueAllUsers; }
            set
            {
                _queueAllUsers = value;
                OnPropertyChanged();
            }
        }

        private List<string> _queueCurrentUsers;
        public List<string> QueueCurrentUsers
        {
            get { return _queueCurrentUsers; }
            set
            {
                _queueCurrentUsers = value;
                OnPropertyChanged();
            }
        }

        private bool _queueActive = false;
        public bool QueueActive
        {
            get { return _queueActive; }
            set
            {
                _queueActive = value;
                OnPropertyChanged();
            }
        }
        public bool QueueInactive
        {
            get { return !( _queueActive ); }
        }

        private bool _queueFollow = true;
        public bool QueueFollow
        {
            get { return _queueFollow; }
            set
            {
                _queueFollow = value;
                OnPropertyChanged();
            }
        }

        private bool _queueSub = false;
        public bool QueueSub
        {
            get { return _queueSub; }
            set
            {
                _queueSub = value;
                OnPropertyChanged();
            }
        }

        private int _queueNumberUsers = 1;
        public int QueueNumberUsers
        {
            get { return _queueNumberUsers; }
            set
            {
                _queueNumberUsers = value;
                OnPropertyChanged();
            }
        }

        private List<Regular> _regulars;
        public List<Regular> Regulars
        {
            get { return _regulars; }
            set
            {
                _regulars = value;
                OnPropertyChanged();
            }
        }

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

        private List<Link> _whitelist;
        public List<Link> Whitelist
        {
            get { return _whitelist; }
            set
            {
                _whitelist = value;
                OnPropertyChanged();
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

        private string _raffleJoinCommand = "!joinraffle";
        public string RaffleJoinCommand
        {
            get { return _raffleJoinCommand; }
            set
            {
                _raffleJoinCommand = value;
                OnPropertyChanged();
            }
        }

        //Temporarily hard-coding the number of simultaneous raffles
        private string _raffleJoinCommand2 = "!joinraffle2";
        public string RaffleJoinCommand2
        {
            get { return _raffleJoinCommand2; }
            set
            {
                _raffleJoinCommand2 = value;
                OnPropertyChanged();
            }
        }

        private string _raffleJoinCommand3 = "!joinraffle3";
        public string RaffleJoinCommand3
        {
            get { return _raffleJoinCommand3; }
            set
            {
                _raffleJoinCommand3 = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleFollow = true;
        public bool RaffleFollow
        {
            get { return _raffleFollow; }
            set
            {
                _raffleFollow = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleFollow2 = true;
        public bool RaffleFollow2
        {
            get { return _raffleFollow2; }
            set
            {
                _raffleFollow2 = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleFollow3 = true;
        public bool RaffleFollow3
        {
            get { return _raffleFollow3; }
            set
            {
                _raffleFollow3 = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleSub = false;
        public bool RaffleSub
        {
            get { return _raffleSub; }
            set
            {
                _raffleSub = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleSub2 = false;
        public bool RaffleSub2
        {
            get { return _raffleSub2; }
            set
            {
                _raffleSub2 = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleSub3 = false;
        public bool RaffleSub3
        {
            get { return _raffleSub3; }
            set
            {
                _raffleSub3 = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleEntries = new List<RaffleUser>();
        public List<RaffleUser> RaffleEntries
        {
            get { return _raffleEntries; }
            set
            {
                _raffleEntries = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleEntries2 = new List<RaffleUser>();
        public List<RaffleUser> RaffleEntries2
        {
            get { return _raffleEntries2; }
            set
            {
                _raffleEntries2 = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleEntries3 = new List<RaffleUser>();
        public List<RaffleUser> RaffleEntries3
        {
            get { return _raffleEntries3; }
            set
            {
                _raffleEntries3 = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleWinners = new List<RaffleUser>();
        public List<RaffleUser> RaffleWinners
        {
            get { return _raffleWinners; }
            set
            {
                _raffleWinners = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleWinners2 = new List<RaffleUser>();
        public List<RaffleUser> RaffleWinners2
        {
            get { return _raffleWinners2; }
            set
            {
                _raffleWinners2 = value;
                OnPropertyChanged();
            }
        }

        private List<RaffleUser> _raffleWinners3 = new List<RaffleUser>();
        public List<RaffleUser> RaffleWinners3
        {
            get { return _raffleWinners3; }
            set
            {
                _raffleWinners3 = value;
                OnPropertyChanged();
            }
        }

        private int _raffleCooldown = 10;
        public int RaffleCooldown
        {
            get { return _raffleCooldown; }
            set
            {
                _raffleCooldown = value;
                OnPropertyChanged();
            }
        }

        private int _raffleCooldown2 = 10;
        public int RaffleCooldown2
        {
            get { return _raffleCooldown2; }
            set
            {
                _raffleCooldown2 = value;
                OnPropertyChanged();
            }
        }

        private int _raffleCooldown3 = 10;
        public int RaffleCooldown3
        {
            get { return _raffleCooldown3; }
            set
            {
                _raffleCooldown3 = value;
                OnPropertyChanged();
            }
        }

        private int _raffleDraw = 0;

        private List<IntervalMessage> _intervalMessages;
        public List<IntervalMessage> IntervalMessages
        {
            get { return _intervalMessages; }
            set
            {
                _intervalMessages = value;
                OnPropertyChanged();
            }
        }

        private IntervalMessage _selectedMessage;
        public IntervalMessage SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage = value;
                OnPropertyChanged();
            }
        }

        private Regular _selectedRegular;
        public Regular SelectedRegular
        {
            get { return _selectedRegular; }
            set
            {
                _selectedRegular = value;
                OnPropertyChanged();
            }
        }

        private BotCommand _selectedCommand;
        public BotCommand SelectedCommand
        {
            get { return _selectedCommand; }
            set
            {
                _selectedCommand = value;
                OnPropertyChanged();
            }
        }

        private Quote _selectedQuote;
        public Quote SelectedQuote
        {
            get { return _selectedQuote; }
            set
            {
                _selectedQuote = value;
                OnPropertyChanged();
            }
        }

        private Link _selectedLink;
        public Link SelectedLink
        {
            get { return _selectedLink; }
            set
            {
                _selectedLink = value;
                OnPropertyChanged();
            }
        }

        private string _selectedPermit;
        public string SelectedPermit
        {
            get { return _selectedPermit; }
            set
            {
                _selectedPermit = value;
                OnPropertyChanged();
            }
        }

        private TriviaQuestion _currentQuestion;
        public TriviaQuestion CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                _currentQuestion = value;
                OnPropertyChanged();
            }
        }

        private bool _triviaActive = false;
        public bool TriviaActive
        {
            get { return _triviaActive; }
            set
            {
                _triviaActive = value;
                OnPropertyChanged();
            }
        }
        public bool TriviaInactive
        {
            get { return !( _triviaActive ); }
        }

        private List<ScheduledMessage> _scheduledMessages = new List<ScheduledMessage>();

        private bool _raffleActive = false;
        public bool RaffleActive
        {
            get { return _raffleActive; }
            set
            {
                _raffleActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RaffleInactive));
            }
        }
        public bool RaffleInactive
        {
            get { return !( _raffleActive ); }
        }

        private bool _raffleActive2 = false;
        public bool RaffleActive2
        {
            get { return _raffleActive2; }
            set
            {
                _raffleActive2 = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RaffleInactive2));
            }
        }
        public bool RaffleInactive2
        {
            get { return !(_raffleActive2); }
        }

        private bool _raffleActive3 = false;
        public bool RaffleActive3
        {
            get { return _raffleActive3; }
            set
            {
                _raffleActive3 = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RaffleInactive3));
            }
        }
        public bool RaffleInactive3
        {
            get { return !(_raffleActive3); }
        }

        private bool _raffleKeepOpen = false;
        public bool RaffleKeepOpen
        {
            get { return _raffleKeepOpen; }
            set
            {
                _raffleKeepOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleKeepOpen2 = false;
        public bool RaffleKeepOpen2
        {
            get { return _raffleKeepOpen2; }
            set
            {
                _raffleKeepOpen2 = value;
                OnPropertyChanged();
            }
        }

        private bool _raffleKeepOpen3 = false;
        public bool RaffleKeepOpen3
        {
            get { return _raffleKeepOpen3; }
            set
            {
                _raffleKeepOpen3 = value;
                OnPropertyChanged();
            }
        }

        public BotViewModel()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Task.Factory.StartNew(async () => await LoadAllDataAsync());

            if (SettingsService.Username != "")
            {
                ConfigureClient();

                Task.Factory.StartNew(async () => { ConsoleText = "Connecting...\r\n"; await Task.Delay(5); client.Connect(); });
            }
            else
            {
                HandleNoLoginAsync();
            }

            TwitchApi.SetClientId(SettingsService.ClientID);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SaveAllData();
            Exception ex = (Exception)e.ExceptionObject;
            ModernDialog.ShowMessage($"An unexpected error has occurred. Data has been saved.\nError message: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK);

            if (e.IsTerminating)
                ModernDialog.ShowMessage($"Mewgibot is now exiting due to the error.", "Exiting", System.Windows.MessageBoxButton.OK);
        }

        private async void HandleNoLoginAsync()
        {
            await Task.Delay(500);
            var dialog = ModernDialog.ShowMessage($"No Twitch account was detected for the bot. Log in as {defaultUsername}?", "Log In", System.Windows.MessageBoxButton.YesNo);
            if (dialog == System.Windows.MessageBoxResult.Yes)
                HandleMewgibotLogin();
            else
                ConnectNewCredentials();
        }

        private void ConfigureClient()
        {
            client = new TwitchClient(new ConnectionCredentials(SettingsService.Username, SettingsService.OAuth))
            {
                ChatThrottler = new TwitchLib.Services.MessageThrottler(5, TimeSpan.FromSeconds(10))
            };

            client.AddChatCommandIdentifier('!');

            client.OnConnected += Client_OnConnected;
            client.OnClientLeftChannel += Client_OnClientLeftChannel;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnMessageSent += Client_OnMessageSent;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnReSubscriber += Client_OnReSubscriber;
            //client.OnHostingStarted += Client_OnHostingStarted;
        }

        private void ConfigureScheduledMessages()
        {
            ConsoleText += "Starting Scheduled Messages...\r\n";

            _scheduledMessages.Clear();

            foreach (IntervalMessage message in IntervalMessages)
            {
                if (message.Message != "Chat Message")
                {
                    ScheduledMessage scheduledMessage = new ScheduledMessage(message);

                    _scheduledMessages.Add(scheduledMessage);

                    //scheduledMessage.OnScheduledMessage -= Message_OnScheduledMessage;
                    scheduledMessage.OnScheduledMessage += Message_OnScheduledMessage;
                    scheduledMessage.StartTimer();
                }
            }
        }

        private void CreateCommand(ChatCommand cmd)
        {
            List<string> args = cmd.ArgumentsAsList;

            bool enabled = true;
            string trigger = null;
            string response = null;
            int interval = 0;
            ChatLevel accessLevel = ChatLevel.None;

            switch (args.Count)
            {
                case 2:
                    trigger = args[0].ToLower();
                    response = args[1];
                    break;
                case 3:
                    if (args[2].ToLower() == "mod" || args[2].ToLower() == "moderator")
                        accessLevel = ChatLevel.Moderator;
                    else if (args[2].ToLower() == "caster" || args[2].ToLower() == "broadcaster")
                        accessLevel = ChatLevel.Broadcaster;
                    goto case 2;
                case 4:
                    if (int.TryParse(args[3], out int result))
                        interval = result;
                    goto case 3;
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
            BotCommand c = _commands.Find(command => command.Trigger.ToLower() == trigger.ToLower());
            if (c != null || botCmds.Contains(trigger))
            {
                client.SendMessage($"!{trigger} already exists.");
                return;
            }

            // Verify command isn't using Twitch commands on bot's behalf
            if (response.StartsWith("/") && !response.StartsWith("/me"))
            {
                client.SendMessage("You think you can control ME!? TooSpicy");
                return;
            }

            // TODO: Format response?

            // Create command
            c = new BotCommand
            {
                Enabled = enabled,
                Trigger = trigger,
                Response = response,
                AccessLevel = accessLevel,
                Interval = interval
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

            string trigger = cmd.ArgumentsAsList[0];

            if (trigger.StartsWith("!"))
                trigger = trigger.Substring(1);

            BotCommand command = _commands.Find(c => c.Trigger.ToLower() == trigger.ToLower());
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
            BotCommand command = _commands.Find(c => c.Trigger.ToLower() == cmd.Command.ToLower());

            // Verify command exists
            if (command == null)
            {
                //client.SendMessage("Invalid command.");
                return;
            }

            // Verify command is enabled
            if (!command.Enabled)
                return;

            // Verify command interval has passed
            if (command.LastUsed + TimeSpan.FromSeconds(command.Interval) > DateTime.Now)
                return;

            // Verify user has permissions to execute command
            if (command.AccessLevel == ChatLevel.Moderator)
            {
                if (!cmd.ChatMessage.IsModerator && !cmd.ChatMessage.IsBroadcaster)
                {
                    client.SendMessage($"Only moderators can use command !{cmd.Command}");
                    return;
                }
            }
            else if (command.AccessLevel == ChatLevel.Broadcaster)
            {
                if (!cmd.ChatMessage.IsBroadcaster)
                {
                    client.SendMessage($"Only the broadcaster can use command !{cmd.Command}");
                    return;
                }
            }

            // Verify bot owner isn't using special commands on bot's behalf
            if (command.Response.StartsWith("/") && !command.Response.StartsWith("/me"))
                return;

            // Retrieve response from command
            string message = command.Response;

            // Replace variables in message
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
            command.LastUsed = DateTimeOffset.Now;
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

            Quote quote;

            if (int.TryParse(cmd.ArgumentsAsList[0], out int id))
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
                if (int.TryParse(cmd.ArgumentsAsList[0], out int id))
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

        private async void ThankCasterAsync(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid syntax.");
                return;
            }
            if (await IsValidChannelAsync(cmd.ArgumentsAsList[0]))
            {
                Caster thanks = new Caster { Username = cmd.ArgumentsAsList[0], Channel = $"https://www.twitch.tv/{cmd.ArgumentsAsList[0]}", Game = await GetGameAsync(cmd.ArgumentsAsList[0]) };

                if (!_thankedCasters.Exists(c => c.Username == thanks.Username))
                    _thankedCasters.Add(thanks);

                client.SendMessage($"Follow { thanks.Username } over at { thanks.Channel } ! Last seen playing { thanks.Game }!");
                SaveThanks();
            }
            else
                client.SendMessage($"{ cmd.ArgumentsAsList[0] } is not a valid Twitch user.");
        }

        private async void ThankCasterAsync(string caster)
        {
            if (await IsValidChannelAsync(caster))
            {
                Caster thanks = new Caster { Username = caster, Channel = $"https://www.twitch.tv/{caster}", Game = await GetGameAsync(caster) };

                if (!_thankedCasters.Exists(c => c.Username == thanks.Username))
                    _thankedCasters.Add(thanks);

                client.SendMessage($"Follow { thanks.Username } over at { thanks.Channel } ! Last seen playing { thanks.Game }!");
                SaveThanks();
            }
            else
                ConsoleText += $"Attempted to thank { caster }, but { caster } is not a valid Twitch user.\r\n";
        }

        private async Task<bool> IsValidChannelAsync(string caster)
        {
            try
            {
                Channel channel = await TwitchApi.Channels.GetChannelAsync(caster);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetGameAsync(string caster)
        {
            try
            {
                Channel channel = await TwitchApi.Channels.GetChannelAsync(caster);

                return channel.Game;
            }
            catch
            {
                return "Unknown";
            }
        }

        private void SaveCommands()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(_commands, @".\Data\commands.json"));
        }

        private void SaveQuotes()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(_quotes, @".\Data\quotes.json"));
        }

        private void SaveScheduledMessages()
        {
            List<IntervalMessage> scheduledMessages = new List<IntervalMessage>();

            foreach (ScheduledMessage message in _scheduledMessages)
            {
                scheduledMessages.Add(new IntervalMessage { Interval = message.intMessage.Interval, Message = message.intMessage.Message });
            }

            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(scheduledMessages, @".\Data\scheduledMessages.json"));
        }

        private void SaveThanks()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(_thankedCasters, @".\Logs\thanks.json"));
        }

        private void SaveRegulars()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(_regulars, @".\Data\regulars.json"));
        }

        private void SaveWhitelist()
        {
            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(_whitelist, @".\Data\whitelist.json"));
        }

        public void SaveLog()
        {
            Log += "End of MewgiBot session.\r\n";
            string date = DateTimeOffset.Now.ToString("MMddyyyy");
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Mewgibot\Logs\log-" + date + ".txt";

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

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            ConsoleText += $"Connected to Twitch chat servers.\r\nJoining { SettingsService.Channel } as { e.Username }\r\n";
            client.JoinChannel(SettingsService.Channel);
        }

        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            ConsoleText += "> " + e.SentMessage.Message + "\r\n";
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleLinks(e.ChatMessage);

            if (TriviaActive)
            {
                HandleTrivia(e.ChatMessage);
            }

            ConsoleText += $"{e.ChatMessage.DisplayName}: {e.ChatMessage.Message}\r\n";
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
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
                else if (SettingsService.QuotesEnabled && e.Command.Command == "delquote")
                {
                    DeleteQuote(e.Command);
                    return;
                }
                else if (e.Command.Command == "caster")
                {
                    ThankCasterAsync(e.Command);
                    return;
                }
                else if (e.Command.Command == "purge")
                {
                    PurgeUser(e.Command);
                    return;
                }
                else if (e.Command.Command == "addregular")
                {
                    AddRegular(e.Command);
                    return;
                }
            }

            if (SettingsService.QuotesEnabled && e.Command.Command == "addquote")
            {
                CreateQuote(e.Command);
                return;
            }
            else if (SettingsService.QuotesEnabled && e.Command.Command == "quote")
            {
                SendQuote(e.Command);
                return;
            }
            else if (e.Command.Command == "song")
            {
                DisplaySong(e.Command);
                return;
            }
            else if (e.Command.Command.ToLower() == RaffleJoinCommand.Substring(1).ToLower() && RaffleActive)
            {
                EnterRaffle(e.Command);
                return;
            }
            else if (e.Command.Command.ToLower() == RaffleJoinCommand2.Substring(1).ToLower() && RaffleActive2)
            {
                EnterRaffle2(e.Command);
                return;
            }
            else if (e.Command.Command.ToLower() == RaffleJoinCommand3.Substring(1).ToLower() && RaffleActive3)
            {
                EnterRaffle3(e.Command);
                return;
            }
            else if (e.Command.Command.ToLower() == QueueJoinCommand.Substring(1).ToLower() && QueueActive)
            {
                JoinQueue(e.Command);
                return;
            }

            ExecuteCommand(e.Command);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ConsoleText += $"Joined { e.Channel }\r\n";

            ConsoleText += "Checking hosts...\r\n";
            HandleHostsAsync(e.Channel);

            ConsoleText += "Now polling for hosts every minute.\r\n";
            hostTimer = new System.Timers.Timer(1 * 60 * 1000);
            hostTimer.Elapsed += (elapsedSender, elapsedE) => HandleHostsAsync(e.Channel);
            hostTimer.AutoReset = true;
            hostTimer.Enabled = true;

            ChannelTitle = TwitchApi.Channels.GetChannel(e.Channel).Status;
            ChannelGame = TwitchApi.Channels.GetChannel(e.Channel).Game;

            Task.Factory.StartNew(async () => await Task.Delay(100));
            ConfigureScheduledMessages();
        }

        private void Client_OnClientLeftChannel(object sender, OnClientLeftChannelArgs e)
        {
            ConsoleText += $"Left { e.Channel }\r\n";
        }

        private void Client_OnHostingStarted(object sender, OnHostingStartedArgs e)
        {
            ConsoleText += $"HOST INFO - { e.TargetChannel } is being hosted by { e.HostingChannel } for { e.Viewers } viewers.\r\n";

            if (e.TargetChannel == client.JoinedChannels[0].Channel)
                ThankCasterAsync(e.HostingChannel);
        }

        private void Client_OnChatCleared(object sender, OnChatClearedArgs e)
        {
            ConsoleText += $"Chat in { e.Channel } was cleared.\r\n";
        }

        private void Client_OnViewerBanned(object sender, OnUserBannedArgs e)
        {
            string reason = (e.BanReason == "") ? "No reasoning provided." : e.BanReason;
            ConsoleText += $"{ e.Username } was banned in { e.Channel } with reasoning: { reason }\r\n";
        }

        private void Client_OnViewerTimedout(object sender, OnUserTimedoutArgs e)
        {
            string reason = (e.TimeoutReason == "") ? "No reasoning provided." : e.TimeoutReason;
            ConsoleText += $"{ e.Username } was banned in { e.Channel }  for { e.TimeoutDuration } seconds with reasoning: { reason }\r\n";
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            if (e.ReSubscriber.IsTwitchPrime)
                ConsoleText += $"{e.ReSubscriber.Login} has resubscribed for {e.ReSubscriber.Months} months, using Twitch Prime.\r\n";
            else
                ConsoleText += $"{e.ReSubscriber.Login} has subscribed for {e.ReSubscriber.Months} months.\r\n";

            if (SettingsService.SubGreetingEnabled)
                client.SendMessage(SettingsService.SubGreeting);
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.IsTwitchPrime)
                ConsoleText += $"{e.Subscriber.Name} has subscribed for the first time, using Twitch Prime.\r\n";
            else
                ConsoleText += $"{e.Subscriber.Name} has subscribed for the first time.\r\n";

            if (SettingsService.SubGreetingEnabled)
                client.SendMessage(SettingsService.SubGreeting);
        }

        private void ChatThrottler_OnClientThrottled(object sender, OnClientThrottledArgs e)
        {
            ConsoleText += $"Outgoing message '{ e.Message }' was blocked by the message throttler.\r\n";
        }

        private void EnterRaffle(ChatCommand cmd)
        {
            string joinCommand;

            if (RaffleJoinCommand.StartsWith("!"))
                joinCommand = RaffleJoinCommand.Substring(1);
            else
                joinCommand = RaffleJoinCommand;

            RaffleUser user = new RaffleUser { Username = cmd.ChatMessage.DisplayName, Raffle = joinCommand };

            if (!RaffleEntries.Exists(u => u.Username == user.Username))
            {
                bool flag = true;

                if (RaffleFollow && !FollowsChannel(user.Username))
                {
                    flag = false;
                }

                if (RaffleSub && !cmd.ChatMessage.Subscriber)
                {
                    flag = false;
                }

                if (cmd.ChatMessage.IsBroadcaster) //Broadcaster can always enter raffle
                {
                    flag = true;
                }

                if (flag)
                {
                    List<RaffleUser> tempEntries = new List<RaffleUser>(RaffleEntries)
                    {
                        user
                    };

                    RaffleEntries = tempEntries;
                }
            }
        }

        private void EnterRaffle2(ChatCommand cmd)
        {
            string joinCommand;

            if (RaffleJoinCommand2.StartsWith("!"))
                joinCommand = RaffleJoinCommand2.Substring(1);
            else
                joinCommand = RaffleJoinCommand2;

            RaffleUser user = new RaffleUser { Username = cmd.ChatMessage.DisplayName, Raffle = joinCommand };

            if (!RaffleEntries2.Contains(user))
            {
                bool flag = true;

                if (RaffleFollow2 && !FollowsChannel(user.Username))
                {
                    flag = false;
                }

                if (RaffleSub2 && !cmd.ChatMessage.Subscriber)
                {
                    flag = false;
                }

                if (cmd.ChatMessage.IsBroadcaster) //Broadcaster can always enter raffle
                {
                    flag = true;
                }

                if (flag)
                {
                    List<RaffleUser> tempEntries = new List<RaffleUser>(RaffleEntries2)
                    {
                        user
                    };

                    RaffleEntries2 = tempEntries;
                }
            }
        }

        private void EnterRaffle3(ChatCommand cmd)
        {
            string joinCommand;

            if (RaffleJoinCommand3.StartsWith("!"))
                joinCommand = RaffleJoinCommand3.Substring(1);
            else
                joinCommand = RaffleJoinCommand3;

            RaffleUser user = new RaffleUser { Username = cmd.ChatMessage.DisplayName, Raffle = joinCommand };

            if (!RaffleEntries3.Contains(user))
            {
                bool flag = true;

                if (RaffleFollow3 && !FollowsChannel(user.Username))
                {
                    flag = false;
                }

                if (RaffleSub3 && !cmd.ChatMessage.Subscriber)
                {
                    flag = false;
                }

                if (cmd.ChatMessage.IsBroadcaster) //Broadcaster can always enter raffle
                {
                    flag = true;
                }

                if (flag)
                {
                    List<RaffleUser> tempEntries = new List<RaffleUser>(RaffleEntries3)
                    {
                        user
                    };

                    RaffleEntries3 = tempEntries;
                }
            }
        }

        private void JoinQueue(ChatCommand cmd)
        {
            string user = cmd.ChatMessage.DisplayName;

            if (!QueueAllUsers.Contains(user))
            {
                bool flag = true;

                if (QueueFollow && !FollowsChannel(user))
                {
                    flag = false;
                }

                if (QueueSub && !cmd.ChatMessage.Subscriber)
                {
                    flag = false;
                }

                if (cmd.ChatMessage.IsBroadcaster) //Broadcaster can always enter queue
                {
                    flag = true;
                }

                if (flag)
                {
                    List<string> temp = new List<string>(QueueAllUsers)
                    {
                        user
                    };

                    QueueAllUsers = temp; 
                }
            }
        }

        private void AddRegular(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count != 1)
            {
                client.SendMessage("Invalid syntax.");
                return;
            }

            string user = cmd.ArgumentsAsList[0];
            Regular regular = new Regular { Username = user };

            List<Regular> tempRegulars = new List<Regular>(Regulars)
            {
                regular
            };

            Regulars = tempRegulars;

            client.SendMessage($"{user} is now a regular.");
        }

        private bool FollowsChannel(string username)
        {
            Follow follow = TwitchApi.Follows.UserFollowsChannel(username, client.JoinedChannels[0].Channel);

            return follow.IsFollowing;
        }

        private void PermitUser(ChatCommand cmd)
        {
            if (cmd.ArgumentsAsList.Count == 0)
            {
                client.SendMessage($"No user specified. Please try again.");
            }
            else
            {
                string username = cmd.ArgumentsAsList[0].ToLower();

                if (!PermittedUsers.Contains(username))
                {
                    List<string> temp = new List<string>(PermittedUsers)
                    {
                        username
                    };

                    PermittedUsers = temp;

                    client.SendMessage($"{username} is permitted to post a single link.");
                }
                else
                    ConsoleText += $"Permit duplicate for {username} ignored.\r\n";
            }
        }

        private void HandleLinks(ChatMessage message)
        {
            if (SettingsService.LinkModEnabled)
            {
                //string linkRegex = @"(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]"; //Triggers too often; too generalized
                //string linkRegex = @"((?!\w+\.+\s\w+\b)\w+\W*(\.|dot|d0t)\W*(aero|asia|biz|cat|com|coop|info|int|jobs|mobi|museum|name|net|org|post|pro|tel|travel|xxx|edu|gov|mil|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b)"; //Triggers too often due to spaces after dots
                //string linkRegex = @"((?!\w+\.+\s\w+\b)\w+(\.|dot|d0t)\W*(aero|asia|biz|cat|com|coop|info|int|jobs|mobi|museum|name|net|org|post|pro|tel|travel|xxx|edu|gov|mil|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b)"; //Triggered on string: kind. "It's
                string linkRegex = @"((?!\w+\.+\s\w+\b)\w+(\.|dot|d0t)(aero|asia|biz|cat|com|coop|info|int|jobs|mobi|museum|name|net|org|post|pro|tel|travel|xxx|edu|gov|mil|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b)";
                Regex reg = new Regex(linkRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                string username = message.DisplayName.ToLower();

                if (reg.IsMatch(message.Message))
                {
                    ConsoleText += $"LINK DETECTED - { message.DisplayName }: { message.Message } \r\n";
                    if (PermittedUsers.Contains(username))
                    {
                        List<string> temp = new List<string>(PermittedUsers);
                        temp.Remove(username);

                        PermittedUsers = temp;

                        return;
                    }
                    else if (message.IsBroadcaster || message.IsModerator || message.IsMe || (Regulars.Exists(r => r.Username.ToLower() == username) && SettingsService.RegularsCanLink) || (message.Subscriber && SettingsService.SubsCanLink))
                    {
                        return;
                    }
                    else
                    {
                        foreach (Link link in _whitelist)
                            if (link.Enabled && message.Message.Contains(link.URL))
                            {
                                ConsoleText += $"LINK MODERATION - {link.URL} is whitelisted. Message allowed.\r\n";
                                return;
                            }

                        client.TimeoutUser(message.Username, TimeSpan.FromSeconds(10), $"{message.DisplayName} has not been allowed to post a link.");
                    }
                }
            }
        }

        private void HandleTrivia(ChatMessage message)
        {
            if (message.Message.ToLower() == CurrentQuestion.Answer.ToLower())
            {
                client.SendMessage($"{ message.DisplayName } has answered correctly! The answer was: { CurrentQuestion.Answer }");

                GetTriviaQuestion();
                triviaTimer.Stop();
                triviaTimer.Start();
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

        private void GetTriviaQuestion()
        {
            string json;

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("http://jservice.io/api/random");
            }

            JArray questionArray = JArray.Parse(json);
            string question = (string)questionArray[0]["question"];
            string answer = (string)questionArray[0]["answer"];

            //Format answer
            answer = Regex.Replace(answer, @"[^\u0000-\u007F]+", string.Empty); //Remove Unicode
            answer = Regex.Replace(answer, @"<[^>]*>", string.Empty); //Remove HTML tags?
            answer = answer.Replace("\'", "'"); //Replace \' with '
            answer = answer.Trim(); //Remove leading/trailing whitespace

            CurrentQuestion = new TriviaQuestion()
            {
                Question = question,
                Answer = answer
            };

            client.SendMessage($"Trivia Question: { CurrentQuestion.Question }");
        }

        private async void HandleHostsAsync(string channel)
        {
            List<string> hosts = await TwitchApi.Channels.GetChannelHostsAsync(channel);

            foreach (string host in hosts)
            {
                if (!ThankedCasters.Exists(c => c.Username == host))
                {
                    ConsoleText += $"{host} is now hosting the channel.\r\n";

                    if (SettingsService.HostAutoThank)
                        ThankCasterAsync(host);
                }
            }
        }

        private async Task LoadAllDataAsync()
        {
            _commands = await JsonFileService.LoadFromFileAsync<List<BotCommand>>(@".\Data\commands.json");
            _quotes = await JsonFileService.LoadFromFileAsync<List<Quote>>(@".\Data\quotes.json");
            _regulars = await JsonFileService.LoadFromFileAsync<List<Regular>>(@".\Data\regulars.json");
            _intervalMessages = await JsonFileService.LoadFromFileAsync<List<IntervalMessage>>(@".\Data\scheduledMessages.json");
            _whitelist = await JsonFileService.LoadFromFileAsync<List<Link>>(@".\Data\whitelist.json");

            if (_commands == null)
                _commands = new List<BotCommand>();
            if (_quotes == null)
                _quotes = new List<Quote>();
            if (_regulars == null)
                _regulars = new List<Regular>();
            if (_whitelist == null)
                _whitelist = new List<Link>();
            if (_intervalMessages == null)
                _intervalMessages = new List<IntervalMessage>() { new IntervalMessage { Interval = 10, Message = "Chat Message" } };

            _permittedUsers = new List<string>();
            _thankedCasters = new List<Caster>();

            string docTempFileName = @".\Docs\Mewgibot Documentation.docx";
            string docFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Mewgibot\Docs\Mewgibot Documentation.docx";

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Mewgibot\Docs"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Mewgibot\Docs");

            File.Copy(docTempFileName, docFileName);
        }

        private void SaveAllData()
        {
            SaveCommands();
            SaveQuotes();
            SaveScheduledMessages();
            SaveRegulars();
            SaveThanks();
            SaveWhitelist();
            SaveLog();
        }

        private void Message_OnScheduledMessage(object sender, Events.ScheduledMessageEventArgs e)
        {
            if (TwitchApi.Streams.BroadcasterOnline(client.JoinedChannels[0].Channel))
                client.SendMessage(e.Message);
        }

        ICommand updateStream;
        public ICommand UpdateStreamCommand =>
            updateStream ?? (updateStream = new Command(() => UpdateStream()));
        private void UpdateStream()
        {
            Task.Factory.StartNew(async () =>
            {
                await TwitchApi.Streams.UpdateStreamTitleAndGameAsync(ChannelTitle, ChannelGame, client.JoinedChannels[0].Channel, ChannelAccessToken);
                ConsoleText += "Channel info updated.\r\n";
            });
        }

        ICommand selectWinner;
        public ICommand SelectWinnerCommand =>
            selectWinner ?? (selectWinner = new Command(() => SelectWinner()));
        private void SelectWinner()
        {
            if (RaffleEntries.Count == 0)
            {
                RaffleActive = false;
                RaffleEntries = new List<RaffleUser>();
                return;
            }

            //Select winner
            Random rand = new Random();
            RaffleUser winner = RaffleEntries[rand.Next(RaffleEntries.Count - 1)];
            winner.LastWin = DateTimeOffset.Now;

            RaffleUser prevWin = RaffleWinners.Find(w => w.Username == winner.Username);

            if (prevWin == null || prevWin.LastWin + TimeSpan.FromMinutes(RaffleCooldown) < DateTimeOffset.Now)
            {
                //Store and announce winner
                List<RaffleUser> tempWinners = new List<RaffleUser>(RaffleWinners)
                {
                    winner
                };

                RaffleWinners = tempWinners;

                client.SendMessage($"Congratulations, { winner.Username }, you have won!");

                if (!RaffleKeepOpen)
                {
                    //Close raffle
                    RaffleActive = false;

                    //Clear entries
                    RaffleEntries = new List<RaffleUser>();
                }
            }
            else
            {
                if (_raffleDraw < 10)
                {
                    _raffleDraw++;
                    SelectWinner();
                }
                else
                {
                    _raffleDraw = 0;
                    client.SendMessage("10 entrants who are still under cooldown were chosen in a row. Try drawing another winner in a few minutes.");
                }
            }
        }

        ICommand selectWinner2;
        public ICommand SelectWinnerCommand2 =>
            selectWinner2 ?? (selectWinner2 = new Command(() => SelectWinner2()));
        private void SelectWinner2()
        {
            if (RaffleEntries2.Count == 0)
            {
                RaffleActive2 = false;
                RaffleEntries2 = new List<RaffleUser>();
                return;
            }

            //Select winner
            Random rand = new Random();
            RaffleUser winner = RaffleEntries2[rand.Next(RaffleEntries2.Count - 1)];
            winner.LastWin = DateTimeOffset.Now;

            RaffleUser prevWin = RaffleWinners2.Find(w => w.Username == winner.Username);

            if (prevWin == null || prevWin.LastWin + TimeSpan.FromMinutes(RaffleCooldown2) < DateTimeOffset.Now)
            {
                //Store and announce winner
                List<RaffleUser> tempWinners = new List<RaffleUser>(RaffleWinners2)
                {
                    winner
                };

                RaffleWinners2 = tempWinners;

                client.SendMessage($"Congratulations, { winner.Username }, you have won!");

                if (!RaffleKeepOpen2)
                {
                    //Close raffle
                    RaffleActive2 = false;

                    //Clear entries
                    RaffleEntries2 = new List<RaffleUser>();
                }
            }
            else
            {
                if (_raffleDraw < 10)
                {
                    _raffleDraw++;
                    SelectWinner2();
                }
                else
                {
                    _raffleDraw = 0;
                    client.SendMessage("10 entrants who are still under cooldown were chosen in a row. Try drawing another winner in a few minutes.");
                }
            }
        }

        ICommand selectWinner3;
        public ICommand SelectWinnerCommand3 =>
            selectWinner3 ?? (selectWinner3 = new Command(() => SelectWinner3()));
        private void SelectWinner3()
        {
            if (RaffleEntries3.Count == 0)
            {
                RaffleActive3 = false;
                RaffleEntries3 = new List<RaffleUser>();
                return;
            }

            //Select winner
            Random rand = new Random();
            RaffleUser winner = RaffleEntries3[rand.Next(RaffleEntries3.Count - 1)];
            winner.LastWin = DateTimeOffset.Now;

            RaffleUser prevWin = RaffleWinners3.Find(w => w.Username == winner.Username);

            if (prevWin == null || prevWin.LastWin + TimeSpan.FromMinutes(RaffleCooldown3) < DateTimeOffset.Now)
            {
                //Store and announce winner
                List<RaffleUser> tempWinners = new List<RaffleUser>(RaffleWinners3)
                {
                    winner
                };

                RaffleWinners3 = tempWinners;

                client.SendMessage($"Congratulations, { winner.Username }, you have won!");

                if (!RaffleKeepOpen3)
                {
                    //Close raffle
                    RaffleActive3 = false;

                    //Clear entries
                    RaffleEntries3 = new List<RaffleUser>();
                }
            }
            else
            {
                if (_raffleDraw < 10)
                {
                    _raffleDraw++;
                    SelectWinner3();
                }
                else
                {
                    _raffleDraw = 0;
                    client.SendMessage("10 entrants who are still under cooldown were chosen in a row. Try drawing another winner in a few minutes.");
                }
            }
        }

        ICommand resetEntries;
        public ICommand ResetEntriesCommand =>
            resetEntries ?? (resetEntries = new Command(() => ResetRaffleEntries()));
        private void ResetRaffleEntries()
        {
            RaffleEntries = new List<RaffleUser>();
        }

        ICommand resetEntries2;
        public ICommand ResetEntriesCommand2 =>
            resetEntries2 ?? (resetEntries2 = new Command(() => ResetRaffleEntries2()));
        private void ResetRaffleEntries2()
        {
            RaffleEntries2 = new List<RaffleUser>();
        }

        ICommand resetEntries3;
        public ICommand ResetEntriesCommand3 =>
            resetEntries3 ?? (resetEntries3 = new Command(() => ResetRaffleEntries3()));
        private void ResetRaffleEntries3()
        {
            RaffleEntries = new List<RaffleUser>();
        }

        ICommand startRaffle;
        public ICommand StartRaffleCommand =>
            startRaffle ?? (startRaffle = new Command(() => StartRaffle()));
        private void StartRaffle()
        {
            RaffleActive = true;
            client.SendMessage($"Raffle started! Type \"{RaffleJoinCommand}\" to enter!");
        }

        ICommand startRaffle2;
        public ICommand StartRaffleCommand2 =>
            startRaffle2 ?? (startRaffle2 = new Command(() => StartRaffle2()));
        private void StartRaffle2()
        {
            RaffleActive2 = true;
            client.SendMessage($"Raffle started! Type \"{ RaffleJoinCommand2 }\" to enter!");
        }

        ICommand startRaffle3;
        public ICommand StartRaffleCommand3 =>
            startRaffle3 ?? (startRaffle3 = new Command(() => StartRaffle3()));
        private void StartRaffle3()
        {
            RaffleActive3 = true;
            client.SendMessage($"Raffle started! Type \"{RaffleJoinCommand3}\" to enter!");
        }

        ICommand cancelRaffle;
        public ICommand CancelRaffleCommand =>
            cancelRaffle ?? (cancelRaffle = new Command(() => CancelRaffle()));
        private void CancelRaffle()
        {
            RaffleActive = false;
            ResetRaffleEntries();
        }

        ICommand cancelRaffle2;
        public ICommand CancelRaffleCommand2 =>
            cancelRaffle2 ?? (cancelRaffle2 = new Command(() => CancelRaffle2()));
        private void CancelRaffle2()
        {
            RaffleActive2 = false;
            ResetRaffleEntries2();
        }

        ICommand cancelRaffle3;
        public ICommand CancelRaffleCommand3 =>
            cancelRaffle3 ?? (cancelRaffle3 = new Command(() => CancelRaffle3()));
        private void CancelRaffle3()
        {
            RaffleActive3 = false;
            ResetRaffleEntries3();
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

        ICommand openQueue;
        public ICommand OpenQueueCommand =>
            openQueue ?? (openQueue = new Command(() => OpenQueue()));
        private void OpenQueue()
        {
            QueueAllUsers = new List<string>();
            QueueCurrentUsers = new List<string>();
            QueueActive = true;

            client.SendMessage($"Queue started, type { QueueJoinCommand } to enter!");
        }

        ICommand closeQueue;
        public ICommand CloseQueueCommand =>
            closeQueue ?? (closeQueue = new Command(() => CloseQueue()));
        private void CloseQueue()
        {
            QueueActive = false;
        }

        ICommand selectUsers;
        public ICommand SelectUsersCommand =>
            selectUsers ?? (selectUsers = new Command(() => SelectUsers()));
        private void SelectUsers()
        {
            List<string> temp = new List<string>();

            for (int i = 0; i < QueueNumberUsers; i++)
            {
                if (QueueAllUsers.Count > i)
                {
                    string user = QueueAllUsers[i];
                    temp.Add(user);
                    QueueAllUsers.Remove(user);
                }
            }

            QueueCurrentUsers = temp;
        }

        ICommand clearQueue;
        public ICommand ClearQueueCommand =>
            clearQueue ?? (clearQueue = new Command(() => ClearQueue()));
        private void ClearQueue()
        {
            QueueCurrentUsers = new List<string>();
            QueueAllUsers = new List<string>();
        }

        ICommand windowClosing;
        public ICommand WindowClosing =>
            windowClosing ?? (windowClosing = new Command(() => HandleClosing()));
        private void HandleClosing()
        {
            SaveAllData();

            if (_thankedCasters.Count > 0)
            {
                string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Mewgibot\Logs\thanks.json";
                Process.Start("notepad.exe", filename);
            }
        }

        ICommand changeBotCredentials;
        public ICommand ChangeBotCredentials =>
            changeBotCredentials ?? (changeBotCredentials = new Command(() => ConnectNewCredentials()));
        private void ConnectNewCredentials()
        {
            Login login = new Login();
            if (login.ShowDialog() == true)
            {
                SettingsService.Username = login.Username;
                SettingsService.OAuth = login.OAuth;

                if (client != null && client.IsConnected)
                    client.Disconnect();

                ConfigureClient();

                Task.Factory.StartNew(async () => { ConsoleText = "Connecting...\r\n"; await Task.Delay(5); client.Connect(); });
            }
        }

        ICommand mewgibotLogIn;
        public ICommand MewgibotLogIn =>
            mewgibotLogIn ?? (mewgibotLogIn = new Command(() => HandleMewgibotLogin()));
        private void HandleMewgibotLogin()
        {
            SettingsService.Username = defaultUsername;
            SettingsService.OAuth = defaultOAuth;

            if (client != null && client.IsConnected)
                client.Disconnect();

            ConfigureClient();

            Task.Factory.StartNew(async () => { ConsoleText = "Connecting...\r\n"; await Task.Delay(5); client.Connect(); });
        }

        ICommand startScheduledMessages;
        public ICommand StartScheduledMessages =>
            startScheduledMessages ?? (startScheduledMessages = new Command(() => ConfigureScheduledMessages()));

        ICommand removeScheduledMessage;
        public ICommand RemoveScheduledMessageCommand =>
            removeScheduledMessage ?? (removeScheduledMessage = new Command(() => RemoveScheduledMessage()));
        private void RemoveScheduledMessage()
        {
            List<IntervalMessage> tempMessages = new List<IntervalMessage>(IntervalMessages);
            tempMessages.Remove(SelectedMessage);

            IntervalMessages = tempMessages;

            ConfigureScheduledMessages();
            SaveScheduledMessages();
        }

        ICommand removeRegular;
        public ICommand RemoveRegularCommand =>
            removeRegular ?? (removeRegular = new Command(() => RemoveRegular()));
        private void RemoveRegular()
        {
            List<Regular> tempRegulars = new List<Regular>(Regulars);
            tempRegulars.Remove(SelectedRegular);

            Regulars = tempRegulars;

            SaveRegulars();
        }

        ICommand removeCommand;
        public ICommand RemoveCommandCommand =>
            removeCommand ?? (removeCommand = new Command(() => RemoveCommand()));
        private void RemoveCommand()
        {
            List<BotCommand> temp = new List<BotCommand>(Commands);
            temp.Remove(SelectedCommand);

            Commands = temp;

            SaveCommands();
        }

        ICommand removeQuote;
        public ICommand RemoveQuoteCommand =>
            removeQuote ?? (removeQuote = new Command(() => RemoveQuote()));
        private void RemoveQuote()
        {
            List<Quote> temp = new List<Quote>(Quotes);
            temp.Remove(SelectedQuote);

            Quotes = temp;

            SaveQuotes();
        }

        ICommand removeLink;
        public ICommand RemoveLinkCommand =>
            removeLink ?? (removeLink = new Command(() => RemoveLink()));
        private void RemoveLink()
        {
            List<Link> temp = new List<Link>(Whitelist);
            temp.Remove(SelectedLink);

            Whitelist = temp;

            SaveWhitelist();
        }

        ICommand removePermit;
        public ICommand RemovePermitCommand =>
            removePermit ?? (removePermit = new Command(() => RemovePermit()));
        private void RemovePermit()
        {
            List<string> temp = new List<string>(PermittedUsers);
            temp.Remove(SelectedPermit);

            PermittedUsers = temp;
        }

        ICommand removeAllPermits;
        public ICommand RemoveAllPermitsCommand =>
            removeAllPermits ?? (removeAllPermits = new Command(() => RemoveAllPermits()));
        private void RemoveAllPermits()
        {
            PermittedUsers = new List<string>();
        }

        ICommand startTrivia;
        public ICommand StartTriviaCommand =>
            startTrivia ?? (startTrivia = new Command(() => StartTrivia()));
        private void StartTrivia()
        {
            triviaTimer = new System.Timers.Timer(1000 * 60 * 1) //Create timer that fires every minute. Eventually alter to allow user to specify number of minutes.
            {
                AutoReset = true
            };
            triviaTimer.Elapsed += TriviaTimer_Elapsed;

            TriviaActive = true;

            client.SendMessage("Trivia is now open.");

            GetTriviaQuestion();
        }

        ICommand stopTrivia;
        public ICommand StopTriviaCommand =>
            stopTrivia ?? (stopTrivia = new Command(() => StopTrivia()));
        private void StopTrivia()
        {
            triviaTimer.Stop();
            TriviaActive = false;
            CurrentQuestion = new TriviaQuestion();

            client.SendMessage("Trivia is now closed.");
        }

        ICommand skipQuestion;
        public ICommand SkipQuestionCommand =>
            skipQuestion ?? (skipQuestion = new Command(() => SkipQuestion()));
        private void SkipQuestion()
        {
            GetTriviaQuestion();
            triviaTimer.Stop();
            triviaTimer.Start();
        }

        private void TriviaTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetTriviaQuestion();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
