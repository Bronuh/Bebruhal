

namespace Bebruhal.Systems
{
	/// <summary>
	/// Отвечает за работу с командами: регистрация, поиск, реакция на события
	/// </summary>
	public sealed class CommandsManager
	{
		/// <summary>
		/// Вызывается когда команда найдена и пытается выполниться
		/// </summary>
		public event AsyncEventHandler<Command, CommandCalledEventArgs>? CommandCalled;

		/// <summary>
		/// Вызывается, когда команда прошла все проверки доступа и вызывает метод действия
		/// </summary>
		public event AsyncEventHandler<Command, CommandExecutedEventArgs>? CommandExecuted;

		/// <summary>
		/// Вызывается, когда вызванная команда прерывает своё выполнение из-за отказа в доступе
		/// </summary>
		public event AsyncEventHandler<Command, CommandCancelledEventArgs>? CommandCancelled;

		private List<Command> commands = new();
		private Dictionary<string,Command> aliasedCommands = new();
		private BotContext context;

		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Создает нового менеджера команд на основе контекста бота
		/// </summary>
		/// <param name="context">Текущий контекст бота</param>
		public CommandsManager(BotContext context)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Регистрирует команду с сохранением её в кэш быстрого доступа. Необходимо вызыать только после окончания инициализации команды.
		/// </summary>
		/// <param name="command">Объект команды</param>
		/// <param name="source">Сборка-источник команды</param>
		public void RegisterCommand(Command command, IAssembly? source)
		{
			if (command is null)
			{
				Logger.Warn($"Попытка зарегистрировать null-команду из источника {source?.Id}");
				return;
			}
			if (command.Name == null || command.Name.Trim() == String.Empty)
			{
				Logger.Warn($"Попытка зарегистрировать команду без имени из источника {source?.Id}.\n" +
					$"Содержащиеся в команде псевдонимы: {command.Aliases.ToLine()}");
				return;
			}

			commands.Add(command);
			RegisterAlias(command.Name, command);

			foreach(var alias in command.Aliases)
			{
				RegisterAlias(alias,command);
			}
			if (source != null)
			{
				command.Source = source;
			}
			Logger.Debug($"Зарегистрирована команда {command.Name}");
		}

		/// <summary>
		/// Добавляет дополнительное имя команды в кэш
		/// </summary>
		/// <param name="alias">Имя команды</param>
		/// <param name="command">Регистрируемая команда</param>
		public void RegisterAlias(string alias, Command command)
		{
			try
			{
				aliasedCommands.Add(alias.ToLower(), command);
			}
			catch (Exception ex)
			{
				Logger.Warn($"Не удалось зарегистрировать команду '{command.Name}' в словаре по ключу {alias.ToLower()}:" +
					$"\n{ex}");
			}
		}

		/// <summary>
		/// Ищет команду по строке
		/// </summary>
		/// <param name="name">Сама команда</param>
		/// <returns>Ссылка на добавленную команду</returns>
		public Command? FindCommand(string name)
		{
			Command? command = null;

			try
			{
				command = aliasedCommands[name.ToLower()];
			}
			catch (Exception ex)
			{
				Logger.Debug($"Не удалось найти команду '{name}' в словаре: {ex.Message}");
			}

			foreach (var cmd in commands)
			{
				if (cmd.CheckCommand(name))
				{
					command = cmd;
					break;
				}
			}

			return command;
		}

		/// <summary>
		/// Пробует выполнить команду из сообщения
		/// </summary>
		/// <param name="msg">Сообщение</param>
		/// <returns></returns>
		public async Task TryExecute(Message msg)
		{
			if (msg.Text.StartsWith(context.Core.GetPrefix()))
			{
				var text = msg.Text.Substring(context.Core.GetPrefix().Length, msg.Text.Length - (context.Core.GetPrefix().Length));
				var cmdName = text.Split(' ')[0].ToLower();
				Command? cmd = null;

				try
				{
					cmd = aliasedCommands[cmdName];
				}
				catch(Exception ex)
				{
					Logger.Warn($"Команда '{cmdName}' не найдена в словаре: \n{ex.Message}");
				}
				if(cmd==null)
					LoggerProxy.Trace($"Поиск команды {cmdName} в списке (бронух, удали это дерьмо, пусть регистрируют команды как положено)");
					foreach (var command in commands)
					{
						if (command.CheckCommand(cmdName))
						{
							cmd = command;
							break;
						}
					}

				if (cmd != null)
					await cmd.TryExecute(msg, context);
			}
		}

		/// <summary>
		/// Пробует выполнить команду от имени консоли
		/// </summary>
		/// <param name="cmd">Текст команды</param>
		/// <returns></returns>
		public async Task TryExecuteConsoleCommand(string cmd)
		{
			var msg = new Message
			{
				Author = BebrUser.ConsoleUser,
				Text = context.Core.GetPrefix() + cmd,
				Module = Bebruhal.CoreModule,
				Source = Bebruhal.CoreModule.Id
			};

			await TryExecute(msg);
		}


		/// <summary>
		/// Возвращает список всех зарегистрированных команд
		/// </summary>
		/// <returns>Список команд</returns>
		public IReadOnlyList<Command> GetCommands()
		{
			return commands;
		}

		/// <summary>
		/// Возвращает список команд, зарегистрированных от имени указанной сборки (плагина/модуля)
		/// </summary>
		/// <param name="assembly">Плагин или сборка, от имени которых должна быть зарегистрирована команда</param>
		/// <returns>Список команд, зарегистрированных указанной сборкой</returns>
		public IEnumerable<Command> GetCommandsByAssembly(IAssembly assembly)
		{
			foreach (var cmd in commands)
			{
				if (cmd.Source == assembly)
				{
					yield return cmd;
				}
			}
		}

		internal void FireCommandCalled(Command cmd, CommandCalledEventArgs args)
		{
			CommandCalled?.Invoke(cmd, args);
		}
		internal void FireCommandExecuted(Command cmd, CommandExecutedEventArgs args)
		{
			CommandExecuted?.Invoke(cmd, args);
		}
		internal void FireCommandCancelled(Command cmd, CommandCancelledEventArgs args)
		{
			CommandCancelled?.Invoke(cmd, args);
		}
	}
}
