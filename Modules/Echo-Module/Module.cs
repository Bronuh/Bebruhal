using Bebruhal.Interfaces;
using Bebruhal.Types;
using Bebruhal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo_Module
{
	internal class Module : IModule
	{
		public string Id => "echo-module";

		public string Name => "Echo Module";

		public string Author => "Bronuh";

		public string Description => "Модуль, генерирующий сообщения по команде echo";

		public string URL => "http://localhost";

		public List<Function> Functions => new List<Function>();

		public bool IsReady => true;

		public void Init(BotContext context)
		{
			LoggerProxy.Trace($"Инициализация {Name}");
			Command cmd = new Command("echo", async (msg,ctx) => 
				{
					var text = msg.GetTextWithoutCommand();
					Message message = new Message()
					{
						Text = text,
						Author = BebrUser.ConsoleUser,
						Source = "echo-module",
						Module = this
					};
					// LoggerProxy.Warn(text);
					await ctx.Core.ProcessMessage(message);
				})
				.SetConsole(true);

			context.CommandsManager.RegisterCommand(cmd, this);
		}

		public void PostInit(BotContext context)
		{

		}

		public void PreInit(BotContext context)
		{

		}

		public void SendMessage(Message msg, string address)
		{

		}
	}
}
