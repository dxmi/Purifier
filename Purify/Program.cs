using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace Purify {
    class Program {
        static List<string> extensionsList;
        static List<string> directoriesList;
        static void Main(string[] args) {
            extensionsList = new List<string>();
            directoriesList = new List<string>();
            AddItems(extensionsList, ConfigurationManager.AppSettings["extensions"]);
            AddItems(directoriesList, ConfigurationManager.AppSettings["directories"]);
            foreach (string arg in args) {
                if (arg != null) {
                    if (arg.ToLower().StartsWith("-e")) {
                        AddItems(extensionsList, arg.Substring(2));
                    } else if (arg.ToLower().StartsWith("-d")) {
                        AddItems(directoriesList, arg.Substring(2));
                    }
                }
            }
            string wkdir = Directory.GetCurrentDirectory();
            ProcessFilesRecursively(wkdir);
        }

        private static void AddItems(List<string> list, string s) {
            if (!string.IsNullOrEmpty(s))
                list.AddRange(s.Split(';'));
        }

        private static void ProcessFilesRecursively(string dir) {
            string[] files = Directory.GetFiles(dir);
            foreach (string s in files) {
                string st = Path.GetExtension(s).ToLower();
                if (st.StartsWith(".")) st = st.Substring(1);
                if (extensionsList.Contains(st))
                    DeleteFileCore(s);
            }
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string s in dirs) {
                string st = new DirectoryInfo(s).Name.ToLower();
                if (directoriesList.Contains(st))
                    DeleteDirectoryCore(s);
                else
                    ProcessFilesRecursively(s);
            }
        }

        private static void DeleteDirectoryCore(string s) {
            Console.WriteLine("Deleting directory {0}", s);
            try {
                Directory.Delete(s, true);
            } catch { }
        }

        private static void DeleteFileCore(string s) {
            Console.WriteLine("Deleting file {0}", s);
            try {
                File.Delete(s);
            } catch { }
        }
    }
}
