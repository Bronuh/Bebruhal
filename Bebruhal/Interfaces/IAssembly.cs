using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Interfaces
{
	/// <summary>
	/// Общий интерфейс для IModule и IPlugin с минимумом полей
	/// </summary>
	public interface IAssembly
	{
		/// <summary>
		/// Полное название плагина
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Идентификатор плагина. Рекомендуется использовать название плагина в kebab-case
		/// </summary>
		public string Id { get; }
	}
}
