using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memator
{
	internal static class MemesController
	{
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		private static BotContext context;
		private static List<Meme> memes = new();
		private static string
			_mematorDir,
			_memesDir,
			_randomDir;

		internal static void Init(BotContext ctx)
		{
			context = ctx;
			_mematorDir = Path.Combine(ctx.Session.GetMediaDir(), "memator");
			_randomDir = Path.Combine(_mematorDir, "random");
			_memesDir = Path.Combine(_mematorDir, "memes");

			Directory.CreateDirectory(_mematorDir);
			Directory.CreateDirectory(_memesDir);
			Directory.CreateDirectory(_randomDir);

			ScanMemes();
		}

		private static void ScanMemes()
		{
			Logger.Info($"Scanning memes...");
			var dirs = Directory.GetDirectories(_memesDir);
			foreach (var dir in dirs)
			{
				ScanMeme(dir);
			}
			foreach(var meme in memes)
			{
				context.CommandsManager.RegisterCommand(meme, Memator.Instance);
			}
		}

		private static void ScanMeme(string path)
		{
			try
			{
				Logger.Info($"Scanning meme dir '{path}'");
				string memeInfoPath = Path.Combine(path, "memeInfo.json");
				if(File.Exists(memeInfoPath))
				{
					try
					{
						var info = MemeInfo.Deserialize(File.ReadAllText(memeInfoPath));
						memes.Add(new Meme(info));
					}
					catch(Exception ex)
					{
						Logger.Warn(ex.Message);
						memes.Add(GenerateMemeFromName(Path.GetFileName(path)));
					}
				}
				else
				{
					Logger.Warn($"Meme info does not exixts. Generating new {Path.GetFileName(path)}");
					memes.Add(GenerateMemeFromName(Path.GetFileName(path)));
				}

			}
			catch(Exception e)
			{
				Logger.Warn(e.Message);
				memes.Add(GenerateMemeFromName(Path.GetDirectoryName(path)));
			}
		}

		public static Meme GenerateMemeFromName(string name)
		{
			MemeInfo info = new MemeInfo();
			info.Name = name;
			info.Tags.Add("meme");
			info.Tags.Add("fun");
			Directory.CreateDirectory(GetMemePath(name));
			Directory.CreateDirectory(GetMemeImagesPath(name));
			var meme = new Meme(info);
			meme.Save();
			return meme;
		}

		public static void SaveAll()
		{
			foreach (var meme in memes)
			{
				meme.Save();
			}
		}

		internal static string GetMemeImagesPath(string name)
		{
			return Path.Combine(_memesDir,name,"images");
		}

		internal static string GetMemePath(string name)
		{
			return Path.Combine(_memesDir, name);
		}

		internal static string GetMemeInfoPath(string name)
		{
			return Path.Combine(_memesDir, name, "memeInfo.json");
		}

		internal static string GetRandomPath()
		{
			return _randomDir;
		}
	}
}
