using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace XwaToolsDownloader
{
    class Program
    {
        const string ArchivesDirectory = "_archives";
        const string ToolsListFileName = "xwa_tools_list.txt";
        const string ToolsListDownloadUrl = @"https://raw.githubusercontent.com/JeremyAnsel/XwaToolsDownloader/master/XwaToolsDownloader/xwa_tools_list.txt";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("XwaToolsDownloader");
                Console.WriteLine();

                DownloadToolsList();

                if (!File.Exists(ToolsListFileName))
                {
                    throw new FileNotFoundException(null, ToolsListFileName);
                }

                CreateArchivesDirectory();
                DownloadTools();
                ExtractTools();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }

        static void DownloadToolsList()
        {
            Console.WriteLine("Download Tools List");

            if (File.Exists(ToolsListFileName))
            {
                File.Delete(ToolsListFileName);
            }

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client.DownloadFile(ToolsListDownloadUrl, ToolsListFileName);
            }

            Console.WriteLine();
        }

        static void CreateArchivesDirectory()
        {
            Console.WriteLine("Create Archives Directory");

            if (Directory.Exists(ArchivesDirectory))
            {
                Directory.Delete(ArchivesDirectory, true);
            }

            Directory.CreateDirectory(ArchivesDirectory);
            Console.WriteLine();
        }

        static void DownloadTools()
        {
            Console.WriteLine("Download Tools");

            string[] toolsList = File.ReadAllLines(ToolsListFileName);

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                foreach (string fileName in toolsList)
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        continue;
                    }

                    if (!Path.HasExtension(fileName) || Path.GetExtension(fileName).ToLowerInvariant() != ".zip")
                    {
                        continue;
                    }

                    string name = Path.GetFileName(fileName);
                    string path = Path.Combine(ArchivesDirectory, name);

                    Console.WriteLine(name);
                    client.DownloadFile(fileName, path);
                    UpdateZipLastWriteTime(path);
                }
            }

            Console.WriteLine();
        }

        static void ExtractTools()
        {
            Console.WriteLine("Extract Tools");

            foreach (string fileName in Directory.EnumerateFiles(ArchivesDirectory, "*.zip"))
            {
                string name = GetFileNameWithoutVersion(fileName);

                if (Directory.Exists(name))
                {
                    Console.WriteLine($"{name} already exists. Please remove it to install the latest version.");
                    continue;
                }

                Console.WriteLine(name);
                ZipFile.ExtractToDirectory(fileName, name);
                Directory.SetLastWriteTimeUtc(name, File.GetLastWriteTimeUtc(fileName));
            }

            Console.WriteLine();
        }

        static string GetFileNameWithoutVersion(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);

            int index = name.LastIndexOf('-');

            if (index != -1)
            {
                name = name.Substring(0, index);
            }

            return name;
        }

        static void UpdateZipLastWriteTime(string path)
        {
            DateTimeOffset date;

            using (var archiveFile = File.OpenRead(path))
            using (var archive = new ZipArchive(archiveFile, ZipArchiveMode.Read))
            {
                date = archive.Entries.Max(t => t.LastWriteTime);
            }

            File.SetLastWriteTimeUtc(path, date.UtcDateTime);
        }
    }
}
