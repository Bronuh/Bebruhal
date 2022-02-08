using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Utils
{
	/// <summary>
	/// Класс-прокси, предоставляющий статичные методы для записи в лог. Не рекомендуется для использования.
	/// </summary>
	public static class LoggerProxy
	{
		internal static bool IsDebugEnabled { get; set; } = false;
		private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Возвращает ссылку на объект логгера.
		/// </summary>
		/// <returns>Используемый логгер</returns>
		public static Logger Instance()
		{
			return Logger;
		}

		/// <summary>
		/// Пишет сообщение в trace
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Trace(string message)
		{
			//if (IsDebugEnabled)
			Logger.Trace(message);
		}

		/// <summary>
		/// Пишет сообщение в info
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Log(string message)
		{
			Logger.Info(message);
		}

		/// <summary>
		/// Пишет сообщение в debug
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Debug(string message)
		{
			if (IsDebugEnabled)
			Logger.Debug(message);
		}

		/// <summary>
		/// Пишет сообщение в warn
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Warn(string message)
		{
			Logger.Warn(message);
		}

		/// <summary>
		/// Пишет сообщение в error
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Error(string message)
		{
			Logger.Error(message);
		}

		/// <summary>
		/// Пишет сообщение в fatal
		/// </summary>
		/// <param name="message">Текст сообщения</param>
		public static void Fatal(string message)
		{
			Logger.Fatal(message);
		}

	}
}
