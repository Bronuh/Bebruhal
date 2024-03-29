﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	/// <summary>
	/// Действие выполняемое при использовании данной команды
	/// </summary>
	/// <param name="msg">Сообщение, содержащее команду</param>
	/// <param name="ctx">Контекст работы бота</param>
	/// <returns></returns>
	public delegate Task CommandAction(Message msg, BotContext ctx);

	/// <summary>
	/// Класс команды. Содержит в себе поля информации о команде, делегат CommandAction, выполняющий основную работу, 
	/// а также методы для создания новых команд. Все команды должны быть зарегистрированы в CommandsManager после завершения их создания.
	/// </summary>
	public class Command
	{

		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Вызывается при попытке выполнить команду
		/// </summary>
		public event AsyncEventHandler<Command, CommandCalledEventArgs>? Called;

		/// <summary>
		/// Вызывается в случае успешного выполения команды
		/// </summary>
		public event AsyncEventHandler<Command, CommandExecutedEventArgs>? Executed;

		/// <summary>
		/// Вызывается, если команда не была выполнена из-за непрохождения проверок доступа
		/// </summary>
		public event AsyncEventHandler<Command, CommandCancelledEventArgs>? Cancelled;

		/// <summary>
		/// Название команды. Является её основным идентификатором
		/// </summary>
		public string? Name { get; protected set; }

		/// <summary>
		/// Описание предназначения и действия команды
		/// </summary>
		public string? Description { get; protected set; }

		/// <summary>
		/// Название разрешения, необходимого для использования команды
		/// </summary>
		public string? Permission { get; protected set; }

		/// <summary>
		/// Строка с описанием синтаксиса использования команды
		/// </summary>
		public string? Help { get; protected set; }

		/// <summary>
		/// Источник команды - плагин или модуль, добавивший команду. Устанавливается в момент регистрации.
		/// </summary>
		public IAssembly? Source { get; internal set; }

		/// <summary>
		/// Псевдонимы команды, позволяющие её вызвать. Может использовано для назначения сокращений или переводов
		/// </summary>
		public List<string> Aliases { get; protected set; } = new List<string>();

		/// <summary>
		/// Теги команды, позволяющие рассортировать команды по функционалу. Рекомендуемые теги: admin, misc, fun, console
		/// </summary>
		public List<string> Tags { get; protected set; } = new List<string>();
		
		/// <summary>
		/// Выполняемое командой действие
		/// </summary>
		public CommandAction? Action { get; protected set; }

		/// <summary>
		/// Ранг, требуемый для использования
		/// </summary>
		public int Rank { get; protected set; } = 0;

		/// <summary>
		/// Доступна ли эта команда только операторам
		/// </summary>
		public bool OpOnly { get; protected set; } = false;

		/// <summary>
		/// Доступна ли эта команда только консолям
		/// </summary>
		public bool ConsoleOnly { get; protected set; } = false;

		/// <summary>
		/// Пустой конструктор
		/// </summary>
		public Command() { }

		/// <summary>
		/// Создаёт новую команду
		/// </summary>
		/// <param name="name">Имя команды</param>
		/// <param name="action">Выполняемое действие</param>
		public Command(string name, CommandAction action)
		{
			Name = name;
			Action = action;
		}

		/// <summary>
		/// Возвращает всю основную информацию о команде, в одной строке
		/// </summary>
		/// <returns></returns>
		public string GetInfo()
		{
			string info = "";
			string aliases = "";
			foreach (string alias in Aliases)
			{
				aliases += alias + (alias == Aliases[Aliases.Count - 1] ? "" : ", ");
			}
			string tags = "";
			foreach (string tag in Tags)
				tags += tag + (tag == Tags[Tags.Count - 1] ? "" : ", ");

			info += "Команда " + Bebruhal.GetCore().GetPrefix() + Name + "\n";
			info += "Аналоги: " + aliases + "\n";
			info += "Использование: " + Help + "\n";
			info += "Описание: " + Description + "\n";
			info += "Требуемый ранг: " + Rank + "\n";
			info += "Только для админов: " + OpOnly + "\n";
			info += "Тэги: *" + tags + "*";

			info = info.Replace("<name>", Name).Replace("<command>", Bebruhal.GetCore().GetPrefix() + Name);

			return info;
		}


		/// <summary>
		/// Задает описание команде
		/// </summary>
		/// <param name="description">Текст описания</param>
		/// <returns>Текущая команда</returns>
		public Command SetDescription(string description)
		{
			Description = description;
			return this;
		}

		/// <summary>
		/// Добавляет тег команде
		/// </summary>
		/// <param name="tag">название тега</param>
		/// <returns>Текущая команда</returns>
		public Command AddTag(string tag)
		{
			Tags.Add(tag.ToLower());
			return this;
		}

		/// <summary>
		/// Добавляет теги команде
		/// </summary>
		/// <param name="tags">Теги</param>
		/// <returns>Текущая команда</returns>
		public Command AddTags(params string[] tags)
		{
			foreach (string tag in tags)
			{
				Tags.Add(tag.ToLower());
			}
			return this;
		}

		/// <summary>
		/// Добавляет описание использования команды. Доступны теги форматирования
		/// </summary>
		/// <param name="usage">Текст описания</param>
		/// <returns>Текущая команда</returns>
		public Command SetHelp(string usage)
		{
			Help = usage;
			return this;
		}

		/// <summary>
		/// Устанавливает минимальный ранг для использования команды
		/// </summary>
		/// <param name="rank">Минимальный ранг</param>
		/// <returns>Текущая команда</returns>
		public Command SetRank(int rank)
		{
			Rank = rank;
			return this;
		}

		/// <summary>
		/// Устанавливает необходимость иметь права оператора для этой команджы
		/// </summary>
		/// <param name="op">Только для админов?</param>
		/// <returns>Текущая команда</returns>
		public Command SetOp(bool op)
		{
			OpOnly = op;
			return this;
		}

		/// <summary>
		/// Ограничивает возможность использования только консолью
		/// </summary>
		/// <param name="console">Только для консоли?</param>
		/// <returns>Текущая команда</returns>
		public Command SetConsole(bool console)
		{
			ConsoleOnly = console;
			return this;
		}

		/// <summary>
		/// Добавляет альтернативное название команды
		/// </summary>
		/// <param name="alias">Альтернативное название</param>
		/// <returns>Текущая команда</returns>
		public Command AddAlias(string alias)
		{
			Aliases.Add(alias.ToLower());
			return this;
		}


		/// <summary>
		/// Добавляет альтернативное название команды
		/// </summary>
		/// <param name="aliases">Перечень альтернативных названий</param>
		/// <returns>Текущая команда</returns>
		public Command AddAliases(params string[] aliases)
		{
			foreach (var alias in aliases)
			{
				AddAlias(alias);
			}

			return this;
		}

		/// <summary>
		/// Проверяет имеет ли команда указанный тег
		/// </summary>
		/// <param name="searchingTag">Искомый тег</param>
		/// <returns>Возвращает true, если такой тег у команды есть</returns>
		public bool HasTag(string searchingTag)
		{
			foreach (string tag in Tags)
			{
				if (tag.ToLower() == searchingTag.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Проверяет, соответствует ли команда искомой строке (проверка по названию и псевдонимам).
		/// </summary>
		/// <param name="text">Название или псевдоним искомой команды</param>
		/// <returns>Возвращает true, если команда имеет данное имя или псевдоним</returns>
		public bool CheckCommand(string text)
		{
			string command = text.Split(' ')[0];

			if (Name != null && Name.Trim() != String.Empty && command.ToLower() == Name.ToLower())
			{
				return true;
			}
			else
			{
				LoggerProxy.Debug("Имя команды не найдено, поиск псевдонимов");
				foreach (string alias in Aliases)
				{
					LoggerProxy.Debug("Псевдоним: " + alias);
					if (command.ToLower() == alias.ToLower())
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Пытается выполнить команду на основе сообщения
		/// </summary>
		/// <param name="message">Сообщение, для которого необходимо выполнить команду</param>
		/// <param name="context">Контекст бота</param>
		/// <returns></returns>
		public async Task TryExecute(Message message, BotContext context)
		{
			string text = message.Text;
			BebrUser author = message.Author;
			text = text.Substring(context.Core.GetPrefix().Length, text.Length - (context.Core.GetPrefix().Length));



			LoggerProxy.Debug($"Попытка выполнения команды {context.Core.GetPrefix()}{Name} ({text})");
			if (CheckCommand(text))
			{
				context.CommandsManager.FireCommandCalled(this, new CommandCalledEventArgs(message));
				Called?.Invoke(this, new CommandCalledEventArgs(message));
				LoggerProxy.Debug("Обнаружена команда " + Name);

				if (!message.Author.IsConsole() && ConsoleOnly)
				{
					Logger.Warn($"Попытка выполнить команду только для консоли из внешнего модуля");
					message.Respond($"Харошоя попытка, {message.Author.Name}, но команда только для консолей");
					context.CommandsManager.FireCommandCancelled(this, new CommandCancelledEventArgs(message));
					Cancelled?.Invoke(this, new CommandCancelledEventArgs(message));
					return;
				}

				if (!message.Author.IsAdmin() && OpOnly)
				{
					Logger.Warn($"Попытка выполнить команду только для администраторов");
					message.Respond($"Харошоя попытка, {message.Author.Name}, но команда только для админов");
					context.CommandsManager.FireCommandCancelled(this, new CommandCancelledEventArgs(message));
					Cancelled?.Invoke(this, new CommandCancelledEventArgs(message));
					return;
				}

				if (message.Author.Rank < Rank)
				{
					Logger.Warn($"Попытка выполнить команду {Rank} ранга пользователем {message.Author.Rank} ранга");
					message.Respond($"Харошоя попытка, {message.Author.Name}, но тебе стоит дорасти до {Rank} ранга");
					context.CommandsManager.FireCommandCancelled(this, new CommandCancelledEventArgs(message));
					Cancelled?.Invoke(this, new CommandCancelledEventArgs(message));
					return;
				}

				context.CommandsManager.FireCommandExecuted(this, new CommandExecutedEventArgs(message));
				Executed?.Invoke(this, new CommandExecutedEventArgs(message));
				if (Action != null)
				{
					await Action(message, context);
				}
				else
				{
					Logger.Warn($"Команда {Name ?? "<безымянная>"} не содержит делегата действия. Пропущено.");
				}
			}
		}
	}
}
