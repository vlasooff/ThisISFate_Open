using LiteNetLib.Utils;
using System;
using System.IO;
using UnityEngine;

public class SaveBehaviour
{
    protected virtual void Save(NetDataWriter data, string nameFile)
    {

        string patch = $"{Environment.CurrentDirectory}/{nameFile}";
        Debug.Log("Patch " + patch);
        File.WriteAllBytes(patch, data.Data);

    }
    protected virtual NetDataReader Load(string nameFile)
    {
        string patch = $"{Environment.CurrentDirectory}/{nameFile}";
        Debug.Log("Patch " + patch);
        if (File.Exists(patch))
        {
            return new NetDataReader(File.ReadAllBytes(patch), 0);
        }
        Debug.Log("Loading problem " + nameFile);
        return null;
    }
    protected virtual void SaveJSON(object obj, string fileName)
    {
        SaveText(JsonUtility.ToJson(obj), fileName);
    }
    protected virtual T LoadJSON<T>(string fileName)
    {
        return JsonUtility.FromJson<T>(LoadText(fileName));
    }
    protected virtual void SaveText(string data, string nameFile)
    {
        Console.WriteLine("Save Data " + data);

        string patch = $"{Environment.CurrentDirectory}/{nameFile}";

        File.WriteAllText(patch, data);

    }
    protected virtual string LoadText(string nameFile)
    {
        string patch = $"{Environment.CurrentDirectory}/{nameFile}";
        Console.WriteLine("Patch " + patch);
        if (File.Exists(patch))
        {
            return File.ReadAllText(patch);
        }
        Console.WriteLine("Loading problem " + nameFile);
        return null;
    }
}
