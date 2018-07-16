using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Service;
using Hibaza.CCP.Service.Facebook;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Models;
using Newtonsoft.Json;
using Hibaza.CCP.Domain.Models.Facebook;
using Hangfire;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Domain.Models.Zalo;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;

namespace Hibaza.CCP.Api.Controllers
{

    [Route("brands/zalos")]
    public class ZaloController : Controller
    {
        private readonly IZaloConversationService _zaloConversationService;
        private readonly IZaloService _zaloService;
        private readonly IChannelService _channelService;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _customerService;
        private readonly IThreadService _threadService;
        private readonly ILoggingService _logService;
        private readonly IAgentService _agentService;
        private readonly IOptions<AppSettings> _appSettings;
        private IBackgroundJobClient _backgroundJobClient;
        public ZaloController(IChannelService channelService, IThreadService threadService, ICustomerService userService, ICustomerService cusstomerService, IMessageService messageService, IAgentService agentService, IOptions<AppSettings> appSettings, IBackgroundJobClient backgroundJobClient, ILoggingService logService, IZaloConversationService zaloConversationService, IZaloService zaloService)
        {
            _channelService = channelService;
            _threadService = threadService;
            _customerService = cusstomerService;
            _messageService = messageService;
            _appSettings = appSettings;
            _agentService = agentService;
            _backgroundJobClient = backgroundJobClient;
            _logService = logService;
            _zaloConversationService = zaloConversationService;
            _zaloService = zaloService;
        }

        [HttpGet("webhook")]
        public IActionResult webhook(ZaloMessage obj)
        {
            var tt = Request.QueryString.Value;
            try
            {

                // add remove like image
                //var p = JsonConvert.SerializeObject(obj);
                // p = p.Replace(_appSettings.Value.BaseUrls.Api, "");
                // obj = JsonConvert.DeserializeObject<FacebookWebhookData>(p);
                _logService.Create(new Log { name = "zalo ", message = "zalo " + tt, details = JsonConvert.SerializeObject(obj) });
                // _facebookService.SaveWebhookData("", "", obj, true);
                _zaloService.SaveWebhookData(obj, true);
                return Ok();
            }
            catch (Exception ex)
            {
                _logService.Create(new Log { name = "Webhook zalo error calledback", message = ex.Message + tt, details = JsonConvert.SerializeObject(ex.StackTrace) });
                throw ex;
            }
        }

        [HttpPost("upsert")]
        public string CreateChannel([FromBody] Channel para)
        {
            //string type, string business_id, string channel_id, string oaid, string oasecret, string page_name, string templateid
            try
            {
                var channel = _channelService.GetById(para.business_id, para.id);
                if (channel == null)
                {
                   // var phones = new List<string>();
                    Channel data = new Channel
                    {
                        id = para.id,
                        business_id = para.business_id,
                        ext_id = para.ext_id,
                        name = para.name,
                        active = true,
                        type = para.type,
                        token = para.token,
                        template_id = para.template_id,
                        secret = para.secret
                    };
                    _channelService.UpsertId(data);
                }
                else
                {
                    channel.name = para.name;
                    channel.ext_id = para.ext_id;
                    channel.token = para.token;
                    channel.template_id = para.template_id;
                    channel.updated_time = DateTime.UtcNow;
                    channel.secret = para.secret;
                    _channelService.UpsertId(channel);
                }
            }
            catch (Exception ex)
            {
                _logService.Create(new Log
                {
                    category = "Channels",
                    link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
                    message = ex.Message,
                    details = ex.StackTrace,
                    name = string.Format("Create channel for zalo page-{0} of business-{1} use token={2}", para.id,para.business_id,para.token)
                });
                throw ex;
            }
            return "";
        }


        //[HttpPost("create/{type}/{business_id}/{page_id}/{page_name}/{token}/{ext_id}")]
        //public string CreateChannel(string type, string business_id, string page_id, string page_name, string token, string ext_id)
        //{
        //    try
        //    {
        //        var phones = new List<string>();
        //        Channel data = new Channel
        //        {
        //            id = ChannelService.FormatId(business_id, page_id),
        //            business_id = business_id,
        //            ext_id = ext_id,
        //            name = string.IsNullOrWhiteSpace(page_name) ? type + page_id : page_name,
        //            active = true,
        //            type = type,
        //            token = token,
        //            phones = phones
        //        };
        //        _channelService.Create(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Create(new Log
        //        {
        //            category = "Channels",
        //            link = $"{Request.HttpContext.Request.Scheme}://{Request.HttpContext.Request.Host}{Request.HttpContext.Request.Path}{Request.HttpContext.Request.QueryString}",
        //            message = ex.Message,
        //            details = ex.StackTrace,
        //            name = string.Format("Create channel for zalo page-{0} of business-{1} use token={2}", page_id, business_id, token)
        //        });
        //        throw ex;
        //    }
        //    return "";
        //}

        //[HttpPost("update/{business_id}/{id}/{page_name}/{token}")]
        //public string UpdateChannel(string business_id, string id, string page_name, string token)
        //{
        //    var resultData = "";
        //    var data = _channelService.GetById(business_id, id);
        //    if (data != null)
        //    {
        //        data.name = page_name ?? data.type + "-" + data.ext_id;
        //        data.token = token;
        //        resultData = _channelService.Create(data);
        //    }
        //    return resultData;
        //}

        [HttpGet("testwebhook")]
        public IActionResult testwebhook()
        {
            try
            {
                //text
                //var eventt = "{\"event\":\"os_send_msg\",\"oaid\":1282263163182093624,\"fromuid\":0,\"appid\":0,\"msgid\":\"d526ebe2e44aea14b35b\",\"message\":null,\"timestamp\":1526961455502,\"mac\":\"5b69238b4bc2c8738bd7d7df7896f0a65a2f44dac8e519e89727f7963a5ed6df\",\"touid\":7908227047883664837,\"msginfo\":null,\"userinfo\":null,\"groupid\":0,\"msgids\":null,\"invitetemplate\":null,\"invitedata\":null,\"href\":null,\"thumb\":null,\"description\":null,\"params\":null,\"stickerid\":null,\"order\":null}";
                //image 
                var eventt = "{\"event\":\"sendimagemsg\",\"oaid\":1282263163182093624,\"fromuid\":9188396093525836793,\"appid\":0,\"msgid\":\"27dd870942bd4ce315ac\",\"message\":null,\"timestamp\":1526964919129,\"mac\":\"43ef97756bb663f2794d556e0dd75e8b5db52f05af4e6537ac12a17e206c158d\",\"touid\":0,\"msginfo\":null,\"userinfo\":null,\"groupid\":0,\"msgids\":null,\"invitetemplate\":null,\"invitedata\":null,\"href\":\"http://f9.photo.talk.zdn.vn/2378923171576343991/b077cc6225e3cbbd92f2.jpg\",\"thumb\":\"http://t.f9.photo.talk.zdn.vn/2378923171576343991/b077cc6225e3cbbd92f2.jpg\",\"description\":null,\"params\":null,\"stickerid\":null,\"order\":null}";
                var obj = JsonConvert.DeserializeObject<ZaloMessage>(eventt);

                _zaloService.SaveWebhookData(obj, true);

                // add remove like image
                //var p = JsonConvert.SerializeObject(obj);
                // p = p.Replace(_appSettings.Value.BaseUrls.Api, "");
                // obj = JsonConvert.DeserializeObject<FacebookWebhookData>(p);
                /// _logService.Create(new Log { name = "zalo ", message = "zalo " , details = JsonConvert.SerializeObject(obj) });
                // _facebookService.SaveWebhookData("", "", obj, true);
                return Ok();
            }
            catch (Exception ex)
            {
                _logService.Create(new Log { name = "Webhook zalo error calledback", message = ex.Message, details = JsonConvert.SerializeObject(ex.StackTrace) });
                throw ex;
            }
        }

        [HttpPost("send")]
        public async Task<ApiResponse> Send([FromForm]MessageFormData form)
        {
            Hibaza.CCP.Data.Common _cm = new Hibaza.CCP.Data.Common();
            var para = JsonConvert.DeserializeObject<dynamic>(form.para);
            var channel = _channelService.GetById(form.business_id, form.channel_id);

            #region luu hinh anh den server rieng
            try
            {
                if (form.channel_format == "image-text" || form.channel_format == "image" || form.channel_format == "receipt")
                {
                    if (para.message != null && para.message.attachment != null && para.message.attachment.payload != null && para.message.attachment.payload.elements != null)
                    {
                        List<payloadFb> elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));
                        for (var i = 0; i < elements.Count; i++)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(elements[i].image_url))
                                {
                                    var newImageUrl = await ImagesService.UpsertImageStore(elements[i].image_url, _appSettings.Value); 
                                    para.message.attachment.payload.elements[i].image_url = newImageUrl;
                                    para.message.attachment.payload.elements[i].default_action.url = newImageUrl;
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }

                    #region file attachment
                    // var data = new MessageData { images = new List<MessageFile>() };
                    var files = Request.Form.Files;
                    var j = 0;
                    if (files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue
                                          .Parse(file.ContentDisposition)
                                          .FileName.ToString();
                                fileName = Core.Helpers.CommonHelper.removeSpecialFile(fileName);

                                var fileId = "";
                                if ((form.thread_id + fileName).Length < 100)
                                    fileId = form.thread_id + "_" + Core.Helpers.CommonHelper.GenerateDigitUniqueNumber() + "_" + fileName;
                                else
                                    fileId = form.thread_id + "_" + fileName;
                                // var path = Path.Combine(Directory.GetCurrentDirectory(), @"Documents", "Attachments", fileId);
                                var dir = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.Value.PathToFileDocuments);
                                var fullName = Path.Combine(dir, fileId);

                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);

                                using (FileStream fs = System.IO.File.Create(fullName))
                                {
                                    file.CopyTo(fs);
                                    fs.Flush();
                                }
                                var bytes = _cm.ConvertToBytes(file);
                                var newUrl = await _cm.HtmlPostBytesAsync<string>(_appSettings.Value.BaseUrls.ApiSaveImage
                                   + "api/UploadFile", form.thread_id + "_" + file.FileName, bytes);

                                var elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));

                                para.message.attachment.payload.elements[j].image_url = newUrl;
                                para.message.attachment.payload.elements[j].title = para.message.attachment.payload.elements[j].title == "" ?
                                    channel.name : para.message.attachment.payload.elements[j].title;
                                para.message.attachment.payload.elements[j].default_action.url = newUrl;
                                j++;

                                //var bytes = _cm.ConvertToBytes(file);
                                //var newUrl = await _cm.HtmlPostBytesAsync<string>(_appSettings.Value.BaseUrls.ApiSaveImage
                                //    + "File_DocumentsView/UploadFile", thread.id + "_" + file.FileName, bytes);

                                //var elements = JsonConvert.DeserializeObject<List<payloadFb>>(JsonConvert.SerializeObject(para.message.attachment.payload.elements));

                                //para.message.attachment.payload.elements[j].image_url = newUrl;
                                //para.message.attachment.payload.elements[j].default_action.url = newUrl;
                                //para.message.attachment.payload.elements[j].title = para.message.attachment.payload.elements[j].title == "" ?
                                //    channel.name : para.message.attachment.payload.elements[j].title;
                                //j++;
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
            // _logService.Create(new Log { name = "send zalo ", message = "", details = JsonConvert.SerializeObject(para).ToString() });
            return await _zaloService.SendMessage(para, channel, form);
        }

        [HttpPost("SendIntereste/{business_id}/{phone}")]
        public async Task<ApiResponse> SendIntereste(string business_id, string phone)
        {
            ApiResponse apiResponse = new ApiResponse();
            var channels = await _channelService.GetChannelsType(business_id, "zalo_personal", 1, 1);
            Channel channel = null;
            if (channels != null && channels.Count() > 0)
                channel = channels[0];
            return await _zaloService.SendIntereste(channel, phone);
        }

    }
}
