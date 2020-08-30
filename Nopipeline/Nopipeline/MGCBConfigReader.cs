using System;
using System.IO;

namespace Nopipeline
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
	public class MGCBConfigReader
	{
		
		public void Read(Content content, string MGCBConfigPath)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Reading MGCB config " + MGCBConfigPath);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Gray;

			string line;
			var collectionState = CollectionStates.Settings;
			Item item = null;
			
			using (var file = new StreamReader(MGCBConfigPath))
			{
				while ((line = file.ReadLine()) != null)
				{
					// Reading settings.
					if (collectionState == CollectionStates.Settings)
					{
						if (line.StartsWith(ContentStructure.ReferenceKeyword))
						{
							collectionState = CollectionStates.References;
							Console.WriteLine();
						}
						else
						{
							if (line.StartsWith(ContentStructure.ContentBeginKeyword))
							{
								collectionState = CollectionStates.Content;
								Console.WriteLine();
							}
							else
							{
								if (line.StartsWith(ContentStructure.KeywordStartingChar))
								{
									Console.WriteLine("Reading setting: " + line);
									content.AddGlobalSetting(line);
								}
							}
						}
					}
					// Reading settings.

					
					// Reading references.
					if (collectionState == CollectionStates.References)
					{
						if (line.StartsWith(ContentStructure.ContentBeginKeyword))
						{
							collectionState = CollectionStates.Content;
							Console.WriteLine();
						}
						else
						{
							if (line.StartsWith(ContentStructure.ReferenceKeyword))
							{
								var reference = line.Substring(ContentStructure.ReferenceKeyword.Length);
								content.AddReference(reference);
								
								Console.WriteLine("Reading reference: " + reference);
							}
						}
					}
					// Reading references.

					// Reading content.
					if (collectionState == CollectionStates.Content)
					{
						if (line.StartsWith(ContentStructure.ContentBeginKeyword))
						{
							item = new Item(line.Substring(ContentStructure.ContentBeginKeyword.Length + 1));
							
							content.AddContentItem(item);
							Console.WriteLine("Reading content:" + item.Path);
						}
						else
						{
							item.Parameters.Add(line);
						}
					}
					// Reading content.
				}
			}

			Console.WriteLine("Finished reading MGCB config! Got " + content.ContentItemsCount + " items.");
			
		}
		

	}
}
