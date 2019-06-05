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
		/* 
		 * Help print utility help on the console
		 */
		static void Help()
		{
			Console.WriteLine("NoPipeline utility");
			Console.WriteLine("Usage:");
			Console.WriteLine("    NoPipeline NoPipeline.json");
			Console.WriteLine("Example:");
			Console.WriteLine("    NoPipeline $(ProjectDir)\\NoPipeline.json");
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

		/*
		 * Main - start point
		 */
		static void Main(string[] args)
		{
			// print help information if parameter was not provided
			if(args.Length != 1)
			{
				Help();
				return;
			}

			// Read config file name from the input parameter
			var configName = args[0];

			// check if configuration file exists
			if(!File.Exists(configName))
			{
				Console.WriteLine("Error: Configuration file not found.");
				Console.WriteLine(Directory.GetCurrentDirectory());
				Help();
				return;
			}

			// read config file
			JObject conf = ReadConfigFile(configName);

			// print config file name to the console
			Console.WriteLine(conf["path"]);

			// Create MGCB object to read mgcb file
			var content = new MGCB(conf);

			// Create ContentProcessor object to read config file and update content
			// content will be overwrited from config file
			var cp = new ContentProcessor(conf, content);

			// Check all rules in content object and update timestamp of files if required.
			content.ContentCheck();

			// Save MGCB file
			content.Save();


			//Console.ReadKey();
		}
	}
}
