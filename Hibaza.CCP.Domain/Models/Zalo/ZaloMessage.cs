using System;
using System.Collections.Generic;
using System.Text;

namespace Hibaza.CCP.Domain.Models.Zalo
{
    public class ZaloMessage
    {

        public String @event { set; get; }
        public long oaid { set; get; }
        public long fromuid { set; get; }
        public long appid { set; get; }
        public String msgid { set; get; }
        public String message { set; get; }
        public long timestamp { set; get; }
        public String mac { set; get; }
        public long touid { set; get; }

        public msginfo msginfo { set; get; }
        public userinfo userinfo { set; get; }
        public long groupid { set; get; }
        public String msgids { set; get; }
        public String invitetemplate { set; get; }
        public String invitedata { set; get; }
        public String href { set; get; }
        public String thumb { set; get; }
        public String description { set; get; }
        public paramss @params { set; get; }
        public String stickerid { set; get; }
        public order order { set; get; }
    }
    public class msginfo
    {
        public String type { set; get; }
        public List<String> message { set; get; }
        public List<String> href { set; get; }
        public List<String> title { set; get; }
        public List<String> thumb { set; get; }
        public String desc { set; get; }
        public links links { set; get; }
        public String stickerId { set; get; }


    }

    public class links
    {
        public List<String> title { set; get; }
        public List<String> href { set; get; }
        public List<String> thumb { set; get; }
        public List<String> desc { set; get; }
    }
    public class userinfo
    {
        public String address { set; get; }
        public long phone { set; get; }
        public String city { set; get; }
        public String district { set; get; }
        public String name { set; get; }
        public String ward { set; get; }
    }
    public class paramss
    {
        public String latitude { set; get; }
        public String longitude { set; get; }
    }

    public class order
    {
        public String id { set; get; }
        public long oaId { set; get; }
        public long userId { set; get; }
        public String productId { set; get; }
        public long price { set; get; }
        public String customerName { set; get; }
        public long customerPhone { set; get; }
        public String deliverAddress { set; get; }
        public String deliverCity { set; get; }
        public String deliverDistrict { set; get; }
        public int numItem { set; get; }
        public int paymentMethod { set; get; }
        public int status { set; get; }
        public String orderCode { set; get; }
        public long createdTime { set; get; }
        public long updatedTime { set; get; }
        public String productName { set; get; }
        public String productImage { set; get; }
        public String cancelReason { set; get; }
        public variation variation { set; get; }

    }
    public class variation
    {
        public String variationId { set; get; }
        public String name { set; get; }
        public String variationCode { set; get; }
        public attributes attributes { set; get; }
    }
    public class attributes
    {
        public String attributeId { set; get; }
        public String name { set; get; }
        public String attributeTypeId { set; get; }
        public String typeName { set; get; }

    }
}
