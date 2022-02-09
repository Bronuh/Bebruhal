using System.Text.RegularExpressions;

namespace Bebruhal
{
	/// <summary>
	/// Содержит в себе публичные константы
	/// </summary>
	public static class Constants
	{
		public static readonly Regex mentionRegex = new Regex(@"(?<=[^\\]|^)<(@)(.*?)>");
	}
}
