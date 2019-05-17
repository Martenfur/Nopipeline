using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoPipeline {
	class Program {
		static void Help() {
			Console.WriteLine("NoPipeline utility");
			Console.WriteLine("Usage:");
			Console.WriteLine("    NoPipeline NoPipeline.json");
			Console.WriteLine("Example:");
			Console.WriteLine("    NoPipeline $(ProjectDir)\\NoPipeline.json");
		}

		static Dictionary<string, string> ReadConfigFile(string fileName) {
			Dictionary<string, string> res = new Dictionary<string, string>();
			try {
				string configText = File.ReadAllText(fileName, Encoding.UTF8);
				res = JsonConvert.DeserializeObject<Dictionary<string, string>>(configText);
			} catch {
				Console.WriteLine("File Not Found");
				Console.WriteLine(Directory.GetCurrentDirectory());
				Help();
			}
			return res;
		}

		static void Main(string[] args) {
			// check and parce parameters
			if (args.Length != 1) {
				Help();
				return;
			}

			// path to config file
			var configName = args[0];

			if (!File.Exists(configName)) {
				Console.WriteLine("Error: Configuration file not found.");
				Console.WriteLine(Directory.GetCurrentDirectory());
				Help();
				return;
			}

			var conf = ReadConfigFile(configName);
			//foreach (var folder in conf) {
			//	CheckFolder.Check(rootName + folder);
			//}


			Console.ReadKey();
		}
	}
}
