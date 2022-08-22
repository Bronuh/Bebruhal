using Config.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_Module.Types
{
	public interface ITelegramConfig : IConfigSaver
	{
		[Option(DefaultValue = "")]
		public string Token { get; set; }

		[Option(DefaultValue = false)]
		public bool PreregistrateUsers { get; set; }
	}
}
