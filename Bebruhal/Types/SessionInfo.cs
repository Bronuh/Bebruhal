using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	/// <summary>
	/// Хранилище информации о текущей сессии
	/// </summary>
	public class SessionInfo
	{
		/// <summary>
		/// Название сессии. Совпадает с названием директории, в которой хранится сессия
		/// </summary>
		public string SessionName { get; internal set; } = "session";

		/// <summary>
		/// Дата создания сессии
		/// </summary>
		public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

		/// <summary>
		/// Версия представления сессии
		/// </summary>
		public Version Version { get; private set; } = new Version(0,1);

		/// <summary>
		/// Список плагинов, использовавшихся при работе с сессией
		/// </summary>
		public List<string> UsedPlugins { get; set; } = new List<string>();

		/// <summary>
		/// Список модулей, использовавшихся при работе с сессией
		/// </summary>
		public List<string> UsedModules { get; set; } = new List<string>();
	}
}
