using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Web.Models
{
    //public class AccountAddEdit : Agent
    //{
    //    public AccountAddEdit(Agent data)
    //    {
    //        this.ext_id = data.ext_id;
    //        this.id = data.id;
    //        this.first_name = data.first_name;
    //        this.last_name = data.last_name;
    //        this.email = data.email;
    //        this.avatar = data.avatar;
    //    }

    //    public string Token { get; set; }
    //}

    public class AccessToken
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
