using Newtonsoft.Json;
using PTLive.Fetcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.PoeJSONFileManager
{
    public static class PoeFilesControl
    {
        public static string RootDir = "PoeDatabase";
        private static string SerializeToJson(LeagueInfo li)
        {
            return JsonConvert.SerializeObject(li);
        }

        private static LeagueInfo DesiarlizeData(string jsonData)
        {
            return JsonConvert.DeserializeObject<LeagueInfo>(jsonData);
        }

        public static void SaveToFile(LeagueInfo li)
        {
            string path = String.Format("{0}/{1}/{2}.json", RootDir, DateTime.Now.ToString("MM.dd"), DateTime.Now.ToString("HH.mm.ss"));
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            System.IO.File.WriteAllText(file.FullName, SerializeToJson(li));
        }

        public static LeagueInfo GetLatestFile()
        {
            var directories = Directory.GetDirectories(RootDir);
            var directory = new DirectoryInfo(directories.Last());
            var myFile = (from f in directory.GetFiles()
                          orderby f.LastWriteTime descending
                          select f).First();
            string text = System.IO.File.ReadAllText(myFile.FullName);
            return DesiarlizeData(text);
        }

        public static LeagueInfo GetFileByIndex(int index)
        {
            List<FileInfo> files = new List<FileInfo>();

            var directories = Directory.GetDirectories(RootDir);
            foreach (var dir in directories)
            {
                files.AddRange(new DirectoryInfo(dir).GetFiles());
            }

            files = files.OrderByDescending(x => x.LastWriteTime).ToList();
            string text = System.IO.File.ReadAllText(files[index].FullName);
            return DesiarlizeData(text);
        }

        public static int TotalFilesAvailable()
        {
            var listOfFiles = Directory.GetFiles(RootDir, "*", SearchOption.AllDirectories);
            return listOfFiles.Length;
        }
    }
}
