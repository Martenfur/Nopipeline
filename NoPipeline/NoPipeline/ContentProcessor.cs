using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NoPipeline {
	class ContentProcessor {

		public ContentProcessor(JObject conf, MGCB content) {
			JObject contentJson = conf["content"] as JObject;
			string rootPath = conf["root"].ToString().TrimEnd('/', '\\') + "/";
			foreach (var itm in contentJson) {
				// Read section
				string sectionName = itm.Key;
				JObject section = itm.Value as JObject;

				// read item
				string path ;
				try {
					path = section["path"].ToString();
				} catch {
					Console.WriteLine($"key 'path' not exist in  {sectionName} ");
					throw new Exception($"key 'path' not exist in  {sectionName} ");
				}
				var fileName = Path.GetFileName(path);
				var filePath = Path.GetDirectoryName(path);
				string[] files = new string[] { };
				try {
					files = Directory.GetFiles($"{rootPath}{filePath}", fileName, SearchOption.TopDirectoryOnly);
				} catch {
					Console.WriteLine($"{section["path"].ToString()} not found");
				}
				foreach (var file in files) {
					var name = file.Remove(0, rootPath.Length);
					var it = new Item() { Name = name };
					foreach (var sect in section) {
						if (sect.Key != "path") {
							it.Add(sect.Key, sect.Value);
						}
					}
					content.Items.Add(it.Name, it);
				}
			}
		}
	}
}
