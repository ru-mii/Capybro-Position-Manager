using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumTrainer
{
    // used to save and read data
    public static class Saves
    {
        // default names
        public static string developer = "rumii";
        public static string software = "Capybro Trainer";
        public static string extension = ".txt";

        // default saves locations
        public static string savesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), developer, software);

        // save data
        public static void Save(string folder, string name, string data)
        {
            // we'll be saving in below path
            string folderPath = Path.Combine(savesPath, folder);
            string filePath = Path.Combine(savesPath, folder, name + extension);

            // check if folder path exists, if not make one
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // write data
            File.WriteAllText(filePath, data);
        }

        // read data
        public static string Read(string folder, string name)
        {
            // file path
            string filePath = Path.Combine(savesPath, folder, name + extension);

            // read data or return empty if file not found
            if (File.Exists(filePath)) return File.ReadAllText(filePath);
            else return "";
        }
    }
}
