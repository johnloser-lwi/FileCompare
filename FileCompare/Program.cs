using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileCompare
{
    internal class Program
    {
        static string path1 = "";
        static string path2 = "";
        private static string output = "";
        private static string[] availableExtension = new string[] { "wem", "bnk" };
        
        public static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--path-old")
                {
                    path1 = args[i + 1];
                    continue;
                }
                if (args[i] == "--path-new")
                {
                    path2 = args[i + 1];
                    continue;
                }

                if (args[i] == "--output")
                {
                    output = args[i + 1];
                    
                    continue;
                }

                if (args[i] == "--extensions")
                {
                    List<string> extensions = new List<string>();
                    for (int j = i + 1; j < args.Length; j++)
                    {
                        if (args[j].StartsWith("--"))
                        {
                            break;
                        }
                        else
                        {
                            extensions.Add(args[j]);
                        }
                    }
                    extensions.AddRange(availableExtension);
                    availableExtension = extensions.ToArray();
                    continue;
                }
            }

            var files1 = GetFiles(path1);
            var files2 = GetFiles(path2);
            
            var newFiles = NewFiles(files1,files2);
            var removedFiles = RemovedFiles(files1,files2);

            var filesToCompare = Array.FindAll(files2, (file) => { return !newFiles.Contains(file); });

            List<string> updateList = new List<string>();

            foreach (var file in filesToCompare)
            {
                if (!CompareFiles(file.Replace(path2, path1), file))
                {
                    updateList.Add(file);
                }
            }

            string outputInfo = "";;
            outputInfo += "\nModified:\n";
            foreach (var file in updateList)
            {
                outputInfo += file.Replace(path1, "").Replace(path2, "") + "\n";
            }
            
            outputInfo += "\nAdded:\n";
            foreach (var file in newFiles)
            {
                outputInfo += file.Replace(path1, "").Replace(path2, "") + "\n";
            }
            
            outputInfo += "\nRemoved:\n";
            foreach (var file in removedFiles)
            {
                outputInfo += file.Replace(path1, "").Replace(path2, "") + "\n";
            }
            
            Console.WriteLine(outputInfo);
            if (!string.IsNullOrEmpty(output)) File.WriteAllText(output + "/file_compare_log.txt", outputInfo);
        }

        public static string[] RemovedFiles(string[] files1, string[] files2)
        {
            List<string> result = new List<string>();
            
            foreach (var file1 in files1)
            {
                if (!files2.Contains(file1.Replace(path1, path2)))
                {
                    result.Add(file1);
                }
            }

            return result.ToArray();
        }

        public static string[] NewFiles(string[] files1, string[] files2)
        {
            List<string> result = new List<string>();
            
            foreach (var file2 in files2)
            {
                if (!files1.Contains(file2.Replace(path2,path1)))
                {
                    result.Add(file2);
                }
            }

            return result.ToArray();
        }

        public static string[] GetFiles(string path)
        {
            List<string> result = new List<string>();

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                foreach (var ext in availableExtension)
                {
                    if (file.EndsWith(ext) || ext == "all")
                    {
                        result.Add(file);
                        break;
                    }
                }
            }
            
            foreach (var dir in dirs)
            {
                result.AddRange(GetFiles(dir));
            }

            return result.ToArray();
        }


        /// <summary>
        /// Binary comparison of two files
        /// </summary>
        /// <param name="fileName1">the file to compare</param>
        /// <param name="fileName2">the other file to compare</param>
        /// <returns>a value indicateing weather the file are identical</returns>
        public static bool CompareFiles(string fileName1, string fileName2)
        {
            FileInfo info1 = new FileInfo(fileName1);
            FileInfo info2 = new FileInfo(fileName2);
            bool same = info1.Length == info2.Length;
            if (same)
            {
                using (FileStream fs1 = info1.OpenRead())
                using (FileStream fs2 = info2.OpenRead())
                using (BufferedStream bs1 = new BufferedStream(fs1))
                using (BufferedStream bs2 = new BufferedStream(fs2))
                {
                    for (long i = 0; i < info1.Length; i++)
                    {
                        if (bs1.ReadByte() != bs2.ReadByte())
                        {
                            same = false;
                            break;
                        }
                    }
                }
            }

            return same;
        }
    }
}