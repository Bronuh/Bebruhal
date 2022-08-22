using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memator
{
	public class MemeInfo
	{
		public string Name { get; set; } = null;
		public string Description { get; set; } = "Очередной мемчик";
		public string Permission { get; set; } = "";
		public List<string> Aliases { get; set; } = new List<string>();
		public List<string> Tags { get; set; } = new List<string>();
		public int Rank { get; set; } = 0;
		public bool OpOnly { get; set; } = false;

		internal string Serialize()
		{
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented
			};
			return JsonConvert.SerializeObject(this, settings);
		}

		internal static MemeInfo Deserialize(string code)
		{
			return JsonConvert.DeserializeObject<MemeInfo>(code);
		}
	}
}
