using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	/// <summary>
	/// Универсальный асинхронный обработчик событий
	/// </summary>
	/// <typeparam name="TSender">Тип отправителя события</typeparam>
	/// <typeparam name="TArgs">Тип аргументов события, наследуемый от AsyncEventArgs</typeparam>
	/// <param name="sender">Объект, вызвавший событие</param>
	/// <param name="e">аргументы события</param>
	/// <returns></returns>
	public delegate Task AsyncEventHandler<in TSender, in TArgs>(TSender sender, TArgs e) where TArgs : AsyncEventArgs;


	public class AsyncEventArgs : EventArgs
	{
		public AsyncEventArgs() { }

		/// <summary>
		/// Выставляется true, если обработка события завершена
		/// </summary>
		public bool Handled { get; set; }
	}

	/// <summary>
	/// Содержит данные события, вызываемого при передаче сообщения из модуля в ядро
	/// </summary>
	public class RecievedMessageEventArgs : AsyncEventArgs
	{
		/// <summary>
		/// Полученное сообщение
		/// </summary>
		public Message Message{ get; private set; }

		/// <summary>
		/// Автор сообщения
		/// </summary>
		public BebrUser Author { get; private set; }

		//public RecievedMessageEventArgs() { }
		/// <summary>
		/// Конструктор данных события
		/// </summary>
		/// <param name="msg">Принятое сообщение</param>
		/// <param name="author">Автор сообщения</param>
		public RecievedMessageEventArgs(Message msg, BebrUser author)
		{
			Message = msg;
			this.Author = author;
		}
	}

	/// <summary>
	/// Содержит данные события, вызываемого при регистрации пользователя
	/// </summary>
	public class RegisteredUserEventArgs : AsyncEventArgs
	{

		/// <summary>
		/// Зарегистрированный пользователь
		/// </summary>
		public BebrUser User { get; private set; }

		/// <summary>
		/// Конструктор данных события
		/// </summary>
		/// <param name="user">Зарегистрированный пользователь</param>
		public RegisteredUserEventArgs(BebrUser user)
		{
			User = user;
		}
	}

	/// <summary>
	/// Хранит данные события, вызываемого в момент слияния двух пользователей
	/// </summary>
	public class MergedUsersEventArgs : AsyncEventArgs
	{
		/// <summary>
		/// Основной пользователь, указанный при слиянии
		/// </summary>
		public BebrUser MainUser { get; private set; }

		/// <summary>
		/// Пользователь, чьи данные были перенесены в основного пользователя
		/// </summary>
		public BebrUser MergedUser { get; private set; }

		/// <summary>
		/// Конструктор данных события
		/// </summary>
		/// <param name="mainUser">Основной пользователь, указанный при слиянии</param>
		/// <param name="mergedUser">Пользователь, чьи данные были перенесены в основного пользователя</param>
		public MergedUsersEventArgs(BebrUser mainUser, BebrUser mergedUser)
		{
			MainUser = mainUser;
			MergedUser = mergedUser;
		}
	}


	public class MessageCreatedEventArgs : AsyncEventArgs
	{
		public Message Message;
		public MessageCreatedEventArgs() { }
		public MessageCreatedEventArgs(Message msg) { Message = msg; }
	}
	public class CommandCalledEventArgs : AsyncEventArgs
	{
		public Message Message;
		public CommandCalledEventArgs() { }
		public CommandCalledEventArgs(Message msg) { Message = msg;}
	}
	public class CommandExecutedEventArgs : AsyncEventArgs
	{
		public Message Message;
		public CommandExecutedEventArgs() { }
		public CommandExecutedEventArgs(Message message) { Message = message; }

	}
	public class CommandCancelledEventArgs : AsyncEventArgs { }
}
