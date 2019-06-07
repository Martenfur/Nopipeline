using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;


namespace NoPipeline
{

	/// <summary>
	/// Item object represents a rule in the config file or section of MGCB file.
	/// </summary>
	public class Item
	{
		
		/// <summary>
		/// Path to the files. Can contain windcards.
		/// </summary>
		public string Path { get; set; }
		
		/// <summary>
		/// If true, file search will be resursive.
		/// </summary>
		public bool Recursive { get; set; }
		
		/// <summary>
		/// List of watch entries.
		/// </summary>
		public List<string> Watch { get; set; }
		
		/// <summary>
		/// File action. Can be build or copy.
		/// </summary>
		public string Action { get; set; } = "";

		/// <summary>
		/// List of processor parameters.
		/// </summary>
		public List<string> Parameters { get; set; }

		public Item()
		{
			Parameters = new List<string>();
			Recursive = false;
			Watch = new List<string>();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			FixPath();
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

		public void FixPath()
		{
			if (Path.StartsWith("/")) // MGCB will melt if the path will start with a slash.
			{
				Path = Path.Substring(1);
			}
		}

		public void Add(string param, JToken value)
		{
			switch(param.ToLower())
			{
				case "path":
					Path = value.ToString().Replace('\\', '/');
					FixPath();
					break;
				case "recursive":
					Recursive = (value.ToString().ToLower() == "true");
					break;
				case "action":
					Action = $"/{value.ToString()}:{Path.Replace('\\', '/')}".ToLower();
					break;
				case "watch":
					Watch = value.ToObject<List<string>>();
					break;
				default:
					Parameters.Add($"/{param}:{value.ToString().Replace('\\', '/')}");
					break;
			}
		}
	}

}
