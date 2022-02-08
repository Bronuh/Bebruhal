using Bebruhal.Interfaces;
using Bebruhal.Utils.Config;
using NLog;

namespace Bebruhal
{
	/// <summary>
	/// Ядро бота. Хранит в себе данные текущей сессии, конфигурацию, а также обеспечивает связь модулей и плагинов.
	/// </summary>
	public class BebruhalCore
	{
		// TODO: сделать контекст нестатичным
		internal static BotContext context = new BotContext();
		private string sessionPath;

		/// <summary>
		/// Экземпляр менеджера плагинов
		/// </summary>
		public PluginsManager PluginsManager { get; internal set; }

		/// <summary>
		/// Экземпляр менеджера модулей
		/// </summary>
		public ModulesManager ModulesManager { get; internal set; }

		/// <summary>
		/// Экземмпляр менеджера команд
		/// </summary>
		public CommandsManager CommandsManager { get; internal set; }

		private IBebruhalConfig config;
		private Logger logger = LoggerProxy.Instance();

		/// <summary>
		/// Экземпляр текущей сессии
		/// </summary>
		public Session Session { get; internal set; }

		private List<Group> groups = new List<Group>();

		internal BebruhalCore() { }
		internal BebruhalCore(IBebruhalConfig config) 
		{
			this.config = config;
			CommandsManager = new CommandsManager(context);
			PluginsManager = new PluginsManager(context);
			ModulesManager = new ModulesManager(context);
		}


		/// <summary>
		/// Поочерёдно три этапа инициализации.
		/// </summary>
		/// <exception cref="NullReferenceException">Может быть вызвано в случае невозможности загрузки/создания сессии</exception>
		internal void Start()
		{
			logger.Debug("Запущено ядро бота");
			context.ModulesManager = ModulesManager;
			context.CommandsManager = CommandsManager;
			context.PluginsManager = PluginsManager;
			InterfaceExecutor.Execute(typeof(IInitializable),"Initialize");

			PreInit();
			Init();
			PostInit();

			context.Config = config;
			context.Core = this;
			logger.Info("Инициализация завершена");
		}


		private void PreInit()
		{
			logger.Debug("Преинициализация...");
			Session = Session.Load(config.Session);
			if (Session is null)
			{
				Session = Session.CreateSession(config.Session);
			}
			if (Session is null)
			{
				throw new NullReferenceException("Не удалось загрузить сессию.");
			}
			sessionPath = Session.GetPath();
			context.Session = Session;
			ModulesManager.PreInit();
			PluginsManager.PreInit();
		}

		/// <summary>
		/// Метод возвращает префикс для команд, указанный в конфигурации
		/// </summary>
		/// <returns></returns>
		internal string GetPrefix()
		{
			return config.CommandPrefix;
		}

		private void Init()
		{
			logger.Debug("Инициализация...");
			ModulesManager.Init();
			PluginsManager.Init();
		}

		private void PostInit()
		{
			logger.Debug("Постинициализация...");
			ModulesManager.PostInit();
			PluginsManager.PostInit();
		}


		/// <summary>
		/// Вызывает обработку сообщения в ядре
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <returns></returns>
		public async Task ProcessMessage(Message message)
		{
			logger.Info($"[{message.Module?.Name}]({message.Info}): {message.Text}");
			RecievedMessage?.Invoke(message.Module, new RecievedMessageEventArgs(message,message.Author));
			await CommandsManager.TryExecute(message);
		}

		// TODO: Events
		/// <summary>
		/// Вызывается модулями для передачи информации о полученном сообщении в бота. Считывается плагинами и
		/// менеджером команд для обработки.
		/// </summary>
		public event AsyncEventHandler<IModule?, RecievedMessageEventArgs> RecievedMessage;

		
	}

	
}
