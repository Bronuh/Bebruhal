using Bebruhal.Interfaces;

namespace Potracheno
{
	public class WastedPlugin : IPlugin
	{
		public string Name => "Wasted Plugin";

		public string Description => "Тратит входной текст";

		public string Id => "wasted-plugin";

		public string Author => "Bronuh";

		public string URL => "https://localhost";

		public string[] RequiredModules => null;


		public void Init(BotContext context)
		{
			Command cmd = new Command("waste", async (msg, ctx) => 
			{
				var text = msg.GetTextWithoutCommand();
				if (text != null)
				{
					msg.Respond(Wastificator.Wastificate(text));
				}
			}).AddAliases("wasted", "потрачено", "потратить")
			.SetDescription("Делает текст потраченым");


			context.CommandsManager.RegisterCommand(cmd, this);
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
	}
}