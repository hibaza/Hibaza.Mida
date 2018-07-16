using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Web
{
    public class Framework
    {
        public static AgentModel CurrentUser(string user_id, string access_token, string apiBaseUrl)
        {
            AgentModel model = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
                {
                    var url = apiBaseUrl + "brands/agents/single/" + user_id + "/?access_token=" + access_token;
                    model = Core.Helpers.WebHelper.HttpGetAsync<AgentModel>(url).Result;
                }
            }
            catch { }
            return model;
        }
        public static bool SetCurrentUserLoginStatus(string user_id, string access_token, string status, string apiBaseUrl)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(access_token))
                {
                    var url = apiBaseUrl + "brands/agents/set_login_status/" + user_id + "/" + status + "/?access_token=" + access_token;
                    result = Core.Helpers.WebHelper.HttpPostAsync(url, "").Result;
                }
            }
            catch {
                
            }
            return result;
        }
    }
}
