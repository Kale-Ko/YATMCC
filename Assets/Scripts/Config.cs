/**
    MIT License
    Copyright (c) 2021 Kale Ko
    See https://kaleko.ga/license.txt
*/

using System.IO;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static string fileName = "config.txt";

    public static float sensitivity = 8f;

    public static int distance = 2;

    public static void Setup()
    {
        if (!File.Exists(Application.persistentDataPath + "/" + fileName)) Save();

        Load();
    }

    public static void Save()
    {
        string data = "";

        data += "sensitivity=" + sensitivity + "\n";
        data += "distance=" + distance + "\n";

        File.WriteAllText(Application.persistentDataPath + "/" + fileName, data);
    }

    public static void Load()
    {
        string data = File.ReadAllText(Application.persistentDataPath + "/" + fileName);

        string[] configs = data.Split('\n');

        foreach (var config in configs)
        {
            if (!config.Contains("="))
            {
                if (config != "" && config != " ") Debug.LogWarning("Invalid line in config \"" + config + "\"");

                continue;
            }

            string key = config.Split('=')[0];
            string value = config.Split('=')[1];

            if (key == "sensitivity") sensitivity = float.Parse(value);
            else if (key == "distance") distance = int.Parse(value);
            else Debug.LogWarning(key + " is an unknown config key");
        }
    }
}