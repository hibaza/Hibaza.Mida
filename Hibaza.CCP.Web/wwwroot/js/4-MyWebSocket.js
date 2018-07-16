


//var lastChatView = "all", chatView = "all", chatViewAgent = !1, chatViewChannel = !1, currentChat = !1, currentChatRef = !1, currentTicket = !1, currentAgent = !1, viewProfile = "client", viewClientOption = "info", viewTicketSelected = !1, viewPaymentSelected = !1, viewAgentSelected = !1, viewAgentOption = "activity", closeEvent = !0, recordingAudio = !1, settingsView = "info", timeoutActivation = !1, reportManager = !1, dashboardManager = !1, lastMessageScroll = 0, howManyMessages = 0, lastMessageToScroll = !1, timeoutUiAutoLoadMessages = !1, timeoutUiAutoScrollMessages = !1, intervalUiAutoScrollMessages = !1, firstMessageToScroll = !1, shortcutsFocused = !1, typeareaFocused = !1, imgSendFlags = [], preloaderEnded = !1, currentLimitContacts = limitContacts, timeoutUiAutoLoadContacts = !1, contactStatusView = "all", lastContactScroll = 0;


//var NP_PAYMENT = {
//        title: trPaymentSent,
//        message: trWeVeSent,
//        icon: "icon-mail",
//        countdown: null
//    },
//    NP_DATA_SAVE = {
//        title: trSaved,
//        message: trDataSaved,
//        icon: "icon-check",
//        countdown: defNotificationTimer
//    };
//jQuery.expr[":"].Contains = jQuery.expr.createPseudo(function(t) {
//    return function(e) {
//        return jQuery(e).text().toUpperCase().indexOf(t.toUpperCase()) >= 0
//    }
//}), String.prototype.capitalizeFirstLetter = function() {
//    return this.charAt(0).toUpperCase() + this.slice(1)
//};
//var deleteDictKey = function(t, e) {
//        "undefined" != typeof t && "undefined" != typeof t[e] && delete t[e]
//    },
//    addDictKey = function(t, e) {
//        "undefined" != typeof t && (t[e] = !0)
//    };
//$.fn.scrollBottom = function() {
//    return this.scrollTop() + this.height()
//}, $.fn.refresh = function() {
//    return $(this.selector)
//};
//var messageZamiTimestamp = !1,
//    messageZamiLastChat = !1;


//SocketService = function (t) {

//    //var e = new Firebase(t);
//    //e.authWithCustomToken(firebaseToken, function (t, e) {
//    //    if (t) console.log("Login Failed!", t);
//    //    else {
//    //        var i = new Date(1e3 * e.expires),
//    //            n = new Date,
//    //            o = i.getTime() - n.getTime();
//    //        setTimeout(function () {
//    //            new NotificationPlugin(trExpiredAuth, trRefresh, "icon-clock")
//    //        }, o)
//    //    }
//    //});


//    firebase.auth().signInWithCustomToken(firebaseToken).catch(function (error) {
//        // Handle Errors here.
//        var errorCode = error.code;
//        var errorMessage = error.message;
//        // ...
//    });

//    firebase.auth().onAuthStateChanged(function (user) {
//        if (user) {
//            // User signed in!
//            var uid = user.uid;
//            var i = new Date(1e3 * e.expires),
//                n = new Date,
//                o = i.getTime() - n.getTime();
//            setTimeout(function () {
//                new NotificationPlugin(trExpiredAuth, trRefresh, "icon-clock")
//            }, o)
//        } else {
//            // User logged out
//        }
//    })

//    var e = firebase.database().ref();
//    var n = limitMessages;

//    var updateCounters = function (t, u) {
//        var i = Object.keys(CONTACTS).length,
//            n = t.key,
//            data = t.val(),
//            o = CONTACTS[n];
//        CONTACTS[n] = new Contact(data), 0 == contactList.find(".js-contact-" + n).size() ? -1 == CONTACTS[n].getChatviews().indexOf(chatView) || chatViewChannel && chatViewChannel != CONTACTS[n].channel_id || chatViewAgent && chatViewAgent != CONTACTS[n].agent ? CONTACTS[n].refresh() : (CONTACTS[n].render(), noCustomersViewHandler()) : CONTACTS[n].refresh();
//        var a = CONTACTS[n].getChannel() || 0,
//            r = CONTACTS[n].getAgent(),
//            s = CONTACTS[n].isUnread();
//        if ("undefined" != typeof o) {
//            var l = o.getChannel() || 0,
//                c = o.getAgent(),
//                d = o.isUnread();
//            l != a && (deleteDictKey(COUNTERS.channels[l], n), addDictKey(COUNTERS.channels[a], n)), c != r && (c && deleteDictKey(COUNTERS.agents[c], n), r && addDictKey(COUNTERS.agents[r], n), s && (c && !r ? (deleteDictKey(COUNTERS.attention_unread, n), addDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)))), d != s ? d ? (deleteDictKey(COUNTERS.channels_unread[l], n), deleteDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.channels_unread[a], n), r ? addDictKey(COUNTERS.attention_unread, n) : addDictKey(COUNTERS.pending_unread, n)) : c != r && s && (c ? r || (addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n)) : (deleteDictKey(COUNTERS.pending_unread, n), addDictKey(COUNTERS.attention_unread, n)))
//        } else {
//            u ? (addDictKey(COUNTERS.channels[a], n), r && addDictKey(COUNTERS.channels[a], n), 0 == i && contactList.find(".js-contact-" + n).trigger("click")) : ($.each(Object.keys(COUNTERS.channels), function (t, e) {
//                deleteDictKey(COUNTERS.channels[e], n)
//            }), addDictKey(COUNTERS.channels[a], n), $.each(Object.keys(COUNTERS.agents), function (t, e) {
//                deleteDictKey(COUNTERS.agents[e], n)
//            }), r && addDictKey(COUNTERS.agents[r], n), $.each(Object.keys(COUNTERS.channels_unread), function (t, e) {
//                deleteDictKey(COUNTERS.channels_unread[e], n)
//            }), s && addDictKey(COUNTERS.channels_unread[a], n), deleteDictKey(COUNTERS.pending_unread, n), !r && s && addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n), r && s && addDictKey(COUNTERS.attention_unread, n))
//        }
//        cloud.refreshCounters(), sortContacts(), currentChat == n && ("client" == viewProfile ? (cloud.getCustomerProfile(n), currentTicket && cloud.getTicketProfile(currentTicket)) : "ticket" == viewProfile && (cloud.getCustomerProfile(n), cloud.getTicketProfile(viewTicketSelected, void 0, !0)), cloud.getLastTicket(n)), $(CONTACTS).size() > 0 && disableTutorial();
//    }

//    e.child("counters").on("child_changed", function (t) {
//        cloud.loadCounters(t.val());
//    });

//    e.child("counters").on("child_added", function (t) {
//        cloud.loadCounters(t.val());
//    });

//    e.child("customers").on("child_changed", function (t) {
//        updateCounters(t, false);
//    });

//    e.child("customers").on("child_added", function (t) {
//        updateCounters(t, true);
//    });

//    e.child("agents").on("child_changed", function (t) {
//        h = t.key;
//        f = t.val().status;
//        refreshAgentStatus(h, f);
//    });

//    e.child("agents").on("child_added", function (t) {
//        h = t.key;
//        f = t.val().status;
//        refreshAgentStatus(h, f);
//    });
//};


//MyWebSocket = function () {
//    var t, e = !0;
//    t = "https:" == window.location.protocol ? "wss://" + window.location.host + ":4445/ws/stream/" : "ws://localhost:4445/ws/stream/", self.socket = new ReconnectingWebSocket(t), self.socket.onmessage = function (t) {
//        var e = JSON.parse(t.data);
//        switch (e.ws_type) {
//            case "counters":
//                cloud.loadCounters(e.ws_data.data);
//                break;
//            case "notification":
//                processNotification(e.ws_data);
//                break;
//            case "customer-updated":
//                var i = CONTACTS.length,
//                    n = e.ws_data.data.id,
//                    o = CONTACTS[n];
//                CONTACTS[n] = new Contact(e.ws_data.data), 0 == contactList.find(".js-contact-" + n).size() ? -1 == CONTACTS[n].getChatviews().indexOf(chatView) || chatViewChannel && chatViewChannel != CONTACTS[n].channel_id || chatViewAgent && chatViewAgent != CONTACTS[n].agent ? CONTACTS[n].refresh() : (CONTACTS[n].render(), noCustomersViewHandler()) : CONTACTS[n].refresh();
//                var a = CONTACTS[n].getChannel() || 0,
//                    r = CONTACTS[n].getAgent(),
//                    s = CONTACTS[n].isUnread();
//                if ("undefined" != typeof o) {
//                    var l = o.getChannel() || 0,
//                        c = o.getAgent(),
//                        d = o.isUnread();
//                    l != a && (deleteDictKey(COUNTERS.channels[l], n), addDictKey(COUNTERS.channels[a], n)), c != r && (c && deleteDictKey(COUNTERS.agents[c], n), r && addDictKey(COUNTERS.agents[r], n), s && (c && !r ? (deleteDictKey(COUNTERS.attention_unread, n), addDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)))), d != s ? d ? (deleteDictKey(COUNTERS.channels_unread[l], n), deleteDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.channels_unread[a], n), r ? addDictKey(COUNTERS.attention_unread, n) : addDictKey(COUNTERS.pending_unread, n)) : c != r && s && (c ? r || (addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n)) : (deleteDictKey(COUNTERS.pending_unread, n), addDictKey(COUNTERS.attention_unread, n)))
//                } else {
//                    var u = e.ws_data.is_new;
//                    u ? (addDictKey(COUNTERS.channels[a], n), r && addDictKey(COUNTERS.channels[a], n), 0 == i && contactList.find(".js-contact-" + n).trigger("click")) : ($.each(Object.keys(COUNTERS.channels), function (t, e) {
//                        deleteDictKey(COUNTERS.channels[e], n)
//                    }), addDictKey(COUNTERS.channels[a], n), $.each(Object.keys(COUNTERS.agents), function (t, e) {
//                        deleteDictKey(COUNTERS.agents[e], n)
//                    }), r && addDictKey(COUNTERS.agents[r], n), $.each(Object.keys(COUNTERS.channels_unread), function (t, e) {
//                        deleteDictKey(COUNTERS.channels_unread[e], n)
//                    }), s && addDictKey(COUNTERS.channels_unread[a], n), deleteDictKey(COUNTERS.pending_unread, n), !r && s && addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n), r && s && addDictKey(COUNTERS.attention_unread, n))
//                }
//                cloud.refreshCounters(), sortContacts(), currentChat == n && ("client" == viewProfile ? (cloud.getCustomerProfile(n), currentTicket && cloud.getTicketProfile(currentTicket)) : "ticket" == viewProfile && (cloud.getCustomerProfile(n), cloud.getTicketProfile(viewTicketSelected, void 0, !0)), cloud.getLastTicket(n)), CONTACTS.length > 0 && disableTutorial();
//                break;
//            case "update-agent-status":
//                var h = e.ws_data.data.id,
//                    f = e.ws_data.data.status;
//                refreshAgentStatus(h, f);
//                break;
//            case "smooch":
//                setSmoochToken(e.ws_data.token);
//                break;
//            case "channel-updated":
//                "qr-scanned" == e.ws_data.type && $(".js-modal-qrcode-close").click();
//                break;
//            default:
//                console.log("Websocket wrong type: " + e)
//        }
//    }, self.socket.onopen = function () {
//        console.log("Connected to websocket"), disconnectionAlert.html(""), e || cloud.getCounters().done(function () {
//            $sideLeft1.find(".js-chats-select .badge").removeClass("loading")
//        }), cloud.changeStatus(onlineStatus).done(firstLoadAgents), e = !1
//    }, self.socket.onclose = function () {
//        console.log("Disconnected from websocket"), e || (cloud.changeStatus(onlineStatus), $sideLeft1.find(".js-chats-select .badge").addClass("loading"), disconnectionAlert.html(disconnectionTemplate.tmpl({})))
//    }
//};

//MyWebSocket.prototype.disconnect = function () {
//    self.socket.close()
//},

//MyWebSocket.prototype.send = function (t) {
//    self.socket.send(JSON.stringify(t))
//}



