using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookUser : BaseModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string avatar { get; set; }
        public string sex { get; set; }
    }
    
    public class FacebookUserParent : BaseModel
    {
        public string created_time { get; set; }
        public FacebookUser from { get; set; }
        public string message { get; set; }
    }

    public class FacebookCommentChilden 
    {

        public List<FacebookCommentChildenData> data { get; set; }
    }
    public class FacebookCommentChildenData
    {
        public string created_time { get; set; }
        public FacebookUser from { get; set; }
        public string message { get; set; }
        public string id { get; set; }
    }

    public class FacebookCommentChildenDataTo
    {
        public string created_time { get; set; }
        public FacebookUser to { get; set; }
        public string message { get; set; }
        public string id { get; set; }
    }

    public class FacebookUserProfile : FacebookUser
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
    public class FacebookPictureData
    {
        public string url { get; set; }
    }
    public class FacebookPicture
    {
        public FacebookPictureData data { get; set; }
    }
    public class FacebookProfile
    {
        public string profile_pic { get; set; }
        public FacebookPicture picture { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string id { get; set; }
    }
    public class FacebookProfileFromMessagerId
    {
        public string from { get; set; }
        public FacebookPicture picture { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string id { get; set; }
    }
}
