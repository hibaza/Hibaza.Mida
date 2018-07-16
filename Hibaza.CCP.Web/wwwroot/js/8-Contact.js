Contact = function (t) {
    var k = t.id,
        e = t.customer_id,
        i = t.owner_name,
        n = t.owner_avatar != null && t.owner_avatar != undefined &&
            t.owner_avatar.indexOf("http://") < 0 && t.owner_avatar.indexOf("https://") < 0 ? baseUrls_Api + t.owner_avatar : t.owner_avatar || "",
        o = t.phone || "",
        a = t.status,
        r = t.channel_type || "facebook",
        s = t.channel_id,
        y = !t.nonreply,
        l = t.unread,
        op = t.open,
        c = t.agent_id,
        b = t.sender_name,
        q = t.owner_ext_id,
        w = t.last_message_ext_id,
        x = t.type,
        d = t.archived,
        u = t.timestamp,
        p = t.last_message || "",
        h = t.created_date,
        f = t.role === "bot" ? t.role : "";

   // console.log("1111111111111");
    this.getSender = function () {
        return x === "comment" ? "" : q;
    },
        this.setKey = function (t) {
            k = t
        },
        this.getKey = function () {
            return k
        }, this.getData = function () {
            return {
                id: e,
                type: x,
                name: i,
                avatar: n,
                phone: o,
                status: a,
                channel: r,
                channel_id: s,
                unread: l,
                agent: c,
                sender: q,
                archived: d,
                last_message: p,
                last_message_id: w,
                created_date: h
            }
        }, this.getRoninStatus = function () {
            return ""; // f
        }, this.setRoninStatus = function (t) {
            f = t, this.refresh(["button", "archive_btn", "status", "input"])
        }, this.getChannel = function () {
            return s
        }, this.getRegisterDate = function () {
            return h
        }, this.getChannelType = function () {
            return r || "lock";
        }, this.getType = function () {
            return x;
        }, this.getName = function () {
            return i ? i : o
        }, this.getLastSenderName = function () {
        return this.getName();
            //return y === true ? (b || this.getChannelName()) : this.getName();
        }, this.getMessage = function () {
            return p
        }, this.getInitials = function () {
            return getInitials(i)
        }, this.getAvatar = function () {
            return n ? n : ""
        }, this.getPhone = function () {
            return o ? o : ""
        }, this.isArchived = function () {
            return d
        }, this.setArchived = function (t) {
            if ("boolean" == typeof t) {
                d = t;
                var i = function () {
                    d && this.isUnread() && this.setUnread(!1), d && this.getAgent() ? this.setAgent(null) : this.refresh(["button", "archive_btn", "status", "input"])
                };
                d && this.isUnread() ? cloud.setReaded(k).done(i.bind(this)) : i.bind(this)
            }
        }, this.getStatus = function () {
            return a || "intro"
        }, this.getAgent = function () {
            return c
        }, this.getAgentName = function () {
            return AGENTS[c] && AGENTS[c].name ? AGENTS[c].name : "";
        }, this.setAgent = function (t) {
            var i = c;
            c = t, c && this.isArchived() ? this.setArchived(!1) : this.refresh(["button", "archive_btn", "status", "input"]), i != c && (i && deleteDictKey(COUNTERS.agents[i], e), c && addDictKey(COUNTERS.agents[c], e), l && (i && !c ? (deleteDictKey(COUNTERS.attention_unread, e), addDictKey(COUNTERS.pending_unread, e)) : (addDictKey(COUNTERS.attention_unread, e), deleteDictKey(COUNTERS.pending_unread, e)))),
                cloud.refreshCounters()
        }, this.setReplied = function (t) {
            y = t;
        }, this.nonReplied = function () {
            return !y;
        }, this.isOpen = function () {
            return op
        }, this.isUnread = function () {
            return l
        }, this.setUnread = function (t) {
            if ("boolean" == typeof t) {
                l = t;
                //l || (deleteDictKey(COUNTERS.channels_unread[s], e), deleteDictKey(COUNTERS.attention_unread, e), deleteDictKey(COUNTERS.pending_unread, e));
                this.refresh(["button", "unread", "status", "input"]);
            }
        }, this.getLast = function () {
            return u
        }, this.getLastMessageExtId = function () {
            return w
        }, this.getRenderedData = function () {
            return formatShortDatetime(dateFromTimestamp(u))
        }, this.getUnreadClass = function () {
            return this.isUnread() ? "js-unread" : ""
        }, this.getContactButton = function () {
            var t;
            var a = this.getAgent();
            return t = this.isArchived() ? {
                "class": "btn btn-warning",
                "function": "CONTACTS['" + this.getKey() + "'].unArchive();",
                text: trOpen
            } : this.getRoninStatus() ? {
                "class": "btn-chatbot",
                "function": "",
                text: ""
            } : this.getAgent() == myID ? {
                "class": "btn btn-info",
                "function": "CONTACTS['" + e + "'].unassignMe();",
                text: trUnassign
            } : a && a != 'ALL' && AGENTS[a] ? {
                "class": "btn btn-primary",
                "function": "",
                text: AGENTS[a].name
            } : {
                                "class": "btn btn-info",
                                "function": "CONTACTS['" + e + "'].assignMe();introJs().exit()",
                                text: trAssign
                            }
        }, this.assignTo = function (t) {
            var i = this;
            cloud.assignCustomerTo(t, e).fail(showDefaultErrorNotification).done(function (r) {
                r && i.setAgent(t)
            })
        }, this.assignMe = function () {
            var t = this;
            cloud.assignCustomer(e).fail(showDefaultErrorNotification).done(function (r) {
                r && t.setAgent(myID)
            })
        }, this.unassignMe = function () {
            var t = this;
            return cloud.unassignCustomer(e).fail(showDefaultErrorNotification).done(function () {
                t.setAgent(null)
            })
        }, this.archive = function () {
            var t = this;
            cloud.archiveCustomer(e).fail(showDefaultErrorNotification).done(function () {
                t.setArchived(!0), firebaseService.deleteOptionMessages(e), mixpanelEvents("archiveChat")
            })
        }, this.unread = function () {
            var t = this;
            cloud.unreadThread(k).fail(showDefaultErrorNotification).done(function () {
                t.setUnread(!0);
                mixpanelEvents("unreadChat")
            })
        }, this.unArchive = function () {
            var t = this;
            cloud.unarchiveCustomer(e).fail(showDefaultErrorNotification).done(function () {
                t.setArchived(!1);
                var e = $(".js-status-select.selected");
                if (e.size() > 0) {
                    var i = e.data("status");
                    ("2" == i || "3" == i) && cloud.createOptionsMessage()
                }
                mixpanelEvents("unarchiveChat")
            })
        }, this.getAssignButton = function () {
            var t;
            return t = this.isArchived() ? {
                "class": "archived",
                text: trOpenAssign
            } : this.getRoninStatus() ? {
                "class": "assigned",
                text: "ChatBot"
            } : this.getAgent() == myID ? {
                "class": "assigned",
                text: trAssignedMe
            } : this.getAgent() && this.getAgent() != 'ALL' ? {
                "class": "assigned",
                text: AGENTS[this.getAgent()] ? AGENTS[this.getAgent()].name : "Unkown Agent"
            } : {
                                "class": "unassigned",
                                text: trUnassigned
                            }
        }, this.getSelector = function () {
            return this.getAgent() ? "active" : d ? "archived" : "pending"
        }, this.getAvatarDisplay = function () {
            var t = -1 != this.getAvatar().indexOf("bot.png"),
                e = t ? "" : "hide",
                i = t ? "hide" : "";
            return {
                div: e + " " + getAvatarColor(this.getKey()),
                img: i
            }
        }, this.getChannelName = function () {
            return void 0 === CHANNELS[this.getChannel()] ? trRemoved : CHANNELS[this.getChannel()].name;
        },
        this.getIcon = function () {
            return (x === "comment" ? "speech-bubble" : this.getChannelType());
        }, this.profile_render = function (t) {
            $(document).find(".js-list-conversations .js-contact-" + k).remove();
            var n = this.getChannelName(),
                o = void 0 === CHANNELS[this.getChannel()] ? "removed" : "",
                i = this.getAgent(),
                a = conversationTemplate.tmpl({
                    id: e,
                    key: k,
                    picture: this.getAvatar() !=null && this.getAvatar().indexOf("http://") < 0 && this.getAvatar().indexOf("https://") < 0 ? baseUrls_Api + this.getAvatar() : this.getAvatar()  ,
                    name: this.getLastSenderName(),
                    initials: this.getInitials(),
                    date: u,
                    last_message: p,
                    render_date: this.getRenderedData(),
                    assigned: i,
                    assigned_function: "",
                    assigned_text: "",
                    channel_name: n,
                    channel_ico: n.toLowerCase().indexOf("hotline") < 0 ? this.getIcon() : "phone",
                    channel_status: o,
                    status: this.getStatus(),
                    selector: this.getSelector(),
                    btn_class: "",
                    avatar_img: this.getAvatarDisplay().img,
                    avatar_div: this.getAvatarDisplay().div,
                    unread: this.getUnreadClass(),
                    archived: this.isArchived() ? "archived" : "",
                    facebook_id: CHANNELS[this.getChannel()].facebook_id
                });
        
            //if (_customer_id == null || _customer_id == undefined || _customer_id == "" ||
            //    _channel_ext_id == null || _channel_ext_id == undefined || _channel_ext_id == "") {
            //    getCustomerAi(e, CHANNELS[this.getChannel()].facebook_id);
            //}

            return $(document).find(".js-list-conversations").append(a.hide().fadeIn(500).css("display", "flex")), t && ($(".js-list-conversations .js-contact-" + e).trigger("click")), this
        }
        , this.render = function (t) {
        try {
            $(".js-list-contacts .js-contact-" + e).remove();
            var i = this.getContactButton(),
                n = void 0 === CHANNELS[this.getChannel()] ? trRemoved : CHANNELS[this.getChannel()].name,
                o = void 0 === CHANNELS[this.getChannel()] ? "removed" : "";
            var pi = this.getAvatar().trim().replace("&quot;", "");
            pi = pi != null && pi.indexOf("http://") < 0 && pi.indexOf("https://") < 0 ? baseUrls_Api + pi : pi;

            $(".icon-phone").parent().parent().parent().each(function (k, v) {
                try {
                    $(v).removeClass("hidden").addClass("unread");
                } catch (e) { console.log(e); }
            })


            var ccc = {
                key: e,
                picture: pi,
                name: this.getName(),
                initials: this.getInitials(),
                date: u,
                last_message: p,
                render_date: this.getRenderedData(),
                assigned: this.getAgent(),
                assigned_function: i["function"],
                assigned_text: i.text,
                channel_name: n,
                channel_ico: n.toLowerCase().indexOf("hotline")<0? this.getIcon():"phone",
                channel_status: o,
                status: this.getStatus(),
                selector: this.getSelector(),
                btn_class: i["class"],
                avatar_img: this.getAvatarDisplay().img,
                avatar_div: this.getAvatarDisplay().div,
                unread: this.getUnreadClass(),
                archived: this.isArchived() ? "archived" : "",
                id: e,
                facebook_id: CHANNELS[this.getChannel()].facebook_id
            };

            var ag = contactTemplate.tmpl(ccc);
        } catch (e) {  console.log(e); }
                //if (_customer_id == null || _customer_id == undefined || _customer_id == "" ||
                //    _channel_ext_id == null || _channel_ext_id == undefined || _channel_ext_id == "") {
                //    getCustomerAi(e, CHANNELS[this.getChannel()].facebook_id);
                //}
            return contactList.append(ag.hide().fadeIn(500).css("display", "flex")), t && ($(".js-list-contacts .js-contact-" + e).trigger("click"), $(".js-list-contacts .js-contact-" + e).addClass("js-search"), this.goToContact()), this
        }, this.refresh = function (t) {

            var i = this,
                o = $(".js-list-contacts .js-contact-" + e);
            if (o.length > 0)
                if (i.isInChatview(chatView, chatViewAgent, chatViewChannel)) o.find(".wz-assigned-change").removeClass("js-show");
                else {
                    var r = i.getContactButton();
                    if ("archived" == i.getSelector() || "archived" == chatView) o.remove();
                    else if (o.hasClass("active")) {
                        var s = o.find(".wz-assigned-change");
                        s.hasClass("js-show") || (s.find("o").text(r.filterText), s.addClass("js-show").data("original-title", r.filterText).attr("onclick", r.filterFunction))
                    } else o.remove();
                    noCustomersViewHandler()
                }

            //var i = this,
            //    o = $(".js-list-contacts .js-contact-" + e);
            //o.size() > 0 && (i.isInChatview(chatView, chatViewAgent, chatViewChannel) || (o.remove(), noCustomersViewHandler()));
            var r = {
                name: function () {
                    $(".js-name-" + e).text(i.getName())
                },
                message: function () {
                    $(".js-last-message-" + e).text(i.getMessage());
                },
                avatar: function () {
                    $(".js-picture-" + e).css("background-image", "url(" + n + ")"), -1 == n.indexOf("bot.png") ? ($(".js-picture-" + e).removeClass("hide"), $(".js-avatar-" + e).addClass("hide")) : ($(".js-picture-" + e).addClass("hide"), $(".js-avatar-" + e).removeClass("hide"), $(".js-avatar-" + e + " p").text(i.getInitials()))
                },                
                status: function () {
                    o.attr("data-status", a);
                },
                button: function () {
                    var t = i.getContactButton(),
                        n = i.getAssignButton();
                    removeClassByRegExp(o.find(".js-status"), /^btn/), o.find(".js-status").addClass(t["class"]).attr("onclick", t["function"]).html(t.text), currentChat == e && setClientInfoBtn("#client-status", i.text, i["class"])
                },
                channel_name: function () {
                    var t = void 0 === CHANNELS[i.getChannel()] ? trRemoved : CHANNELS[i.getChannel()].name;
                    o.find(".js-channel-name").text(t)
                },
                icon: function () {
                    var t = o.find(".js-channel-icon");
                    t.size() > 0 && (removeClassByRegExp(t, /^icon-/), i.getIcon().toLowerCase().indexOf("hotline") < 0 ? t.addClass("icon-" + i.getIcon()) : t.addClass("icon-phone"));
                },

                unread: function () {
                    i.isUnread() ? (o.addClass("js-unread")) : (o.removeClass("js-unread"));
                },
                timestamp: function () {
                    o.find(".js-render-date").text(i.getRenderedData()), o.attr("timestamp", i.getLast())
                },
                input: function () {
                    var t = ".wz-col-central footer .assignation";
                    //var t = "#client-status";
                    currentChat == e && (sendStatus.removeClass("archived assignedToOther channelDeleted"), void 0 === CHANNELS[i.getChannel()] ? (sendStatus.addClass("channelDeleted"), txt.prop("disabled", !0).prop("placeholder", trChatRemovedChannel)) : i.isArchived() ? (sendStatus.addClass("archived"), txt.prop("disabled", !0).prop("placeholder", trChatArchived)) : i.getAgent() && i.getAgent() != myID || i.getRoninStatus() ? (sendStatus.addClass("assignedToOther"), txt.prop("disabled", !0).prop("placeholder", trChatAssignedToOther), $(t).empty(), $(t).append($assignTemplate.tmpl()), setAssignationListeners(t)) : txt.prop("disabled", !1).prop("placeholder", trTypeMessage), $(t).empty())
                },
                archive_btn: function () {
                    currentChat == e && (i.isArchived() ? (sendStatus.addClass("archived"), archiveCurrent.addClass("hidden"), openCurrent.removeClass("hidden")) : (i.getAgent() && i.getAgent() != myID || i.getRoninStatus() ? sendStatus.addClass("assignedToOther") : sendStatus.removeClass("assignedToOther"), sendStatus.removeClass("archived"), openCurrent.addClass("nohidden"), archiveCurrent.removeClass("hidden_true")))
                }

            };
            t && t.length > 0 ? $.each(Object.keys(r), function (e, i) {
                t.indexOf(i) > -1 && r[i]()
            }) : $.each(Object.keys(r), function (t, e) {
                r[e]()
            })
        }, this.profile_refresh = function (t) {
            var i = this,
                o = $(document).find(".js-list-conversations .js-contact-" + k);
            if ("object" != typeof i) return;
            //noCustomersViewHandler();
          // console.log("555555");
            var r = {
                name: function () {
                    $(document).find(".js-name-" + k).text(i.getLastSenderName())
                },
                message: function () {
                    $(document).find(".js-last-message-" + k).text(i.getMessage());
                },
                avatar: function () {
                    $(document).find(".js-picture-" + k).css("background-image", "url(" + n + ")"), -1 == n.indexOf("bot.png") ? ($(document).find(".js-picture-" + k).removeClass("hide"), $(document).find(".js-avatar-" + k).addClass("hide")) : ($(document).find(".js-picture-" + k).addClass("hide"), $(document).find(".js-avatar-" + k).removeClass("hide"), $(document).find(".js-avatar-" + k + " p").text(i.getInitials()))
                },
                
                status: function () {
                    o.attr("data-status", a);
                },
                channel_name: function () {
                    var t = void 0 === CHANNELS[i.getChannel()] ? trRemoved : CHANNELS[i.getChannel()].name;
                    o.find(".js-channel-name").text(t)
                },
                icon: function () {
                    var t = o.find(".js-channel-icon");
                    t.size() > 0 && (removeClassByRegExp(t, /^icon-/), i.getIcon().toLowerCase().indexOf("hotline") < 0 ? t.addClass("icon-" + i.getIcon()) : t.addClass("icon-phone"));
                },
                unread: function () {
                    i.isUnread() ? (o.addClass("js-unread"), unreadCurrent.addClass("hidden")) : (o.removeClass("js-unread"),
                        unreadCurrent.removeClass("hidden"));
                },
                timestamp: function () {
                    o.find(".js-render-date").text(i.getRenderedData()), o.attr("timestamp", i.getLast())
                },
                //input: function () {
                //    currentChat == e && (sendStatus.removeClass("archived assignedToOther channelDeleted"), void 0 === CHANNELS[i.getChannel()] ? (sendStatus.addClass("channelDeleted"), txt.prop("disabled", !0).prop("placeholder", trChatRemovedChannel)) : i.isArchived() ? (sendStatus.addClass("archived"), txt.prop("disabled", !0).prop("placeholder", trChatArchived)) : i.getAgent() && i.getAgent() != myID || i.getRoninStatus() ? (sendStatus.addClass("assignedToOther"), txt.prop("disabled", !0).prop("placeholder", trChatAssignedToOther)) : txt.prop("disabled", !1).prop("placeholder", trTypeMessage))
                //}

            };
            t && t.length > 0 ? $.each(Object.keys(r), function (e, i) {
                t.indexOf(i) > -1 && r[i]()
            }) : $.each(Object.keys(r), function (t, e) {
                r[e]()
            })
        }, this.block = function () {
            $.post(cloud.getURLs().customers.block + businessID + '/' + e)
        }, this.getReceiver = function () {
            return this.getChannel() || 0
        }, this.getChatviews = function () {
            var t = [!1];
            t.push("all");
            this.isUnread() && t.push("unread");
            (this.nonReplied() || this.isUnread()) && t.push("nonreply");
            this.isOpen() && t.push("open");
            return this.isArchived() ? t.push("archived") : this.getAgent() ? t.push("active") : t.push("pending"), t
        }, this.isInChatview = function (t, e, i) {
            //var rs = !((void 0 != t && t != !1 && t != 'all' && (t.length > 0) && -1 == this.getChatviews().indexOf(t)) || (e != void 0 && e.length > 0 && e != 'all' && e != !1 && (this.getAgent() != e)) || (i != void 0 && i.length > 0 && i != 'all' && i != !1 && (this.getChannel() != i)));
            var rs = ((void 0 == t || t == !1 || t == 'all' || (t.length == 0) || this.getChatviews().indexOf(t) >= 0) && (e == void 0 || e.length == 0 || e == 'all' || e == !1 || (this.getAgent() == e)) && (i == void 0 || i.length == 0 || i == 'all' || i == !1 || (this.getChannel() == i)));
            return rs;
        }, this.goToContact = function () {
            var t = contactList.find(".js-contact-" + e);
            if (t.size() > 0) {
                var i = t.position().top + contactListContainer.scrollTop() - t.innerHeight(),
                    n = contactList.outerHeight() - contactListContainer.outerHeight();
                i >= n && (i = n - 1), lastContactScroll = t.scrollBottom(), contactListContainer.scrollTop(i - 1), $sideleft2Scroll.perfectScrollbar("update")
            }
        }
};



