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
		public static BebrUser Empty { get; } = new BebrUser() { ExternalId = "empty",  isEmpty = true, Name = "empty"};

		/// <summary>
		/// Пользователь, от чьего имени должны выполнятся консольные команды
		/// </summary>
		public static BebrUser ConsoleUser { get; } = new BebrUser { isAdmin = true, isConsole = true, Name = "Console", ExternalId = "-1", Id = -1 };

		/// <summary>
		/// Имя пользователя. Указывается при возможности модулем во время регистрации
		/// </summary>
		[JsonProperty]
		public string? Name { get; set; } = null;

		/// <summary>
		/// Дополнительная информациия о пользователе
		/// </summary>
		[JsonProperty]
		public string About { get; internal set; } = "Пользователь не указал дополнительной информации о себе";

		/// <summary>
		/// Внешний идентификатор пользователя
		/// </summary>
		[JsonProperty]
		public string? ExternalId { get; set; } = null;

		/// <summary>
		/// Уникальный иденификатор пользователя, позволяющий осуществлять быстрый поиск по списку.
		/// Также является именем файла, в котором хранятся данные пользователя.
		/// </summary>
		[JsonProperty]
		public string GUID { get; internal set; } = "";

		/// <summary>
		/// Внутренний идентификатор пользователя. Назначается исходя из свободных Id сессии при регистрации.
		/// </summary>
		[JsonProperty]
		public int Id { get; internal set; } = 0;

		/// <summary>
		/// Время создания/регистрации пользователя
		/// </summary>
		[JsonProperty]
		public long RegistrationTime { get; internal set; }

		/// <summary>
		/// Имя (Id) источника (модуля) из которого зарегистрирован пользователь
		/// </summary>
		[JsonProperty]
		public string Source { get; internal set; } = "";

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
		private List<string> aliases = new();
		[JsonProperty]
		private List<string> modules = new();
		[JsonProperty]
		private List<string> tags = new();
		[JsonProperty]
		internal CompoundProperty properties = new();


		private bool isEmpty = false;
		/// <summary>
		/// Пустой конструктор
		/// </summary>
		public BebrUser() { }

		internal BebrUser(IModule source, int groupId) 
		{
			Source = source.Id;
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

		/// <summary>
		/// Дает пользователю права администратора
		/// </summary>
		/// <param name="isAdmin"></param>
		public void SetOp(bool isAdmin)
		{
			this.isAdmin = isAdmin;
		}

		/// <summary>
		/// Возвращает true, если пользователь является BebrUser.ConsoleUser
		/// </summary>
		/// <returns></returns>
		public bool IsConsole()
		{
			return isConsole;
		}

		/// <summary>
		/// Возвращает true, если пользователь является администратором или консолью
		/// </summary>
		/// <returns></returns>
		public bool IsAdmin()
		{
			return isAdmin || isConsole;
		}

		/// <summary>
		/// Возвращает список псевдонимов пользователя. Список создается с нуля и не является ссылкой на список псевдонимов, 
		/// содержащийся внутри пользователя.
		/// </summary>
		/// <returns>Список псевдонимов</returns>
		public IEnumerable<string> GetAliases()
		{
			foreach (var alias in aliases)
			{
				yield return alias;
			}
		}

		/// <summary>
		/// Возвращает тру, если пользователь является заглушкой BebrUser.Empty
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			return isEmpty;
		}

		/// <summary>
		/// Добавляет тег пользователю
		/// </summary>
		/// <param name="tag">Тег</param>
		public void AddTag(string tag)
		{
			if (!tags.Contains(tag.ToLower()))
			{
				tags.Add(tag.ToLower());
			}
		}

		/// <summary>
		/// Добавляет несколько тегов пользователю
		/// </summary>
		/// <param name="tags">Список тегов</param>
		public void AddTags(params string[] tags)
		{
			foreach (var tag in tags)
			{
				AddTag(tag);
			}
		}

		/// <summary>
		/// Добавляет псевдоним пользователю
		/// </summary>
		/// <param name="alias">Псевдоним</param>
		public void AddAlias(string alias)
		{
			if (!aliases.Contains(alias))
			{
				aliases.Add(alias);
			}
		}

		/// <summary>
		/// Добавляет несколько псевдонимов пользователю
		/// </summary>
		/// <param name="aliases">Список псевдонимов</param>
		public void AddAliases(params string[] aliases)
		{
			foreach (var alias in aliases)
			{
				AddAlias(alias);
			}
		}

		/// <summary>
		/// Проверяет пользователя на соответствие строке. Проверка проводится на полное соответствие после ToLower() в следующем 
		/// порядке: ExternalId > GUID > Name > aliases[]
		/// </summary>
		/// <param name="text">Искомый текст</param>
		/// <returns>true, если найдено совпадение хотя бы в одном изпроверяемых свойств</returns>
		public bool CheckUser(string text)
		{
			var key = text.ToLower();

			if (key == ExternalId?.ToLower())
			{
				return true;
			}
			else if (key == GUID?.ToLower())
			{
				return true;
			}
			else if (key.Length>2)
			{
				try { if (Name.ToLower().Contains(key)) return true; } catch { }
				
			}
			else
			{
				LoggerProxy.Debug("Имя пользователя не найдено, поиск псевдонимов");
				foreach (string alias in aliases)
				{
					LoggerProxy.Debug("Псевдоним: " + alias);
					if (key.Length > 2)
					{
						try { if (alias.ToLower().Contains(key)) return true; } catch { }

					}
				}
			}
			return false;
		}

		/// <summary>
		/// Пытается удалить указанный тег
		/// </summary>
		/// <param name="tag">Тег</param>
		public void RemoveTag(string tag)
		{
			try
			{
				tags.Remove(tag.ToLower());
			}
			catch(Exception ex)
			{
				Logger.Warn($"Не удалось удалить тег: {ex}");
			}
		}

		/// <summary>
		/// Пытается удалить указанный ключ из списка псевдонимов
		/// </summary>
		/// <param name="key">искомый псевдоним</param>
		public void RemoveAlias(string key)
		{
			string found = null;
			foreach(var alias in aliases)
			{
				if (alias.ToLower().Equals(key)) found = alias;
				break;
			}
			if(found != null)
				aliases.Remove(found);
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

		internal void AddModule(string id)
		{
			modules.Add(id);
		}
	}
}
