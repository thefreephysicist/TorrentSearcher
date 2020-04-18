using System;
using System.Collections.Generic;
using System.Text;

namespace ThePirateBay
{
	public class Logger : ILogger
	{
		public Logger() 
		{
			Logs = new List<string>();
		}

		/// <summary>
		/// Adds a log to the logger
		/// </summary>
		/// <param name="s">the log string</param>
		public void Log(string s)
		{
			Logs.Add(s);
		}

		private IList<string> Logs
		{
			get;
			set;
		}
	}
}
