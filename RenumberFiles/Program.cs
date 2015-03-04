using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenumberFiles
{
    class Program
    {
        private static string[] mediaExtensions = 
        {
            ".png", ".jpg", ".jpeg", ".bmp", ".gif", 
            ".wav", ".mid", ".midi", ".wma", ".mp3", ".ogg", ".rma", 
            ".avi", ".mp4", ".divx", ".wmv", ".mkv"
        };

        private static bool IsMediaFileExtension(string path)
        {
            return mediaExtensions.Contains(path.ToLowerInvariant());
        }

        static void Main(string[] args)
        {
            var targetFilesExtension = string.Empty;
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

            if (args.Length == 1)
            {
                Console.WriteLine("No file extension to search files to renumber provided");
                return;
            }

            targetFilesExtension = args[1];
            if (targetFilesExtension[0] != '.')
            {
                targetFilesExtension = "." + targetFilesExtension;
            }

            if (!IsMediaFileExtension(targetFilesExtension))
            {
                Console.WriteLine("The file extension provided is not media file extension");
                return;
            }

            RenumberFiles(parentFolderPath, "*" + targetFilesExtension);
        }

        private static void RenumberFiles(string parentDirectoryPath, string searchPattern)
        {
            var allDirectories = GetSubDirectories(parentDirectoryPath);
            if (allDirectories.Count == 0)
            {
                allDirectories.Add(parentDirectoryPath);
            }

            int counter = 0;
            foreach (var directory in allDirectories)
            {
                var files = Directory.GetFiles(directory, searchPattern);
                var sortedFiles = files.OrderBy(f => GetTrackNumber(f)).ToList();
                foreach (var file in sortedFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var newFileName = string.Format("{0:D3} {1}", counter, fileName);
                    File.Move(file, Path.Combine(directory, newFileName));
                    counter++;
                }
            }
        }

        private static IList<string> GetSubDirectories(string parentDirectoryPath)
        {
            var directories = Directory.GetDirectories(parentDirectoryPath).ToList();
            for (int i = 0; i < directories.Count; i++)
            {
                var currentDirrectory = directories[i];
                var subDirectories = GetSubDirectories(currentDirrectory);
                if (subDirectories.Count == 0)
                {
                    continue;
                }

                directories.RemoveAt(i);
                directories.InsertRange(i, subDirectories);
            }

            return directories;
        }

        private static uint GetTrackNumber(string filePath)
        {
            using (var tagFile = TagLib.File.Create(filePath))
            {
                return tagFile.Tag.Track;
            }
        }
    }
}
