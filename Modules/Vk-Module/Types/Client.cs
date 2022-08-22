using Bebruhal.Types;
using Config.Net;
using NLog;
using SixLabors.ImageSharp;
using Vk_Module.Types;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using Message = Bebruhal.Types.Message;
using VkMessage = VkNet.Model.Message;
using Bebruhal;

namespace Vk_Module.Types
{
	internal static class Client
	{
		private static BotContext _botContext;
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		private static Session Session { get; set; }
		internal static BotContext BotContext { get; set; }
		public static VkApi API;
		public static IVkConfig Config { get; set; }


		internal static void Restart() { }

		internal static async void SendMessage(Message msg, string address)
		{
			long target;
			try
			{
				target = Int64.Parse(address.Split('.').Last());
			}
			catch (FormatException)
			{
				return;
			}
			Dictionary<string, Stream> files = new Dictionary<string, Stream>();
			int filesCount = 0;
			using var cts = new CancellationTokenSource();
			//botClient.SendPhotoAsync();

			var text = msg.ReplaceMentions((user) => { return $"@{user.Name}"; });

			Random rnd = new Random();
			try
			{
				API.Messages.Send(new MessagesSendParams()
				{
					RandomId = rnd.Next(),
					ChatId = target,
					Message = msg.Text
				});
			}catch(Exception e)
			{
				Logger.Warn(e.Message);
			}

			if (msg.Images.Count!=0)
			{
				Console.WriteLine("Sending image");
				rnd = new Random();
				foreach (var image in msg.Images)
				{
					API.Messages.Send(new MessagesSendParams()
					{
						RandomId = rnd.Next(),
						ChatId = target,
						Attachments = new List<MediaAttachment> { Utils.ImageDocument(image,target) }
					});
				}
			}
		}



		internal static async Task Start(IVkConfig config, BotContext ctx)
		{
			Logger.Info($"Попытка подключения к Vk-API с помощью токена {config.Token}");

			API = new VkApi();
			BotContext = ctx;
			Session = ctx.Session;
			Config = config;

			API.Authorize(new ApiAuthParams()
			{
				AccessToken = config.Token,
				
			});
			Utils.SetApi(API);
			// Initialize();

			new Thread(() => {
				Upd();
			}).Start();
		}


		static void Upd()
		{
			while (VkModule.isRunning) // Бесконечный цикл, получение обновлений
			{
				try
				{
					var s = API.Groups.GetLongPollServer(204006161);
					var poll = API.Groups.GetBotsLongPollHistory(
					   new BotsLongPollHistoryParams()
					   { Server = s.Server, Ts = s.Ts, Key = s.Key, Wait = 10 });
					if (poll?.Updates == null) continue; // Проверка на новые события


					foreach (var a in poll.Updates)
					{
						if (a.Type == GroupUpdateType.MessageNew)
						{
							string userMessage = a.MessageNew.Message.Text ?? "NULL";//a.Message.Body?.ToLower() ?? "NULL";
							long? userID = a.MessageNew.Message.UserId;
							Console.WriteLine("Всосал сообщеньку: " + userMessage);
								HandleMessageAsync(a.MessageNew.Message).GetAwaiter().GetResult();
								//HandleCommand(userMessage, a.MessageNew.Message.PeerId - 2000000000);
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e.Message+" ("+e.Source+"):\n"+e.StackTrace);
				}

			}
		}

		static async Task HandleMessageAsync(VkMessage tMsg)
		{
			Logger.Info("Обработка сообщения");
			if (tMsg.FromId is null)
			{
				Logger.Info("Отправитель неизвестен");
				return;
			}

			string userId = tMsg.FromId.ToString();
			long[] list = { (long)tMsg.FromId };
			var userData = API.Users.Get(list).First();
			Logger.Info($"{userData.FirstName} {userData.LastName} ({userId}): {tMsg.Text}");
			BebrUser user = BebrUser.Empty;
			try
			{
				user = Session.GetUser(userId);
			}
			catch(Exception e)
			{
				Logger.Error("[VkHandleMessageAsync] "+e.Message + " (" + e.Source + "):\n" + e.StackTrace);
			}
			Logger.Info($"Пользователь \"найден\"");

			if (user.IsEmpty())
			{
				Logger.Info($"Не удалось найти пользователя по Id {userId}. Попытка регистрации.");
				user = Session.RegisterUser($"{userData.FirstName}_{userData.LastName}", userId, VkModule.Instance, null);
				Session.AddAliases(user, userData.Nickname);
			}

			user.AddTempAlias($"{userData.FirstName} {userData.LastName}");


			Message msg = new Message();
			msg.Author = user;
			msg.Text = tMsg.Text;
			msg.Module = VkModule.Instance;
			msg.RawAuthor = userId;
			msg.Source = $"{VkModule.Instance.Id}.chat.{tMsg.PeerId - 2000000000}";
			msg.Info = $"{user.Name}@{tMsg.PeerId}";

			await BotContext.Core.ProcessMessage(msg);
		}
	}
}
