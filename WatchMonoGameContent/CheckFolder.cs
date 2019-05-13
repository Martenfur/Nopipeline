using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchMonoGameContent {
    public static class CheckFolder {
        public static void Check(string path) {
            if (File.Exists(path)) {
                // This path is a file
                ProcessFile(path);
            } else if (Directory.Exists(path)) {
                // This path is a directory
                ProcessDirectory(path);
            } else {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
        }

        public static void ProcessDirectory(string folder) {
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
        public static void ProcessFile(string file) {
            string ext = Path.GetExtension(file);
            if (ext.ToLower() == ".spritegroup") {
                new Spritegroup(file);
            }
            if (ext.ToLower() == ".tmx") {
                new Tmx(file);
            }
        }
    }
}
