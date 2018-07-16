

var _index = 0;
var _sended = [];
var _customer_id = "";
var _channel_ext_id = "";
var urlServiceAi = baseUrls_ApiAi;
var _pageNextBot = 0;

// tu click khach hang cu the
function getCustomerAi(customer_id, channel_ext_id) {
    _channel_ext_id = channel_ext_id;
    _customer_id = customer_id;
}
// click vao 1 khach hang
function forwardToBot(last_message) {
    try {
        getCart(_customer_id);
        resetDefaultBot();
        addSessionBot(_customer_id, "", _channel_ext_id, "customer", "");
    } catch (e) { console.log(e); };
}

// nhan tu cua so chat sang o tim kiem bot
function editToBot(last_message) {
    try {
        resetDefaultBot();
        $(".txtSearchAi").val(last_message);
        $(".txtSearchAi").html(last_message);
        $(".lableQuestionAi").html(last_message);
        $(".lableQuestionAi").attr("title", last_message);
        addSessionBot(_customer_id, last_message, _channel_ext_id, "editsearch", "ManualAgents");
    } catch (e) { console.log(e); };
}


// nhan enter trong o search bot
function searchEventAi(e, thiss) {
    try {
        if (e.keyCode == 13) {
            resetDefaultBot();
            addSessionBot(_customer_id, $(".txtSearchAi").val(), _channel_ext_id, "search", "ManualAgents");
            return false;
        }

        try {
            var lst = localStorage.getItem("SuggestionsSearch");
            if (lst == null) {
                //var para = "Web_GetSuggestionsSearch('" + (_customer_id == null ? "" : _customer_id) + "'," +
                //    "" + ($(".defaultSearch").prop('checked') == true ? 1 : 0) + "" +
                //    ",'" + $(".txtSearchAi").val() + "')";

                //var config = {
                //    mongoconnect: "ConnAi",
                //    mongodb: "AiDb",
                //    collectionname: "SuggestionsSearch",
                //    type: "procedure"
                //};
                //var jsonpara = { config: JSON.stringify(config), para: JSON.stringify(para) };
                //lst = ExecuteChatBotSyns(jsonpara, urlServiceAi + "api/procedure/execute", "POST",
                //    "", true);
                lst = ExecuteChatBotSyns(null, urlServiceAi + "api/ai-suggestions/get-by-business/" + businessID, "GET",
                    "", true);
                if (lst != null && lst != undefined)
                    localStorage.setItem("SuggestionsSearch", lst);
            }
            if (lst == null || lst == "" || lst == "[]")
                return;
            var data = JSON.parse(lst);
           
            $(thiss).autocomplete({
                source: data,
                select: function (event, ui) {
                    try {
                            setTimeout(function () {
                                $(".txtSearchAi").val(ui.item.data);
                            }, 100);
                    } catch (e) { console.log(e); }
                }
            });

            return true;
        } catch (ex) {
            console.log(ex);
        }

        return true;
    } catch (ex) { console.log(ex); }
}

function GetSuggestionsSearch_result(lst, jsonPara) {
    try {

        if (lst == null || lst == "" || lst == "[]")
            return;
        var data = JSON.parse(lst);
        $(_autocomplateSearch).autocomplete({
            lookup: data,
            onSelect: function (suggestion) {
                $(".txtSearchAi").val(suggestion);
            }
        });
    } catch (e) { console.log(e); }
}
function defaultCombobox() {
    $(".js-channel-format-chat").val("image-text");
    $(".js-config-chat").val("");
    $(".js-para-chat").val("");
}
function selectFormatSend(e) {
    try {
        $(".textCommon").removeClass("hidden");
        $(".textWithImage").removeClass("hidden");
        $("#js-gallery-image-upload").removeClass("hidden");
        if ($(e).val() == "multiimagesonetext") {
            $(".textWithImage").addClass("hidden");
            inputModalMessageTag.val("sale");
        }
        if ($(e).val() == "multiimagesmiltitext") {
            $(".textCommon").addClass("hidden");
            inputModalMessageTag.val("multiimagesmiltitext");
        }
        if ($(e).val() == "quickreply") {
            $("#js-gallery-image-upload").addClass("hidden");
            $(".textWithImage").addClass("hidden");
            inputModalMessageTag.val("quickreply");
        }
        if ($(e).val() == "onetext") {
            $("#js-gallery-image-upload").addClass("hidden");
            $(".textWithImage").addClass("hidden");
            inputModalMessageTag.val("sale");
        }
    } catch (e) { console.log(e); }
}

function sendChatBotAi() {
    try {

        buttonSendimage.click();
        _sended = [];
        var dataSum = null;
        var format = "";
        if (_sended.length == 0)
            $(".viewSelect").each(function (i, thiss) {
                try {
                    var imageurl = $($(thiss).find('img')).attr("src");
                    var link = $($(thiss).find('.cp-image a')).attr("href");
                    var title = $($(thiss).find('.cp-name a')).text();
                    var checked = $($(thiss).find('.icon-p-liked')).length;
                    var im = (imageurl != null && imageurl != undefined && imageurl != "" && imageurl != "undefined") ? imageurl : "";
                    var tt = (title != null && title != undefined && title != "" && title != "undefined") ? title : "";

                    if (format == "") {
                        format = $(thiss).attr("reply_format");
                    }
                    if (checked > 0) {
                        var item = {
                            title: tt,
                            subtitle: "",
                            weburl: link,
                            image_url: im,
                            item: JSON.parse(str_replace_all($(thiss).attr("item"), "'", "\"")),
                            customer: JSON.parse(str_replace_all($(thiss).attr("customer"), "'", "\""))
                        };
                        _sended.push(item);
                    }
                } catch (e) { console.log(e); }
            });
        var data = {
            elements: _sended,
            message: _sended[0].title
        };
        $(".js-channel-format-chat").val(format);

        // thêm chia thành mỗi lần gửi 4 sản phẩm 
        if (data.elements.length > 4) {
            var next = _index + 4;
            var sp = [];
            for (_index; _index < data.elements.length; _index++) {
                if (_index < next) {
                    sp.push(data.elements[_index]);
                }
                else
                    break;
            }
            var arr = {
                elements: sp,
                message: data.message
            };

            loadImageToDropableZone("facebook", format, arr);
        }
        else
            loadImageToDropableZone("facebook", format, data);

    } catch (e) { console.log(e); }
}

function addSessionBot(customer_id, last_message, channel_ext_id, receiveType, autoAgents) {
    try {
        // ẩn hiện nút
        if ($(".assignedToOther").length == 1)
            $(".btnPreviews").attr("disabled", "disabled");
        else
            $(".btnPreviews").removeAttr("disabled");

        // bắt đầu sử lý
        if (_customer_id == "") {
            console.log("Khách hàng null");
            return;
        }
        var config = {
            session_customer: _customer_id,
            receive_type: receiveType,
            using_full_search: $(".defaultSearch").prop('checked') == true ? "1" : "0",
            page_id: channel_ext_id,
            auto_agents: autoAgents,
            page_next_bot: _pageNextBot,
            business_id: businessID
        };

        var para = {
            q: last_message == null || last_message == undefined ? "" : last_message.replace("'", " ").replace("\\", " ").replace("\n", " ").replace("/", " ").replace("&#13;&#10", " ").replace("\"", " ")
        };
        var jsonpara = { config: JSON.stringify(config), para: JSON.stringify(para) };
        ExecuteChatBotSyns(jsonpara, urlServiceAi + "api/AiExcuteAll/Excute", "POST",
            "addSessionBot_result", true);

    } catch (e) {
        console.log(e);
    }
}

function addSessionBot_result(data, jsonPara) {
    try {
        var config = JSON.parse(jsonPara.config);
        if (data == null || data == undefined || data == "" || data == "[]") {
            if (config.autoAgents == "ManualAgents");
            $("#suggestAi").html("No answer");
            return;
        }

        if (config.receiveType == "webhook" && $(".txtSearchAi").val() != "")
            return;

        var para = JSON.parse(jsonPara.para);

        var sumHtml = '';
        var lst = JSON.parse(data);
        if (lst != null && lst.length > 0) {
            var last_message = "";
            var currentSession = "";
            $.each(lst, function (key, val) {
                last_message = val.last_message != "" ? val.last_message : para.q;
                currentSession = val.currentsession != "" ? val.currentsession : "";

                var mes = val.OrderItems != null && val.OrderItems != undefined && val.OrderItems.length > 0 && (val.reply_format == "orderview" || val.reply_format == "incart" || val.reply_format == "orderconfirm") ? (val.OrderItems[0].ProductSKU + " - " + val.TotalMoney) : val.messager;
                var img = val.OrderItems != null && val.OrderItems != undefined && val.OrderItems.length > 0 && (val.reply_format == "orderview" || val.reply_format == "incart" || val.reply_format == "orderconfirm") ? val.OrderItems[0].ProductPhotoUrl : val.image;
                if (mes != "" || img != "") {
                    sumHtml += formatDataForSelect(mes, val.webdetailurl == null || val.webdetailurl == undefined ?
                        "" : val.webdetailurl, img, val.reply_format, val, lst[lst.length - 1]);
                }
            });
            $(".lableQuestionAi").html(last_message);
            $(".lableQuestionAi").attr("title", last_message);
            $(".defaultSearch").prop('checked', lst.length > 0 && (lst[0].using_full_search == "1" || lst[0].using_full_search > 0) ? true : false);
            if (sumHtml != "")
                $("#suggestAi").append(sumHtml);
          //  else
          //      $("#suggestAi").html("No answer");
            $(".lableSessionAi").html("Current session: " + currentSession);
            $(".lableSessionAi").attr("title", "Current session: " + currentSession);
            reziseButton($("#suggestAi"));
        }
    } catch (e) {
        $("#suggestAi").html("No answer");
        console.log(e);
        console.log(data);
        console.log(jsonPara);
    }
}

//function clearSession() {
//    try {
//        var config = {
//            mongodbConnect: "MongodbConnect", mongodbDatabase: "Baza", collectionname: "ParentSession", type: "procedure"
//        };
//        var para = "jsClearSession('" + _customer_id + "')";
//        var jsonpara = { config: JSON.stringify(config), para: JSON.stringify(para) };
//        ExecuteChatBotSyns(jsonpara, urlServiceAi + "api/procedure/execute", "POST",
//            "clearSession_result", true);

//    } catch (e) {
//        console.log(e);
//    }
//}
//function clearSession_result() {

//}
function checkUncheckBot(thiss) {
    try {
        if ($(thiss).hasClass("icon-p-liked") == true) {
            $(thiss).removeClass("icon-p-liked").addClass("icon-p-like");
        }
        else
            $(thiss).removeClass("icon-p-like").addClass("icon-p-liked");
    } catch (e) { console.log(e); }
}
function formatDataForSelect(content, webLink, imgLink, reply_format, data, customerInfo) {
    try {
        var html = '<div class="cp-row cp-left viewSelect" reply_format="' + reply_format + '" item = "' + str_replace_all(JSON.stringify(data), "\"", "'")
            + '" customer="' + str_replace_all(JSON.stringify(customerInfo), "\"", "'") + '">' +
            '<div class="cp-content" >';
        if (imgLink != null && imgLink != undefined && imgLink != "" && imgLink != "undefined")
            html += '<div class="cp-image">' +
                '<a href="' + webLink + '" target="_blank">' +
                '<img src="' + imgLink + '" alt=""></a>' +
                '</div>';
        if (content != null && content != undefined && content != "" && content != "undefined")
            html += '<div class="cp-name">' +
                '<h2><a href="' + webLink + '" target="_blank" title="' + content + '">' + content + '</a></h2>' +
                '</div>';

        html += '</div>' +
            '<div class="icon-p-like" onclick="checkUncheckBot(this)">' +
            '<div></div>' +
            '</div>' +
            '</div >';

        if ((imgLink != null && imgLink != undefined && imgLink != "" && imgLink != "undefined") ||
            (content != null && content != undefined && content != "" && content != "undefined"))
            return html;
        else return "";

    } catch (e) { console.log(e); }
}

function ExecuteChatBotSyns(jsonpara, urli, type, callback, isJson) {
    try {
        var lstData = null;
        var syns = false;
        if (callback != null && callback != '')
            syns = true;
        if (isJson) {
            $.ajax({
                url: urli,
                type: type,
                data: JSON.stringify(jsonpara),
                async: syns,
                dataType: 'json',
                contentType: 'application/json',
                success: function (data, textStatus, jqXHR) {
                    if (syns)
                        eval(callback)(data, jsonpara);
                    else {
                        lstData = data;
                        return data;
                    }
                },
                complete: function () {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
        else
            $.ajax({
                url: urli,
                type: type,
                data: jsonpara,
                async: syns,
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    if (syns)
                        eval(callback)(data, jsonpara);
                    else {
                        lstData = data;
                        return data;
                    }
                },
                complete: function () {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        return lstData;
    } catch (e) {
        console.log(e);
        return null;
    }
}

//===================== ORDER=============================
function oderBotAi() {
    try {
        $("#wz-modal-order").modal("show");
        $(".js-cancel-order").click(function () {
            $("#wz-modal-order").modal("hide");
        });
    } catch (e) { console.log(e); }
}

//====================== SỬA BOT ========================
//function editSearchBot() {
//    try {
//      //  _autoType = false;
//        resetDefaultBot();

////if ($(".txtSearchAi").attr("disabled") == "disabled") {
////    $(".txtSearchAi").removeAttr("disabled");
////    _autoType = false;
////}
////else {
////    $(".txtSearchAi").attr("disabled", "disabled");
////    _autoType = true;
////}


//      //  $(".txtSearchAi").removeAttr("disabled");
//    } catch (e) { console.log(e); }
//}

function successSendAi() {
    try {
        $(".js-channel-format-chat").val("message");
        $(".js-tag-chat").val("ISSUE_RESOLUTION");
        $(".js-config-chat").val("");
        $(".js-para-chat").val("");
        txt.val("");
        $(".js-submit-image").removeClass("hidden");
        $("#js-gallery-image-upload").html(""); imgSendUrls = []; inputImage.val(""); txtImage.val("");
        inputImage.parent().each(function (k, v) {
            if (k > 0)
                v.remove();
        });
    } catch (e) { console.log(e); }
}

function cancelSendAi() {
    try {
        $(".js-channel-format-chat").val("message");
        $(".js-tag-chat").val("ISSUE_RESOLUTION");
        $(".js-config-chat").val("");
        $(".js-para-chat").val("");
        $(".js-submit-image").removeClass("hidden");
        $("#js-gallery-image-upload").html(""); imgSendUrls = []; inputImage.val(""); txtImage.val("");
        inputImage.parent().each(function (k, v) {
            if (k > 0)
                v.remove();
        });

        _index = _index - 4;
        if (_index < 0)
            _index = 0;
    } catch (e) { console.log(e); }
}

function resetDefaultBot() {
    try {
        $("#suggestAi").empty();
        _index = 0;
        _sended = [];
        _pageNextBot = 0;
    } catch (e) { console.log(e); }
}

function orderSaveToShop() {
    try {
        if ($(".js-receipt-fullname").val() == undefined || $(".js-receipt-fullname").val() == "") {
            new NotificationPlugin("Error", "Fullname not empty", "icon-bell");
            return;
        }
        if ($(".js-receipt-phone").val() == undefined || $(".js-receipt-phone").val() == "") {
            new NotificationPlugin("Error", "Phone not empty", "icon-bell");
            return;
        }
        modalImg.modal("hide");
        var elements = postOrderToShop();
        
        elements.Imei = _customer_id;
        elements.LogonId = elements.LogonId;
        elements.Message = elements.Link;
        elements.PaymentOption = 3;
        elements.TransType = "";

        delete elements.CouponName;
        delete elements.OrderStatus;
        delete elements.OrderStatusInt;
        delete elements.PaymentStatusInt;
        delete elements.PaymentUrl;
        delete elements.Success;
        delete elements.TransactStatus;
        delete elements.TransactStatusInt;
        delete elements.Link;
        delete elements.PaymentStatus;

        var url = baseUrls_ApiOrder + "api/order/PostOrder2";


        var obj = getPostHtml(elements, "POST", url, "");
        //})
        
        if (obj == null || obj == undefined || obj == "")
            new NotificationPlugin("Error", "Error create order", "icon-bell");
        // var data = JSON.parse(obj);
        if (obj.Success)
        {
            updateCustomerInfo($(".js-receipt-address").val(), $(".js-receipt-email").val(),
                $(".js-receipt-province option:selected").val(), _customer_id, $(".js-business-input").val());
            // luu gio hang
            var paraUpsertNew = convertOrderAdminCart();
            paraUpsertNew.order_status = "Chờ xử lý";
            paraUpsertNew.order_status_int = 0;
            upsertDocoments({}, { orderid: paraUpsertNew.orderid }, "Carts", "delete", "", "mongoDbHibaza");
            paraUpsertNew.id = paraUpsertNew.orderid;
            paraUpsertNew._id = paraUpsertNew.orderid;
            paraUpsertNew.async = true;
            upsertDocoments({ $set: paraUpsertNew }, { orderid: paraUpsertNew.orderid }, "Orders", "upsert", "", "mongoDbHibaza");

            new NotificationPlugin("Ok", "Success order", "icon-bell");
        }
        else
            new NotificationPlugin("Error", "Error create order", "icon-bell");
        getCart();
        defaultCart();
    } catch (e) { console.log(e); }
}

function defaultCart() {
    $("#js-gallery-image-upload").empty();
    $("#js-preview-more").empty();
    $(".js-channel-format-chat").val("message");
}


function reziseButton(element) {
    try {
        var warbot = $(document).height();
        $(".war-bot-content").height(warbot - ($(".wz-col-sideright section footer").height() + 163));
        //var warbot = $(document).height();
        //console.log($("footer .panel-default").height());
        //console.log("'" + (warbot - ($("footer .panel-default").height() + 317)) + "px !important'");
        ////element.css('height', "'" + (warbot - ($("footer .panel-default").height() + 317)) + "px !important'");
        //element.css('height', '408px !important;');
        ////overflow-y: auto !important;
    } catch (e) { console.log(e); }
}

function viewMoreBot() {
    try {
        var scroll = $('.bot-body-search').scrollTop();
        var leng = $(".viewSelect").size();
        if (scroll > 0 && leng >= _pageNextBot + 50) {
            _pageNextBot = _pageNextBot + 50;
            addSessionBot(_customer_id, $(".txtSearchAi").val(), _channel_ext_id, "search", "ManualAgents");
        }
    } catch (e) { console.log(e); }
}

