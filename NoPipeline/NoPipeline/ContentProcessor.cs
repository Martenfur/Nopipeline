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

		public ContentProcessor(JObject conf, MGCB content)
		{
			JObject contentJson = conf["content"] as JObject;
			string rootPath = conf["root"].ToString().TrimEnd('/', '\\') + "/";
			foreach(var itm in contentJson)
			{
				// Read section
				string sectionName = itm.Key;
				JObject section = itm.Value as JObject;
				//StringBuilder processorParamsStr = new StringBuilder();

				// read item
				string path;
				try
				{
					path = section["path"].ToString();
				}
				catch
				{
					Console.WriteLine($"key 'path' not exist in  {sectionName} ");
					throw new Exception($"key 'path' not exist in  {sectionName} ");
				}
				//if (section.ContainsKey("processorParams")) {
				//	JObject processorParams = section["processorParams"] as JObject;
				//	foreach (var pp in processorParams) {
				//		StringBuilder processorParamsStr = new StringBuilder();

				//	}
				//}
				var fileName = Path.GetFileName(path);
				var filePath = Path.GetDirectoryName(path);
				string[] files = new string[] { };

				try
				{
					var searchOpt = SearchOption.TopDirectoryOnly;
					if(section.ContainsKey("recursive"))
					{
						searchOpt = (section["recursive"].ToString() == "True") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
					}
					files = Directory.GetFiles($"{rootPath}{filePath}", fileName, searchOpt);
				}
				catch
				{
					Console.WriteLine($"{section["path"].ToString()} not found");
				}

				foreach(var file in files)
				{
					var name = file.Remove(0, rootPath.Length).Replace('\\', '/');
					var it = new Item() { Path = name };

					foreach(var sect in section)
					{
						if(sect.Key == "path")
						{ // path - already get - ignore
							continue;
						}
						if(sect.Key == "processorParam")
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

					//it.Param.Append(System.Environment.NewLine); // last empty line
					if(content.Items.ContainsKey(it.Path))
					{
						content.Items[it.Path] = it;
					}
					else
					{
						content.Items.Add(it.Path, it);
						Console.WriteLine("NEW BOI: " + it.Path + " " + content.Items.Count);
					}
				}

			}
		}
	}
}
