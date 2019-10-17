using System;
using System.Collections.Generic;
using System.Text;

namespace NoPipeline
{
	public class Content
	{
		public StringBuilder Header;
		public Dictionary<string, Item> Items;
		public HashSet<string> References;

		public Content()
		{
			Header = new StringBuilder();
			Items = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
			References = new HashSet<string>();
		}

	}
}
