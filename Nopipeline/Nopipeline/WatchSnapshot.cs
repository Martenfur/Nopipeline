using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nopipeline
{
	public class WatchSnapshot
	{
		private Content _content;
		private string _rootPath;

		private Dictionary<string, HashSet<string>> _oldSnapshot = new Dictionary<string, HashSet<string>>();

		private readonly string _tempSnapshotPath;
		private readonly string _tempSnapshotDirectory;


		public WatchSnapshot(Content content, string rootPath)
		{
			_content = content;
			_rootPath = rootPath;

			_tempSnapshotPath = Path.Combine(_rootPath, "bin/watch.npl.temp");
			_tempSnapshotDirectory = Path.GetDirectoryName(Path.GetDirectoryName(_tempSnapshotPath));
			ReadSnapshot();
		}


		public bool CheckMatch(Item item)
		{
			if (item.Watch.Count == 0)
			{ 
				return true;
			}

			var fullItemPath = Path.Combine(_rootPath, Path.GetDirectoryName(item.Path));
			var watchRecords = new HashSet<string>();

			foreach (var checkWildcard in item.Watch)
			{

				var fileName = Path.GetFileName(checkWildcard);
				var filePath = Path.GetDirectoryName(checkWildcard);

				string[] files;

				try
				{
					files = Directory.GetFiles(Path.Combine(fullItemPath, filePath), fileName, SearchOption.AllDirectories);
				}
				catch
				{
					continue;
				}

				foreach (var file in files)
				{
					watchRecords.Add(GetFileId(file));
				}
			}

			if (_oldSnapshot.TryGetValue(Path.GetFullPath(item.Path), out var watchItems))
			{
				return watchItems.SetEquals(watchRecords); // TODO: Rename.
			}

			return false;
		}


		private string GetFileId(string path) =>
			Path.GetFullPath(path) + " | " + File.GetLastWriteTime(path);


		private void ReadSnapshot()
		{
			if (!File.Exists(_tempSnapshotPath))
			{
				return;
			}
			_oldSnapshot = JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(File.ReadAllText(_tempSnapshotPath));

			foreach (var kok in _oldSnapshot) // TODO: REMOVE
			{
				Console.WriteLine("AAAAAAAAAAAAAA" + kok.Key + " " + kok.Value);
			}
		}

		public void WriteSnapshot()
		{
			var contents = new Dictionary<string, HashSet<string>>();

			foreach (var item in _content.ContentItems.Values)
			{
				Console.WriteLine("Dumping: " + item.Path + " | " + item.Watch.Count);
				if (item.Watch.Count == 0)
				{
					continue;
				}

				// Don't include if the file doesn't exist.
				if (File.Exists(Path.Combine(_rootPath, item.Path)))
				{
					var relativeItemPath = Path.Combine(_rootPath, Path.GetDirectoryName(item.Path));
					var watchRecords = new HashSet<string>();

					foreach (var checkWildcard in item.Watch)
					{

						var fileName = Path.GetFileName(checkWildcard);
						var filePath = Path.GetDirectoryName(checkWildcard);

						string[] files;

						try
						{
							files = Directory.GetFiles(Path.Combine(relativeItemPath, filePath), fileName, SearchOption.AllDirectories);
						}
						catch
						{
							continue;
						}

						foreach (var file in files)
						{
							watchRecords.Add(GetFileId(file));
						}
					}

					contents.Add(Path.GetFullPath(item.Path), watchRecords);
				}
				else
				{
					Console.WriteLine(item.Path + " doesn't exist anymore. Removing it from the config.");
				}
			}

			Console.WriteLine();

			if (Directory.Exists(_tempSnapshotDirectory))
			{
				Directory.CreateDirectory(_tempSnapshotDirectory);
			}

			File.WriteAllText(_tempSnapshotPath, JsonConvert.SerializeObject(contents));

		}

	}
}
