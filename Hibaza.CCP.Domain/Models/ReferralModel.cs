using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class ReferralModel : BaseModel
    {
        public string created_time { get; set; }
        public DateTime? updated_time { get; set; }
        public string sender_ext_id { get; set; }
        public string recipient_ext_id { get; set; }
        public string product_sku { get; set; }
        public string product_id { get; set; }
        public string product_url { get; set; }
        public string product_photo_url { get; set; }
        public string thread_id { get; set; }
        public string business_id { get; set; }
        public string data { get; set; }
        public string type { get; set; }
        public long timestamp { get; set; }
        public ReferralModel() { }
        public ReferralModel(Referral referral)
        {
            id = referral.id;
            created_time = referral.created_time.ToLocalTime().ToString("hh:mm dd/MM/yyyy");
            sender_ext_id = referral.sender_ext_id;
            recipient_ext_id = referral.recipient_ext_id;
            product_id = referral.product_id;
            product_photo_url = referral.product_photo_url;
            product_sku = referral.product_sku;
            product_url = referral.product_url;
            thread_id = referral.thread_id;
            business_id = referral.business_id;
            data = referral.data;
            type = referral.type;
            timestamp = referral.timestamp;
            updated_time = referral.updated_time;
        }
    }
}
