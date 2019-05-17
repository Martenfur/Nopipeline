using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

		static JObject ReadConfigFile(string fileName) {
			string configText = File.ReadAllText(fileName, Encoding.UTF8);
			JObject res = JObject.Parse(File.ReadAllText(fileName));
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
			Console.WriteLine(conf["path"]);
			var content = new MGCB(conf["path"].ToString());
			content.Check();
			content.Save();
			//foreach (var folder in conf) {
			//	CheckFolder.Check(rootName + folder);
			//}


			Console.ReadKey();
		}
	}
}
