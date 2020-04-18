using System;
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
			PirateBayParser p = new PirateBayParser(url);
			await p.Search(args[0]);

			Console.ReadLine();
			// Test().GetAwaiter().GetResult();
		}

		private static async Task Test()
		{
			string url = "https://thepiratebay.zone/";
			PirateBayParser p = new PirateBayParser(url);
			await p.Search("joker");
		}
	}
}
