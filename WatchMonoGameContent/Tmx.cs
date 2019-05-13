using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WatchMonoGameContent {
    class Tmx {
        public DateTime Modification { get; set; }
        public string Name { get; set; }
        public string CurrentFolder { get; set; }
        public bool IsUpdRequired { get; set; }

        public Tmx(string fileName) {
            this.Modification = File.GetLastWriteTime(fileName);
            this.Name = fileName;
            //filename = Path.GetFileName(strPath);
            CurrentFolder = Path.GetDirectoryName(fileName);
            IsUpdRequired = false;
            if (!CurrentFolder.EndsWith("\\") && (!CurrentFolder.EndsWith("/"))) {
                CurrentFolder += "\\";
            }

            StreamReader file = new System.IO.StreamReader(fileName);
            string line;
            Regex regex = new Regex("\\s*(source|trmplate)\\s*=\\s*\"([^\"]*)\"");
            while ((line = file.ReadLine()) != null) {
                Match match = regex.Match(line);
                if (match.Success) {
                    var subFile = match.Groups[2].Value;
                    ProcessFile(CurrentFolder + subFile);
                }
            }
            file.Close();
            if (this.IsUpdRequired) {
                Console.WriteLine($"Update Image file:{Name}");
                File.SetLastWriteTime(this.Name, DateTime.Now);
            }
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string file) {
            Console.WriteLine("Processed file '{0}'.", file);

            string ext = Path.GetExtension(file);
            if (ext.ToLower() == ".tmx") {
                new Tmx(file);
                var subFileModification = File.GetLastWriteTime(file);
                if (subFileModification > this.Modification) {
                    this.IsUpdRequired = true;  // Update required
                }
            } else if (ext.ToLower() == ".tx") {
                new Tmx(file);
                var subFileModification = File.GetLastWriteTime(file);
                if (subFileModification > this.Modification) {
                    this.IsUpdRequired = true;  // Update required
                }
            } else if (ext.ToLower() == ".tsx") {
                new Tmx(file);
                var subFileModification = File.GetLastWriteTime(file);
                if (subFileModification > this.Modification) {
                    this.IsUpdRequired = true;  // Update required
                }
            } else {
                var subFileModification = File.GetLastWriteTime(file);
                if (subFileModification > this.Modification) {
                    this.IsUpdRequired = true;  // Update required
                }
            }
        }
    }
}
