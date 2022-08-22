using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memator
{
	internal class MainCommands
	{
		static List<Command> commands = new List<Command>();
		private static IPlugin Plugin;

		public static void Initialize(IPlugin plugin, BotContext ctx)
		{
			Plugin = plugin;
			Command cmd;

			cmd = new Command("meme", async (msg, ctx) =>
			{
				string respondText = "Мемосек: \n";
				Message response = new Message();
				response.Text = respondText;
				response.Images = new List<Image>();

				response.Images.Add(Network.LoadImage("https://puu.sh/IWMBm.png"));

				msg.Respond(response);
			})
				.SetDescription("Делает мемасек")
				.AddAliases("мем");
			commands.Add(cmd);

			foreach (var command in commands)
			{
				ctx.CommandsManager.RegisterCommand(command, plugin);
			}

		}
	}
}
