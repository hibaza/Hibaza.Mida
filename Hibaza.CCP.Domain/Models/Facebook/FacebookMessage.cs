using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookMessage : BaseModel
    {
        public string link { get; set; }
        public string message { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public FacebookUser from { get; set; }
        public FacebookUserFeed to { get; set; }
        public FacebookAttachmentFeed attachments { get; set; }
    }

    public class FacebookMessageParent : BaseModel
    {
        public FacebookUserParent parent { get; set; }
    }

    public class FacebookCommnet
    {
        public FacebookCommentChilden comments { get; set; }

    }


    public class FacebookEntry
    {
        public string id { get; set; }
        public List<string> changed_fields { get; set; }
        public List<FacebookChangesEvent> changes { get; set; }
        public long time { get; set; }
        public List<FacebookMessagingEvent> messaging { get; set; }
    }

    public class FacebookChangesEvent
    {
        public string field { get; set; } //feed, conversations
        public FacebookChangesValue value { get; set; }
    }


    public class FacebookChangesValue
    {
        public string item { get; set; } //"comment"
        public string thread_id { get; set; } //t_mid.1489313372083:7d5fdc5613
        public string page_id { get; set; } //310467822386873
        public string sender_name { get; set; }
        public string photo { get; set; }
        public string comment_id { get; set; } //"1375377682520684_1375386365853149",
        public string sender_id { get; set; } //"1922473854648248,
        public string post_id { get; set; } //"140691416376122_1375377682520684",
        public string verb { get; set; } //"add"
        public string parent_id { get; set; } //"140691416376122_1375377682520684",
        public long created_time { get; set; } //timestamp, ex: 1489314725
        public string message { get; set; }
    }

    public class FacebookMessageObject
    {
        public string msgId { get; set; }
        public string msgParentId { get; set; }
        public string msgRootId { get; set; }
        public string conversationId { get; set; }
        public string ownerId { get; set; }
        public string senderId { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public string recipientId { get; set; }
        public string recipient_name { get; set; }
        public string recipient_avatar { get; set; }
        public string customer_id { get; set; }
        public long timestamp { get; set; }
        public string type { get; set; } = "text";
        public string thread_type { get; set; }
        public string channel_type { get; set; } = "facebook";
        public bool liked { get; set; }
        public bool hidden { get; set; }
        public string url { get; set; }
        public List<string> urls { get; set; }
        public string text { get; set; }
        public string tag { get; set; }
        public string template { get; set; }
        public List<FacebookAttachmentGet> attachements { get; set; }
        public List<string> titles { get; set; }
    }

    public class FacebookMessagingEvent
    {
        public FacebookUser sender { get; set; }
        public FacebookUser recipient { get; set; }
        public long timestamp { get; set; }
        public FacebookMessageGet message { get; set; }
        public FacebookReferral referral { get; set; }
        public FacebookPostback postback { get; set; }
        public FacebookDelivery delivery { get; set; }
    }

    public class FacebookDelivery
    {
        public List<string> mids { get; set; }
        public long watertermark { get; set; }
        public int seq { get; set; }
    }

    public class FacebookPostback
    {
        public string payload { get; set; }
        public FacebookReferral referral { get; set; }
    }

    public class FacebookReferral
    {
        public string Ref { get; set; }
    }

    //public class FacebookMessagingReferralEvent
    //{
    //    public FacebookUser sender { get; set; }
    //    public FacebookUser recipient { get; set; }
    //    public long timestamp { get; set; }
    //    public FacebookReferral referral { get; set; }
    //}


    public class FacebookUserPost
    {
        public string id { get; set; }
    }
    public class FacebookMessagePostData
    {
        public FacebookUserPost recipient { get; set; }
        public FacebookMessagePost message { get; set; }
        public string tag { get; set; } //SHIPPING_UPDATE, RESERVATION_UPDATE, ISSUE_RESOLUTION

    }

    public class FacebookTextMessageResponse
    {
        public string id { get; set; }
    }

    public class FacebookMessagePostResponse
    {
        public string recipient_id { get; set; }
        public string message_id { get; set; }
        public string attachment_id { get; set; }
    }


    public class FacebookPhotoPost
    {
        public string url { get; set; }
        public bool published { get; set; }
        public bool no_story { get; set; } = true;
    }

    public class FacebookCommentPost
    {
        public string message { get; set; }
        public string attachment_url { get; set; }
        public string attachment_id { get; set; }
    }

    public class FacebookHideCommentPost
    {
        public bool is_hidden { get; set; }
    }

    public class FacebookPostLink
    {
        public string id { get; set; }
        public string permalink_url { get; set; }
    }

    public class FacebookCommentPostResponse
    {
        public string id { get; set; }
    }

    public class FacebookPayloadElementAction
    {
        public string type { get; set; } = "web_url";
        public string url { get; set; }
        public bool messenger_extensions { get; set; }
        public string webview_height_ratio { get; set; } = "tall";
        public string fallback_url { get; set; }
    }

    public class FacebookPayloadElementButton
    {
        public string type { get; set; } = "web_url"; //postback, web_url
        public string url { get; set; }
        public string title { get; set; }
        public string payload { get; set; }
    }

    public class FacebookPayloadElement
    {
        public string title { get; set; } //max 80 chars
        public string image_url { get; set; }
        public string subtitle { get; set; } //max 80 chars
        public FacebookPayloadElementAction default_action { get; set; }
        public List<FacebookPayloadElementButton> buttons { get; set; } //max 3
    }

    public class FacebookPayload
    {
        public string url { get; set; }
        public bool is_reusable { get; set; }
        public string attachment_id { get; set; }
        public string template_type { get; set; } //generic
        public List<FacebookPayloadElement> elements { get; set; } //max 10
    }

    public class FacebookAttachmentPayload
    {
        public string url { get; set; }
        public bool is_reusable { get; set; }
        public string attachment_id { get; set; }
    }

    public class FacebookTemplatePayload
    {
        public string template_type { get; set; } //generic
        public List<FacebookPayloadElement> elements { get; set; } //max 10
    }


    public class FacebookAttachmentGet
    {
        public string type { get; set; } //template, text, image, video, audio, file
        public FacebookPayload payload { get; set; }
    }

    public class FacebookAttachmentPost
    {
        public string type { get; set; } //template, text, image, video, audio, file
        public dynamic payload { get; set; }
    }

    public class FacebookMessagePost
    {
        public string text { get; set; }
        public FacebookAttachmentPost attachment { get; set; }
    }

    public class FacebookMessageGet
    {
        public string mid { get; set; }
        public string text { get; set; }

        public List<FacebookAttachmentGet> attachments { get; set; }
    }

    public class FacebookWebhookData
    {
        public string @object { get; set; }
        public List<FacebookEntry> entry { get; set; }
    }

    public class FacebookHub
    {
        public string mode { get; set; }
        public string verify_token { get; set; }
        public int challenge { get; set; }
    }

    public class FacebookIdForPage
    {
        public string id { get; set; }
        public dynamic page { get; set; }
    }

    public class FacebookIdForApp
    {
        public string id { get; set; }
        public dynamic app { get; set; }
    }

    public class FacebookIdForPageFeed
    {
        public FacebookPaging paging { get; set; }
        public IEnumerable<FacebookIdForPage> data { get; set; }
    }

    public class FacebookIdForAppFeed
    {
        public FacebookPaging paging { get; set; }
        public IEnumerable<FacebookIdForApp> data { get; set; }
    }

    public class MessageFile
    {
        public string FileId { get; set; }
        public Stream Stream { get; set; }
    }

    public class MessageData
    {
        public string tag { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public string template { get; set; }
        public List<MessageFile> images { get; set; }
        public List<string> image_urls { get; set; }
        public List<string> attachment_urls { get; set; }
        public List<string> titles { get; set; }
        public string button_title { get; set; }
    }

    public class FacebookShares : BaseModel
    {
        public FacebookSharesChilden shares { get; set; }
        public string message { get; set; }
        public DateTime created_time { get; set; }

    }
    public class FacebookSharesChilden
    {

        public List<FacebookSharesChildenData> data { get; set; }
    }

    public class FacebookSharesChildenData
    {
        public string description { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public string name { get; set; }
    }
}
