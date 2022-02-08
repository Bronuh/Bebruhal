using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Systems
{
	public sealed class PluginsManager
	{
		private const string _pluginsDir = "plugins";
		private List<Assembly> _assemblies = new List<Assembly>();
		internal List<string> modulesPaths = new List<string>();
		private List<IPlugin> _plugins = new List<IPlugin>();
		private Dictionary<string, IPlugin> _pluginsCache = new Dictionary<string, IPlugin>();
		private BotContext context;

		public PluginsManager(BotContext context)
		{
			this.context = context;
		}

		internal void PreInit()
		{
			ScanDlls();

			foreach (var path in modulesPaths)
			{
				_assemblies.Add(LoadPlugin(path));
			}

			ScanModules();

			foreach (var plugin in _plugins)
			{
				_pluginsCache.Add(plugin.Id, plugin);
			}

			foreach (var plugin in _plugins)
			{
				plugin.PreInit(context);
			}
		}

		internal void Init()
		{
			foreach (var plugin in _plugins)
			{
				plugin.Init(context);
			}
		}

		internal void PostInit()
		{
			foreach (var plugin in _plugins)
			{
				plugin.PostInit(context);
			}
		}

		internal void SaveAll()
		{
			foreach (var plugin in _plugins)
			{
				plugin.Save();
			}
		}

		/// <summary>
		/// Возвращает список загруженных плагинов
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IPlugin> GetPlugins() { return _plugins; }




		// PRIVATE REGION //

		private void ScanModules()
		{
			foreach (var assembly in _assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (typeof(IPlugin).IsAssignableFrom(type))
					{
						IPlugin result = Activator.CreateInstance(type) as IPlugin;
						if (result != null)
						{
							_plugins.Add(result);
						}
					}
				}
			}
		}

		private Assembly LoadPlugin(string path)
		{
			/*LoggerProxy.Log($"Загрузка модуля {path}");
			ModuleLoadContext loadContext = new ModuleLoadContext(path);
			return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));*/
			LoggerProxy.Log($"Загрузка сборки плагина {path}");
			AssemblyLoadContext loadContext = AssemblyLoadContext.Default;//new ModuleLoadContext(path);
			return loadContext.LoadFromAssemblyPath(Path.GetFullPath(path));//.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
		}

		private void ScanDlls()
		{
			Filesystem.EnsureDirExists(_pluginsDir);
			var list = Directory.GetFiles(_pluginsDir);

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
