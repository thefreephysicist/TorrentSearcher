using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace ThePirateBay
{
	public class PirateBayParser
	{
		/// <summary>
		/// The logger
		/// </summary>
		private ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="PirateBayParser"/> class.
		/// </summary>
		/// <param name="url">the url of the pirate bay</param>
		public PirateBayParser(string url, ILogger logger)
		{
			_logger = logger;
			Url = url;
		}
		
		/// <summary>
		/// Gets the pirate bay results model
		/// </summary>
		public PirateBayModel PirateBayResults
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the pirate bay urls
		/// </summary>
		private string Url
		{
			get;
			set;
		}

		/// <summary>
		/// Search a query
		/// </summary>
		/// <param name="searchQuery">the search query</param>
		/// <returns>A results model</returns>
		public async Task<PirateBayModel> Search(Query searchQuery, int maxPages = 3)
		{
			// construct the search url

			var searchAddress = ConstructSearchUrl(searchQuery.SearchTerm, 1);

			PirateItemCollection pirateItems = new PirateItemCollection();
			// scrape the search page
			for (int i = 0; i < maxPages; i++)
			{
				PirateItemCollection pirateItemsPage = await ScrapeSearchPage(searchAddress);
				if (pirateItems is null)
				{
					// something has gone wrong
					_logger.Log("Failed to get PirateItems");
					return null;
				}

				if (pirateItems.Count == 0)
				{
					// we're done searching as there are no more results
					break;
				}

				pirateItems.AddRange(pirateItemsPage);
			}

			PirateBayModel results = new PirateBayModel()
			{
				PirateItems = pirateItems,
				Query = searchQuery
			};

			PirateBayResults = results;
			return results;
		}

		/// <summary>
		/// Get page from url
		/// </summary>
		/// <param name="address">the search address</param>
		/// <returns>the IDocument for the page</returns>
		private async Task<IDocument> DownloadPage(string address) 
		{
			var config = Configuration.Default.WithDefaultLoader();

			IDocument document = await
						BrowsingContext.New(config).OpenAsync(address);

			return document;
		}

		/// <summary>
		/// Scrape search page
		/// </summary>
		/// <param name="address">the url to search</param>
		/// <param name="getMagnetLinks">get magnet links</param>
		/// <returns>the pirate items</returns>
		private async Task<PirateItemCollection> ScrapeSearchPage(string address, bool getMagnetLinks = false)
		{
			IDocument document = await DownloadPage(address);

			// get the table and then get all the rows
			var table = document.GetElementById("searchResult");

			// get all rows
			var rows = table.GetElementsByTagName("tr");

			PirateItemCollection pirateItemCollection = new PirateItemCollection();

			foreach (var row in rows)
			{
				var titleAndLink = ParseTitleAndUrl(row);
				
				// if null, not a valid torrent row (first row is not a valid row)
				if (!titleAndLink.HasValue)
				{
					continue;
				}

				var size = ParseSize(row);

				var seedersAndLeechers = ParseSeedersAndLeechers(row);

				PirateItem pirateItem = new PirateItem(titleAndLink.Value.title, titleAndLink.Value.url);
				if (size != null)
				{
					pirateItem.Size = size;
				}

				if (seedersAndLeechers.HasValue)
				{
					pirateItem.SeedersCount = seedersAndLeechers.Value.seeders;
					pirateItem.LeechersCount = seedersAndLeechers.Value.leechers;
				}

				pirateItemCollection.Add(pirateItem);
			}

			if (getMagnetLinks)
			{
				await UpdatePirateItemsMagnetUrlAsync(pirateItemCollection);
			}

			return pirateItemCollection;
		}
		
		private static IElement GetTitleCell(IElement row)
		{
			var div = row.QuerySelector("div.detName");
			if (div is null)
			{
				// could be title bar row?
				return null;
			}

			return	div.ParentElement;
		}

		/// <summary>
		/// Parse title and url from row
		/// </summary>
		/// <param name="row">the row</param>
		/// <returns>tuple of title and url</returns>
		private static (string title, string url)? ParseTitleAndUrl(IElement row)
		{
			var titleCell = GetTitleCell(row);

			if (titleCell == null)
			{
				return null;
			}

			// get title and link
			var div = row.QuerySelector("div.detName");
			var title = div.Text().Trim();
			var url = titleCell.QuerySelector("a").GetAttribute("href");

			return (title, url);
		}

		private static string ParseSize(IElement row)
		{
			var titleCell = GetTitleCell(row);

			var fontTag = titleCell.QuerySelector("font[class='detDesc']");

			var text = fontTag.Text();
			var firstIndex = text.IndexOf("Size");
			if (firstIndex == -1)
			{
				return null;
			}
			firstIndex += 5;

			var lastIndex = text.LastIndexOf(",");
			if (lastIndex == -1)
			{
				return null;
			}

			if (firstIndex >= lastIndex)
			{
				return null;
			}

			string sizeString = text.Substring(firstIndex, lastIndex - firstIndex).Trim();
			return sizeString;
		}

		/// <summary>
		/// Parse seeders and leechers
		/// </summary>
		/// <param name="row">the row</param>
		/// <returns>tuple of seeders and leechers</returns>
		private static (int seeders, int leechers)? ParseSeedersAndLeechers(IElement row)
		{
			var seedAndLeechCells = row.QuerySelectorAll("td[align='right']").ToList(); ;

			if (seedAndLeechCells.Count != 2)
			{
				throw new Exception("count must be 2 (two cells)");
			}

			var se = seedAndLeechCells[0].Text();
			var le = seedAndLeechCells[1].Text();

			int seeders;
			int leechers;

			if (!int.TryParse(se, out seeders))
			{
				// could not parse seeders
			}

			if (!int.TryParse(le, out leechers))
			{
				// could not parse leechers
			}

			return (seeders, leechers);
		}

		/// <summary>
		/// Construct a search url from a search term
		/// </summary>
		/// <param name="searchTerm">the search term</param>
		/// <returns>the constructer string</returns>
		private string ConstructSearchUrl(string searchTerm, int pageNumber = 1)
		{
			var search = Uri.EscapeUriString(searchTerm);
			string address = Flurl.Url.Combine(Url, "search", search, pageNumber.ToString(), "99", "0");
			return address;
		}
		
		/// <summary>
		/// Parse a magnet link from torrent url
		/// </summary>
		/// <param name="torrentUrl">the torrent url</param>
		/// <returns>the magnet url. null if failed</returns>
		private static async Task<string> ParseMagnetLink(string torrentUrl)
		{
			var config = Configuration.Default.WithDefaultLoader();

			IDocument document = await
						BrowsingContext.New(config).OpenAsync(torrentUrl);

			var div = document.QuerySelector("div.download");

			if (div is null)
			{
				return null;
			}

			var magnetUrl = div.QuerySelector("a").GetAttribute("href");
			return magnetUrl;
		}

		/// <summary>
		/// Update all <see cref="PirateItem"/> objects with their magnet link
		/// </summary>
		/// <param name="pirateItems">the pirate items collection</param>
		/// <returns>Task</returns>
		private static async Task UpdatePirateItemsMagnetUrlAsync(IEnumerable<PirateItem> pirateItems)
		{
			foreach (var pirateItem in pirateItems)
			{
				var magnetLink = await ParseMagnetLink(pirateItem.Url);
				pirateItem.MagnetUrl = magnetLink;
			}
		}
	}
}
