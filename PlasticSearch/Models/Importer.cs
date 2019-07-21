using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlasticSearch.Models
{
    class Importer
    {
        private readonly string filesPath = @"C:\test_files";
        private static readonly string logPath = @"C:\log\log.txt";

        public List<Task> readers { get; } = new List<Task>();
        public void ReadFiles()
        {
            ReadDirectory(filesPath);
        }

        public void ReadFile(string path)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                DatabaseController.Instance.AddFile(path, text);
            }
        }

        public void ReadDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string filePath in files)
                {
                    ReadFile(filePath);
                }
                string[] directories = Directory.GetDirectories(path);
                foreach (string filePath in directories)
                {
                    ReadDirectory(filePath);
                }
            }
        }

        public static void WriteLog(string log)
        {
            File.AppendAllText(logPath, log + "\n");
        }

        public static void CreateLog()
        {
            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }
            File.WriteAllText(logPath, "");
        }
    }

}