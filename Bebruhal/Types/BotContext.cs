using Bebruhal.Utils.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	public sealed class BotContext
	{
		public Session Session { get; internal set; }
		public CommandsManager CommandsManager { get; internal set; }
		public IBebruhalConfig Config { get; internal set; }
		public BebruhalCore Core { get; internal set; }
		public ModulesManager ModulesManager { get; internal set; }
		public PluginsManager PluginsManager { get; internal set; }
	}
}
