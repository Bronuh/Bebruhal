using Config.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_Module.Types;
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

		internal bool isStarted = false;

		public List<Function> Functions => new();

		static ITelegramConfig config = new ConfigurationBuilder<ITelegramConfig>()
		.UseIniFile(Path.Combine("config", "telegramModule.ini"))
		.Build();
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		static BotContext Context;

		internal static TelegramModule Instance;

		public void Broadcast(Message message)
		{

		}

		public void Init(BotContext context)
		{
			if (!String.IsNullOrEmpty(config.Token.Trim()))
			{
				Task.Factory.StartNew(() => Client.Start(config, context).GetAwaiter().GetResult());
			}
			else
			{
				Logger.Error($"Не удалось запустить  telegram-клиент: не указан токен. Используйте !telegram-token чтобы установить новый токен");
			}
		}

		public void PostInit(BotContext context)
		{

		}

		public void PreInit(BotContext context)
		{
			Instance = this;
			Context = context;
			Command token = new Command("telegram-token", async (msg, ctx) =>
			{
					var parts = msg.Text.Split(' ');
					var token = parts[1];

					config.Token = token;
				if (!isStarted)
				{
					Restart();
				}
			}).SetConsole(true)
			.SetDescription("Устанавливает токен для подключения Telegram-клиента");

			context.CommandsManager.RegisterCommand(token,this);
		}

		private void Restart()
		{
			if (!isStarted)
				Task.Factory.StartNew(() => Client.Start(config, Context).GetAwaiter().GetResult());
		}

		public void Save()
		{

		}

		public void SendMessage(Message msg, string address)
		{
			Task.Factory.StartNew(() => Client.SendMessage(msg, address));
		}

		
	}
}