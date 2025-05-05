using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace XwaToolsLauncher
{
    public class Tool
    {
        public Tool(string exe)
        {
            string path = Path.GetFullPath(exe);

            Name = Path.GetFileNameWithoutExtension(path);
            Category = Path.GetFileName(Path.GetDirectoryName(path));
            ExePath = path;
            VersionInfo = FileVersionInfo.GetVersionInfo(path);
            ToolIcon = Interop.GetLargeIcon(path);
        }

        public string Name { get; set; }

        public string Category { get; set; }

        public string ExePath { get; set; }

        public FileVersionInfo VersionInfo { get; set; }

        public ImageSource ToolIcon { get; set; }

        public static List<Tool> GetToolsList(string root)
        {
            var tools = new List<Tool>();

            foreach (string directory in Directory.EnumerateDirectories(root))
            {
                foreach (string exe in Directory.EnumerateFiles(directory, "*.exe"))
                {
                    var tool = new Tool(exe);
                    tools.Add(tool);
                }
            }

            return tools;
        }

        public void Launch()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                WorkingDirectory = Path.GetDirectoryName(ExePath)
            };

            Process.Start(startInfo);
        }
    }
}
