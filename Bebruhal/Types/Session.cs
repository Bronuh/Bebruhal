using Bebruhal.Types.Builders;
using NLog;

namespace Bebruhal.Types
{
	/// <summary>
	/// Класс обеспечивает хранение данных о сессии - пользователях, медиа-данных и другой информации. 
	/// Предоставляет поля и методы для простого доступа к данным сессии.
	/// </summary>
	public class Session
	{
		private const string _sessionsDir = "sessions";
		private const string _usersDir = "users";
		private const string _mediaDir = "media";
		private const string _dataDir = "data";

		private const string _fileExtension = "json";

		static Logger logger = LogManager.GetCurrentClassLogger();
		static int defaultGroupId = 1;
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		[JsonProperty]
		private int usersCount = 0;

		/// <summary>
		/// Список пользователей, зарегистрированных в сессии. Не сериализуется.
		/// </summary>
		[JsonIgnore]
		public List<BebrUser> Users { get; private set; } = new List<BebrUser>();


		/// <summary>
		/// Мета-информация о сессии.
		/// </summary>
		[JsonProperty]
		public SessionInfo SessionInfo { get; internal set; } = new SessionInfo();


		private Dictionary<string, BebrUser> _guidUsers = new();
		private Dictionary<string, BebrUser> _nameUsers = new();

		
		/// <summary>
		/// Пустой конструктор
		/// </summary>
		internal Session() { }




		/// <summary>
		/// Пытаетмя создать новую сессию с указанным именем. Сессия не будет создана, если отсутствуют права на запись в ./sessions/<paramref name="name"/>, 
		/// или если сессия с таким именем уже существует.
		/// </summary>
		/// <param name="name">Имя сессии</param>
		/// <returns>Созданная сессия, либо null в случае неудачи</returns>
		public static Session? CreateSession(string name)
		{
			if (Directory.Exists(Path.Combine(_sessionsDir, name)))
			{
				logger.Error($"Session.CreateSession: Попытка создания уже существующей сессии {name}. Был возвращен null");
				return null;
			}
			var _session = new Session();
			_session.SessionInfo.SessionName = name;
			Filesystem.EnsureDirExists(Path.Combine(_sessionsDir, name));
			Filesystem.EnsureDirExists(Path.Combine(_session.GetPath(), _usersDir));
			Filesystem.EnsureDirExists(Path.Combine(_session.GetPath(), _dataDir));
			Filesystem.EnsureDirExists(Path.Combine(_session.GetPath(), _mediaDir));
			_session.Save();
			return _session;
		}




		/// <summary>
		/// Загружает сессию с указанным именем. Если бот не имеет доступа на чтение из ./sessions/<paramref name="name"/>, либо при невозможности
		/// прочитать ./sessions/<paramref name="name"/>/sessionInfo.json, то буудет возвращен null
		/// </summary>
		/// <param name="name">Имя сессии</param>
		/// <returns>Загруженная сессия или null в случае неудачи</returns>
		public static Session? Load(string name)
		{
			if (!Directory.Exists(Path.Combine(_sessionsDir,name)))
			{
				logger.Error($"Session.Load: Попытка загрузки несуществующей сессии {name}. Был возвращен null");
				return null;
			}
			var _session = new Session();
			//_session.SessionInfo.SessionName = name;
			var json = File.ReadAllText(Path.ChangeExtension(Path.Combine(_sessionsDir,name,"sessionInfo"),_fileExtension));
			_session.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(json) ?? new SessionInfo();
			_session.Prepare();
			return _session;
		}




		/// <summary>
		/// Производит сохранение данных сессии.
		/// </summary>
		public void Save()
		{
			try
			{
				var json = JsonConvert.SerializeObject(this,Formatting.Indented);
				File.WriteAllText(Path.ChangeExtension(Path.Combine(_sessionsDir, SessionInfo.SessionName, "sessionInfo"), _fileExtension), json);
				SaveUsers();
			}
			catch (Exception ex)
			{
				logger.Error($"Не удалось сохранить сессию:\n{ex}");
			}
		}




		internal void Prepare()
		{
			logger.Info($"Подготовка сессии {SessionInfo.SessionName}");
			Filesystem.EnsureDirExists(GetPath());
			Filesystem.EnsureDirExists(Path.Combine(GetPath(), _usersDir));
			Filesystem.EnsureDirExists(Path.Combine(GetPath(), _dataDir));
			Filesystem.EnsureDirExists(Path.Combine(GetPath(), _mediaDir));
			PrepareLoadUsers();
		}




		private void PrepareLoadUsers()
		{
			logger.Info("Загрузка данных пользователей");
			Directory.CreateDirectory(Path.Combine(GetPath(), _usersDir));
			try
			{
				var files = Directory.GetFiles(Path.Combine(GetPath(), _usersDir));
				logger.Info($"В разделе {Path.Combine(GetPath(), _usersDir)} обнаружено {files.Count()} файлов");
				foreach (var file in files)
				{
					var json = File.ReadAllText(file);
					BebrUser? user = JsonConvert.DeserializeObject<BebrUser>(json);
					if (user is not null)
					{
						Users.Add(user);
						_guidUsers.Add(user.GUID, user);
						if(user.Name != null)
							try
							{
								_nameUsers.Add(user.Name.ToLower(), user);
							}
							catch (Exception ex)
							{
								Logger.Warn($"Не удалось добавить псевдоним '{user.Name.ToLower()}' пользователя '{user.Name ?? "<безымянный>"}':" +
									$"\n{ex.Message}");
							}

						foreach (var alias in user.GetAliases())
						{
							try
							{
								_nameUsers.Add(alias.ToLower(), user);
							}
							catch (Exception ex)
							{
								Logger.Warn($"Не удалось добавить псевдоним '{alias.ToLower()}' пользователя '{user.Name?.ToLower() ?? "<безымянный>"}':" +
									$"\n{ex.Message}");
							}
						}
						logger.Debug($"{file} успешно десериализован и добавлен в список пользователей");
					}
					else
					{
						logger.Warn($"Не удалось десериализовать файл {file}");
					}
				}
			}
			catch (Exception ex)
			{
				logger.Warn($"Не удалось загрузить пользователей из директории {Path.Combine(GetPath(),_usersDir)}:\n{ex}");
			}
		}





		/// <summary>
		/// Регистрирует нового пользователя в сессии.
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="identifier">Внешний идентификатор пользователя</param>
		/// <param name="source">Модуль из которого производится регистрация</param>
		/// <param name="tags">Список тегов пользователя</param>
		/// <returns>Зарегистрированный пользователь</returns>
		public BebrUser RegisterUser(string name, string identifier, IModule source, IEnumerable<string>? tags)
		{
			logger.Info("Попытка зарегистрировать нового пользователя...");
			var user = BebrUser.Empty;

			try
			{
				user = new BebrUserBuilder().Source(source).GroupId(defaultGroupId).Build();
			}
			catch (Exception ex)
			{
				logger.Warn("Исключение во время подготовки пользователя к регистрации:\n"+ex);
			}

			if (user == BebrUser.Empty)
			{
				logger.Warn("Новый пользователь не зарегистрирован. Будет возвращен пустой пользователь.");
			}
			else
			{
				user.ExternalId = identifier;
				user.GUID = Guid.NewGuid().ToString();
				user.properties = new CompoundProperty("main");
				user.properties.SetCompound(source.Id, new CompoundProperty(source.Id));
				user.Id = usersCount;
				user.Name = name;
				user.AddModule(source.Id);
				if(tags != null)
					user.AddTags(tags.ToArray());

				_guidUsers.Add(user.GUID,user);
				_nameUsers.Add(user.ExternalId.ToLower(), user);

				Users.Add(user);
				usersCount++;
				Save();
			}
			RegisteredUser?.Invoke(source, new RegisteredUserEventArgs(user));
			return user;
		}


		/// <summary>
		/// Пытается найти пользователя в списке зарегистрированных
		/// </summary>
		/// <param name="identifier">Строка для поиска - имя, псевдоним, id или GUID</param>
		/// <returns>Найденный пользователь, либо BebrUser.Empty</returns>
		public BebrUser GetUserByIdentifier(string identifier)
		{
			var user = BebrUser.Empty;

			try
			{
				user = _nameUsers[identifier];
			}
			catch
			{
				Logger.Debug($"Не удалось найти в словаре пользователя по ключу '{identifier}'. Поиск будет продолжен перебором.");
			}
			if(user.IsEmpty())
			foreach (var _user in Users)
			{
				if (user.CheckUser(identifier))
				{
					user = _user;
				}
			}

			return user;
		}

		/// <summary>
		/// Возвращает относительный путь до директории сессии
		/// </summary>
		/// <returns>Путь до корня сессии</returns>
		public string GetPath()
		{
			return Path.Combine(_sessionsDir, SessionInfo.SessionName);
		}



		/// <summary>
		/// TODO: Доделать сериализацию пользователя
		/// Сериализует и сохраняет пользователей в папку users в директории сессии. Каждый пользователь сохраняется в отдельный файл.
		/// </summary>
		private void SaveUsers()
		{
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented
			};

			foreach (var user in Users)
			{
				var json = JsonConvert.SerializeObject(user,settings);
				var path = Path.ChangeExtension(Path.Combine(GetPath(), _usersDir, user.GUID), _fileExtension);
				try
				{
					File.WriteAllText(path,json);
				}
				catch (Exception e)
				{ 
					logger.Error(e, $"Ошибка при записи пользователя {user.ExternalId} с GUID {user.GUID} в файл {path}:\n{e}");
				}
			}
		}


		/// <summary>
		/// Вызывается модулями, сообщая системе об окончании предварительной регистрации нового пользователя
		/// </summary>
		public event AsyncEventHandler<IModule, RegisteredUserEventArgs>? RegisteredUser;

		/// <summary>
		/// Вызывается в момент слияния пользователей
		/// </summary>
		public event AsyncEventHandler<IModule, MergedUsersEventArgs>? MergedUsers;

	}
}
