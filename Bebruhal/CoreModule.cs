using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal
{
	public class CoreModule : IModule
	{
		static Logger logger = LogManager.GetCurrentClassLogger();

		public string Id => "core-module";

		public string Name => "Bebruhal Core Module";

		public string Author => "Bronuh";

		public string Description => "Встроенный модуль бота. От его имени зарегистрированы все внутренние команды.";

		public string URL => "http://localhost";

		public bool IsReady => true;

		public List<Function> Functions => new();

		public void Broadcast(Message message)
		{
			SendMessage(message,"broadcast");
		}

		public void Init(BotContext context)
		{

		}

		public void PostInit(BotContext context)
		{

		}

		public void PreInit(BotContext context)
		{

		}

		public void Save()
		{

		}

		public void SendMessage(Message msg, string address)
		{
			
			logger.Info($"{(address=="broadcast" ? "*BROADCAST*" : "")}[{Name}](): {msg.Text}");
		}
	}
}
