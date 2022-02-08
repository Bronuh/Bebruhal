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

		public void Initialize()
		{
			var cmd = new Command("stop", async (msg, ctx) => 
			{
				Bebruhal.ShutDown();
			})
				.SetConsole(true)
				.AddAliases("exit","shutdown");

			commands.Add(cmd);

			foreach (var command in commands)
			{
				Bebruhal.GetCore().CommandsManager.RegisterCommand(command, null);
			}
		}
	}
}
