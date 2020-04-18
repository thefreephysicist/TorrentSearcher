using System;
using System.Collections.Generic;
using System.Text;

namespace ThePirateBay
{
	public class PirateBayModel
	{
		public PirateBayModel()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="PirateItemCollection"/>
		/// </summary>
		public PirateItemCollection PirateItems
		{
			get;
			set;
		}

		public Query Query
		{
			get;
			set;
		}
	}
}
