using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NoPipeline
{
	/*
	 * ContentProcessor class reads json configuration file and update MGCB object
	 * Input:
	 *   conf:JObject - config object
	 *   content:MGCB - MGCB object
	 */
	class NPLConfigReader : IConfigReader
	{

		public void Read(Content content, string configPath)
		{
			var config = JObject.Parse(File.ReadAllText(configPath, Encoding.UTF8));
			var rootDir = Path.GetDirectoryName(configPath) + "/";

			var contentJson = (JObject)config["content"];
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Reading NPL config.");
			Console.ForegroundColor = ConsoleColor.Gray;


			Console.WriteLine();
			ParseReferences(config, content);
			Console.WriteLine();


			foreach (var item in contentJson)
			{
				// Read section.
				string sectionName = item.Key;
				var section = (JObject)item.Value;
				
				// Read item.
				string path;
				try
				{
					path = section["path"].ToString();
				}
				catch
				{
					Console.WriteLine($"Key 'path' doesn't exist in  {sectionName}!");
					throw new Exception($"Key 'path' doesn't exist in  {sectionName}!");
				}

				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Reading content for: " + path);
				Console.ForegroundColor = ConsoleColor.Gray;

				var fileName = Path.GetFileName(path);
				var filePath = Path.GetDirectoryName(path);
				var files = new string[] { };

				try
				{
					var searchOpt = SearchOption.TopDirectoryOnly;
					if (section.ContainsKey("recursive"))
					{
						searchOpt = (section["recursive"].ToString().ToLower() == "true") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
					}
					files = Directory.GetFiles(Path.Combine(rootDir, filePath), fileName, searchOpt);
				}
				catch(Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"    Error reading files from {rootDir}{filePath}: ");
					Console.WriteLine("    " + e.Message);
					Console.ForegroundColor = ConsoleColor.Gray;
				}

				foreach (var file in files)
				{
					var name = file.Remove(0, rootDir.Length).Replace('\\', '/');
					var newItem = new Item(name);
					
					Console.WriteLine("    Reading " + name);

					foreach(var sect in section)
					{
						if (sect.Key == "path")
						{ // path - already get - ignore
							continue;
						}
						if (sect.Key == "processorParam")
						{ // read processor's parameters
							JObject processorParam = section["processorParam"] as JObject;
							foreach(var pp in processorParam)
							{
								newItem.Add("processorParam", $"{pp.Key}={pp.Value}");
							}
						}
						else
						{
							newItem.Add(sect.Key, sect.Value);
						}
					}
					
					content.AddContentItem(newItem);
				}

			}

			Console.WriteLine();
			Console.WriteLine("Finished reading NPL config!");
			Console.WriteLine();
			

		}


		void ParseReferences(JObject config, Content content)
		{
			if (!config.ContainsKey("references"))
			{
				return;
			}

			var contentJson = config["references"];
			foreach (var item in contentJson)
			{
				var reference = Environment.ExpandEnvironmentVariables(item.ToString());
				Console.WriteLine("Reading reference: " + reference);
				content.AddReference(reference);
			}
		}
	}
}
