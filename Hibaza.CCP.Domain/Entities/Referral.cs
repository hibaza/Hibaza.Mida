using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Referral : BaseEntity
    {
        public string sender_ext_id { get; set; }
        public string recipient_ext_id { get; set; }
        public string product_sku { get; set; }
        public string product_id { get; set; }
        public string product_url { get; set; }
        public string product_photo_url { get; set; }
        public string thread_id { get; set; }
        public string customer_id { get; set; }
        public string business_id { get; set; }
        public string data { get; set; }
        public string type { get; set; } //product
        public long timestamp { get; set; }
    }
    public class payloadFb
    {
        public string image_url { set; get; }
        public string title { set; get; }
    }
}
