namespace Bebruhal.Utils
{
    /// <summary>
    /// Предоставляет методы для работы с изображениями в сети.
    /// </summary>
	public static class Network
	{
        /// <summary>
        /// Скачивает изображение по ссылке и сохраняет его по указанному пути в формате png
        /// </summary>
        /// <param name="uri">Ссылка на изображение</param>
        /// <param name="path">Путь до изображения</param>
		public static void DownloadImageToPng(string uri, string path)
        {
            string extension = ".png";
            string fileName = path.EndsWith(extension) ? path : Path.Combine(path, extension);
            Image bitmap = LoadImage(uri);
            if (bitmap != null)
                bitmap.Save(fileName);
        }

        /// <summary>
        /// Скачивает изображение по ссылке и сохраняет его по указанному пути в формате jpg
        /// </summary>
        /// <param name="uri">Ссылка на изображение</param>
        /// <param name="path">Путь до изображения</param>
		public static void DownloadImageToJpg(string uri, string path)
        {
            string extension = ".jpg";
            string fileName = path.EndsWith(extension)|| path.EndsWith(".jpeg") ? path : Path.Combine(path, extension);
            Image bitmap = LoadImage(uri);
            if (bitmap != null)
                bitmap.Save(fileName);
        }

        /// <summary>
        /// Скачивает изображение по ссылке и сохраняет его по указанному пути в формате gif
        /// </summary>
        /// <param name="uri">Ссылка на изображение</param>
        /// <param name="path">Путь до изображения</param>
		public static void DownloadImageToGif(string uri, string path)
        {
            string extension = ".gif";
            string fileName = path.EndsWith(extension) ? path : Path.Combine(path, extension);
            Image bitmap = LoadImage(uri);
            if (bitmap != null)
                bitmap.Save(fileName);
        }

        /// <summary>
        /// Загружает изображение по ссыылке
        /// </summary>
        /// <param name="uri">Ссылка на изображение</param>
        /// <returns></returns>
        public static Image LoadImage(string uri)
		{
            var client = new HttpClient();
            Stream stream = client.GetStreamAsync(uri).GetAwaiter().GetResult();
            Image bitmap = Image.Load(stream);

            stream.Flush();
            stream.Close();
            client.Dispose();

            return bitmap;
        }
	}
}
