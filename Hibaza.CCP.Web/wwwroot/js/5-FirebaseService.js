
FirebaseService = function (e) {
    try {
        var i = e.child("threads-messages"),
            n = limitMessages;
        var updateCounters = function (t, u) {
            //  if (!t.val().active_thread) return;
            var i = Object.keys(CONTACTS).length,
                n = t.key,
                data = JSON.parse(t.val().active_thread),
                o = CONTACTS[n];
            //if (data.customer_id == _customer_id && data.channel_ext_id != data.sender_ext_id) {
            //    addSessionBot(data.customer_id, data.last_message, data.channel_ext_id, "webhook","ManualAgents");
            //}
            
            data.agent_id = t.val().agent_id;
            var k = currentChat == n && CONTACTS[currentChat] ? CONTACTS[currentChat].getKey() : data.id;
            THREADS[data.id] = new Contact(data);
            data.unread = t.val().unread, data.nonreply = t.val().nonreply, data.open = t.val().open;
            CONTACTS[n] = new Contact(data);
            CONTACTS[n].setKey(k);
            0 == contactList.find(".js-contact-" + n).size() && currentChat == n && ("client" == viewProfile ? (cloud.getCustomerProfile(n), currentTicket && cloud.getTicketProfile(currentTicket)) : "ticket" == viewProfile && (cloud.getCustomerProfile(n), cloud.getTicketProfile(viewTicketSelected, void 0, !0)), cloud.getLastTicket(n));
            //0 == contactList.find(".js-contact-" + n).size() ? -1 == CONTACTS[n].getChatviews().indexOf(chatView) || chatViewChannel && chatViewChannel != CONTACTS[n].channel_id || chatViewAgent && chatViewAgent != CONTACTS[n].agent ? CONTACTS[n].refresh() : (CONTACTS[n].render(), CONTACTS[n].refresh()) : CONTACTS[n].refresh();
            CONTACTS[n].render() && CONTACTS[n].refresh();
            //currentChat == n && (0 == $(document).find(".js-list-conversations .js-contact-" + CONTACTS[n].getKey()).size() ? (THREADS[CONTACTS[n].getKey()].profile_render(), THREADS[CONTACTS[n].getKey()].profile_refresh()) : THREADS[CONTACTS[n].getKey()].profile_refresh());
            currentChat == n && THREADS[CONTACTS[n].getKey()] && THREADS[CONTACTS[n].getKey()].profile_render() && THREADS[CONTACTS[n].getKey()].profile_refresh();

            currentChat == n && $(document).find(".js-list-conversations .js-contact-" + CONTACTS[n].getKey()).addClass("active")
                && $(".js-list-contacts .js-contact-" + currentChat).addClass("active");


            var a = CONTACTS[n].getChannel() || 0,
                r = CONTACTS[n].getAgent(),
                s = CONTACTS[n].isUnread();
            if ("undefined" != typeof o) {
                var l = o.getChannel() || 0,
                    c = o.getAgent(),
                    d = o.isUnread();
                l != a && (deleteDictKey(COUNTERS.channels[l], n), addDictKey(COUNTERS.channels[a], n)), c != r && (c && deleteDictKey(COUNTERS.agents[c], n), r && addDictKey(COUNTERS.agents[r], n), s && (c && !r ? (deleteDictKey(COUNTERS.attention_unread, n), addDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)))), d != s ? d ? (deleteDictKey(COUNTERS.channels_unread[l], n), deleteDictKey(COUNTERS.attention_unread, n), deleteDictKey(COUNTERS.pending_unread, n)) : (addDictKey(COUNTERS.channels_unread[a], n), r ? addDictKey(COUNTERS.attention_unread, n) : addDictKey(COUNTERS.pending_unread, n)) : c != r && s && (c ? r || (addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n)) : (deleteDictKey(COUNTERS.pending_unread, n), addDictKey(COUNTERS.attention_unread, n)))
            } else {
                u ? (addDictKey(COUNTERS.channels[a], n), r && addDictKey(COUNTERS.channels[a], n), 0 == i && contactList.find(".js-contact-" + n).trigger("click")) : ($.each(Object.keys(COUNTERS.channels), function (t, e) {
                    deleteDictKey(COUNTERS.channels[e], n)
                }), addDictKey(COUNTERS.channels[a], n), $.each(Object.keys(COUNTERS.agents), function (t, e) {
                    deleteDictKey(COUNTERS.agents[e], n)
                }), r && addDictKey(COUNTERS.agents[r], n), $.each(Object.keys(COUNTERS.channels_unread), function (t, e) {
                    deleteDictKey(COUNTERS.channels_unread[e], n)
                }), s && addDictKey(COUNTERS.channels_unread[a], n), deleteDictKey(COUNTERS.pending_unread, n), !r && s && addDictKey(COUNTERS.pending_unread, n), deleteDictKey(COUNTERS.attention_unread, n), r && s && addDictKey(COUNTERS.attention_unread, n))
            }
            cloud.refreshCounters();
            sortContacts();
            $(CONTACTS).size() > 0 && disableTutorial();

        };
    } catch (exx) { console.log(exx); }
    this.getRef = function () {
        return i
    }, this.getMessage = function (t, e) {
        var n, o = new $.Deferred;
        return $.get(self.urls.messages.get + businessID + '/' + e, function (t) {
            o.resolve(t)
        }), o.promise()
    }, this.getAttach = function (t, e) {
        var n = new $.Deferred;
        return $.get(self.urls.messages.get + businessID + '/' + e, function (t) {
            n.resolve(t)
        }), n.promise()
    }, this.deleteOptionMessages = function (t) {
        var e = "number" == typeof t ? t : currentChat,
            n = new $.Deferred;
        return e ? i.child(e).child("messages").once("value", function (t) {
            t.forEach(function (t) {
                "bot" == t.val().type && "openTicket" == t.val().template && t.ref.remove()
            }), n.resolve()
        }) : n.reject(), n.promise()
    }, this.setChat = function (t, k, e) {
        var o = new $.Deferred,
            a = this,
            r = function (e) {
                var i = null,
                    n = 0;
                e.forEach(function (t) {
                    a.loadMessage(t), i = t.val().timestamp + .1, n += 1;
                }), 0 == n || limitMessages > n ? fillerMessageList.hide() : fillerMessageList.show(), $centralScroll.perfectScrollbar("update"), o.resolve(),
                    currentChatRef.orderByChild("timestamp").startAt(i).limitToLast(limitMessages).on("child_added", function (e) {
                        firstMessageToScroll = !1, a.loadMessage(e), "object" == typeof CONTACTS[t] && cloud.setReaded(t)
                    }),
                    currentChatRef.on("child_removed", function (t) {
                        var e = $('.js-list-chat .js-chat-message[data-key="' + t.key + '"]');
                        if (e.size() > 0) {
                            var i = getClassByName(e, /^js-day-/),
                                n = e.hasClass("js-chat-groupped");
                            n || e.next().removeClass("js-chat-groupped"), e.remove(), $sameDayMessages = $(".js-list-chat ." + i), 1 == $sameDayMessages.size() && $sameDayMessages.remove()
                        }
                    })
                //,currentChatRef.on("child_changed", function (t) {
                //   currentChat == t.val().thread_id && "client" == viewProfile && cloud.getCustomerProfile(t.val().thread_id);
                //})
            };
        var lm = function (r, k, n, f) {
            $.ajax({
                url: self.urls.messages.list + businessID + '/' + k,
                type: "POST",
                data: JSON.stringify({ 'first': f, 'quantity': n, 'search': '', 'channel_id': '' }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (res) {
                    var l = [];
                    res.data && res.data.forEach(function (ee) {
                        var e = {
                            key: ee.id,
                            val: function () {
                                return ee;
                            }

                        }
                        l.push(e);
                    });
                    r(l);
                }
            });
        };
        //if ("object" == typeof CONTACTS[t] && CONTACTS[t].isUnread() && cloud.setReaded(t), currentChat != t || e) {
        if ("object" == typeof CONTACTS[t]) {
            
            k = k || CONTACTS[t].getKey();

            CONTACTS[t].setKey(k);
            cloud.getCustomerProfile(t);

            $(".js-list-contacts .js-contact-" + currentChat).removeClass("active");
            CONTACTS[t].refresh(), $(".js-list-contacts .js-contact-" + t).addClass("active");

            currentChatRef && currentChatRef.off(), currentChat = t, currentChatRef = i.child(k).child("messages");
            chatList.html(""), phoneInput.val(THREADS[k].getSender()), channelInput.val(THREADS[k].getChannel()), threadInput.val(k), MESSAGES = [], n = limitMessages, channelType.val(CONTACTS[t].getChannelType().toLowerCase());
            var s = 9999999999;
            //firstMessageToScroll = !1, e && (s = e), 9999999999 > s ? currentChatRef.orderByChild("timestamp").startAt(s).once("value", function (t) {
            firstMessageToScroll = !1, e && (s = e), 9999999999 > s ? lm(function (t) {
                n = 0, t.forEach(function (t) {
                    0 == n && (firstMessageToScroll = t.key), n += 1
                }), limitMessages > n && (n = limitMessages), lm(r, k, n, 0) //currentChatRef.orderByChild("timestamp").limitToLast(n).once("value", r)
            }, k, limitMessages, s) : lm(r, k, n, 0) //currentChatRef.orderByChild("timestamp").limitToLast(n).once("value", r)
            //firstMessageToScroll = !1, e && (s = e), s = 9999999999 > s ? s : 0;
            //lm(r, k, n, s);

        } else $(".js-list-contacts .js-contact-" + t).addClass("active")
        return o.promise()
    }, this.loadMessage = function (t) {
        if (MESSAGES[t.key] == undefined || MESSAGES[t.key].length == 0) {
            MESSAGES[t.key] = new Message(t);
            var e = dateFromTimestamp(t.val().timestamp),
                i = new Date(formatShortDate(e, !0)),
                n = "js-day-" + formatShortDate(e, !0).replace(/\//g, "-"),
                o = i.getTime() / 1e3;
            0 === $(".js-chat-date-subhead." + n).size() && chatList.append('<li class="list-group-item js-chat-date-subhead ' + n + '" data-day="' + formatShortDate(e, !0) + '" data-timestamp="' + o + '"><div class="wz-date-day">' + formatShortDate(i) + "</div></li>"), MESSAGES[t.key].render(), scrollMessages(), this.sortMessages("." + n)
        }
    },
        this.registerUpdates = function () {

        e.child("customers-counters-unread").child("customers-counters-unread-count").on("child_changed", function (t) {
           
                cloud.loadCounters(t.val());
            });

        e.child("customers-counters-unread").child("customers-counters-unread-count").on("child_added", function (t) {
        
                cloud.loadCounters(t.val());
            });

        e.child("customers-counters-unread").child("customers-counters-unread-count").on("child_removed", function (t) {
           
                cloud.loadCounters(t.val());
            });

        e.child("customers").orderByChild("timestamp").limitToLast(limitContacts).on("child_changed", function (t) {
           
                if (inputFinder2.val().length == 0) updateCounters(t, false);
            });

        e.child("customers").orderByChild("timestamp").startAt(new Date().getTime() / 1000).limitToLast(limitContacts).on("child_added", function (t) {
            
                if (inputFinder2.val().length == 0) updateCounters(t, true);
            });

        e.child("threads").orderByChild("timestamp").limitToLast(limitContacts).on("child_changed", function (t) {
            
                THREADS[t.key] = new Contact(t.val());
                CONTACTS[currentChat] && THREADS[t.key].setAgent(CONTACTS[currentChat].getAgent());
                currentChat == THREADS[t.key].getData().id && (0 == $(document).find(".js-list-conversations .js-contact-" + t.key).length ? (THREADS[t.key].profile_render(), noCustomersViewHandler()) : THREADS[t.key].profile_refresh());
            });

        e.child("threads").orderByChild("timestamp").limitToLast(limitContacts).on("child_added", function (t) {
            
                THREADS[t.key] = new Contact(t.val());
                CONTACTS[currentChat] && THREADS[t.key].setAgent(CONTACTS[currentChat].getAgent());
                currentChat == THREADS[t.key].getData().id && (0 == $(document).find(".js-list-conversations .js-contact-" + t.key).length ? (THREADS[t.key].profile_render(), noCustomersViewHandler()) : THREADS[t.key].profile_refresh());
            });

        e.child("agents").on("child_changed", function (t) {
           
                h = t.key;
                f = t.val().status;
                refreshAgentStatus(h, f);
            });

        e.child("agents").on("child_added", function (t) {
            
                h = t.key;
                f = t.val().status;
                refreshAgentStatus(h, f);
            });
        }
};

FirebaseService.prototype.setMainChat = function (t, k) {
    this.setChat(t, k, null), txt.focus(), checkNote.hasClass("js-checked") && checkNote.click(), tooltips(), loadOldMessages(), closeContact.trigger("click");



}, FirebaseService.prototype.setGrouped = function (t) {
    if (lastMessage) {
        var e = $(t).data("timestamp"),
            i = $(lastMessage).data("timestamp"),
            n = e - i,
            o = dateFromTimestamp(e),
            a = dateFromTimestamp(i),
            r = formatShortDate(o, !0),
            s = formatShortDate(a, !0),
            l = $(t).data("author"),
            c = $(lastMessage).data("author"),
            d = $(t).hasClass("js-chat-zami"),
            u = $(lastMessage).hasClass("js-chat-zami"),
            h = e >= i,
            f = l === c,
            p = limitTimestampSameGroup >= n,
            g = r === s,
            m = d === u;
        //h && f && p && g && m ? $(t).addClass("js-chat-groupped") : $(t).removeClass("js-chat-groupped")
        h && f ? $(t).addClass("js-chat-groupped") : $(t).removeClass("js-chat-groupped")
    } else $(t).removeClass("js-chat-groupped");
    lastMessage = t
}, FirebaseService.prototype.sortMessages = function (t, e) {

    function i(t, e) {
        return $(e).data("timestamp") < $(t).data("timestamp") ? 1 : -1
    }
    t = t ? t : "", $selector = $(".js-list-chat li" + t);
    var n = this,
        o = $selector.size(),
        a = $(".js-list-chat li").size();
    e && o != a ? $selector.sort(i).insertBefore(e) : $selector.sort(i).appendTo(".js-list-chat"), lastMessage = !1, $(".js-list-chat li.js-chat-message" + t).each(function (t, e) {
        CONTACTS[currentChat] && THREADS[CONTACTS[currentChat].getKey()] && THREADS[CONTACTS[currentChat].getKey()].getType() === "message" && n.setGrouped(e)
    })
}, FirebaseService.prototype.getOlder = function () {

    var t = new $.Deferred;
    return $renderBefore = $(".js-list-chat li.js-chat-message").first(), dateFirst = $renderBefore.data("timestamp"), relativeFirst = !0,
        $.ajax({
            url: self.urls.messages.list + businessID + '/' + CONTACTS[currentChat].getKey(),
            type: "POST",
            data: JSON.stringify({ 'first': dateFirst, 'quantity': limitMessages, 'search': '', 'channel_id': '' }),
            contentType: 'application/json',
            dataType: 'json',
            success: function (res) {
                e = res.data;
                var i = 0,
                    n = 0;
                e && e.forEach(function (t) {
                    t.id in MESSAGES || (n += 1)
                }), n > 0 ? e && e.forEach(function (ee) {
                    var e = {
                        key: ee.id,
                        val: function () {
                            return ee;
                        }
                    }
                    if (!(e.key in MESSAGES)) {
                        MESSAGES[e.key] = new Message(e);
                        var o = !1,
                            a = dateFromTimestamp(e.val().timestamp),
                            r = new Date(formatShortDate(a, !0)),
                            s = "js-day-" + formatShortDate(a, !0).replace(/\//g, "-"),
                            l = r.getTime() / 1e3;
                        $(".js-list-chat li." + s).size() > 0 ? $renderBefore = $(".js-list-chat li." + s).first() : $renderBefore = $(".js-list-chat li").first();
                        var c = getClassByName($renderBefore, /^js-day-/),
                            d = new Date($renderBefore.data("day"));
                        if (d > r) o = $(".js-list-chat li." + c).first(), 0 === o.size() && (o = !1);
                        else {
                            var u = $(".js-list-chat li." + c).last().data("key");
                            for (o = $('.js-list-chat li[data-key="' + u + '"]').next(); o.size() > 0 && new Date(o.data("day")) < r;) {
                                var h = getClassByName(o, /^js-day-/),
                                    f = $(".js-list-chat li." + h).last().data("key");
                                o = $('.js-list-chat li[data-key="' + f + '"]').next()
                            }
                            0 === o.size() && (o = $(".js-list-chat li").last())
                        }
                        0 === $(".js-chat-date-subhead." + s).size() && $renderBefore.before('<li class="list-group-item js-chat-date-subhead ' + s + '" data-day="' + formatShortDate(a, !0) + '" data-timestamp="' + l + '"><div class="wz-date-day">' + formatShortDate(r) + "</div></li>"), MESSAGES[e.key].renderBefore($renderBefore), this.sortMessages("." + s, o), i += 1, i === n && t.resolve()
                    }
                }.bind(this)) : t.resolve(), $(".js-list-chat li.js-chat-message:first").removeClass("js-chat-groupped")
            }.bind(this)
        }), t.promise()
},
    FirebaseService.prototype.likeMessage = function (t, e) {
        return "object" == typeof CONTACTS[currentChat] && CONTACTS[currentChat].getAgent() != myID && false ? void introJsAssignate() :
            void MESSAGES[t].setLiked(!MESSAGES[t].getLiked(), function (r) {
                if (r.ok) {
                    MESSAGES[t].getLiked() ? $(e).text("Unlike") : $(e).text("Like"), mixpanelEvents(MESSAGES[t].getLiked() ? "likedMessage" : "unlikedMessage")
                }
            })
    },
    FirebaseService.prototype.starMessage = function (t) {
        return "object" == typeof CONTACTS[currentChat] && CONTACTS[currentChat].getAgent() != myID && false ? void introJsAssignate() :
            void MESSAGES[t].setStarred(!MESSAGES[t].getStarred(), function (e) {
                if ("object" == typeof CONTACTS[currentChat]) {
                    if (currentChat == MESSAGES[t].getCustomer() && "client" == viewProfile) cloud.getCustomerProfile(MESSAGES[t].getCustomer());
                    var i = !1,
                        n = chatList.find("li[data-key=" + t + "] .wz-starred");
                    MESSAGES[t].getStarred() ? (n.addClass("selected"), currentTicket && (i = cloud.saveTicketAttach(currentTicket, 0, t))) : (n.removeClass("selected"), currentTicket && (i = cloud.deleteTicketAttach(currentTicket, 0, t))), i && i.fail(function () {
                        console.log("[Firebase starMessage] Error: Save ticket attachment failed!")
                    }), mixpanelEvents(MESSAGES[t].getStarred() ? "starredMessage" : "unstarredMessage")
                }
            })
    };

//FirebaseService.prototype.starMessage = function (t) {
//    return "object" == typeof CONTACTS[currentChat] && CONTACTS[currentChat].getAgent() != myID ? void introJsAssignate() : 
//        void currentChatRef.child(t).child("starred").set(!MESSAGES[t].getStarred(), function (e) {
//        if (e) console.log("[Firebase starMessage] Error: Synchronization failed!");
//        else {
//            var i = !1,
//                n = chatList.find("li[data-key=" + t + "] .wz-starred");
//            MESSAGES[t].setStarred(!MESSAGES[t].getStarred()), MESSAGES[t].getStarred() ? (n.addClass("selected"), currentTicket && (i = cloud.saveTicketAttach(currentTicket, 0, t))) : (n.removeClass("selected"), currentTicket && (i = cloud.deleteTicketAttach(currentTicket, 0, t))), i && i.fail(function () {
//                console.log("[Firebase starMessage] Error: Save ticket attachment failed!")
//            }), mixpanelEvents(MESSAGES[t].getStarred() ? "starredMessage" : "unstarredMessage")
//        }
//    })
//};