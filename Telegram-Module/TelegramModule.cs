using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Bebruhal.Types.Message;
using TMessage = Telegram.Bot.Types.Message;

namespace Telegram_Module
{
	public class TelegramModule : IModule
	{
		public string Id => "telegram-module";

		public string Name => "Telegram Module";

		public string Author => "Bronuh";

		public string Description => "Тилеграмъ модаль шоб песать саббщеньки ъеъ";

		public string URL => "http://localhost";

		public bool IsReady => false;

		public List<Function> Functions => new();


		public void Broadcast(Message message)
		{

		}

		public void Init(BotContext context)
		{
			Task.Factory.StartNew(Start);
		}

		public void PostInit(BotContext context)
		{

		}

		public void PreInit(BotContext context)
		{

		}

		public void Save()
		{

		}

		public void SendMessage(Message msg, string address)
		{

		}

		internal async Task Start()
		{
			var botClient = new TelegramBotClient("5294348512:AAEdZkfTq5QXRT7kADDHitjxTA6PR1GEgSw");

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




		async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			// Only process Message updates: https://core.telegram.org/bots/api#message
			if (update.Type != UpdateType.Message)
				return;
			// Only process text messages
			if (update.Message!.Type != MessageType.Text)
				return;

			var chatId = update.Message.Chat.Id;
			var messageText = update.Message.Text;
			var username =  "@"+update.Message.From.Username;
			var sender = $"{update.Message.From.FirstName} {update.Message.From.LastName}";


			Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from {sender} ({username})");

			// Echo received message text
			TMessage sentMessage = await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: $"{sender} ({username}) said:\n" + messageText,
				cancellationToken: cancellationToken);
		}

		Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
	}
}