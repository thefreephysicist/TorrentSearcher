using System;
using System.Collections.Generic;
using System.Text;

namespace ThePirateBay
{
	class Logger
	{
		static Logger()
		{
			Logs = new List<string>();
		}

		private Logger()
		{
		}

		/// <summary>
		/// Adds a log to the logger
		/// </summary>
		/// <param name="s">the log string</param>
		public static void Log(string s)
		{
			Logs.Add(s);
		}

		private static IList<string> Logs
		{
			get;
			set;
		}
	}
}
