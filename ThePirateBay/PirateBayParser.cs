using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePirateBay
{
	public class PirateBayParser
	{ 
		public PirateBayParser(string url)
		{
			Url = url;
		}

		private string Url
		{
			get;
			set;
		}

		public async Task Search(string searchTerm)
		{
			var searchAddress = ConstructSearch(searchTerm);
			PirateItemCollection pirateItems = await ScrapeSearchPage(searchAddress);

			if (pirateItems is null)
			{
				return;
			}
			
			/*
			foreach (var pirateItem in pirateItems)
			{
				var magnetLink = await GetLink(pirateItem.Link);
				pirateItem.MagnetLink = magnetLink;
			}
			*/

			Console.WriteLine(pirateItems);
		}

		private string ConstructSearch(string searchTerm)
		{
			var search = Uri.EscapeUriString(searchTerm);
			string address = Flurl.Url.Combine(Url, "search", search, "1","99","0");
			return address;
		}

		private async Task<string> GetLink(string link)
		{
			var config = Configuration.Default.WithDefaultLoader();

			IDocument document = await
						BrowsingContext.New(config).OpenAsync(link);

			var div = document.QuerySelector("div.download");

			if (div is null)
			{
				return null;
			}

			var magnetLink = div.QuerySelector("a").GetAttribute("href");
			return magnetLink;
		}

		public async Task<PirateItemCollection> ScrapeSearchPage(string address)
		{
			var config = Configuration.Default.WithDefaultLoader();

			IDocument document = await
						BrowsingContext.New(config).OpenAsync(address);

			var table = document.GetElementById("searchResult");

			// get all rows
			var rows = table.GetElementsByTagName("tr");

			PirateItemCollection pirateItemCollection = new PirateItemCollection();

			foreach (var row in rows)
			{
				var cells = row.GetElementsByTagName("td");
				var typeCell = row.QuerySelector("td.vertTh");
				var div = row.QuerySelector("div.detName");
				if (div is null)
				{
					// could be title bar row?
					continue;
				}

				// get title and link
				var titleCell = div.ParentElement;
				var title = div.Text().Trim();
				var link = titleCell.QuerySelector("a").GetAttribute("href");

				// TODO: get seeders and leechers
				// var seedCell = row.

				PirateItem pirateItem = new PirateItem(title, link);
				pirateItemCollection.Add(pirateItem);
			}

			return pirateItemCollection;
		}
	}
}
