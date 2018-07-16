
var showAgentDetail = false;
var agentEvent;
var agentAndPhone;
$(document).ready(function () {
    setHotline("offline");
});
// lang nghe tu ifram
window.addEventListener('message', function (e) {
    try {
        //console.log(e);
        var data = JSON.parse(e.data);
        if (data == null || data == undefined)
            return;

        if (data.click_action != null && data.click_action != undefined) {

            if (data.click_action == "call") {
                showAgentDetail = true;
                showCallDetail();
            }
            if (data.click_action == "terminated") {
                showAgentDetail = false;
                hideCallDetail();
            }
            if (data.click_action == "connect") {
                showAgentDetail = true;
                showCallDetail();
                statusHotline();
            }
        }

        if (data.type != null && data.type != undefined && data.type == "channels") {
            hotline_drawChannal(data.data[0]);       
        }
        if (data.type != null && data.type != undefined && data.type == "agents") {
            hotline_drawAgent(data.data[0]);
        }
        
        if (data.type != null && data.type != undefined && data.type == "upsert_hibaza" && data.data.agent_id == myID) {
            agentEvent = data.data;
        }     
        //if (data.type != null && data.type != undefined && (data.type == "channels_agents" || data.type == "signalr" || data.type =="agent")) {
        //    setStatusHotline("online","");
        //}
    } catch (ex) {  }
});

function hotline_drawChannal(data) {
    try {
        if (data != null && data != undefined) {
            $.each(data, function (key, val) {
                try {
                    
                    var li_channel = $("[data-channel=" + key + "]");
                    li_channel.removeClass("hidden").addClass("unread");
                    li_channel.find(".js-chat-count-" + key).html(val);
                    li_channel.find(".wz-sideleft-channel-icon").removeClass("icon-facebook").addClass("icon-phone");
                    //if (val > 0) {
                    //    li_channel.removeClass("hidden").addClass("unread");
                    //    li_channel.find(".js-chat-count-" + key).html(val);
                    //    li_channel.find(".wz-sideleft-channel-icon").removeClass("icon-facebook").addClass("icon-phone");
                    //}
                    //else {
                    //    li_channel.removeClass("unread").addClass("hidden");
                    //    li_channel.find(".js-chat-count-" + key).html(0);
                    //    li_channel.find(".wz-sideleft-channel-icon").removeClass("icon-facebook").addClass("icon-phone");
                    //}
                } catch (et) { console.log(et) }
            });
            var countAll = 0;
            $(".js-channel-list").find(".badge").each(function (k, v) {
                if (k > 0) {
                    countAll = countAll + parseInt($(v).html());
                }
            });
            $(".js-channel-list-all").find(".js-chat-count").html(countAll);

        }
    } catch (e) { console.log(e); }
}

function hotline_drawAgent(data) {
    try {
        if (data != null && data != undefined) {
            $.each(data, function (key, val) {
                $("[data-agent=" + key + "]").each(function (k, v) {
                    if ($(v).hasClass("js-agent-list")) {
                        if (val > 0) {
                            $(v).find(".js-agent-phone").removeClass("hidden");
                           // $(v).addClass("icon-phone");
                        }
                        else {
                            $(v).find(".js-agent-phone").addClass("hidden");
                           // $(v).removeClass("icon-phone");
                        }
                    }
                })
            })
        }
    } catch (e) { console.log(e); }
}

function showCallDetail() {
    try {
        if (agentEvent != null && agentEvent != undefined && showAgentDetail) {
            setTimeout(showCallDetail_click, 1000);
        }

    } catch (e) { console.log(e); }
}

function hideCallDetail() {
    try {
        showAgentDetail = false;
    } catch (e) { console.log(e); }
}
function showCallDetail_click() {
    if (agentEvent != null && agentEvent != undefined && agentEvent.customer_id != null && agentEvent.customer_id != undefined) {
        $("[data-key=" + agentEvent.customer_id + "]").click();
        showAgentDetail = false;
    }
}

//function changeStatusHotline(thiss) {
//    try {
//        //statusHotline(true, thiss, "");
//    } catch (e) { console.log(e); }
//}
//wz-status-phone">Online phone
//function statusHotline(change, thiss, status) {
//    try {
//        var jsStatus = $(".wz-col-sideright").find(".wz-status-phone");
//        console.log(jsStatus);
//        if (change != null && change != undefined && change) {
//            if (jsStatus.text() == "ONLINE PHONE") {            
//                setStatusHotline("offline", "OFFLINE PHONE");
//            }
//            else {
//                setStatusHotline("online", "ONLINE PHONE");
//            }
//        }
//        else {
//            if (status != null && status != undefined && status == "online") {
//                jsStatus.text("ONLINE PHONE");
//            }
//            else {
//                jsStatus.text("OFFLINE PHONE");
//            }
//        }
//    } catch (e) { console.log(e); }
//}

function setHotline(status) {
    ExecuteServiceSyns(baseUrls_ApiHotline + "api/PhoneAccounts/changeStatus/" + myID + "/" + status+"/?access_token=" + accessToken,
        "GET", "setHotline_result", true);
}

function setHotline_result(list) {

}

function statusHotline() {
    ExecuteServiceSyns(baseUrls_ApiHotline + "api/PhoneAccounts/getPhoneStatus/" + myID +"/?access_token=" + accessToken,
        "GET", "statusHotline_result", true);
    //$(".wz-col-sideright").find(".wz-status-phone").text(text);
}

function statusHotline_result(list) {
    try {
        var data = JSON.parse(list);
        hotline_drawAgent(data);
        //var data = JSON.parse(list);
        //if (data != null && data != undefined && (data.IsModifiedCountAvailable || data.MatchedCount > 0 || data.ModifiedCount > 0)) {
        //    document.getElementsByClassName('phone-container')[0].src = document.getElementsByClassName('phone-container')[0].src
        //    new NotificationPlugin("Ok", "Change ok", "icon-bell");
        //}
        //else
        //    new NotificationPlugin("Error", "Change error", "icon-bell");

    } catch (e) { console.log(e); }
}
