using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data
{
    public class Common
    {
        public async Task<List<T>> DeserializeBsonarrayToEntity<T>(string data) where T : class
        {
            try
            {
                var t = Task<List<T>>.Factory.StartNew(() =>
                {
                    BsonArray array = BsonSerializer.Deserialize<BsonArray>(data);
                    if (!array.IsBsonArray)
                        return null;

                    List<T> lst = new List<T>();
                    for (var i = 0; i < array.Count; i++)
                    {
                        try
                        {

                            var tt = array[i].ToJson();

                            lst.Add(JsonConvert.DeserializeObject<T>(array[i].ToJson()));
                            //var dynamic = BsonSerializer.Deserialize<dynamic>(array[i].AsBsonDocument);
                            //string str = JsonConvert.SerializeObject(dynamic);
                            //var json = JsonConvert.DeserializeObject(str).ToString();
                            //lst.Add(JsonConvert.DeserializeObject<T>(json));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    return lst;
                });
                return await t;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void PostAsync(string url, string jsonContent)
        {
            try
            {
                var client = new HttpClient();
                client.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            }
            catch (Exception ex) { }
        }

        public async Task<JsonCommand<BsonDocument>> formatCommandProcedure(string proceduce, Dictionary<string, object> paras)
        {
            var t = Task<JsonCommand<BsonDocument>>.Factory.StartNew(() =>
            {
                var q = "";
                if (paras != null && paras.Count > 0)
                    foreach (var para in paras)
                    {
                        q += "'" + para.Value + "',";
                    }
                return new JsonCommand<BsonDocument>("{ eval: \"" + proceduce + "(" + (q == "" ? q : q.Substring(0, q.Length - 1)) + ")\"}");
            });
            return await t;
        }

        public string removeSpecialString(string str)
        {
            return Regex.Replace(str, "[^0-9a-zA-Z]+", "");
        }

        public async Task<T> HttpPostAsync<T>(string url, dynamic value)
        {
            using (var client = new HttpClient())
            {
                var data = JsonConvert.SerializeObject(value).ToString();
                var response = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
            }
        }

        public async Task<T> HtmlPostBytesAsync<T>(string actionUrl, string nameFile, byte[] paramFileBytes)
        {
            HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(bytesContent, nameFile, nameFile);
                var response = await client.PostAsync(actionUrl, formData);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
            }
        }

        public byte[] ConvertToBytes(IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

}
