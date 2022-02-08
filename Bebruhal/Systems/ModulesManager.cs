using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Systems
{
	/// <summary>
	/// Реализует общий фасад для всех модулей. Отвечает за подключение модулей, отправку сообщений в модули и вызов нестандартных функций в модулях.
	/// </summary>
	public sealed class ModulesManager
	{
		private const string _modulesDir = "modules";
		private List<Assembly> _assemblies = new List<Assembly>();
		internal List<string> modulesPaths = new List<string>();
		private List<IModule> _modules = new List<IModule>();
		private Dictionary<string, IModule> _modulesCache = new Dictionary<string,IModule>();
		private BotContext context;
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		// PUBLIC REGION //
		/// <summary>
		/// Создает новый экземпляр на основе указанного контекста
		/// </summary>
		/// <param name="context">Контекст работы бота</param>
		public ModulesManager(BotContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Возвращает список идентификаторов загруженных модулей
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetModulesList()
		{
			foreach (var module in _modules)
			{
				yield return module.Id;
			}
		}

		/// <summary>
		/// Возвращает список подключенных модулей
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IModule> GetModules() { return _modules; }

		/// <summary>
		/// Вызывает нестандартную функцию в указанном модуле
		/// </summary>
		/// <param name="targetModuleId">Целевой модуль для выполнения функции</param>
		/// <param name="functionId">Id требуемой функции</param>
		/// <param name="args">Параметры, передаваемые в функцию</param>
		public void CallModule(string targetModuleId, string functionId, params object[] args)
		{
			try
			{
				var funcs = _modulesCache[targetModuleId].Functions;
				foreach (var func in funcs)
				{
					if (func.Id == functionId)
					{
						if (func == null || func.Run == null)
						{
							continue;
						}
						func.Run(args);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warn($"Ошибка при попытке вызова функции '{functionId}' в модуле '{targetModuleId}':\n{ex}");
			}
		}

		/// <summary>
		/// Попытается вызвать нестандартную функцию во всех модулях
		/// </summary>
		/// <param name="functionId"></param>
		/// <param name="args"></param>
		public void CallAll(string functionId, params object[] args)
		{
			foreach (var moduleId in GetModulesList())
			{
				CallModule(moduleId, functionId, args);
			}
		}



		// INTERNAL REGION //

		internal void PreInit()
		{
			ScanDlls();

			foreach (var path in modulesPaths)
			{
				_assemblies.Add(LoadModule(path));
			}

			ScanModules();

			foreach (var module in _modules)
			{
				_modulesCache.Add(module.Id, module);
			}

			foreach (var module in _modules)
			{
				module.PreInit(context);
			}

		}

		internal void Init()
		{
			foreach (var module in _modules)
			{
				Logger.Debug($"ModelesManager: Инициализация {module.Id}");
				module.Init(context);
			}
		}

		internal void PostInit()
		{
			foreach (var module in _modules)
			{
				module.PostInit(context);
			}
		}

		internal void SendMessage(Message message, string address)
		{
			var parts = address.Split('.');
			string targetModuleId = parts[0];
			try
			{
				var module = _modulesCache[targetModuleId];
				module.SendMessage(message, address);
			}
			catch (Exception ex)
			{
				Logger.Warn($"Ошибка при попытке отправить сообщение по адресу {address}:\n{ex}");
			}
		}

		internal void Broadcast(Message message)
		{
			foreach (var module in _modules)
			{
				module.Broadcast(message);
			}
		}

		internal void SaveAll()
		{
			foreach(var module in _modules)
			{
				module.Save();
			}
		}




		
		// PRIVATE REGION //

		private void ScanModules()
		{
			_modules.Add(Bebruhal.CoreModule);
			foreach (var assembly in _assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (typeof(IModule).IsAssignableFrom(type))
					{
						IModule? result = Activator.CreateInstance(type) as IModule;
						if (result != null)
						{
							_modules.Add(result);
							LoggerProxy.Debug($"Создан экземпляр IModule: {result.Id}");
						}
					}
				}
			}
		}

		private Assembly LoadModule(string path)
		{
			LoggerProxy.Log($"Загрузка сборки модуля {path}");
			AssemblyLoadContext loadContext = AssemblyLoadContext.Default;
			return loadContext.LoadFromAssemblyPath(Path.GetFullPath(path));
		}

		private void ScanDlls()
		{
			Filesystem.EnsureDirExists(_modulesDir);
			var list = Directory.GetFiles(_modulesDir);

			foreach (var file in list)
			{
				var ext = Path.GetExtension(file);
				if (ext == ".dll")
				{
					modulesPaths.Add(file);
				}
				else
				{
					LoggerProxy.Log($"Файл {file} пропущен, так как имеет расширение {ext}.");
				}
			}
		}
	}
}
