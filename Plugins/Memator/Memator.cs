using Bebruhal.Interfaces;
using Bebruhal.Types;

namespace Memator
{
	public class Memator : IPlugin
	{
		public string Name => "Memator";

		public string Description => "Делает мемесы";

		public string Id => "memator";

		public string Author => "Bronuh";

		public string URL => "http://localhost";

		public string[]? RequiredModules => null;

		internal static Memator Instance { get; private set; }

		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		public void PreInit(BotContext context)
		{
			MainCommands.Initialize(this, context);
			MemesController.Init(context);
			Instance = this;
		}

		public void Init(BotContext context)
		{
			context.CommandsManager.RegisterCommand(new Command("random", async (msg, ctx) =>
			{
				try
				{
					Message response = new Message();
					var imagesPath = MemesController.GetMemeImagesPath(Name);
					var files = Directory.GetFiles(MemesController.GetRandomPath());

					response.Images = new List<Image>();
					response.Text = "";
					response.Images.Add(Image.Load(files.GetRandom()));

					msg.Respond(response);
				}
				catch(Exception e)
				{
					Logger.Warn(e.Message);
				}
			}
			).AddAliases("rand","рандом","случайный"), this);
		}

		public void PostInit(BotContext context)
		{
			
		}

		public void Save()
		{
			MemesController.SaveAll();
		}
	}
}