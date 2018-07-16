using Firebase.Storage;
using Hangfire;
using Hibaza.CCP.Core;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Zalo;
using Hibaza.CCP.Service.SQL;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Facebook
{
    public interface IZaloService
    {
        void SaveWebhookData(ZaloMessage data, bool real_time_update);

        Task<ApiResponse> SendMessage(dynamic para, Channel channel, MessageFormData form);

        Task<ApiResponse> SendIntereste(Channel channel, string phone);
    }

    public class ZaloService : IZaloService
    {
        private readonly IMessageService _messageService;
        private readonly IChannelService _channelService;
        private readonly ICustomerService _customerService;
        private readonly IThreadService _threadService;
        private readonly IAgentService _agentService;
        private readonly ILoggingService _logService;
        private readonly string _zaloGraphApiUrl = "https://openapi.zaloapp.com/oa/v1/";
        private readonly IOptions<AppSettings> _appSettings;

        public ZaloService(ICustomerService customerService, IThreadService threadService, IChannelService channelService, IMessageService messageService, ILoggingService logService, IOptions<AppSettings> appSettings, IAgentService agentService)
        {
            _messageService = messageService;
            _channelService = channelService;
            _customerService = customerService;
            _threadService = threadService;
            _logService = logService;
            _appSettings = appSettings;
            _agentService = agentService;
        }

        public void SaveWebhookData(ZaloMessage data, bool real_time_update)
        {
            // Make sure this is a page subscription
            //if (data.@event == "sendmsg" || data.@event == "sendimagemsg" || data.@event == "sendlinkmsg" || data.@event== "2691206174905801178")
            //{
            try
            {
                BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(data, null, real_time_update, null));
                //ZaloConversationService conversationService = new ZaloConversationService(_channelService, _threadService, _agentService, _customerService, _messageService, _appSettings, _logService);
                //conversationService.SaveWebhookMessaging(data, null, real_time_update, null).Wait();

            }
            catch (Exception ex)
            {
                // BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(data, null, real_time_update, null));
                _logService.Create(new Log { name = "Webhook zalo calledback 2", message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), details = JsonConvert.SerializeObject(data).ToString() });
                throw ex;
            }
            // }
        }

        public async Task<JObject> SendText(string message, string toUid, string oaId, string oaSecret, long timestamp)
        {
            var data = new Dictionary<string, object>();
            data.Add("message", message);
            data.Add("uid", toUid);
            var mac = Core.Helpers.CommonHelper.sha256(oaId + JsonConvert.SerializeObject(data) + timestamp + oaSecret);

            var para = "oaid=" + oaId +
                "&data=" + JsonConvert.SerializeObject(data) +
                "&timestamp=" + timestamp +
                "&mac=" + mac;

            return await Core.Helpers.WebHelper.HttpPostAsync<JObject>(_zaloGraphApiUrl + "sendmessage/text?" + para, null);
        }

        public async Task<JObject> SendImage(byte[] imageByte, string message, string toUid, string oaId, string oaSecret, long timestamp)
        {
            var mac = Core.Helpers.CommonHelper.sha256(oaId + timestamp + oaSecret);

            var para = new Dictionary<string, string>();
            para.Add("oaid", oaId);
            para.Add("timestamp", timestamp + "");
            para.Add("mac", mac);

            var nameFile = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var rs = await Core.Helpers.WebHelper.sendHttpUploadRequest<JObject>(_zaloGraphApiUrl + "upload/image", imageByte, nameFile, para, new Dictionary<string, string>());

            if ((string)rs["errorCode"] == "1")
            {
                //var rs = await Core.Helpers.WebHelper.HttpPostAsync<JObject>(_zaloGraphApiUrl + "upload/image", para);
                var imageid = (string)rs["data"]["imageId"];

                var data = new Dictionary<string, object>();
                data.Add("uid", toUid);
                data.Add("imageid", imageid);
                data.Add("message", message);

                mac = Core.Helpers.CommonHelper.sha256(oaId + JsonConvert.SerializeObject(data) + timestamp + oaSecret);

                var para1 = "oaid=" + oaId +
                    "&data=" + JsonConvert.SerializeObject(data) +
                    "&timestamp=" + timestamp +
                    "&mac=" + mac;

                return await Core.Helpers.WebHelper.HttpPostAsync<JObject>(_zaloGraphApiUrl + "sendmessage/image?" + para1, null);
            }
            return null;

        }

        public async Task<ApiResponse> SendMessage(dynamic para, Channel channel, MessageFormData form)
        {
            var response = new ApiResponse();

            try
            {
                var recipient_id = form.recipient_id;
                var type = form.channel_format;

                if (string.IsNullOrWhiteSpace(recipient_id) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(form.business_id) || string.IsNullOrWhiteSpace(form.channel_id))
                    return response;

                JObject rs = null;
                var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);

                var zalo = new ZaloMessage();
                zalo.oaid = Convert.ToInt64(channel.ext_id);
                zalo.touid = Convert.ToInt64(recipient_id);
                zalo.timestamp = timestamp;
                zalo.@event = type == "text" || type == "message" ? "sendmsg" : "sendimagemsg";

                if (type == "text" || type == "message")
                {
                    rs = await SendText(form.message, recipient_id, channel.ext_id, channel.token, timestamp);
                    if ((string)rs["errorCode"] == "1")
                    {
                        zalo.message = form.message;
                        zalo.msgid = (string)rs["data"]["msgId"];
                        response.ok = true;

                        try
                        {
                            BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, recipient_id, true, channel));
                            //ZaloConversationService conversationService = new ZaloConversationService(_channelService, _threadService, _agentService, _customerService, _messageService, _appSettings, _logService);
                            //conversationService.SaveWebhookMessaging(zalo, recipient_id, true, channel).Wait();
                        }
                        catch (Exception ex)
                        {
                            //BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, recipient_id, true, channel));
                            _logService.Create(new Log { name = "send zalo error 1", message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), details = JsonConvert.SerializeObject(zalo).ToString() });
                            throw ex;
                        }
                    }
                }
                else
                {
                    List<payloadFb> elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                    for (var i = 0; i < elements.Count; i++)
                    {
                        var imageByte = await Core.Helpers.WebHelper.HttpGetAsyncByte(elements[i].image_url);
                        rs = await SendImage(imageByte, elements[i].title, recipient_id, channel.ext_id, channel.token, timestamp);

                        if ((string)rs["errorCode"] == "1")
                        {
                            response.ok = true;
                            zalo.message = elements[i].title;
                            zalo.href = elements[i].image_url;
                            zalo.msgid = (string)rs["data"]["msgId"];

                            try
                            {
                                BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, recipient_id, true, channel));
                            }
                            catch (Exception ex)
                            {
                                //BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, recipient_id, true, channel));
                                _logService.Create(new Log { name = "send zalo error 2", message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), details = JsonConvert.SerializeObject(zalo).ToString() });
                                throw ex;
                            }
                        }
                    }
                }

            }
            catch { }
            return response;
        }


        public async Task<ApiResponse> SendIntereste(Channel channel, string phone)
        {
            var response = new ApiResponse();

            try
            {
                var timestame = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);

                var newPhone = phone;
                if (newPhone.Substring(0, 1) == "0")
                    newPhone = "84" + newPhone.Substring(1, newPhone.Length - 1);

                //var templatedata = new Dictionary<string, string>();
                //templatedata.Add("DAY", "20");
                //templatedata.Add("TIME_24HR", "24h");

                var data = new Dictionary<string, object>();
                data.Add("phone", newPhone);
                data.Add("templateid", "c8754d9b71de9880c1cf");
                data.Add("templatedata", "{}");
                data.Add("callbackdata", _appSettings.Value.BaseUrls.Api+ "brands/zalos/webhook");

                var mac = Core.Helpers.CommonHelper.sha256(channel.ext_id + JsonConvert.SerializeObject(data) + timestame + channel.token);
                var uri = "https://openapi.zaloapp.com/oa/v1/sendmessage/phone/invite_v2?oaid="+ channel.ext_id + "&data=" + JsonConvert.SerializeObject(data) + "&timestamp=" + timestame + "&mac=" + mac;
                var rs = Core.Helpers.WebHelper.HttpPostAsync<JObject>(uri, null).Result;

                var timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
                var zalo = new ZaloMessage();
                zalo.oaid = Convert.ToInt64(channel.ext_id);
                zalo.touid = Convert.ToInt64(phone);
                zalo.timestamp = timestamp;
                zalo.@event ="sendmsg" ;
                zalo.message = "Đã gửi yêu cầu kết bạn tới " + phone ;
                zalo.msgid =DateTime.Now.ToString("yyyyMMddHHmmssfff");// (string)rs["data"]["msgId"];
                response.ok = true;

                BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, null, true, channel));
                return response;
            }
            catch (Exception ex)
            {
                response.ok = false;
                //BackgroundJob.Enqueue<ZaloConversationService>(x => x.SaveWebhookMessaging(zalo, recipient_id, true, channel));
                _logService.Create(new Log { name = "send zalo error 2", message = ex.Message + JsonConvert.SerializeObject(ex.StackTrace).ToString(), details = "" });
                return response;
            }
        }
    }


}