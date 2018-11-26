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
                CreateArchivesDirectoryIfNotExist();
                DownloadTools();
                ExtractTools();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void DownloadToolsList()
        {
            if (File.Exists(ToolsListFileName))
            {
                return;
            }

            Console.WriteLine("Download Tools List");

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client.DownloadFile(ToolsListDownloadUrl, ToolsListFileName);
            }

            Console.WriteLine();
        }

        static void CreateArchivesDirectoryIfNotExist()
        {
            if (Directory.Exists(ArchivesDirectory))
            {
                return;
            }

            Console.WriteLine("Create Archives Directory");
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

                    if (File.Exists(path))
                    {
                        continue;
                    }

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
                string name = Path.GetFileNameWithoutExtension(fileName);

                if (Directory.Exists(name))
                {
                    continue;
                }

                Console.WriteLine(name);
                ZipFile.ExtractToDirectory(fileName, name);
                Directory.SetLastWriteTimeUtc(name, File.GetLastWriteTimeUtc(fileName));
            }

            Console.WriteLine();
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
