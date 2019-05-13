using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchMonoGameContent {

    class Program {
        static void Help() {
            Console.WriteLine("WatchMonoGameContent utility");
            Console.WriteLine("Usage:");
            Console.WriteLine("    WatchMonoGameContent <Project Folder>");
            Console.WriteLine("Example:");
            Console.WriteLine("    WatchMonoGameContent $(ProjectDir)");
        }

        static List<string> ReadConfigFile(string fileName) {
            List<string> res = new List<string>();
            try {
                string configText = File.ReadAllText(fileName, Encoding.UTF8);
                res = JsonConvert.DeserializeObject<List<string>>(configText);
            } catch {
                Console.WriteLine("File Not Found");
                Console.WriteLine(Directory.GetCurrentDirectory());
                Help();
            }
            return res;
        }

        static void Main(string[] args) {
            if (args.Length != 1) {
                Help();
            } else {
                var rootName = args[0];
                if (!rootName.EndsWith("\\") && (!rootName.EndsWith("/"))) {
                    rootName += "\\";
                }
                var fileName = rootName + "WatchMonoGameContent.json";
                if (!File.Exists(fileName)) {

                    Console.WriteLine("Error: Configuration file not found.");
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    Help();
                } else {
                    var conf = ReadConfigFile(fileName);
                    foreach (var folder in conf) {
                        CheckFolder.Check(rootName + folder);
                    }
                }
            }


            //Console.ReadKey();
        }


    }


}

