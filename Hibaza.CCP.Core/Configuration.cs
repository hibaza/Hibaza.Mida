using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hibaza.CCP.Core
{

    public class AppSettings
    {
        public string Version { get; set; }
        public BaseUrls BaseUrls { get; set; }
        public string AppAccessToken { get; set; }
        public string UserAccessToken { get; set; }
        public string ConnectionString { get; set; }
        public FirebaseDB FirebaseDB { get; set; }
        public MongoDBs MongoDB { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string PathToFileDocuments { get; set; }
        public string PhoneNonDisplay { get; set; }
        public string LimitAssign { get; set; }
       
    }

    public class BaseUrls
    {
        public string Api { get; set; }
        public string Auth { get; set; }
        public string Web { get; set; }
        public string ApiAi { get; set; }
        public string ApiSaveImage { get; set; }
        public string ApiOrder { get; set; }
        public string ApiHotline { get; set; }
        public string PhoneWeb { get; set; }        
    }

    public class MongoDBs
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringHangfire { get; set; }
        public string Database { get; set; }
        public string DatabaseAi { get; set; }
        public string HibazaDBHangfile { get; set; }
    }

  

    public class FirebaseDB
    {
        public string APIKey { get; set; }
        public string AuthToken { get; set; }
        public string AuthDomain { get; set; }
        public string DatabaseURL { get; set; }
        public string StorageBucket { get; set; }
        public string MessagingSenderId { get; set; }
    }

    public class Configuration
    {
        private static string GetKey(string key)
        {
            return ""; //ConfigurationManager.AppSettings[key];
        }

        public static string BaseUrl
        {
            get
            {
                string baseUrl = GetKey("BaseUrl");
                if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Remove(baseUrl.Length - 1);
                return baseUrl;
            }
        }


        public static string CloudBaseUrl
        {
            get
            {
                string baseUrl = GetKey("CloudBaseUrl");
                if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Remove(baseUrl.Length - 1);
                return baseUrl;
            }
        }

        public static string DataFolderPath
        {
            get
            {
                string path = GetKey("DataFolderPath");
                if (path.EndsWith("\\")) path = path.Remove(path.Length - 1);
                return path;
            }
        }


        public static int DefaultPageSize
        {
            get
            {
                return int.Parse(GetKey("DefaultPageSize"));
            }
        }

        public static string EncryptionKey
        {
            get
            {
                return "Tien2015BazaVietnam";
            }
        }

        public static string ApiBaseUrl
        {
            get
            {
                return GetKey("ApiBaseUrl");
            }
        }

    }
}
