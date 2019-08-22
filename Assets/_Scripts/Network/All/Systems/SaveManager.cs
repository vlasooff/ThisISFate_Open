using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Community.Other
{
    public static class SaveManager
    {
        public static void CreateFolder(string Folder)
        {
            Directory.CreateDirectory($"{Environment.CurrentDirectory}/{Folder}");
        }
        
        public static void SaveJSON(object obj,string fileName)
        {
            SaveText(JsonUtility.ToJson(obj),fileName);
        }
        public  static T LoadJSON<T>(string fileName)
        {
             return  JsonUtility.FromJson<T>(LoadText(fileName));
        }
        private static readonly XmlWriterSettings XML_WRITER_SETTINGS = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            Encoding = new UTF8Encoding()
        };
        private static readonly XmlSerializerNamespaces XML_SERIALIZER_NAMESPACES = new XmlSerializerNamespaces(new XmlQualifiedName[]
          {
            XmlQualifiedName.Empty
          });
        public static void SaveText(string data, string nameFile)
        {
            Debug.Log("Save Data " + data);
           
            string patch = $"{Environment.CurrentDirectory}/{nameFile}";
     
            File.WriteAllText(patch, data);

        }
        public static string LoadText(  string nameFile)
        {
            string patch = $"{Environment.CurrentDirectory}/{nameFile}";
            Debug.Log("Patch " + patch);
            if (File.Exists(patch))
            {
                return File.ReadAllText(patch);
            }
            Debug.Log("Loading problem " + nameFile);
            return null;
        }
        public static void Save(NetDataWriter data,string nameFile)
        {
      
            string patch = $"{Environment.CurrentDirectory}/{nameFile}";
            Debug.Log("Patch " + patch);
            File.WriteAllBytes(patch, data.Data);
             
        }
        public static NetDataReader Load(string nameFile)
        {
            string patch = $"{Environment.CurrentDirectory}/{nameFile}"; 
            Debug.Log("Patch " + patch);
            if (File.Exists(patch))
            { 
                return  new NetDataReader(File.ReadAllBytes(patch), 0); 
            } 
            Debug.Log("Loading problem " + nameFile);
            return null;
        }
        public static T deserializeXML<T>(string path, bool useCloud, bool usePath)
        {
            T result = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            if (useCloud)
            {
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    result = (T)((object)xmlSerializer.Deserialize(memoryStream));
                }
                finally
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
                return result;
            }
            if (usePath)
            {
               
            }
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            if (!File.Exists(path))
            {
                Debug.Log("Failed to find file at: " + path);
                return result;
            }
            StreamReader streamReader = new StreamReader(path);
            try
            {
                result = (T)((object)xmlSerializer.Deserialize(streamReader));
            }
            finally
            {
                streamReader.Close();
                streamReader.Dispose();
            }
            return result;
        }
        public static void serializeXML<T>(string path, bool useCloud, bool usePath, T instance)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            if (useCloud)
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlWriter xmlWriter = XmlWriter.Create(memoryStream, XML_WRITER_SETTINGS);
                try
                {
                    xmlSerializer.Serialize(xmlWriter, instance, XML_SERIALIZER_NAMESPACES);
                  //  ReadWrite.cloudFileWrite(path, memoryStream.GetBuffer(), (int)memoryStream.Length);
                }
                finally
                {
                    xmlWriter.Close();
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }
            else
            {
                if (usePath)
                { 
                }
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                StreamWriter streamWriter = new StreamWriter(path);
                try
                {
                    xmlSerializer.Serialize(streamWriter, instance, XML_SERIALIZER_NAMESPACES);
                }
                finally
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }
    }
}
