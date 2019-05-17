using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;
using System.IO;

namespace NoPipeline {
	public class Item {
		public StringBuilder Param { get; set; }
		public string Name { get; set; }

		public Item() {
			Param = new StringBuilder();
		}

		public override string ToString() {
			return $"#begin {Name}" + System.Environment.NewLine + Param.ToString();
		}
	}

	public class MGCB {
		public StringBuilder Header { get; set; }
		public Dictionary<string, Item> Items { get; set; }
		public string CfgName { get; set; }
		public string CfgPath { get; set; }

		public MGCB(string name) {   // Read mgcb config file
			if (!File.Exists(name)) {
				throw new Exception($"{name} file not found!");
			}
			CfgName = name;
			CfgPath = Path.GetDirectoryName(name);
			string line;
			bool isItemSection = false;
			Item it = new Item();
			Header = new StringBuilder();
			Items = new Dictionary<string, Item>();

			using (StreamReader file = new StreamReader(name)) {
				while ((line = file.ReadLine()) != null) {
					if (!isItemSection) {
						if (line.StartsWith("#begin")) {
							isItemSection = true;   // found first begin - stop collecting header
						} else {
							Header.AppendLine(line);
						}
					}
					if (isItemSection) {
						if (line.StartsWith("#begin")) {
							it = new Item {
								Name = line.Substring(7)
							};
							Items.Add(it.Name, it); // add to the dictionary
						} else {
							it.Param.AppendLine(line);
						}
					}
				}
			}

		}

		public void Add(Item it) {
			Items.Add(it.Name, it);
		}

		public void Check() {   // check all items exist
			var ItemsCheck = new Dictionary<string, Item>();
			foreach (Item it in Items.Values) {
				if (File.Exists(CfgPath + "/" + it.Name)) {  // not exists - remove from Items
					ItemsCheck.Add(it.Name, it);
				}
			}
			Items = ItemsCheck;
		}

		public void Save() { // save config file
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(CfgName + ".new")) {
				// header
				file.Write(Header.ToString());
				// items
				foreach (Item it in Items.Values) {
					file.Write(it.ToString());
				}

			}

		}

	}
}
