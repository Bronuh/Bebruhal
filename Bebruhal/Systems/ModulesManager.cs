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

		public ModulesManager(BotContext context)
		{
			this.context = context;
		}

		public IEnumerable<string> GetModulesList()
		{
			foreach (var module in _modules)
			{
				yield return module.Name;
			}
		}

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







		
		// PRIVATE REGION //

		private void ScanModules()
		{
			foreach (var assembly in _assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (typeof(IModule).IsAssignableFrom(type))
					{
						IModule result = Activator.CreateInstance(type) as IModule;
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
			AssemblyLoadContext loadContext = AssemblyLoadContext.Default;//new ModuleLoadContext(path);
			return loadContext.LoadFromAssemblyPath(Path.GetFullPath(path));//.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
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
