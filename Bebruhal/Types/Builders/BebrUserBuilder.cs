

namespace Bebruhal.Types.Builders
{
	internal class BebrUserBuilder
	{
		private IModule source = null;
		private int groupId = 1;

		public BebrUserBuilder()
		{

		}

		public BebrUserBuilder Source(IModule source)
		{
			this.source = source;
			return this;
		}

		public BebrUserBuilder GroupId(int groupId)
		{
			this.groupId = groupId;
			return this;
		}

		public BebrUser Build()
		{
			if (source == null) 
			{
				throw new InvalidOperationException("Попытка создать учетную запись пользователя без указания её источника (souce == null).");
			}
			return new BebrUser(source,	groupId);
		}
	}
}
