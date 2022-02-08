using Bebruhal.Utils.Config;
using Config.Net;
using NLog;

namespace Bebruhal;

/// <summary>
/// Точка входа в программу
/// </summary>
public class Bebruhal
{
	static IBebruhalConfig config =  new ConfigurationBuilder<IBebruhalConfig>()
		.UseIniFile("bebruhal.ini")
		.Build();
	private static bool running = true;
	private static BebruhalCore core = new BebruhalCore(config);
	static Logger logger = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Точка входа в программу
	/// </summary>
	/// <param name="args"></param>
	public static void Main(String[] args)
	{
		СrutchSaveConfig(config);
		var dummy = BebrUser.Dummy;
		LoggerProxy.Trace("Pre-core-start");
		Task.Factory.StartNew(() => core.Start());
		logger.Debug("Started new core task");

		StartConsole();
	}

	/// <summary>
	/// Запускает обработчик консоли в главном потоке
	/// </summary>
	private static void StartConsole()
	{
		while (running)
		{
			var str = Console.ReadLine();
			core.CommandsManager.TryExecuteConsoleCommand(str).GetAwaiter().GetResult();
		}
	}

	/// <summary>
	/// Возвращает ссылку на объект ядра бота
	/// </summary>
	/// <returns></returns>
	public static BebruhalCore GetCore()
	{
		return core;
	}

	public static void ShutDown()
	{
		running = false;
		core.Session.Save();
	}

	/// <summary>
	/// Метод нужен для создания файла конфигурации в директории с программой. 
	/// Прокси класса конфигурации пытается вносить изменения в файл каждый раз, когда в объекте 
	/// конфига меняется какое-то значение, при этом в файле изменяется именно это значение.
	/// </summary>
	/// <param name="config"></param>
	//private static void СrutchSaveConfig(IBebruhalConfig config)
	public static void СrutchSaveConfig(IConfigSaver config)
	{
		logger.Info("Пересохранение файла конфигурации");
		Type type = config.GetType();

		/// Попытка присвоить всем свойствам конфига их же значения, чтобы все они отразились в создаваемом файле конфигурации.
		foreach (var property in type.GetProperties())
		{
			try
			{
				logger.Debug($"Переназначение свойства {type.FullName}.{property.Name}");
				property.SetValue(config, property.GetValue(config));
			}
			catch (Exception ex)
			{
				logger.Warn(ex,$"Не удалось переназначить значение для свойства {type.FullName}.{property.Name}:\n{ex}");
			}
		}
	}
}
