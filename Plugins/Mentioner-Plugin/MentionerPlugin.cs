using Bebruhal.Interfaces;
using Bebruhal.Types;

namespace Mentioner_Plugin
{
	public class MentionerPlugin : IPlugin
	{
		public string Description => "Позволяет палкопиздыкать людишек";

		public string Author => "Bronuh";

		public string URL => "http://localhost";

		public string[]? RequiredModules => null;

		public string Name => "Mentioner";

		public string Id => "mentioner-plugin";

		static Logger logger = LogManager.GetCurrentClassLogger();

		internal static MentionerPlugin Instance { get; private set; }
		internal static BotContext Context { get; private set; }

		public void Init(BotContext context)
		{
			context.Core.RecievedMessage += MentionsController.ProcessMention;
		}

		public void PostInit(BotContext context)
		{
			
		}

		public void PreInit(BotContext context)
		{
			Instance = this;
			Context = context;

			MentionsController.InitializeMentions();
		}

		public void Save()
		{
			
		}
	}
}