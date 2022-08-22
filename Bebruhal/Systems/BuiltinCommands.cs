using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Systems
{
	internal class BuiltinCommands : IInitializable
	{
		List<Command> commands = new List<Command>();
		private IModule module = Bebruhal.CoreModule;

		public void Initialize()
		{
			var cmd = new Command("stop", async (msg, ctx) => 
			{
				Bebruhal.ShutDown();
			})
				.SetConsole(false)
				.SetOp(true)
				.AddAliases("exit","shutdown","kill");
			commands.Add(cmd);


			cmd = new Command("op", async (msg, ctx) =>
			{
				var user = ctx.Session.GetUser(msg.GetTextWithoutCommand());
				user.SetOp(true);
				msg.Respond($"{user.Name} ({user.Source}.{user.ExternalId}) получил права администратора");
			})
				.SetDescription("Выдает права администратора указанному пользователю")
				.SetOp(true);
			commands.Add(cmd);


			cmd = new Command("deop", async (msg, ctx) =>
			{
				var user = ctx.Session.GetUser(msg.GetTextWithoutCommand());
				user.SetOp(false);
				msg.Respond($"{user.Name} ({user.Source}.{user.ExternalId}) лишен прав администратора");
			})
				.SetDescription("Лишает указанного пользователя прав администратора")
				.SetOp(true);
			commands.Add(cmd);


			cmd = new Command("plugins", async (msg, ctx) =>
			{
				string respondText = "Список загруженных плагинов: \n";
				foreach (var plugin in ctx.PluginsManager.GetPlugins())
				{
					respondText += $"{plugin.Name} ({plugin.Id}) by {plugin.Author}\n";
				}
				msg.Respond(respondText);
			})
				.SetDescription("Выводит список подключенных плагинов, их Id и авторов")
				.AddAliases("плагины");
			commands.Add(cmd);


			cmd = new Command("modules", async (msg, ctx) =>
			{
				string respondText = "Список загруженных модулей: \n";
				foreach (var module in ctx.ModulesManager.GetModules())
				{
					respondText += $"{module.Name} ({module.Id}) by {module.Author}\n";
				}
				msg.Respond(respondText);
			})
				.SetDescription("Выводит список подключенных модулей, их Id и авторов")
				.AddAliases("модули");
			commands.Add(cmd);


			cmd = new Command("alias", async (msg, ctx) =>
			{
				var parts = msg.GetTextWithoutCommand().Split(' ');
				try
				{
					var targetKey = parts[0];
					var newAlias = parts[1];
					var user = ctx.Session.GetUser(targetKey);
					if (user.IsEmpty())
					{
						msg.Respond($"Не удалось найти пользователя по ключу '{targetKey}'");
						return;
					}

					ctx.Session.AddAliases(user,newAlias);
					msg.Respond($"Запомнил: {user.Name} = {newAlias}");
				}
				catch (Exception ex)
				{
					msg.Respond($"Ошибка при выполнении команды 'alias': {ex.Message}");
				}
			})
				.SetHelp("команда цель новый_псевдоним")
				.SetDescription("Добавляет псевдоним пользователю")
				.AddAliases("запомни", "алиас");
			commands.Add(cmd);


			cmd = new Command("unalias", async (msg, ctx) =>
			{
				var parts = msg.GetTextWithoutCommand().Split(' ');
				try
				{
					var targetKey = parts[0];
					var user = ctx.Session.GetUser(targetKey);
					if (user.IsEmpty())
					{
						msg.Respond($"Не удалось найти пользователя по ключу '{targetKey}'");
						return;
					}

					ctx.Session.RemoveAliases(targetKey);
					msg.Respond($"Забыл псевдоним пользователя: {user.Name}");
				}
				catch (Exception ex)
				{
					msg.Respond($"Ошибка при выполнении команды 'unalias': {ex.Message}");
				}
			})
				.SetHelp("команда псевдоним")
				.SetDescription("Добавляет псевдоним пользователю")
				.AddAliases("забудь", "forget");
			commands.Add(cmd);


			cmd = new Command("about", async (msg, ctx) =>
			{
				var parts = msg.GetTextWithoutCommand().Split(' ');
				var respond = "";

				try
				{
					var targetKey = parts[0];
					var user = ctx.Session.GetUser(targetKey);
					if (user.IsEmpty())
					{
						msg.Respond($"Не удалось найти пользователя по ключу '{targetKey}'");
						return;
					}

					//ctx.Session.RemoveAliases(targetKey);
					//msg.Respond($"Забыл псевдоним пользователя: {user.Name}");
				}
				catch (Exception ex)
				{
					msg.Respond($"Ошибка при выполнении команды 'about': {ex.Message}");
				}
			})
			.SetHelp("команда цель")
			.SetDescription("Выводит информацию о пользователе")
			.AddAliases("кто", "who", "whois");
			commands.Add(cmd);


			cmd = new Command("help", async (msg, ctx) =>
			{
				msg.Respond($"Помащь!\n" +
					$"Используй !help, чтобы еще разок посмотреть на этот великолепный текст\n" +
					$"Используй !commands, чтобы посмотреть список всех доступных комманд\n" +
					$"Ну и в общем то всё...");
			})
			.SetHelp("команда")
			.SetDescription("Показывает основную информацию о боте")
			.AddAliases("помощь", "man");
			commands.Add(cmd);


			cmd = new Command("commands", async (msg, ctx) =>
			{
				string respond = "Список команд: ";
				List<string> lines = new();
				foreach (var command in ctx.CommandsManager.GetCommands())
				{
					string commandInfo = "";

					string commandPerm = (command.ConsoleOnly) ? "(CONSOLE) " : (command.OpOnly) ? "(OP) " : "";
					string commandName = ctx.Config.CommandPrefix + command.Name + " - ";
					string commandDesc = command.Description+".";
					string commandAliases = $"Альтернативы: {command.Aliases.ToLine()}.";
					string commandSource = $"Источник: {((command.Source is null) ? "CORE" : command.Source.Name)}";

					commandInfo = $"{commandPerm}{commandName}{commandDesc} {commandAliases} {commandSource}";
					lines.Add(commandInfo);
				}
				int linesPerMessage = 5;

				int linesCounter = 0;
				string text = "";
				foreach (var line in lines)
				{
					text += line+"\n";
					linesCounter++;
					if(linesCounter >= linesPerMessage)
					{
						msg.Respond(text);
						text = "";
						linesCounter = 0;
					}
				}
				msg.Respond(text);
			})
			.SetHelp("команда")
			.SetDescription("Выводит список команд")
			.AddAliases("команды");
			commands.Add(cmd);


			foreach (var command in commands)
			{
				Bebruhal.GetCore().CommandsManager.RegisterCommand(command, module);
			}
		}
	}
}
