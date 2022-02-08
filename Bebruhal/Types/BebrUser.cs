namespace Bebruhal.Types
{
	/// <summary>
	/// Класс пользователя в ядре бота
	/// </summary>
	public class BebrUser
	{
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// Экземпляр класса, необходимый для возврата вместо null
		/// </summary>
		public static BebrUser Empty { get; } = new BebrUser();
		public static BebrUser ConsoleUser { get; } = new BebrUser { isAdmin = true, isConsole = true, ExternalId = "Console" };

		/// <summary>
		/// Имя пользователя. Указывается при возможности модулем во время регистрации
		/// </summary>
		[JsonProperty]
		public string Name { get; set; } = null;

		/// <summary>
		/// Внешний идентификатор пользователя
		/// </summary>
		[JsonProperty]
		public string ExternalId { get; set; } = null;

		/// <summary>
		/// Уникальный иденификатор пользователя, позволяющий осуществлять быстрый поиск по списку.
		/// Также является именем файла, в котором хранятся данные пользователя.
		/// </summary>
		[JsonProperty]
		public string GUID { get; internal set; }

		/// <summary>
		/// Внутренний идентификатор пользователя. Назначается исходя из свободных Id сессии при регистрации.
		/// </summary>
		[JsonProperty]
		public int Id { get; internal set; }

		/// <summary>
		/// Время создания/регистрации пользователя
		/// </summary>
		[JsonProperty]
		public long RegistrationTime { get; internal set; }

		/// <summary>
		/// Имя (Id) источника (модуля) из которого зарегистрирован пользователь
		/// </summary>
		[JsonProperty]
		public string Source { get; internal set; }

		/// <summary>
		/// Id группы, к которой пренадлежит пользователь. Группы определяют уровень доступа и дополнительные разрешения.
		/// </summary>
		[JsonProperty]
		public int Rank { get; set; }

		[JsonIgnore]
		private bool isAdmin = false;

		[JsonIgnore]
		private bool isConsole = false;

		[JsonProperty]
		private List<string> aliases = new List<string>();
		[JsonProperty]
		private List<string> modules = new List<string>();
		[JsonProperty]
		private List<string> tags = new List<string>();
		[JsonProperty]
		internal CompoundProperty properties = new CompoundProperty();


		public BebrUser() { }

		internal BebrUser(IModule source, int groupId) 
		{
			Source = source.GetType().Name;
			Rank = groupId;
			RegistrationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			properties = new CompoundProperty("main");
		}

		/// <summary>
		/// Возвращает корневое составное свойство
		/// </summary>
		/// <returns>Корневое составное свойство, хранящее остальные свойства</returns>
		public CompoundProperty GetProperties()
		{
			return properties;
		}

		/// <summary>
		/// Дает пользователю права консоли
		/// </summary>
		/// <param name="isConsole"></param>
		public void SetConsole(bool isConsole)
		{
			this.isConsole = isConsole;
		}

		public bool IsConsole()
		{
			return isConsole;
		}

		public bool IsAdmin()
		{
			return isAdmin || isConsole;
		}

		public bool IsEmpty()
		{
			return this == Empty;
		}

		public void AddTag(string tag)
		{
			if (!tags.Contains(tag))
			{
				tags.Add(tag);
			}
		}

		public void AddTags(params string[] tags)
		{
			foreach (var tag in tags)
			{
				AddTag(tag);
			}
		}

		public void AddAlias(string alias)
		{
			if (!aliases.Contains(alias))
			{
				aliases.Add(alias);
			}
		}

		public void AddAliases(params string[] aliases)
		{
			foreach (var alias in aliases)
			{
				AddAlias(alias);
			}
		}

		public bool CheckUser(string text)
		{

			if (text.ToLower() == ExternalId?.ToLower())
			{
				return true;
			}
			else if (text.ToLower() == GUID?.ToLower())
			{
				return true;
			}
			else
			{
				LoggerProxy.Debug("Имя пользователя не найдено, поиск псевдонимов");
				foreach (string alias in aliases)
				{
					LoggerProxy.Debug("Псевдоним: " + alias);
					if (text.ToLower() == alias.ToLower())
					{
						return true;
					}
				}
			}
			return false;
		}

		public void RemoveTag(string tag)
		{
			try
			{
				tags.Remove(tag);
			}
			catch(Exception ex)
			{
				Logger.Warn($"Не удалось удалить тег: {ex}");
			}
		}

		/// <summary>
		/// Захардкоженый пользователь для тестов
		/// </summary>
		public static BebrUser Dummy { get; } = new BebrUser()
		{
			Id = 1,
			RegistrationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
			Source = "bebruhal",
			Rank = 1,
			aliases = new List<string>(new[] { "alpha", "beta" }),
			modules = new List<string>(new[] { "vk", "discord" }),
			tags = new List<string>(new[] { "dummy", "useless", "fuck me" }),
			properties = new CompoundProperty("main")
		};
	}
}
