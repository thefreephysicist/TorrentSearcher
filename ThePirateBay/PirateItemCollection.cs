using System;
using System.Collections.Generic;
using System.Text;

namespace ThePirateBay
{
	public class PirateItemCollection : List<PirateItem>
	{
		public PirateItemCollection() :
			base()
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var p in this)
			{
				stringBuilder.Append(p);
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}
	}
}
