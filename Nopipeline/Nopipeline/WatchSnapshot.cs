using System;
using System.Collections.Generic;
using System.IO;

namespace Nopipeline
{
	public class WatchSnapshot
	{
		private Content _content;
		private string _rootPath;

		private HashSet<string> _oldSnapshot = new HashSet<string>();

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


		public bool CheckMatch(string path) =>
			_oldSnapshot.Contains(GetFileId(path));


		private string GetFileId(string path) =>
			Path.GetFullPath(path) + " | " + File.GetLastWriteTime(path);


		private void ReadSnapshot()
		{
			if (!File.Exists(_tempSnapshotPath))
			{
				return;
			}
			var snapshotItems = File.ReadAllLines(_tempSnapshotPath);
			foreach (var item in snapshotItems)
			{
				_oldSnapshot.Add(item);
			}
		}

		public void WriteSnapshot()
		{
			var contents = new HashSet<string>();

			foreach (var item in _content._contentItems.Values)
			{
				// Don't include if the file doesn't exist.
				if (File.Exists(Path.Combine(_rootPath, item.Path)))
				{
					var relativeItemPath = Path.Combine(_rootPath, Path.GetDirectoryName(item.Path));

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
							contents.Add(GetFileId(file));
						}
					}

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
			File.WriteAllLines(_tempSnapshotPath, contents);

		}

	}
}
