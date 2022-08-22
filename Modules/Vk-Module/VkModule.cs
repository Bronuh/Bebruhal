using Bebruhal.Interfaces;
using Bebruhal.Types;
using Config.Net;
using NLog;
using Vk_Module.Types;

namespace Vk_Module
{
	public class VkModule : IModule
	{
		public string Id => "vk-module";

		public string Name => "Vk Module";

		public string Author => "Bronuh";

		public string Description => "Делает текст картинки поигрульки смотреть мемы";

		public string URL => "http://localhost";

		public bool IsReady => false;

		internal static bool isRunning = true;

		public List<Function> Functions => new();

		internal static IVkConfig config = new ConfigurationBuilder<IVkConfig>()
		.UseIniFile(Path.Combine("config", "vkModule.ini"))
		.Build();
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		internal static VkModule Instance;
		
		internal bool isStarted = false;
		static BotContext Context;

		public void Broadcast(Message message)
		{
			//throw new NotImplementedException();
		}

		public void Init(BotContext context)
		{
			if (!String.IsNullOrEmpty(config.Token.Trim()))
			{
				Task.Factory.StartNew(() => Client.Start(config, context).GetAwaiter().GetResult());
			}
			else
			{
				Logger.Error($"Не удалось запустить  vk-клиент: не указан токен. Используйте !vk-token чтобы установить новый токен");
			}
		}

		public void PostInit(BotContext context)
		{
			Directory.CreateDirectory("vkImages");
			var files = Directory.GetFiles("vkImages");
			foreach (var file in files)
			{
				try
				{
					File.Delete(file);
				}
				catch(Exception e)
				{
					Logger.Warn(e.Message);
				}
			}
		}

		public void PreInit(BotContext context)
		{
			Instance = this;
			Context = context;
			Command token = new Command("vk-token", async (msg, ctx) =>
			{
				var parts = msg.Text.Split(' ');
				var token = parts[1];

				config.Token = token;
				if (!isStarted)
				{
					Restart();
				}
			}).SetConsole(true)
			.SetDescription("Устанавливает токен для подключения Vk-клиента");
			context.CommandsManager.RegisterCommand(token, this);

			Command gpId = new Command("vk-groupid", async (msg, ctx) =>
			{
				var parts = msg.Text.Split(' ');
				var groupId = parts[1];

				try
				{
					config.GroupId = Int32.Parse(groupId);
				}catch(Exception e)
				{
					Logger.Warn(e);
				}

			}).SetConsole(true)
			.SetDescription("Устанавливает id группы для определения сервера загрузки файлов");
			context.CommandsManager.RegisterCommand(gpId, this);
		}

		private void Restart()
		{
			if (!isStarted)
				Task.Factory.StartNew(() => Client.Start(config, Context).GetAwaiter().GetResult());
		}

		public void Save()
		{
			isRunning = false;
			//throw new NotImplementedException();
		}

		public void SendMessage(Message msg, string address)
		{
			Task.Factory.StartNew(() => Client.SendMessage(msg, address));
		}
	}
}