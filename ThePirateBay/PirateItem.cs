using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ThePirateBay
{
	public class PirateItem
	{
		public PirateItem(string title, string link)
		{
			Title = title;
			Link = link;
		}

		public string Title
		{
			get;
			private set;
		}

		public string Link
		{
			get;
			private set;
		}
		public string MagnetLink { get; internal set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Link))
			{
				return "No Desc";
			}
			string str = string.Format("{0}\n{1}\n", Title, Link);
			
			if (!string.IsNullOrEmpty(MagnetLink))
			{
				str += MagnetLink + "\n";
			}
			return str;
		}
	}
}
