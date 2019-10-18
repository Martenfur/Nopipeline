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

		public string Build()
		{
			var builder = new StringBuilder();

			builder.AppendLine();
			builder.AppendLine(ContentStructure.GlobalPropertiesDivider);
			builder.AppendLine();

			builder.Append(Header);

			builder.AppendLine();
			builder.AppendLine(ContentStructure.ReferencesDivider);
			builder.AppendLine();

			// References.
			foreach (var reference in References)
			{
				builder.AppendLine(ContentStructure.ReferenceKeyword + reference);
			}

			builder.AppendLine();
			builder.AppendLine(ContentStructure.ContentDivider);
			builder.AppendLine();

			// Items.
			foreach (var item in Items.Values)
			{
				builder.Append(item.ToString());
			}

			builder.AppendLine();

			return builder.ToString();
		}

		public void AddReference(string reference)
		{
			var normalizedReference = reference.Replace("\\", "/");
			if (!References.Contains(reference))
			{
				References.Add(reference);
			}
		}


	}
}
