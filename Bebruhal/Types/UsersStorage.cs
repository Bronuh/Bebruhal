using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebruhal.Types
{
	internal class UsersStorage
	{

		private UsersStorage(IModule sourceModule)
		{
			ModuleId = sourceModule.Id;
		}

		public string ModuleId { get; private set; }
		private static Logger logger { get; set; } = LogManager.GetCurrentClassLogger();


		internal List<BebrUser> Users { get; private set; } = new List<BebrUser>();

		private Dictionary<string, BebrUser> _guidUsers = new();
		private Dictionary<string, BebrUser> _nameUsers = new();
		private Dictionary<string, BebrUser> _idUsers = new();
		
		


		private void CacheUser(BebrUser user)
		{
			try
			{
				try { _guidUsers.Add(user.GUID, user); } catch (Exception ex) { throw; }
				try { _nameUsers.Add(user.Name.ToLower(), user); } catch (Exception ex) { throw; }
				try { _idUsers.Add(user.ExternalId.ToLower(), user); } catch (Exception ex) { throw; }

				foreach (var alias in user.GetAliases())
				{
					try
					{
						_nameUsers.Add(alias.ToLower(), user);
					}
					catch (Exception ex)
					{
						throw;
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message);
			}
		}



		public BebrUser GetUser(string identifier)
		{
			var user = BebrUser.Empty;
			var key = identifier.ToLower();
			try
			{
				user = _guidUsers[key];
				return user;
			}
			catch
			{
				logger.Debug($"Не удалось найти в словаре пользователя по GUID '{identifier}'.");
			}
			try
			{
				user = _nameUsers[key];
				return user;
			}
			catch
			{
				logger.Debug($"Не удалось найти в словаре пользователя по ключу '{identifier}'.");
			}
			try
			{
				user = _idUsers[key];
				return user;
			}
			catch
			{
				logger.Debug($"Не удалось найти в словаре пользователя по Id '{identifier}'. Поиск будет продолжен перебором.");
			}
			if (user.IsEmpty())
				foreach (var _user in Users)
				{
					if (_user.CheckUser(key))
					{
						user = _user;
					}
				}

			return user;
		}
	}
}
