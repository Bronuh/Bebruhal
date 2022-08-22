using Bebruhal.Interfaces;
using Bebruhal.Types;
using Bebruhal.Utils;
using Config.Net;
using Discord_Module.Types;
using NLog;

namespace Discord_Module
{
	public class DiscordModule : IModule
	{
		public static DiscordModule Instance { get; private set; }
		private BotContext botContext;

		static IDiscordConfig config = new ConfigurationBuilder<IDiscordConfig>()
		.UseIniFile(Path.Combine("config","discordModule.ini"))
		.Build();


		private Logger logger = LoggerProxy.Instance();

		public string Id { get; } = "discord-module";

		public string Name { get; } = "Discord Module";

		public string Author { get; } = "Bronuh";

		public string Description { get; } = "Модуль бебрухала, предоставляющий адаптер к Discord API";

		public string URL { get; } = "https://github.com/Bronuh";

		public List<Function> Functions => new List<Function>();

		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		public bool IsReady { get; internal set; } = false;

		internal bool firstStart = true;

		internal void Restart()
		{
			firstStart = false;
			Logger.Info($"Перезапуск модуля {Name}");
			Init(botContext);
			PostInit(botContext);
		}


		public void Init(BotContext context)
		{
			Logger.Debug($"Инициализация {Name}");
			Task.Factory.StartNew(() => Client.Start(config, context));
		}

		public void PostInit(BotContext context)
		{

		}

		public void PreInit(BotContext context)
		{
			Instance = this;
			botContext = context;
			Bebruhal.Bebruhal.СrutchSaveConfig(config);

			Command token = new Command("discord-token", async (msg, ctx) =>
			{
				var parts = msg.Text.Split(' ');
				var token = parts[1];

				config.Token = token;

				Restart();
			}).SetConsole(true)
			.SetDescription("Устанавливает токен для подключения Discord-клиента");

			context.CommandsManager.RegisterCommand(token, this);
		}

		public void SendMessage(Message msg, string address)
		{
			Task.Factory.StartNew(()=>Client.SendMessage(msg,address));
		}

		public void Save()
		{

		}

		public void Broadcast(Message message)
		{
			Logger.Warn($"Широковещательные сообщения в {Id} ещё не реализованы");
		}
	}
}