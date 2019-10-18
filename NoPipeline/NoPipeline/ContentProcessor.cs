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
	class ContentProcessor
	{

		public ContentProcessor(Content content, string configPath)
		{
			var config = JObject.Parse(File.ReadAllText(configPath, Encoding.UTF8));
			var rootDir = Path.GetDirectoryName(configPath) + "/";

			JObject contentJson = config["content"] as JObject;
			
			ParseReferences(config, content);

			Console.WriteLine("Reading NPL config.");

			foreach(var item in contentJson)
			{
				// Read section.
				string sectionName = item.Key;
				JObject section = item.Value as JObject;
				
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
				
				Console.WriteLine("Rule: " + path);
				
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
					Console.WriteLine($"Error reading files from {rootDir}{filePath}: " + e.Message);
				}

				foreach(var file in files)
				{
					var name = file.Remove(0, rootDir.Length).Replace('\\', '/');
					var newItem = new Item() { Path = name };
					newItem.FixPath();
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
					
					if (content.Items.ContainsKey(newItem.Path))
					{
						content.Items[newItem.Path] = newItem;
					}
					else
					{
						content.Items.Add(newItem.Path, newItem);
					}
				}

			}

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
				Console.WriteLine("Reference: " + item);
				var reference = Path.GetFullPath(Environment.ExpandEnvironmentVariables(item.ToString()));

				if (!content.References.Contains(reference))
				{
					content.References.Add(reference);
				}
			}
		}
	}
}
