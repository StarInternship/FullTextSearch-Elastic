using PlasticSearch.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PlasticSearch.Models
{
    class Importer
    {
        private readonly string filesPath = @"C:\test_files";
        public  List<Task> readers { get; } = new List<Task>();
        public void ReadFiles()
        {
            ReadDirectory(filesPath);

        }

        public void ReadFile(string path)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                SearchController.Instance.AddFile(path, text);
            }
        }

        public void ReadDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Task reader = new Task(() =>
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
                });
                reader.Start(); 

                readers.Add(reader);
            }
        }
    }

}