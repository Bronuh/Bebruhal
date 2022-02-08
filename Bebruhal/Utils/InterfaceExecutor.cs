using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bebruhal.Utils
{
	static class InterfaceExecutor
	{
		/// <summary>
		/// Создает экземпляр класса, реализующего интерфейс, и выполнеят у него указанный метод
		/// </summary>
		/// <param name="interfaceName">Искомый интерфейс</param>
		/// <param name="methodName">Выполняемый метод</param>
		public static void Execute(string interfaceName, string methodName)
		{
			try
			{
				List<Type> types = new List<Type>(Assembly.GetExecutingAssembly().GetTypes());
				foreach (Type type in types)
				{
					List<Type> interfaces = new List<Type>(type.GetInterfaces());
					if (interfaces.Find(i => i.Name == interfaceName) != null)
					{
						type.GetMethod(methodName).Invoke(Activator.CreateInstance(type), new object[] { });
					}
				}
			}catch(Exception e)
			{
				LoggerProxy.Warn("Исключение вызвано в InterfaceExecutor.Execute()");
				LoggerProxy.Warn(e.Message);
			}
		}

		/// <summary>
		/// Создает экземпляр класса, реализующего интерфейс, и выполнеят у него указанный метод
		/// </summary>
		/// <param name="interfaceName">Искомый интерфейс</param>
		/// <param name="methodName">Выполняемый метод</param>
		public static void Execute(Type interfaceType, string methodName)
		{
			try
			{
				List<Type> types = new List<Type>(Assembly.GetExecutingAssembly().GetTypes());
				foreach (Type type in types)
				{
					List<Type> interfaces = new List<Type>(type.GetInterfaces());
					if (interfaces.Find(i => i == interfaceType) != null)
					{
						type.GetMethod(methodName).Invoke(Activator.CreateInstance(type), new object[] { });
					}
				}
			}
			catch (Exception e)
			{
				LoggerProxy.Warn("Исключение вызвано в InterfaceExecutor.Execute()");
				LoggerProxy.Warn(e.Message);
			}
		}

		/// <summary>
		/// Вызывает статический метод у всех классов, отмеченных указанным атрибутом
		/// </summary>
		/// <param name="attributeType">Искомый атрибут</param>
		/// <param name="methodName">Выполняемый метод</param>
		public static void ExecuteStatic(Type attributeType, string methodName)
		{
			try
			{
				List<Type> types = new List<Type>(Assembly.GetExecutingAssembly().GetTypes());
				foreach (Type type in types)
				{
					var attributes = type.GetCustomAttributes();
					foreach (var attribute in attributes)
					{
						if (attribute.GetType() == attributeType)
						{
							try
							{
								type.GetMethod(methodName).Invoke(null, null);
							}
							catch (Exception e)
							{
								LoggerProxy.Warn("Исключение вызвано в InterfaceExecutor.ExecuteStatic()");
								LoggerProxy.Warn(e.Message);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerProxy.Warn("Исключение вызвано в InterfaceExecutor.ExecuteStatic() (before)");
				LoggerProxy.Warn(e.Message);
			}
		}
	}
}
