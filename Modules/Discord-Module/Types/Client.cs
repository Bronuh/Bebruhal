using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Module.Types
{
	internal static class Client
	{
		internal static ulong BotUserId;
		private static int starts = 0;
		internal static Task _task;
		internal static string Token;
		internal static IDiscordConfig Config { get; set; }
		internal static BotContext BotContext { get; set; }

		internal static List<CustomGuild> CustomGuilds { get; private set; } = new List<CustomGuild>();
		internal static List<DiscordGuild> guilds = new List<DiscordGuild>();

		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		private static Session Session { get; set; }

		public static DiscordClient Discord { get; private set; }
		public static bool Ready = false;

		private static Task currentTask;
		internal static Dictionary<ulong, DiscordChannel> Channels { get; private set; } = new Dictionary<ulong, DiscordChannel>();

		private static List<QueuedMessage> messages = new List<QueuedMessage>();


		internal static void Start(IDiscordConfig config, BotContext botContext)
		{
			// Сброс всех значений
			if (currentTask != null)
			{
				Discord?.Dispose();
				currentTask = null;
				Ready = false;
			}
			CustomGuilds = new List<CustomGuild>();
			guilds = new List<DiscordGuild>();
			Discord = null;
			Channels = new Dictionary<ulong, DiscordChannel>();

			BotContext = botContext;
			Config = config;
			Bebruhal.Bebruhal.СrutchSaveConfig(Config);
			Session = BotContext.Session;

			Logger.Info($"Подключение дискорд-клиента с токеном {config.Token}");
			if (config.Token == "")
			{
				Logger.Error($"Невозможно загрузить клиент дискорда: не указан токен.\nИспользуйте команду" +
					$"'discord-token <токен>' чтобы установить токен, либо укажите токен в файле конфигурации и перезапустите бота.");
				return;
			}
			Token = config.Token;
			starts++;
			try
			{
				currentTask = Task.Factory.StartNew(() => MainAsync(starts));
			}
			catch (Exception ex)
			{
				Logger.Error($"Не удалось запустить асинхронную задачу MainAsync в {DiscordModule.Instance.Name}");
				return;
			}
		}




		private static async Task MainAsync(int start)
		{
			Logger.Trace($"Запущен MainAsync ({start})");
			DiscordClient client;
			Task.Factory.StartNew(() => StartChecker());
			client = new DiscordClient(new DiscordConfiguration
			{
				Token = Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.All,
				AutoReconnect = false
			});

			Logger.Trace($"Создан DiscordClient");
			if (start != starts)
			{
				Logger.Warn($"Во время одного из запусков ({start}) был произведён повторный запуск клиента ({starts})." +
					$"Устаревший клиент далее не будет инициализирован.");
				Task.Factory.StartNew(() => client?.Dispose());
				return;
			}

			client.Ready += OnReady;
			client.MessageCreated += OnMessageCreated;
			client.SocketClosed += async (sender, args) =>
			{
				Logger.Fatal($"Сокет был закрыт. Перезапуск модуля {DiscordModule.Instance.Id}: \n{args.CloseMessage}");
				Task.Factory.StartNew(() => StopAllShitAndRestart());
			};
			/*Discord.SocketErrored += async (sender, args) =>
			{
				Logger.Fatal($"Произошла ошибка сокета. Перезапуск модуля {DiscordModule.Instance.Id}:\n{args.Exception}");
				Task.Factory.StartNew(() => StopAllShitAndRestart());
			};*/

			Logger.Trace($"Добавлен обработчик события Discord.Ready");

			DiscordModule.Instance.IsReady = true;
			Discord = client;
			await client.ConnectAsync();
			await Task.Delay(-1);
		}

		private static void StopAllShitAndRestart()
		{
			Discord.Dispose();
			Logger.Warn("Перезапуск короче, вот");
			DiscordModule.Instance.Restart();
		}

		private static void PreregistrateUsers(IEnumerable<DiscordMember> users)
		{
			foreach(var user in users)
			{
				try
				{
					var bebrUser = Session.RegisterUser(user.Username, user.Id.ToString(), DiscordModule.Instance, null);
					bebrUser.AddTempAlias(user.DisplayName);
				}
				catch (Exception ex)
				{
					Logger.Debug($"Не удалось зарегистрировать пользователя {user.Username}:\n{ex.Message}");
				}
			}
		}

		internal static async void SendMessage(Message msg, string address)
		{
			ulong target;
			try
			{
				target = UInt64.Parse(address.Split('.').Last());
			}
			catch (FormatException)
			{
				return;
			}

			Dictionary<string,Stream> files = new Dictionary<string,Stream>();
			int filesCount = 0;

			foreach (var image in msg.Images)
			{
				if (filesCount < 10)
				{
					MemoryStream ms = new MemoryStream();
					image.SaveAsGif(ms);
					ms.Position = 0;

					files.Add($"{filesCount}.gif", ms);
					filesCount++;
				}
				else
				{
					break;
				}
			}

			foreach (var file in msg.Files)
			{
				if (filesCount<10)
				{
					files.Add(file.Name, file.ToStream());
					filesCount++;
				}
				else
				{
					break;
				}
			}

			var text = msg.ReplaceMentions((user) => { return $"<@!{user.ExternalId}>"; });

			DiscordMessageBuilder builder = new DiscordMessageBuilder();
			builder.WithContent(text)
				.WithFiles(files);

			try
			{
				await Channels[target].SendMessageAsync(builder);
			}
			catch (Exception ex)
			{
				Logger?.Error($"Не удалось отправить сообщение в канал {target}: \n{ex.Message}");
				messages.Add(new QueuedMessage() { address = address, message = msg });
			}
			
		}







		private static async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
		{
			BebrUser user = Session.GetUser(e.Message.Author.Id.ToString());
			if (user.IsEmpty())
			{
				Logger.Debug($"Не удалось найти пользователя по Id {e.Message.Author.Id.ToString()}. Попытка регистрации.");
				user = Session.RegisterUser(e.Message.Author.Username, e.Message.Author.Id.ToString(), DiscordModule.Instance, null);
				Session.AddAliases(user, e.Author.Username);
			}

			user.AddTempAlias((await e.Guild.GetMemberAsync(e.Author.Id)).DisplayName);

			Message msg = new Message();
			msg.Author = user;
			msg.Text = e.Message.Content;
			msg.Module = DiscordModule.Instance;
			msg.RawAuthor = e.Message.Author.Id.ToString();
			msg.Source = $"{DiscordModule.Instance.Id}.channel.{e.Channel.Id}";
			msg.Info = $"{user.Name}@{e.Guild.Name}#{e.Channel.Name}";
			try
			{
				Channels.Add(e.Channel.Id,e.Channel);
			}
			catch (Exception ex)
			{
				Logger.Debug($"Не удалось добавить канал в словарь: {ex.Message}");
			}

			await BotContext.Core.ProcessMessage(msg);
		}






		private static async Task OnReady(DiscordClient sender, ReadyEventArgs args)
		{
			Logger.Trace($"Discord.Ready случился");
			Ready = true;
			BotUserId = sender.CurrentUser.Id;
			await ScanServers(sender, args);

			if(DiscordModule.Instance.firstStart)
				foreach (var guild in CustomGuilds)
				{
					Logger.Debug($"Попытка отправки сообщения в {guild.BotChannel.Name}");
					await guild.BotChannel.SendMessageAsync("Бебр запущен");
					var chans = await guild.GetChannelsAsync();
					foreach (var channel in chans)
					{
						Channels.Add(channel.Id,channel);
					}
				}

			if (Config.PreregistrateUsers)
				await Task.Factory.StartNew(async () =>
				{
					foreach (var guild in CustomGuilds)
					{
						PreregistrateUsers(await guild.Source.GetAllMembersAsync());
					}
				});

			foreach (var msg in messages)
			{
				SendMessage(msg.message,msg.address);
			}
		}





		private static void CheckReady()
		{
			if (!Ready)
			{
				Logger.Warn($"Клиент бота не был готов после 30 секунд после запуска.");
				StopAllShitAndRestart();
			}
		}




		private static void StartChecker()
		{
			Thread.Sleep(30000);
			CheckReady();
		}

		private static async Task ScanServers(DiscordClient sender, ReadyEventArgs args)
		{
			foreach (var guild in Discord.Guilds.Values)
			{
				Logger.Debug($"Найден сервер: {guild.Name} ({guild.Id})");
				guilds.Add(guild);
				CustomGuilds.Add(await CustomGuild.BuildCustomGuild(guild));
			}
		}

	}
}
