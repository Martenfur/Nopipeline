using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;


namespace NoPipeline
{
	/*
 * Item object represents a rule in the config file or section of MGCB file
 * Parameters:
 *   Param:List of strings - contains all item's configuration parameters to generate MGCB file
 *   Name:String - file name
 *   Recursive:Bool - True=search files recursively
 *   Watch:List of strings - contains masks of files
 * Methods:
 *   ToString() - generate recod of the item to store in MGCB file
 *   Add() - add a new value of Item
 */
	public class Item
	{
		public List<string> Parameters { get; set; }
		public string Path { get; set; }
		public bool Recursive { get; set; }
		public List<string> Watch { get; set; }
		public string Action { get; set; } = "";

		public Item()
		{
			Parameters = new List<string>();
			Recursive = false;
			Watch = new List<string>();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append($"#begin {Path.Replace('\\', '/')}{System.Environment.NewLine}");
			foreach(var parameter in Parameters)
			{
				builder.Append(parameter + System.Environment.NewLine);
			}
			if (Action != "")
			{
				builder.Append(Action + System.Environment.NewLine + System.Environment.NewLine);
			}

			return builder.ToString();
		}

		public void Add(string param, JToken value)
		{
			switch(param)
			{
				case "path":
					Path = value.ToString().Replace('\\', '/');
					break;
				case "recursive":
					Recursive = (value.ToString() == "True");
					break;
				case "action":
					Action = $"/{value.ToString()}:{Path.Replace('\\', '/')}";
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
