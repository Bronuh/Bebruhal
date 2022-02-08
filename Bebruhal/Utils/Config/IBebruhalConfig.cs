using Config.Net;

namespace Bebruhal.Utils.Config
{
	/// <summary>
	/// Интерфейс, хранящий модель конфигурации с значениямми по умолчанию
	/// </summary>
	public interface IBebruhalConfig : IConfigSaver
	{
		/// <summary>
		/// Название сессии
		/// </summary>
		[Option(DefaultValue = "session")]
		String Session { get; set; }

		/// <summary>
		/// Префикс команды
		/// </summary>
		[Option(DefaultValue = "!")]
		String CommandPrefix { get; set; }

		/// <summary>
		/// Не трош, убьет. Я сам не знаю что оно делает
		/// </summary>
		[Option(DefaultValue = true)]
		Boolean Debug { get; set; }

	}
}
