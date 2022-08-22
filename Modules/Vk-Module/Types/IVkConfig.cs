using Bebruhal.Interfaces;
using Config.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vk_Module.Types
{
	public interface IVkConfig : IConfigSaver
	{
		[Option(DefaultValue = "")]
		public string Token { get; set; }

		[Option(DefaultValue = 88798690)]
		public int GroupId{ get; set; }
	}
}
