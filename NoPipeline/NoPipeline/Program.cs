using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NoPipeline
{
	
	class Program
	{
		public const string Version = "1.1.0.0";
		
		static void Main(string[] args)
		{
			Run(args);

			#if DEBUG
				Console.ReadKey();
			#endif
		}


		static void Run(string[] args)
		{
			Console.WriteLine("NoPipeline v" + Version);

			// Print help information if parameter was not provided.
			if (args.Length != 1)
			{
				PrintHelp();
				return;
			}

			// Read config file name from the input parameter.

			string MGCBConfigPath, NPLConfigPath;

			var configPath = Path.Combine(Environment.CurrentDirectory, args[0].Replace("\\", "/"));
			
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

			
			// Create MGCB object to read mgcb file.
			var content = new MGCB(MGCBConfigPath);

			// Create ContentProcessor object to read config file and update content
			// content will be overwrited from config file.
			var cp = new ContentProcessor(content.Content, NPLConfigPath);

			// Check all rules in content object and update timestamp of files if required.
			content.ContentCheck();

			// Save MGCB file.
			content.Save();

		}

		/// <summary>
		/// Prints help message.
		/// </summary>
		static void PrintHelp()
		{
			Console.WriteLine("Run with path to .mgcb or .npl config as an argument:");
			Console.WriteLine("    NoPipeline.exe Content/Content.mgcb");
			Console.WriteLine("or");
			Console.WriteLine("    NoPipeline.exe Content/Content.npl");
		}

	}
}
