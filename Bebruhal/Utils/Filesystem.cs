namespace Bebruhal.Utils
{
	/// <summary>
	/// Предоставляет бесполезные методы для работы с файловой системой
	/// </summary>
	public static class Filesystem
	{
		/// <summary>
		/// Создаёт указанную директорию, елсли таковая не существует
		/// </summary>
		/// <param name="path">Путь до интересующей директории</param>
		public static void EnsureDirExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Создаёт новый файл, ясли таковой не существует
		/// </summary>
		/// <param name="path">Путь до файла</param>
		public static void EnsureFileExists(string path)
		{
			if (!File.Exists(path))
			{
				File.Create(path);
			}
		}
	}
}
