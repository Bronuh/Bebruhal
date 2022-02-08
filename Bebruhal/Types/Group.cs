using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	/// <summary>
	/// Хранит в себе информацию об уровнях доступа
	/// </summary>
	public class Group
	{
		/// <summary>
		/// Уровень доступа. Больше значение - выше уровень доступа. Обычные пользователи имеют уровень 1, ограниченные - 0
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// Отображаемое имя группы
		/// </summary>
		public string Name { get; }

		private List<string> permissions { get; } = new List<string>();

		/// <summary>
		/// Проверяет наличие разрешения у данной группы.
		/// </summary>
		/// <param name="permission">Строка разрешения</param>
		/// <returns>true - если разрешение есть</returns>
		public bool HasPermission(string permission)
		{
			return false;
		}
	}
}
