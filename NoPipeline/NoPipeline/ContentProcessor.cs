using System;
using System.IO;
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

		public ContentProcessor(JObject config, MGCB content, string rootPath)
		{
			JObject contentJson = config["content"] as JObject;
			
			ParseReferences(config);

			Console.WriteLine("Reading NPL config.");

			foreach(var itm in contentJson)
			{
				// Read section
				string sectionName = itm.Key;
				JObject section = itm.Value as JObject;
				
				// read item
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
					if(section.ContainsKey("recursive"))
					{
						searchOpt = (section["recursive"].ToString().ToLower() == "true") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
					}
					files = Directory.GetFiles($"{rootPath}{filePath}", fileName, searchOpt);
				}
				catch
				{
					Console.WriteLine($"Error reading files from {rootPath}{filePath}");
				}

				foreach(var file in files)
				{
					var name = file.Remove(0, rootPath.Length).Replace('\\', '/');
					var it = new Item() { Path = name };
					it.FixPath();
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
								it.Add("processorParam", $"{pp.Key}={pp.Value}");
							}
						}
						else
						{
							it.Add(sect.Key, sect.Value);
						}
					}
					Console.WriteLine(content.Items.ContainsKey(it.Path) + " : " + it.Path);
					Console.WriteLine(content.Items.Count);
					foreach(var i in content.Items)
					{
						Console.WriteLine("Item: " + i.Value.Path);
					}

					if (content.Items.ContainsKey(it.Path))
					{
						content.Items[it.Path] = it;
					}
					else
					{
						content.Items.Add(it.Path, it);
					}
				}

			}

			Console.WriteLine("Finished reading NPL config!");
			Console.WriteLine();
		}


		void ParseReferences(JObject config)
		{
			var contentJson = config["references"];
			foreach (var item in contentJson)
			{
				Console.WriteLine("Reference: " + item);
			}
		}
	}
}
