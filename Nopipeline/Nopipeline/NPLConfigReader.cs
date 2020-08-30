using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Nopipeline
{
	/*
	 * ContentProcessor class reads json configuration file and update MGCB object
	 * Input:
	 *   conf:JObject - config object
	 *   content:MGCB - MGCB object
	 */
	public class NPLConfigReader
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
			try
			{
				Content.Root = config["root"].ToString().Replace("\\", "/");
				if (Content.Root.StartsWith("./")) // Ignoring whatever this is. I'm a professional I swear.
				{
					Content.Root = Content.Root.Substring(2, Content.Root.Length - 2);
				}
				Console.WriteLine("Root: " + Content.Root);
			}
			catch
			{
				Console.WriteLine("No root found! Using default paths.");
			}
			Console.WriteLine();

			Console.WriteLine();
			ParseReferences(config, content, rootDir);
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
					path = section["path"].ToString().Replace("\\", "/");
				}
				catch
				{
					Console.WriteLine($"Key 'path' doesn't exist in  {sectionName}!");
					throw new Exception($"Key 'path' doesn't exist in  {sectionName}!");
				}
				if (path.Contains("../"))
				{
					throw new Exception("'path' is not allowed to contain '../'! Use 'root' property to specify a different root instead.");
				}

				var appendRoot = false;
				if (Content.Root != "")
				{
					if (path.StartsWith('$'))
					{
						// $ means that the path will not have the root appended to it.
						path = path.TrimStart('$');
						appendRoot = false;
					}
					else
					{
						// Appending root now so that we would work with full paths.
						// We don't care if it's a link or not for now.
						path = Path.Combine(Content.Root, path);
						appendRoot = true;
					}
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
						searchOpt = (string.Compare(section["recursive"].ToString(), "true", true) == 0) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
					}
					files = Directory.GetFiles(Path.Combine(rootDir, filePath), fileName, searchOpt);
				}
				catch (Exception e)
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
					newItem.AppendRoot = appendRoot;

					Console.WriteLine("    Reading " + name);

					foreach (var sect in section)
					{
						if (sect.Key == "path")
						{ // path - already get - ignore
							continue;
						}
						if (sect.Key == "processorParam")
						{ // read processor's parameters
							JObject processorParam = section["processorParam"] as JObject;
							foreach (var pp in processorParam)
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


		private void ParseReferences(JObject config, Content content, string rootDir)
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

				var refName = Path.GetFileName(reference);
				var refPath = Path.GetDirectoryName(reference);
				var dlls = Directory.GetFiles(Path.Combine(rootDir, refPath), refName, SearchOption.TopDirectoryOnly);
				foreach (var dll in dlls)
				{
					var resultPath = Path.Combine(refPath, Path.GetFileName(dll)).Replace('\\', '/');
					Console.WriteLine(resultPath);
					content.AddReference(resultPath);
				}
			}
		}
	}
}
