using System;
using System.IO;
using System.Reflection;

namespace Nopipeline
{

	public class Program
	{
		private static string _mgcbConfigPath; 
		private static string _nplConfigPath;

		private static bool _exit = false;

		public static void Main(string[] args)
		{

			// Print help information if parameter was not provided.
			if (args.Length != 1 || args[0] == "")
			{
				PrintHelp();
				return;
			}

			var configPath = Path.Combine(Environment.CurrentDirectory, args[0].Replace("\\", "/"));

			RestoreConfigs(configPath);

			if (_exit)
			{ 
				return;
			}
			Run();
		}


		private static void Run()
		{
			Console.WriteLine("Nopipeline v" + Assembly.GetAssembly(typeof(Program)).GetName().Version.ToString());

			var rootPath = Path.GetDirectoryName(_mgcbConfigPath);
			var content = new Content();
			var snapshot = new WatchSnapshot(content, rootPath);
			
			// Create MGCB object to read mgcb file.
			var MGCBReader = new MGCBConfigReader();
			MGCBReader.Read(content, _mgcbConfigPath);

			Console.WriteLine();
			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			// Create ContentProcessor object to read config file and update content
			// content will be overwrited from config file.
			var NPLReader = new NPLConfigReader();
			NPLReader.Read(content, _nplConfigPath);

			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			// Check all rules in content object and update timestamp of files if required.
			content.CheckIntegrity(snapshot, rootPath);
			snapshot.WriteSnapshot();

			// Saving MGCB file.

			Console.WriteLine();
			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			Console.WriteLine("Saving new config as " + _mgcbConfigPath);
			Console.WriteLine();

			File.WriteAllText(_mgcbConfigPath, content.Build());
	
			if (content.CreateContentList)
			{
				Console.WriteLine("Creating ContentList in " + "ContentList.cs");
				Console.WriteLine();

				File.WriteAllText("ContentList.cs", content.BuildContentList());
			}
			else
			{
				Console.WriteLine("Skipping ContentList");
			}

			Console.WriteLine("DONE. o-o");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();

		}

		private static void RestoreConfigs(string configPath)
		{
			// Read config file name from the input parameter.
			if (configPath.EndsWith(".mgcb"))
			{
				_mgcbConfigPath = configPath;
				_nplConfigPath = Path.ChangeExtension(configPath, ".npl");

				if (File.Exists(_nplConfigPath))
				{
					_exit = true;
					return;
				}
			}
			else
			{
				_nplConfigPath = configPath;
				_mgcbConfigPath = Path.ChangeExtension(configPath, ".mgcb");
			}

			// Generate empty config files if they're not present.
			GenerateConfig(_nplConfigPath);
			GenerateConfig(_mgcbConfigPath);
		}


		/// <summary>
		/// Retrieves empty npl or mgcb config from embeded resources.
		/// </summary>
		private static void GenerateConfig(string path)
		{
			if (File.Exists(path))
			{
				return;
			}

			Console.WriteLine("Resources: ");
			foreach(var s in Assembly.GetAssembly(typeof(Program)).GetManifestResourceNames())
			{
				Console.WriteLine(s);
			}
			var extension = Path.GetExtension(path);
			Console.WriteLine("Nopipeline.EmptyContent" + extension + " wat");
			var stream = Assembly.GetAssembly(typeof(Program)).GetManifestResourceStream("Nopipeline.EmptyContent" + extension);
			var reader = new StreamReader(stream);

			var configContents = reader.ReadToEnd();

			stream.Dispose();
			reader.Dispose();

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, configContents);
		}


		/// <summary>
		/// Prints help message.
		/// </summary>
		private static void PrintHelp()
		{
			Console.WriteLine("Run with path to .mgcb or .npl config as an argument:");
			Console.WriteLine("    dotnet npl.dll Content/Content.mgcb");
			Console.WriteLine("or");
			Console.WriteLine("    dotnet npl.dll Content/Content.npl");
		}

	}
}
