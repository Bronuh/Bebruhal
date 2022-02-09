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
				.SetConsole(true)
				.AddAliases("exit","shutdown");
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
				.SetHelp("команда цель новый_псевдоним")
				.SetDescription("Добавляет псевдоним пользователю")
				.AddAliases("забудь", "forget");
			commands.Add(cmd);


			foreach (var command in commands)
			{
				Bebruhal.GetCore().CommandsManager.RegisterCommand(command, module);
			}
		}
	}
}
