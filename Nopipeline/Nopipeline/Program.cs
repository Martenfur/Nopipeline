using System;
using System.IO;

namespace Nopipeline
{

	public class Program
	{
		public const string Version = "2.0.1.0"; // TODO: Replace
		
		public static void Main(string[] args)
		{
			Console.WriteLine("NoPipeline v" + Version);

			// Print help information if parameter was not provided.
			if (args.Length != 1)
			{
				PrintHelp();
				return;
			}
			
			var configPath = Path.Combine(Environment.CurrentDirectory, args[0].Replace("\\", "/"));

			Run(configPath);
		}



		private static void Run(string configPath)
		{
			
			// Read config file name from the input parameter.

			string MGCBConfigPath, NPLConfigPath;

			if (configPath.EndsWith(".mgcb"))
			{
				MGCBConfigPath = configPath;
				NPLConfigPath = Path.ChangeExtension(configPath, ".npl");
			}
			else
			{
				NPLConfigPath = configPath;
				MGCBConfigPath = Path.ChangeExtension(configPath, ".mgcb");
			}

			// Check if configuration file exists.
			if (!File.Exists(NPLConfigPath) || !File.Exists(NPLConfigPath))
			{
				Console.WriteLine(NPLConfigPath + " not found!");
				PrintHelp();
				return;
			}

			var content = new Content();
			
			// Create MGCB object to read mgcb file.
			var MGCBReader = new MGCBConfigReader();
			MGCBReader.Read(content, MGCBConfigPath);

			Console.WriteLine();
			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			// Create ContentProcessor object to read config file and update content
			// content will be overwrited from config file.
			var NPLReader = new NPLConfigReader();
			NPLReader.Read(content, NPLConfigPath);

			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			// Check all rules in content object and update timestamp of files if required.
			content.CheckIntegrity(Path.GetDirectoryName(MGCBConfigPath));

			// Saving MGCB file.

			Console.WriteLine();
			Console.WriteLine("-------------------------------------");
			Console.WriteLine();

			Console.WriteLine("Saving new config as " + MGCBConfigPath);
			Console.WriteLine();

			File.WriteAllText(MGCBConfigPath, content.Build());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done! \\^u^/");
			Console.ForegroundColor = ConsoleColor.Gray;

		}

		/// <summary>
		/// Prints help message.
		/// </summary>
		private static void PrintHelp()
		{
			Console.WriteLine("Run with path to .mgcb or .npl config as an argument:");
			Console.WriteLine("    NoPipeline.exe Content/Content.mgcb");
			Console.WriteLine("or");
			Console.WriteLine("    NoPipeline.exe Content/Content.npl");
		}

	}
}
