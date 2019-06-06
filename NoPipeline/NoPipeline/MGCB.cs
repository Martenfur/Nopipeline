using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NoPipeline
{
	/*
	 * MGCB class - represents the mgcb configuration file
	 * Input:
	 *   mgcb file name
	 * Parameters:
	 *   Header:List of strings - contains all global configuration parameters to generate MGCB file
	 *   Items:List of Item - contains list of Item objects
	 *   CfgName:String - Config file name
	 *   CfgPath:String - Config file path
	 * Methods:
	 *   MGCB(conf) - read mgcb config file and return MGCB object
	 *   Add(Item) - Add new Item
	 *   ContentCheck() - Check configuration. Find all files related to the Item and check the modified date of each file.
	 *	                  if file was modified - update the item file modified time to current(if applicable)
	 *	 Save() - store the MGCB object into mgcb congiguration file
	 */
	public class MGCB
	{
		public StringBuilder Header { get; set; }
		public Dictionary<string, Item> Items { get; set; }
		public string CfgName { get; set; }
		public string CfgPath { get; set; }

		public MGCB(JObject conf, string rootPath)
		{   
			// Read mgcb config file
			
			var name = rootPath.TrimEnd('/', '\\') + "/" + conf["path"].ToString();  // path to Content.mgcb
			
			if(!File.Exists(name))
			{
				throw new Exception($"{name} file not found!");
			}

			Console.WriteLine("Reading MGCB config " + name);

			CfgName = name;
			CfgPath = Path.GetDirectoryName(name);
			string line;
			var isItemSection = false;
			Item it = null;
			Header = new StringBuilder();
			Items = new Dictionary<string, Item>();

			using(StreamReader file = new StreamReader(name))
			{
				while((line = file.ReadLine()) != null)
				{
					if(!isItemSection)
					{
						if(line.StartsWith("#begin"))
						{
							isItemSection = true;   // found first begin - stop collecting header
						}
						else
						{
							Header.AppendLine(line);
						}
					}
					if(isItemSection)
					{
						if(line.StartsWith("#begin"))
						{
							it = new Item
							{
								Path = line.Substring(7)
							};
							Items.Add(it.Path, it); // add to the dictionary
							Console.WriteLine("Reading " + it.Path);
						}
						else
						{
							it.Parameters.Add(line);
						}
					}
				}
			}

			Console.WriteLine("Finished reading MGCB config! Got " + Items.Count + " items.");
			Console.WriteLine();
				
		}

		public void ContentCheck()
		{   // check all items exist
			var ItemsCheck = new Dictionary<string, Item>();

			foreach(Item it in Items.Values)
			{
				if(File.Exists(CfgPath + "/" + it.Path))
				{  // not exists - not include to Items
					DateTime lastModified = File.GetLastWriteTime(CfgPath + "/" + it.Path);
					// check if "watch" present
					
					var relativeItemPath =  CfgPath + "/" + Path.GetDirectoryName(it.Path);

					
					
					foreach(var checkWildcard in it.Watch)
					{
						Console.WriteLine("Checking watch for " + checkWildcard);

						var fileName = Path.GetFileName(checkWildcard);
						var filePath = Path.GetDirectoryName(checkWildcard);

						string[] files;
					
						try
						{
							files = Directory.GetFiles($"{relativeItemPath}/{filePath}", fileName, SearchOption.AllDirectories);
						}
						catch
						{
							Console.WriteLine(checkWildcard + " wasn't found. Skipping.");
							continue;
						}

						foreach(var f in files)
						{
							DateTime f_lastModified = File.GetLastWriteTime(f);
							if(lastModified < f_lastModified)
							{
								// change datetime required
								Console.WriteLine("Modifying: " + f);
								File.SetLastWriteTime(CfgPath + "/" + it.Path, DateTime.Now);
								break;
							}
						}
					}
					
					
					ItemsCheck.Add(it.Path, it);
				}
				else
				{
					Console.WriteLine(it.Path + " doesn't exists anymore. Removing it from the config.");
				}
			}
			Items = ItemsCheck;
		}

		public void Save()
		{ 
			// save config file
			Console.WriteLine("Saving new config.");
			using(var file = new StreamWriter(CfgName))
			{
				// header
				file.Write(Header.ToString());
				// items

				foreach(Item it in Items.Values)
				{
					file.Write(it.ToString());
				}

			}
			Console.WriteLine("Done! :o");
		}

	}
}
