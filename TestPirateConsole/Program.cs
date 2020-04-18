using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThePirateBay;

namespace TestPirateConsole
{
	class Program
	{
		static async Task Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Need a search term");
				Console.ReadLine();
				return;
			}

			string url = "https://thepiratebay.zone/";
			ILogger logger = new Logger();
			PirateBayParser p = new PirateBayParser(url, logger);

			Query query = new Query()
			{
				SearchTerm = args[0]
			};

			await p.Search(query);

			PrintResults(p.PirateBayResults.PirateItems);

			Console.ReadLine();
		}

		private static void PrintResults(IEnumerable<PirateItem> pirateItems)
		{
			string[] headers = new string[]
			{
				"ID",
				"Title",
				"Size",
				"SE",
				"LE"
			};

			// create table
			var table = new ConsoleTable(headers);

			foreach(var pirateItem in pirateItems)
			{
				string[] row = new string[]
				{
					pirateItem.ID.ToString(),
					pirateItem.Title,
					pirateItem.Size,
					pirateItem.SeedersCount.ToString(),
					pirateItem.LeechersCount.ToString()
				};

				table.AddRow(row);
			}

			Console.WriteLine(table);
		}
	}
}
