using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memator
{
	internal class Meme : Command
	{
		private Meme() { }


		public Meme(MemeInfo info)
		{
			Name = info.Name;
			Description = info.Description;
			Rank = info.Rank;
			Permission = info.Permission;
			Tags = info.Tags;
			Aliases = info.Aliases;
			OpOnly = info.OpOnly;
			Action = async (msg, ctx) =>
			{
				Message response = new Message();
				var imagesPath = MemesController.GetMemeImagesPath(Name);
				var files = Directory.GetFiles(imagesPath);
				response.Text = "";

				response.Images = new List<Image>();

				response.Images.Add(Image.Load(files.GetRandom()));

				msg.Respond(response);
			};
		}


		public MemeInfo GetMemeInfo()
		{
			var info = new MemeInfo();

			info.Name = Name;
			info.Description = Description;
			info.Rank = Rank;
			info.Permission = Permission;
			info.Tags = Tags;
			info.Aliases = Aliases;
			info.OpOnly = OpOnly;

			return info;
		}

		internal void Save()
		{
			var info = GetMemeInfo();
			var infoJson = info.Serialize();

			var path = MemesController.GetMemeInfoPath(Name);
			File.WriteAllText(path, infoJson);
		}
	}
}
