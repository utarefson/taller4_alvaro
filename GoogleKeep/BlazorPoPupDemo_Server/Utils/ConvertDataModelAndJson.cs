using BlazorPoPupDemo_Server.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace BlazorPoPupDemo_Server.Utils
{
    public class ConvertDataModelAndJson
    {
        public static string ConvertObjectToJson(Models.Data data) 
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(data);
                return jsonString;
            }
            catch
            {
                return null;
            }
            
        }
        public static Models.Data ConvertJsonToObject(string json) 
        {
            try
            {
                Models.Data data =
                JsonSerializer.Deserialize<Models.Data>(json);
                return data;
            }
            catch
            {
                return null;
            }
            
        }
        public static byte[] ObjectToByteArray(Models.Data obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
