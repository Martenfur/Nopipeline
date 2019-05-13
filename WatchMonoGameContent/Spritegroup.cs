using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WatchMonoGameContent {
    public class Spritegroup {
        public DateTime Modification { get; set; }
        public string Name { get; set; }
        public string CurrentFolder { get; set; }
        public bool IsUpdRequired { get; set; }

        public Spritegroup(string fileName) {
            this.Modification = File.GetLastWriteTime(fileName);
            this.Name = fileName;
            //filename = Path.GetFileName(strPath);
            CurrentFolder = Path.GetDirectoryName(fileName);
            IsUpdRequired = false;

            StreamReader file = new System.IO.StreamReader(fileName);
            string line;
            Regex regex = new Regex("^\\s*\"{0,1}rootDir\":\\s+\"(.*)\"");
            while ((line = file.ReadLine()) != null) {
                Match match = regex.Match(line);
                if (match.Success) {
                    var folder = match.Groups[1].Value;
                    ProcessDirectory(CurrentFolder + folder);
                }
            }
            file.Close();
            if (this.IsUpdRequired) {
                Console.WriteLine($"Update Image file:{Name}");
                File.SetLastWriteTime(this.Name, DateTime.Now);
            }
        }

        public void ProcessDirectory(string folder) {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(folder);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subfolders = Directory.GetDirectories(folder);
            foreach (string subfolder in subfolders)
                ProcessDirectory(subfolder);
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string file) {
            Console.WriteLine("Processed file '{0}'.", file);
            var fileModification = File.GetLastWriteTime(file);
            if (fileModification > this.Modification) {
                this.IsUpdRequired = true;  // Update required
            }
        }
    }
}
