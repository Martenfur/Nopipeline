using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoPipeline
{
	/// <summary>
	/// MGCB config object. Everything is merged in here.
	/// </summary>
	public class Content
	{
		public int ContentItemsCount => _contentItems.Count;
		/// <summary>
		/// All the global settings.
		/// TODO: Add support in the NPL config.
		/// </summary>
		private StringBuilder _globalSettings = new StringBuilder();

		/// <summary>
		/// Referenced libraries.
		/// </summary>
		private HashSet<string> _references = new HashSet<string>();

		/// <summary>
		/// Actual content items.
		/// </summary>
		private Dictionary<string, Item> _contentItems = new Dictionary<string, Item>();
		

		public string Build()
		{
			var builder = new StringBuilder();

			builder.AppendLine();
			builder.AppendLine(ContentStructure.GlobalPropertiesDivider);
			builder.AppendLine();

			builder.Append(_globalSettings);

			builder.AppendLine();
			builder.AppendLine(ContentStructure.ReferencesDivider);
			builder.AppendLine();

			// References.
			foreach (var reference in _references)
			{
				builder.AppendLine(ContentStructure.ReferenceKeyword + reference);
			}

			builder.AppendLine();
			builder.AppendLine(ContentStructure.ContentDivider);
			builder.AppendLine();

			// Items.
			foreach (var item in _contentItems.Values)
			{
				builder.Append(item.ToString());
			}

			builder.AppendLine();

			return builder.ToString();
		}

		public void AddGlobalSetting(string setting) =>
			_globalSettings.AppendLine(setting);



		public void AddContentItem(Item item)
		{
			if (_contentItems.ContainsKey(item.Path))
			{
				_contentItems[item.Path] = item;
			}
			else
			{
				_contentItems.Add(item.Path, item);
			}
		}


		public void AddReference(string reference)
		{
			var normalizedReference = reference.Replace("\\", "/");
			if (!_references.Contains(normalizedReference))
			{
				_references.Add(normalizedReference);
			}
		}


		/// <summary>
		/// Checks if content files exist and checks watched files.
		/// </summary>
		public void CheckIntegrity(string rootPath)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Checking integrity of the final config. Hold on tight!");
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Gray;

			var checkedItems = new Dictionary<string, Item>();

			foreach (Item item in _contentItems.Values)
			{
				Console.WriteLine("Checking " + Path.Combine(rootPath, item.Path));

				// Don't include if the file doesn't exist.
				if (File.Exists(Path.Combine(rootPath, item.Path)))
				{

					DateTime itemLastModified = File.GetLastWriteTime(rootPath + item.Path);
					
					var relativeItemPath = Path.Combine(rootPath + Path.GetDirectoryName(item.Path));

					// Watched files are files which aren't tracked by the content pipeline.
					// But they are tracked by us! We look which files were recently modified
					// and, if their modification date is more recent than the date of tracked file,
					// we "modify" the tracked file by changing its Last Modified date. This way 
					// Pipeline thinks the file has been updated and rebuilds it.
					foreach (var checkWildcard in item.Watch)
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

						foreach (var file in files)
						{
							Console.WriteLine("Checking " + file);
							DateTime fileLastModified = File.GetLastWriteTime(file);
							DateTime fileCreationTime = File.GetCreationTime(file);

							if (itemLastModified < fileLastModified || itemLastModified < fileCreationTime)
							{
								Console.WriteLine("Modifying: " + file);
								File.SetLastWriteTime(rootPath + item.Path, DateTime.Now);
								break;
							}
						}
					}
					
					checkedItems.Add(item.Path, item);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(item.Path + " doesn't exist anymore. Removing it from the config.");
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
			_contentItems = checkedItems;

			Console.WriteLine();

			var checkedReferences = new HashSet<string>();
			foreach (var reference in _references)
			{
				Console.WriteLine("Checking reference: " + Path.Combine(rootPath, reference));
				if (File.Exists(Path.Combine(rootPath, reference)))
				{
					checkedReferences.Add(reference);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(reference + " wasn't found! Deleting it from the config.");
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
			_references = checkedReferences;

		}


	}
}
