namespace Bebruhal.Types
{
	/// <summary>
	/// Интерфейс, позволяющий реализовывать нестандартные функции API
	/// </summary>
	public class Function
	{
		/// <summary>
		/// Id функции, по которому происходит поиск
		/// </summary>
		public string? Id { get; }

		/// <summary>
		/// Описание функции
		/// </summary>
		public string? Description { get; }

		/// <summary>
		/// Метод, реализующий требуемый функционал
		/// </summary>
		public Action<object[]>? Run;
	}
}