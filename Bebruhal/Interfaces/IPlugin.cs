using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Interfaces
{
	/// <summary>
	/// Интерфейс для реализации плагина.
	/// </summary>
	public interface IPlugin : IAssembly
	{
		/// <summary>
		/// Описание плагина, его функционала и предназначения
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Имя/псевдоним автора плагина
		/// </summary>
		public string Author { get; }

		/// <summary>
		/// Ссылка на сайт/репозиторий плагина
		/// </summary>
		public string URL { get; }

		/// <summary>
		/// Список модулей, необходимых для работы
		/// </summary>
		public string[]? RequiredModules { get; }

		/// <summary>
		/// Сразу после загрузки и регистрации всех модулей и плагинов. Эатп загрузки плагинов и разрешения зависимостей
		/// </summary>
		public void PreInit(BotContext context);

		/// <summary>
		/// Загрузка и подготовка данных. Запуск работы.
		/// </summary>
		public void Init(BotContext context);

		/// <summary>
		/// Дополнительные действия.
		/// </summary>
		public void PostInit(BotContext context);
	}
}
