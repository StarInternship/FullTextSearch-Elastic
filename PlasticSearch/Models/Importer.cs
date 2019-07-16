using PlasticSearch.Controllers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlasticSearch.Models
{
    class Importer
    {
        private readonly string filesPath = @"C:\test_files";

        public void ReadFiles()
        {
            ReadDirectory(filesPath);
        }

        public void ReadFile(string path)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                SearchController.Instance.addFile(path, text);
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
    }

}