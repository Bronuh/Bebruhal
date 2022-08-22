using Config.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_Module.Types;
using Message = Bebruhal.Types.Message;
using TMessage = Telegram.Bot.Types.Message;

namespace Telegram_Module.Types
{
	internal static class Client
	{
		private static BotContext _botContext;
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		private static Session Session { get; set; }
		private static TelegramBotClient botClient;
		internal static BotContext BotContext { get; set; }


		internal static void Restart() { }

		internal static async void SendMessage(Message msg, string address)
		{
			long target;
			try
			{
				target = Int64.Parse(address.Split('.').Last());
			}
			catch (FormatException)
			{
				return;
			}
			Dictionary<string, Stream> files = new Dictionary<string, Stream>();
			int filesCount = 0;
			using var cts = new CancellationTokenSource();
			//botClient.SendPhotoAsync();

			var text = msg.ReplaceMentions((user) => { return $"@{user.Name}"; });

			try
			{
				TMessage sentMessage = await botClient.SendTextMessageAsync(
					chatId: target,
					text: $"{text}\n",
					cancellationToken: cts.Token);
			}catch(Exception e)
			{
				Logger.Warn(e.Message);
			}
			if (msg.Images.Count!=0)
			{
				foreach (var image in msg.Images)
				{
					MemoryStream ms = new MemoryStream();
					image.SaveAsGif(ms);
					ms.Position = 0;

					files.Add($"{filesCount}.gif", ms);
					filesCount++;
					TMessage animMessage = await botClient.SendAnimationAsync(
						chatId: target,
						animation: new Telegram.Bot.Types.InputFiles.InputOnlineFile(ms,$"{filesCount}.gif")
						);
				}
			}
		}



		internal static async Task Start(ITelegramConfig config, BotContext ctx)
		{
			Logger.Info($"Попытка подключения к Telegram-API с помощью токена {config.Token}");
			botClient = new TelegramBotClient(config.Token);
			Session = ctx.Session;
			BotContext = ctx;

			using var cts = new CancellationTokenSource();

			// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = { } // receive all update types
			};
			botClient.StartReceiving(
				HandleUpdateAsync,
				HandleErrorAsync,
				receiverOptions,
				cancellationToken: cts.Token);

			var me = await botClient.GetMeAsync();

			Console.WriteLine($"Start listening for @{me.Username}");
			//Console.ReadLine();

			// Send cancellation request to stop bot
			// cts.Cancel();
		}




		static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			// Only process Message updates: https://core.telegram.org/bots/api#message
			if (update.Type != UpdateType.Message)
				return;
			// Only process text messages
			if (update.Message!.Type != MessageType.Text)
				return;

			var chatId = update.Message.Chat.Id;
			var messageText = update.Message.Text;
			var username = "@" + update.Message.From.Username;
			var sender = $"{update.Message.From.FirstName} {update.Message.From.LastName}";


			Logger.Info($"Received a '{messageText}' message in chat {chatId} from {sender} ({username})");
			// Echo received message text
			await HandleMessageAsync(update.Message);
		}



		static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}



		static async Task HandleMessageAsync(TMessage tMsg)
		{
			BebrUser user = Session.GetUser(tMsg.From.Id.ToString());
			if (user.IsEmpty())
			{
				Logger.Debug($"Не удалось найти пользователя по Id {tMsg.From.Id.ToString()}. Попытка регистрации.");
				user = Session.RegisterUser(tMsg.From.Username, tMsg.From.Id.ToString(), TelegramModule.Instance, null);
				Session.AddAliases(user, tMsg.From.Username);
			}

			user.AddTempAlias($"{tMsg.From.FirstName} {tMsg.From.LastName}");

			Message msg = new Message();
			msg.Author = user;
			msg.Text = tMsg.Text;
			msg.Module = TelegramModule.Instance;
			msg.RawAuthor = tMsg.From.Id.ToString();
			msg.Source = $"{TelegramModule.Instance.Id}.channel.{tMsg.Chat.Id}";
			msg.Info = $"{user.Name}@{tMsg.Chat.Title}";

			await BotContext.Core.ProcessMessage(msg);
		}
	}
}
