using SixLabors.ImageSharp.Formats;

namespace Bebruhal.Utils
{
	/// <summary>
	/// Класс, содержащий в себе служебные методы расширения
	/// </summary>
    public static class Extensions
    {
		/// <summary>
		/// Выводит все строковые значения в одну строку, с разделением запятыми
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string ToLine(this List<string> list)
		{
			string respond = "";
			foreach (string word in list)
				respond += word + (word == list[list.Count - 1] ? "" : ", ");

			return respond;
		}

		/// <summary>
		/// Выводит все строковые значения в одну строку, с разделением запятыми
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string ToLine(this IEnumerable<string> list)
        {
			return new List<string>(list).ToLine();
        }

		/// <summary>
		/// Возвращает случайный элемент списка
		/// </summary>
		/// <typeparam name="T">Тип хранимых значений в списке</typeparam>
		/// <param name="list">Список</param>
		/// <returns>Случайный элемент списка <paramref name="list"/></returns>
		public static T GetRandom<T>(this List<T> list)
		{
			return list[new Random().Next(0, list.Count)];
		}

		/// <summary>
		/// Возвращает случайный элемент списка
		/// </summary>
		/// <typeparam name="T">Тип хранимых значений в списке</typeparam>
		/// <param name="list">Список</param>
		/// <returns>Случайный элемент списка <paramref name="list"/></returns>
		public static T GetRandom<T>(this IEnumerable<T> list)
		{
			return list.ElementAt(new Random().Next(0, list.Count()));
		}

		/// <summary>
		/// Разбивает перечисление на куски размером chunkSize
		/// </summary>
		/// <typeparam name="T">Тип хранимых значений в списке</typeparam>
		/// <param name="items">Список объектов</param>
		/// <param name="chunkSize">Максимальный размер выходного списка</param>
		/// <returns>Список, содержащий списки размером <paramref name="chunkSize"/></returns>
		public static IEnumerable<List<T>> ToChunks<T>(this IEnumerable<T> items, int chunkSize)
		{
			List<T> chunk = new List<T>(chunkSize);
			foreach (var item in items)
			{
				chunk.Add(item);
				if (chunk.Count == chunkSize)
				{
					yield return chunk;
					chunk = new List<T>(chunkSize);
				}
			}
			if (chunk.Any())
				yield return chunk;
		}

		/// <summary>
		/// Асинхронно выполняет действие над каждым элементом списка
		/// </summary>
		/// <typeparam name="T">Тип хранимых значений в списке</typeparam>
		/// <param name="list">Список объектов</param>
		/// <param name="action">Требуемое действие</param>
		[Obsolete("Используйте Parallel.ForEach вмместо этого")]
		public static void EachAsync<T>(this List<T> list, Action<T> action)
		{
			int threads = Environment.ProcessorCount;
			int listCount = list.Count;
			int perThread = (int)Math.Ceiling((double)listCount / threads);
			
			List<Task> tasks = new List<Task>();
			var parts = list.ToChunks(perThread);

			foreach (var part in parts)
			{
				tasks.Add(Task.Factory.StartNew(() => {
					foreach (var item in part)
					{
						action(item);
					}
				}));
			}
			Task.WaitAll(tasks.ToArray());
		}

		/// <summary>
		/// Преобразует строку с поток
		/// </summary>
		/// <param name="s">Входная строка</param>
		/// <returns>Поток байт, соответствующий строке</returns>
		public static Stream ToStream(this string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}


		public static Stream ToStream(this byte[] s)
		{
			var stream = new MemoryStream(s);
			stream.Position = 0;
			return stream;
		}




		public static Stream ToStream(this Image bitmap)
		{
			MemoryStream memoryStream = new MemoryStream();
			//bitmap.Save(memoryStream, SixLabors.ImageSharp.Formats.Png.PngEncoder);
			bitmap.SaveAsPng(memoryStream);
			memoryStream.Position = 0;
			return memoryStream;
		}
	}
}

