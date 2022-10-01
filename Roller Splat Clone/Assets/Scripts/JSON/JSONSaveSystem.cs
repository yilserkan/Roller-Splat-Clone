using System.Collections.Generic;
using System.IO;
using System.Linq;
using LevelSystem;
using UnityEngine;
using Utils;

namespace Json
{
    public static class JSONSaveSystem
    {
        public static string filename = "Levels.json";
        public static string resourcesFilename = "Levels";

        public static void SaveToJSON<T>(T saveData, bool prettyPrint)
        {
            List<T> dataInJson = ReadFromJson<T>();
            string content = JsonHelper.ToJson(saveData, prettyPrint, dataInJson);
            WriteFile(GetPath(), content);
        }

        public static List<T> ReadFromJson<T>()
        {
            string content = ReadFile(GetPath());
            if (string.IsNullOrEmpty(content) || content == "")
            {
                return new List<T>();
            }
            List<T> res = JsonHelper.FromJson<T>(content).ToList();

            return res;
        }
        
        private static string GetPath()
        {
            // return Path.Combine(Application.persistentDataPath, filename);
            return Application.dataPath + "/Resources/" + filename;
        }

        private static void WriteFile(string path, string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
            fileStream.Close();
        }

        public static List<Level> ReadLevels()
        {
            var levels = Resources.Load<TextAsset>(resourcesFilename);
            List<Level> levelsList = JsonHelper.FromJson<Level>(levels.ToString()).ToList();
            return levelsList;
        }

        public static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    reader.Close();
                    return content;
                    
                }
            }
            return "";
        }
    }
}