using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoPipeline
{
	public enum CollectionStates
	{
		Settings,
		References,
		Content
	}

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

		public List<string> References;

		const string _referenceKeyword = "/reference";

		public MGCB(string MGCBConfigPath)
		{
			// Read mgcb config file

			Console.WriteLine("Reading MGCB config " + MGCBConfigPath);

			CfgName = MGCBConfigPath;
			CfgPath = Path.GetDirectoryName(MGCBConfigPath);
			
			string line;
			var collectionState = CollectionStates.Settings;
			Item it = null;
			Header = new StringBuilder();
			Items = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
			References = new List<string>();

			using (StreamReader file = new StreamReader(MGCBConfigPath))
			{
				while ((line = file.ReadLine()) != null)
				{
					if (collectionState == CollectionStates.Settings)
					{
						if (line.StartsWith(_referenceKeyword))
						{
							collectionState = CollectionStates.References;
						}
						else
						{
							if (line.StartsWith("#begin"))
							{
								collectionState = CollectionStates.Content;
							}
							else
							{
								Header.AppendLine(line);
							}
						}
					}

					if (collectionState == CollectionStates.References)
					{
						if (line.StartsWith("#begin"))
						{
							collectionState = CollectionStates.Content;   // found first begin - stop collecting header
						}
						else
						{
							if (line.StartsWith(_referenceKeyword))
							{
								References.Add(line.Substring(_referenceKeyword.Length));
								Console.WriteLine("Reading reference");
							}
						}
					}


					if (collectionState == CollectionStates.Content)
					{
						if (line.StartsWith("#begin"))
						{
							it = new Item
							{
								Path = line.Substring(7)
							};
							if (!Items.ContainsKey(it.Path))
							{
								it.FixPath();
								Items.Add(it.Path, it); // add to the dictionary
								Console.WriteLine("Reading " + it.Path);
							}
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

		/// <summary>
		/// Checks if content files exist and checks watch files.
		/// </summary>
		public void ContentCheck()
		{
			var ItemsCheck = new Dictionary<string, Item>();

			foreach (Item it in Items.Values)
			{
				
				if (File.Exists(Path.Combine(CfgPath, it.Path)))
				{  // not exists - not include to Items
					DateTime lastModified = File.GetLastWriteTime(CfgPath + it.Path);
					// check if "watch" present

					var relativeItemPath = Path.Combine(CfgPath + Path.GetDirectoryName(it.Path));


					foreach (var checkWildcard in it.Watch)
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

						foreach (var f in files)
						{
							Console.WriteLine("Checking " + f);
							DateTime f_lastModified = File.GetLastWriteTime(f);
							DateTime f_creationTime = File.GetCreationTime(f);

							if (lastModified < f_lastModified || lastModified < f_creationTime)
							{
								// change datetime required
								Console.WriteLine("Modifying: " + f);
								File.SetLastWriteTime(CfgPath + it.Path, DateTime.Now);
								break;
							}
						}
					}


					ItemsCheck.Add(it.Path, it);
				}
				else
				{
					Console.WriteLine(it.Path + " doesn't exist anymore. Removing it from the config.");
				}
			}
			Items = ItemsCheck;
		}

		/// <summary>
		/// Saves resulting config.
		/// </summary>
		public void Save()
		{
			Console.WriteLine("Saving new config.");
			using (var file = new StreamWriter(CfgName))
			{
				// header
				file.Write(Header.ToString());

				// items
				foreach (Item it in Items.Values)
				{
					file.Write(it.ToString());
				}

			}
			Console.WriteLine("Done! :o");
		}

	}
}
