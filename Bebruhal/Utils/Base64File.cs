namespace Bebruhal.Utils
{
	/// <summary>
	/// Класс для конвертации файлов в Base64 и обратно
	/// </summary>
	public class Base64File
	{
		/// <summary>
		/// Имя файла
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Тело файла
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Пустой конструктор
		/// </summary>
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
		public Base64File() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

		/// <summary>
		/// Загружает файл по указанному пути, преобразуя его в Base64
		/// </summary>
		/// <param name="path">Путь до файла</param>
		private Base64File(string path)
		{
			try
			{
				Byte[] bytes = File.ReadAllBytes(path);
				Body = Convert.ToBase64String(bytes);
				Name = Path.GetFileName(path);
			}
			catch (Exception e)
			{
				LoggerProxy.Error(e.Message);
				throw;
			}
		}



		/// <summary>
		/// Сохраняет данные в указанный файл
		/// </summary>
		/// <param name="path">Путь до файла</param>
		public void SaveToFile(string path)
		{
			try
			{
				Byte[] bytes = Convert.FromBase64String(Body);
				File.WriteAllBytes(path, bytes);
			}
			catch(Exception e)
			{
				LoggerProxy.Error(e.Message);
			}
		}



		/// <summary>
		/// Сохраняет файл в указанную папку, используя хранимое имя файла
		/// </summary>
		/// <param name="path">Путь до папки</param>
		public void SaveToDir(string path)
		{
			try
			{
				Byte[] bytes = Convert.FromBase64String(Body);
				Directory.CreateDirectory(path);
				File.WriteAllBytes(Path.Combine(path,Name), bytes);
			}
			catch (Exception e)
			{
				LoggerProxy.Error(e.Message);
			}
		}


		/// <summary>
		/// Возвращает содержимое файла в виде массива байтов
		/// </summary>
		/// <returns></returns>
		public byte[] ToByteArray()
		{
			return Convert.FromBase64String(Body);
		}


		/// <summary>
		/// Возвращает поток байтов содержимого файла
		/// </summary>
		/// <returns></returns>
		public Stream ToStream()
		{
			return ToByteArray().ToStream();
		}


		/// <summary>
		/// Переводит указанный файл в Base64 код
		/// </summary>
		/// <param name="path">Путь к файлу</param>
		/// <returns></returns>
		public static Base64File FromFile(string path)
		{
			return new Base64File(path);
		}



		/// <summary>
		/// Создаёт объект, используя указанные имя файла и данные в Base64
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		/// <param name="data">Содержимое файла в Base64</param>
		/// <returns></returns>
		public static Base64File FromData(string fileName, string data)
		{
			var file = new Base64File
			{
				Name = fileName,
				Body = data
			};
			return file;
		}
	}
}
