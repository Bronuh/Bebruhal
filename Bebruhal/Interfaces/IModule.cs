using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Interfaces
{
	/// <summary>
	/// Интерфейс для реализации модуля.
	/// </summary>
	public interface IModule : IAssembly
	{
		/// <summary>
		/// Идентификатор модуля. Рекомендуется использовать имя модуля в kebab-case
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Имя модуля.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// имя/псевдоним автора
		/// </summary>
		public string Author { get; }

		/// <summary>
		/// Описание модуля, его предназначение
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Ссылка на сайт или репозиторий модуля
		/// </summary>
		public string URL { get; }

		/// <summary>
		/// Возвращает true, если асинхронная инициализация завершена
		/// </summary>
		public bool IsReady { get; }

		/// <summary>
		/// Список нестандартных функций
		/// </summary>
		public List<Function> Functions { get; }









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

		/// <summary>
		/// Производит сохранение данных
		/// </summary>
		public void Save();

		/// <summary>
		/// Отпрявляет сообщение в реализуемый сервис
		/// </summary>
		/// <param name="msg">Отправляемое сообщение</param>
		/// <param name="address">Адрес назначения</param>
		public void SendMessage(Message msg, string address);

		/// <summary>
		/// Отправляет широковещательное сообщение по всем допустимым адресам
		/// </summary>
		/// <param name="message">Отправляемое сообщение</param>
		public void Broadcast(Message message);
	}
}
