namespace Bebruhal.Types
{
	public class Message
	{
		/// <summary>
		/// Адрес источника сообщения
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Автор сообщения. Определение отправителя возлагается на модуль из которого было отправлено сообщение.
		/// </summary>
		public BebrUser Author { get; set; }

		/// <summary>
		/// Сырые идентификационные данные об авторе сообщения. Может быть Id или другой краткой информацией, позволяющей однозначно определить атвора.
		/// </summary>
		public string RawAuthor { get; set; }

		/// <summary>
		/// Текст сообщения
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Изображения, прикреплённые к сообщению
		/// </summary>
		public List<Image> Images { get; set; } = new();

		/// <summary>
		/// Файлы, прикреплённые к сообщению
		/// </summary>
		public List<Base64File> Files { get; set; } = new();

		/// <summary>
		/// Модуль, в котором было сформировано сообщение
		/// </summary>
		public IModule Module { get; set; }

		/// <summary>
		/// Для сгенерированных ядром служебных сообщений.
		/// </summary>
		internal bool IsResponse { get; private set; } = false;

		/// <summary>
		/// Дополнительная краткая информация о сообщении. Может включать источник и внутреннее имя пользователя.
		/// </summary>
		public string Info { get; set; }


		/// <summary>
		/// Отправляет сообщение в ответ
		/// </summary>
		/// <param name="message">Ответное сообщение</param>
		public void Respond(Message message)
		{
			message.IsResponse = true;
			if (IsRespondable())
			{
				message.IsResponse = true;
				BebruhalCore.context.ModulesManager.SendMessage(message, Source);
			}
			else
			{
				LoggerProxy.Warn($"Попытка ответить на сообщение с неверно указанным источником: {Source}");
			}
		}


		/// <summary>
		/// Отвечает на сообщение
		/// </summary>
		/// <param name="message"></param>
		public void Respond(string message)
		{
			var msg = new Message();
			msg.Text = message;
			Respond(msg);
		}


		/// <summary>
		/// Отправляет изображения в ответ на сообщение
		/// </summary>
		/// <param name="images"></param>
		public void Respond(params Image[] images)
		{
			var msg = new Message();
			msg.Images = new List<Image>(images);
			Respond(msg);
		}


		/// <summary>
		/// Отправляет файлы в ответ на сообщение
		/// </summary>
		/// <param name="files"></param>
		public void Respond(params Base64File[] files)
		{
			var msg = new Message();
			msg.Files = new List<Base64File>(files);
			Respond(msg);
		}

		public string GetTextWithoutPrefix()
		{
			var text = Text;
			text = text.Substring(Bebruhal.GetCore().GetPrefix().Length, text.Length - (Bebruhal.GetCore().GetPrefix().Length));
			return text;
		}

		public string GetTextWithoutCommand()
		{
			var text = Text;
			var len = text.Split(' ').First().Length;
			return text.Substring(len, text.Length - len).Trim();
		}

		/// <summary>
		/// Отвечает на сообщение с возможностью прикрепления вложений
		/// </summary>
		/// <param name="text">Текст сообщения</param>
		/// <param name="images">Прикрепляемые изображения</param>
		/// <param name="files">Прикрепляемые файлы</param>
		public void Respond(string? text, List<Image>? images, List<Base64File>? files)
		{
			var msg = new Message { Text = text, Images = images, Files = files };
			Respond(msg);
		}


		private bool IsRespondable()
		{
			return Source != null && Source != String.Empty;
		}
	}
}
