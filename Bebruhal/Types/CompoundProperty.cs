using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	/// <summary>
	/// Составное свойство, предназначенное для хранения информации
	/// </summary>
	public class CompoundProperty
	{
		private static IReadOnlyList<string> _addProperties = new List<string> { "messagesCount", "experience" };
		private static IReadOnlyList<string> _multiplyProperties = new List<string> {  };
		private static IReadOnlyList<string> _maxProperties = new List<string> { "rank" };
		private static IReadOnlyList<string> _minProperties = new List<string> {  };

		/// <summary>
		/// Имя свойства, по которому производится его поиск
		/// </summary>
		[JsonProperty]
		public string Key { get; internal set; }
		[JsonProperty]
		private List<Property<string>> stringProperties = new List<Property<string>>();
		[JsonProperty]
		private List<Property<decimal>> decimalProperties = new List<Property<decimal>>();
		[JsonProperty]
		private List<Property<CompoundProperty>> compoundProperties = new List<Property<CompoundProperty>>();

		public CompoundProperty() { }
		public CompoundProperty(string key)
		{
			Key = key;
		}

		/// <summary>
		/// TODO: реализоать слияние свойств
		/// Производит перенос свойств из <paramref name="other"/> в текущее свойство
		/// </summary>
		/// <param name="other">Переносимое второстепенное свойство.</param>
		public void MergeCompoundProperty(CompoundProperty other)
		{
			// TODO: Вызвать TryMergeEach для каждого списка свойств
			TryMergeEach(decimalProperties, other.decimalProperties);
			TryMergeEach(stringProperties, other.stringProperties);

			// TODO: Просканировать составные свойства на наличие совпадений и провести попытку слияния

		}

		private static void TryMergeEach<TValue>(List<Property<TValue>> main, List<Property<TValue>> other)
		{
			Type valueType = typeof(TValue);
			if (!(valueType == typeof(string) || valueType == typeof(decimal)))
			{
				LoggerProxy.Warn($"Попытка слияния свойств, для типа значения которых ({valueType.FullName}) не описаны правила слияния");
				return;
			}


			foreach (Property<TValue> property in main)
			{
				// TODO: попытаться найти свойтсво с таким именем в other и провести слияние
				//Property<TValue> 
			}
		}

		/// <summary>
		/// Возвращает действие при слиянии по умолчанию для объекта типа decimal
		/// </summary>
		/// <param name="key">ключ для поиска действия</param>
		/// <returns></returns>
		private static Action<Property<decimal>, Property<decimal>> GetAction(string key)
		{
			if (_addProperties.Contains(key))
			{
				return (main, other) => { main.Value += other.Value; };
			}

			if (_multiplyProperties.Contains(key))
			{
				return (main,other) => { main.Value *= other.Value; };
			}

			if (_maxProperties.Contains(key))
			{
				return (main, other) => Math.Max(main.Value, other.Value);
			}

			if (_minProperties.Contains(key))
			{
				return (main, other) => Math.Min(main.Value, other.Value);
			}

			return (main, other) => main.Value = main.Value;
		}

		private static Property<TValue>? GetPropertyByKey<TValue>(string key, List<Property<TValue>> list)
		{
			if(list.Count == 0) { return null; }
			Property<TValue> property = list.Find(x => x.Key == key);
			return property;
		}

		

		/// <summary>
		/// Возвращает значение, хранимое в свойстве
		/// </summary>
		/// <typeparam name="TValue">Тип хранимого значения</typeparam>
		/// <param name="key">Ключ</param>
		/// <param name="list">Список значений, в котором происходит поиск</param>
		/// <returns></returns>
		private TValue GetValue<TValue>(string key, List<Property<TValue>> list)
		{
			TValue returnValue = default;
			if (string.IsNullOrEmpty(key))
			{
				return default;
			}

			var found = list.FindAll(p => p.Key.ToLower().Equals(key.ToLower()));

			if (found.Count == 0)
			{
				return default;
			}
			if (found.Count >= 1)
			{
				if (found.Count > 1)
					LoggerProxy.Warn($"Поиск по ключу '{key}' почему-то вернул список из {found.Count} элементов. Будет возвращен первый из них");

				returnValue = found.First().Value;
			}

			return returnValue;
		}

		/// <summary>
		/// Устанавливает значение свойства для ключа в указанном списке
		/// </summary>
		/// <typeparam name="TValue">Тип хранимого свойством значения</typeparam>
		/// <param name="key">Ключ</param>
		/// <param name="value">Значение свойства</param>
		/// <param name="list">Список свойств типа TValue</param>
		private void SetValue<TValue>(string key, TValue value, List<Property<TValue>> list)
		{
			var found = list.FindAll(p => p.Key.ToLower().Equals(key.ToLower()));
			Property<TValue> firstFound;
			if(found.Count == 0) 
			{
				firstFound = new Property<TValue>(key, value);
				list.Add(firstFound);
			}
			else
			{
				found.First().Value = value;
			}
		}

		/// <summary>
		/// Возвращает составное свойство по указанному ключу. Если значение в списке отсутствует - вернёт пустой default.
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns></returns>
		public string GetString(string key)
		{
			return GetValue(key, stringProperties);
		}

		/// <summary>
		/// Возвращает составное свойство по указанному ключу. Если значение в списке отсутствует - вернёт пустой default.
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns></returns>
		public decimal GetDecimal(string key)
		{
			return GetValue(key, decimalProperties);
		}

		/// <summary>
		/// Возвращает составное свойство по указанному ключу. Если значение в списке отсутствует - вернёт пустой default.
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns></returns>
		public CompoundProperty GetCompound(string key)
		{
			return GetValue(key, compoundProperties);
		}

		/// <summary>
		/// Устанавливает строковое значение указанному свойству
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <param name="value">Строковое значение свойства</param>
		public void SetString(string key, string value)
		{
			SetValue(key, value, stringProperties);
		}

		/// <summary>
		/// Устанавливает численное значение для указанного ключа
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <param name="value">Численное значение свойства</param>
		public void SetDecimal(string key, decimal value)
		{
			SetValue(key, value, decimalProperties);
		}


		/// <summary>
		/// Назначает составное свойство для указанного ключа
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <param name="value">Составное свойство</param>
		public void SetCompound(string key, CompoundProperty value)
		{
			SetValue(key, value, compoundProperties);
		}
	}
}
