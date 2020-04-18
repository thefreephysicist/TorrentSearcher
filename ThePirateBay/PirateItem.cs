using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ThePirateBay
{
	public class PirateItem
	{
		private static int id = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="PirateItem"/> class.
		/// </summary>
		/// <param name="title">the title of the item</param>
		/// <param name="url">the url of item</param>
		public PirateItem(string title, string url)
		{
			ID = ++id;
			Title = title;
			Url = url;
		}

		public int ID
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the torrent title
		/// </summary>
		public string Title
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the torrent url
		/// </summary>
		public string Url
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the number of seeders
		/// </summary>
		public int SeedersCount
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the number of leechers
		/// </summary>
		public int LeechersCount
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets or set the size
		/// </summary>
		public string Size
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the torrent magnet link
		/// </summary>
		public string MagnetUrl { get; internal set; }

		/// <summary>
		/// Creates a string representation of the <see cref="PirateItem"/>
		/// </summary>
		/// <returns>a string</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Url))
			{
				return "No Desc";
			}

			string str = string.Format("{0}\n{1}\n", Title, Url);

			str += string.Format("SE: {0}, LE: {1}\n", SeedersCount, LeechersCount);
			
			if (!string.IsNullOrEmpty(MagnetUrl))
			{
				str += MagnetUrl + "\n";
			}

			
			return str;
		}
	}
}
