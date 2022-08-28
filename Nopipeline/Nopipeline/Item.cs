using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;


namespace Nopipeline
{

	/// <summary>
	/// Item object represents a rule in the config file or section of MGCB file.
	/// </summary>
	public class Item
	{
		
		/// <summary>
		/// Path to the files. Can contain windcards.
		/// </summary>
		public string Path 
		{ 
			get => _path;
			set
			{
				_path = value.Replace("\\", "/");
				if (_path.StartsWith("/")) // MGCB will melt if the path will start with a slash.
				{
					_path = Path.Substring(1);
				}
			}
		}
		private string _path;

		/// <summary>
		/// If true, file search will be resursive.
		/// </summary>
		public bool Recursive;
		
		/// <summary>
		/// List of watch entries.
		/// </summary>
		public List<string> Watch;
		
		/// <summary>
		/// File action. Can be build or copy.
		/// </summary>
		public string Action = "";
		
		/// <summary>
		/// If true, Path will have Root appended to it.
		/// </summary>
		public bool AppendRoot = true;

		/// <summary>
		/// List of processor parameters.
		/// </summary>
		public List<string> Parameters;

		/// <summary>
		/// The processor used for this item
		/// </summary>
		public string Processor;

		/// <summary>
		/// The section this item belongs to
		/// </summary>
		public string Section;

		/// <summary>
		/// Should this item be included in the ContentList
		/// </summary>
		public bool IncludeContentList;

		public Item(string path)
		{
			Path = path;
			Parameters = new List<string>();
			Recursive = false;
			Watch = new List<string>();
			Processor = "Notdefined;";
			IncludeContentList = true;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append($"#begin {Path.Replace('\\', '/')}{System.Environment.NewLine}");
			foreach(var parameter in Parameters)
			{
				builder.Append(parameter + System.Environment.NewLine);
			}
			if (Action != "") // Action ALWAYS should come last.
			{
				builder.Append(Action + System.Environment.NewLine + System.Environment.NewLine);
			}

			return builder.ToString();
		}

		public string ToContentListItem()
		{
			return  $"new ContentListItem(\"{Section}\",\"{Path.Replace('\\', '/').Remove(Path.LastIndexOf("."))}\",\"{Processor}\"),";
		}
		
		public void Add(string param, JToken value)
		{
			switch(param.ToLower())
			{
				case "path":
					Path = value.ToString();
					break;
				case "recursive":
					Recursive = (string.Compare(value.ToString(), "true", true) == 0);
					break;
				case "action":
					Action = $"/{value.ToString()}:{GetLinkedPath()}";
					break;
				case "watch":
					Watch = value.ToObject<List<string>>();
					break;
				case "processor":
					Processor = value.ToString();
					goto default; // processor is also used as parameter
				case "section":
					Section = value.ToString();
					break;
				case "contentlist":
					IncludeContentList = value.ToObject<bool>();
					break;
				default:
					Parameters.Add($"/{param}:{value.ToString()}");
					break;
			}
		}

		private string GetLinkedPath()
		{ 
			if (AppendRoot && Path.StartsWith(Content.Root))
			{
				return Path + ";" + Path.Replace(Content.Root, "").TrimStart('/');
			}
			return Path;
		}
	}

}
