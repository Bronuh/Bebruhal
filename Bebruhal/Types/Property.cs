namespace Bebruhal.Types
{
	/// <summary>
	/// Хранилище свойства
	/// </summary>
	/// <typeparam name="TValueType">Тип хранимого значения</typeparam>
	public class Property <TValueType>
	{
		/// <summary>
		/// Ключ по которому происходит обращение к свойству. Не чувствителен к регистру.
		/// </summary>
		public string Key { get; internal set; }

		/// <summary>
		/// Хранимое значение
		/// </summary>
		public TValueType Value { get; set; }


		public Property() { }

		/// <summary>
		/// Инициализирует новое свойство
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <param name="value">Значение</param>
		public Property(string key, TValueType value)
		{
			Key = key;
			Value = value;
		}
	}
}
