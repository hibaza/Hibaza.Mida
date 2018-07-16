using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Web.Models
{
    public class ChannelAddEdit
    {
        public string AppId { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string ChannelId { get; set; }
        public string BusinessId { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; } = "";
        public string TemplateId { get; set; } = "";
        public string Secret { get; set; } = "";
        public IEnumerable<Channel> Channels { get; set; }
    }

    public class ChannelSettings
    {
        public string business_id { get; set; }
        public string ZaloAuth { get; set; } = "";
        public IEnumerable<Channel> Channels { get; set; }
    }
}
