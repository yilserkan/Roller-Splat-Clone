using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class JSONSaveSystem
{
    public static string filename = "Levels.json";

    public static void SaveToJSON<T>(List<T> saveData, bool prettyPrint)
    {
        List<T> dataInJson = ReadFromJson<T>();
        string content = JsonHelper.ToJson(saveData, prettyPrint, dataInJson);
        WriteFile(GetPath(), content);
    }

    public static List<T> ReadFromJson<T>()
    {
        string content = ReadFile(GetPath());
        if (string.IsNullOrEmpty(content)|| content == "")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;
    }

    private static string GetPath()
    {
        return Application.dataPath + "/JSON Files/" + filename;
    }

    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);            
        }
    }

    public static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        return "";
    }
}