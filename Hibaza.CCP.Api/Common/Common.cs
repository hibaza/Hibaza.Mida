using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Api
{
    public class Common
    {
        public Dictionary<string, string> convertParaToDic(string para)
        {
            try
            {
                var dicPara = new Dictionary<string, string>();

                //para
                if (para == null || para == "" || para == "[]")
                    dicPara = null;
                else
                {
                    JObject pr = JObject.Parse(para);
                    foreach (var j in pr)
                    {
                        dicPara.Add(j.Key.Trim().ToLower(), j.Value.ToString().Trim());
                    }
                }
                return dicPara;
            }
            catch (Exception e)
            { return null; }
        }
        public Dictionary<string, string> ConvertConfigToDic(string config)
        {
            try
            {
                var dicConfig = new Dictionary<string, string>();

                if (config == null || config == "" || config == "[]")
                    return null;
                JObject cf = JObject.Parse(config);
                foreach (var j in cf)
                {
                    dicConfig.Add(j.Key.ToLower().Trim(), j.Value.ToString().Trim());
                }

                return dicConfig;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
    }
}
