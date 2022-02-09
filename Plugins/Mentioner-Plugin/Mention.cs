using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentioner_Plugin
{
	public delegate void MentionAction(BebrUser sender, BebrUser target);
	internal class Mention
	{
		public String Prefix, Suffix, Message;
		public int Rank;
		public MentionAction CustomAction = (s, t)=>{};

		static Logger logger = LogManager.GetCurrentClassLogger();

		internal bool Match(string parse)
		{
			logger.Debug(parse + " startsWith " + Prefix + " == " + parse.StartsWith(Prefix));
			logger.Debug(parse + " endsWith " + Suffix + " == " + parse.EndsWith(Suffix));
			if (parse.StartsWith(Prefix) && parse.EndsWith(Suffix))
			{
				return true;
			}

			return false;
		}

		internal string GetClearText(string parse)
		{
			string _return = parse;
			if (Prefix != "")
			{
				_return = _return.Replace(Prefix, "");
			}

			if (Suffix != "")
			{
				_return = _return.Replace(Suffix, "");
			}

			return _return;
		}
	}
}
