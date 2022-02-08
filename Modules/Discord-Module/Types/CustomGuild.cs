using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Module.Types
{
	internal class CustomGuild
	{
		internal DiscordGuild Source { get; private set; } = null;
		internal DiscordChannel BotChannel { get; private set; } = null;
		internal List<DiscordChannel> Channels { get; private set; }
		internal ulong Id { get; private set; }
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		private CustomGuild() { }
		internal CustomGuild(DiscordGuild source)
		{
			Id = source.Id;
			Source = source;
			

			

			if (BotChannel is null)
			{
				BotChannel = source.Channels.Values.First();
			}
		}

		internal static async Task<CustomGuild> BuildCustomGuild(DiscordGuild source)
		{
			var guild = new CustomGuild();
			guild.Id = source.Id;
			guild.Source = source;
			guild.Channels = new List<DiscordChannel>(await source.GetChannelsAsync());

			foreach (var channel in guild.Channels)
			{
				var lower = channel.Name.ToLower();
				if (lower == "bot" || lower == "bots")
				{
					guild.BotChannel = channel;
					Logger.Debug($"Найден канал для ботов: {channel.Name} ({channel.Id})");
					break;
				}
				if (guild.BotChannel is null)
				{
					guild.BotChannel = guild.Channels.First();
				}
			}

			return guild;
		}

		internal async Task<IReadOnlyList<DiscordChannel>> GetChannelsAsync()
		{
			return await Source.GetChannelsAsync();
		}
	}
}
