using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NoPipeline
{
	/* 
	 * The main class
	 * Run point
	 */
	class Program
	{
		public const string Version = "1.0.0";

		/* 
		 * Help print utility help on the console
		 */
		static void Help()
		{
			Console.WriteLine("Run with path to .mgcb or .npl config as an argument:");
			Console.WriteLine("    NoPipeline.exe Content/Content.mgcb");
			Console.WriteLine("or");
			Console.WriteLine("    NoPipeline.exe Content/Content.npl");
		}

		/*
		 * ReadConfigFile reading Json configuration file
		 * Parameter:
		 *   fileName - config file name
		 * Return:
		 *   JObject contains all configuration
		 */
		static JObject ReadConfigFile(string fileName)
		{
			var configText = File.ReadAllText(fileName, Encoding.UTF8);
			return JObject.Parse(configText);
		}
		
		static void Main(string[] args)
		{
			Console.WriteLine("NoPipeline v" + Version);

			// print help information if parameter was not provided
			if(args.Length != 1)
			{
				Help();
				return;
			}

			// Read config file name from the input parameter

			string MGCBConfigPath, NPLConfigPath;

			var configPath = args[0].Replace("\\", "/");
			if (configPath.EndsWith(".mgcb"))
			{
				MGCBConfigPath = configPath;
				NPLConfigPath = Path.GetDirectoryName(configPath) 
					+ "/"
					+ Path.GetFileNameWithoutExtension(configPath) 
					+ ".npl";
			}
			else
			{
				NPLConfigPath = configPath;
				MGCBConfigPath = Path.GetDirectoryName(configPath) 
					+ "/"
					+ Path.GetFileNameWithoutExtension(configPath) 
					+ ".mgcb";
			}

			// check if configuration file exists
			if (!File.Exists(NPLConfigPath) || !File.Exists(NPLConfigPath))
			{
				Console.WriteLine(NPLConfigPath + " not found!");
				Help();
				return;
			}

			
			JObject conf = ReadConfigFile(NPLConfigPath);
			
			var rootPath = Path.GetDirectoryName(configPath).Replace("\\", "/") + "/";

			// Create MGCB object to read mgcb file
			var content = new MGCB(conf, MGCBConfigPath);

			// Create ContentProcessor object to read config file and update content
			// content will be overwrited from config file
			var cp = new ContentProcessor(conf, content, rootPath);

			// Check all rules in content object and update timestamp of files if required.
			content.ContentCheck();

			// Save MGCB file
			content.Save();


			//Console.ReadKey();
		}
	}
}
