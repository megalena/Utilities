using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFileTitles
{
    class Program
    {
        /// <summary>
        /// This utility deletes all files' Tiltle tags in the selected folder and all its subfolders
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No folder path provided.");
                return;
            }

            var parentFolderPath = args[0];
            if (!Directory.Exists(parentFolderPath))
            {
                Console.WriteLine("The folder path is not valid.");
                return;
            }

            CleanFileTitles(parentFolderPath);
            Console.WriteLine("Titles were cleaned successfully.");
        }

        private static void CleanFileTitles(string parentFolderPath)
        {
            foreach (var file in Directory.GetFiles(parentFolderPath))
            {
                using (var tagFile = TagLib.File.Create(file))
                {
                    tagFile.Tag.Title = String.Empty;
                    tagFile.Save();
                }
            }

            foreach (var folder in Directory.GetDirectories(parentFolderPath))
            {
                CleanFileTitles(folder);
            }
        }
    }
}
