using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentioner_Plugin
{
	internal static class MentionsController
	{
		private static List<Mention> mentions = new();

		internal static async Task ProcessMention(IModule? module, ReceivedMessageEventArgs args)
		{
			string text = args.Message.Text;
			string[] words = text.Split(' ');
			string msg = "";
			bool mentioned = true;

			foreach (string word in words)
			{
				if (mentioned)
				{
					foreach (Mention mention in mentions)
					{
						if (mention.Match(word))
						{
							BebrUser author = args.Author;
							if (author.Rank >= mention.Rank)
							{
								var clearText = mention.GetClearText(word);
								if (clearText.Length >= 3)
								{
									BebrUser target = MentionerPlugin.Context.Session.GetUser(clearText);
									if (!target.IsEmpty())
									{
										msg += mention.Message.Replace("%MENTION%", target.GetMention()) + "\n";
										mention.CustomAction(author,target);
									}
								}
							}
							else
							{
								msg += "Слишком низкий ранг для этого действия (" + author.Rank + "/" + mention.Rank + ")\n";
							}
							mentioned = true;
							break;
						}
						mentioned = false;
					}
				}
				else
				{
					break;
				}
			}
			if (msg != "")
			{
				args.Message.Respond(msg);
			}
		}

		internal static void InitializeMentions()
		{
			mentions.Add(new Mention()
			{
				Prefix = "",
				Suffix = ".",
				Message = "%MENTION%, веточкой тык.",
				Rank = 0
			});

			mentions.Add(new Mention()
			{
				Prefix = "",
				Suffix = "!!!",
				Message = "%MENTION%, бревном хуяк!!!",
				Rank = 0
			});

			mentions.Add(new Mention()
			{
				Prefix = "",
				Suffix = "!",
				Message = "%MENTION%, палкой пиздык!",
				Rank = 0
			});

			mentions.Add(new Mention()
			{
				Prefix = "-",
				Suffix = "",
				Message = "%MENTION%, деревом еблысь!!11!1!1111",
				Rank = 0
			});
		}
	}
}
