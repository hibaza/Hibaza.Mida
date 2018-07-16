using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Hibaza.CCP.Core.Helpers
{
    public class WebHelper
    {

        public static T ConvertJsonToClass<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T HttpGet<T>(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var json = client.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
        public async static Task<T> HttpGetAsync<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public async static Task<T> HttpGetAsync01<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

               return JsonConvert.DeserializeObject<T>(result);
            }
        }

       
        public async static Task<string> HttpGetAsyncSting(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                return await client.GetStringAsync(url);
            }
        }

        public async static Task<byte[]> HttpGetAsyncByte(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                return await client.GetByteArrayAsync(url);
            }
        }

        public async static Task<T> HttpPostAsync<T>(string url, dynamic value)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var data = JsonConvert.SerializeObject(value).ToString();
                var response = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
            }
        }

        public  async static Task<T> sendHttpUploadRequest<T>(string endpoint, byte[] fileByte ,string fileName, Dictionary<string, string> param, Dictionary<string, string> header)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            if (param != null)
            {
                foreach (KeyValuePair<string, string> entry in param)
                {
                    form.Add(new StringContent(entry.Value), entry.Key);
                }
            }
            form.Add(new ByteArrayContent(fileByte), "file", Path.GetFileName(fileName));

            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, form);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
        }


        public async static Task<bool> HttpPostAsync(string url, dynamic value)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var data = JsonConvert.SerializeObject(value).ToString();
                var result = client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
                if (result.ToString().IndexOf("200")>=0)
                    return true;
                return false;
            }
        }

        public async static Task<bool> HttpDeleteAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var result = await client.DeleteAsync(url);
            }
            return true;
        }

        public static dynamic HttpPost(string url, dynamic value)
        {
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var data = JsonConvert.SerializeObject(value).ToString();
                result = client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
            }
            return result;
        }


        public static dynamic JsonDeserializeDynamic(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var json = client.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject(json);
            }
        }
        public static string JsonDeserializeString(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var json = client.GetStringAsync(url).Result;
                return json;
            }
        }

       

    }
}
