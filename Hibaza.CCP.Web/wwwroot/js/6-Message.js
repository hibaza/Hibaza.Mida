
Message = function (t) {
    function e() {
        var t = "frontendEmail" === Y ? $messageTemplateEmail : (G === true ? $messageTemplateAgent : $messageTemplate),
            e = t.tmpl({
                key: k,
                author_picture: b(),
                author_name: y(),
                author_key: _(),
                author_id: w(),
                author_url: purl(),
                permalink: permalink(),
                date: formatShortDatetime(dateFromTimestamp(A)),
                day: D,
                dayClass: M,
                timestamp: A,
                message: x,
                like: _like(),
                deleted: _deleted(),
                hidden: _hidden(),
                readed: f()["class"],
                readed_ico: f().icon,
                agent: h(),
                subject: V,
                avatar_div: m().div,
                avatar_img: m().img,
                initials: v(),
                starred: p()
            });
        return linkify(e)
    }

    function i() {

        var t = $messageTemplateFullImage.tmpl({
            url: R,
            message: x
        });
        return t
    }

    function n() {
        var t = $messageTemplateImage.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            url: R,
            message: x,
            like: _like(),
            deleted: _deleted(),
            hidden: _hidden(),
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p()
        });
        return t.find("img.wz-chat-image").click(function () {
            openImageMessage(S, k)
        }), t
    }

    function o() {
        var t = $messageTemplateMultipleImages.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            urls: urls(),
            message: x,
            like: _like(),
            deleted: _deleted(),
            hidden: _hidden(),
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p(),
            titles: titles()
        });
        return t.find("img.wz-chat-image").click(function () {
            openImageUrl($(this).attr("src"))
        }), t
    }


    function r() {
        var t= $messageTemplateDocument.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            url: R,
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            file_name: N,
            size: L,
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p()
        })
        return t;
    }

    function s() {
        t= $messageTemplateVideo.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            url: R,
            like: _like(),
            deleted: _deleted(),
            hidden: _hidden(),
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p()
        })
        return t;
    }

    function l() {
        var t= $messageTemplateAudio.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            url: R,
            like: _like(),
            deleted: _deleted(),
            hidden: _hidden(),
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p(),
            message: x,
            hotline_icon_status: (x.toLowerCase().indexOf("busy") >= 0 ? "&#xe61f;" : "&#xe61d;"),
            hotline_status:(x.toLowerCase().indexOf("busy") >= 0 ? "hidden" : "")
        })
        return t;
    }

    function c() {
        var t = $messageTemplateNote.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            message: x,
            deleted: _deleted(),
            readed: f()["class"],
            readed_ico: f().icon,
            agent: h(),
            subject: V,
            avatar_div: m().div,
            avatar_img: m().img,
            initials: v(),
            starred: p()
        });
        return linkify(t)
    }

    function d() {
        var t = $messageTemplateZami.tmpl({
            key: k,
            author_picture: staticZamiAvatar,
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            message: x,
            agent: h(),
            subject: V,
            template: W
        }),
            e = $(".js-messageZami-" + W).tmpl({
                data: H
            });
        return t.find(".wz-container").append(e), linkify(strongify(t))
    }

    function u() {
        var t = $messageTemplateDialog.tmpl({
            key: k,
            author_picture: b(),
            author_name: y(),
            author_key: _(),
            author_id: w(),
            author_url: purl(),
            permalink: permalink(),
            date: formatShortDatetime(dateFromTimestamp(A)),
            day: D,
            dayClass: M,
            timestamp: A,
            message: x,
            agent: h(),
            subject: V,
            template: "dialog",
            title: U
        }),
            e = t.find(".wz-button-container");
        
        return $.each(z, function (t, i) {
            var n = $('<a class="btn btn-pink btn-warning" href="#" onclick="putText(this);">').text(i);
            e.append(n)
        }), linkify(t)
    }

    function h() {
        var h = E || 0 == E ? 1 : 0;
        return h;
    }

    function f() {
        var t= PR && !E ? {
            "class": "wz-checks-2",
            icon: staticImgURL + "blue-check.png"
        } : {
                "class": "wz-checks-1 js-message-check-" + k,
                icon: staticImgURL + "gray-check.png"
            }
        return t;
    }

    function p() {
        return B ? "selected" : ""
    }

    function g() {
        //O.on("child_added", function (t) {
        //    "readed_at" == t.key && (messageCheckBox = $(".js-message-check-" + k), messageCheckBox.css("background-image", "url(" + staticImgURL + "blue-check.png)"), messageCheckBox.removeClass("wz-checks-1"), messageCheckBox.addClass("wz-checks-2"))
        //})
    }

    function m() {
        var t = b(),
            e = -1 != t.indexOf("bot.png") && !E && 0 != E,
            i = e ? "" : "hide",
            n = e ? "hide" : "";
        jj= {
            div: i + " " + getAvatarColor(S),
            img: n
        }
        return jj;
    }

    function v() {
        return getInitials(y())
    }

    function b() {
        var t = "avatars/bot.png";                
        var gg = E || 0 == E ? AGENTS[E] && AGENTS[E].avatar && void 0 !== AGENTS[E].avatar ? AGENTS[E].avatar : cloud.getMediaUrl() + t : CONTACTS[S] ? CONTACTS[S].getAvatar() : cloud.getMediaUrl() + t;
        gg =  (gg !=null && gg.indexOf("http://") < 0 && gg.indexOf("https://") < 0) ? baseUrls_Api + gg : gg;
        return gg;
    }

    function y() {
        return E || 0 == E ? AGENTS[E] ? (hideGhostChat(), (AGENTS[E].name ? AGENTS[E].name : (AGENTS[E].first_name + " " + AGENTS[E].last_name)).capitalizeFirstLetter()) : SN || trDeletedAgent : CONTACTS[S] ? CONTACTS[S].getName() ? CONTACTS[S].getName() : SN : SN;
    }

    function w() {
        var contact = E || 0 == E ? "agent_" + E : CONTACTS[S] ? "contact_" + CONTACTS[S].getKey() : S;
        return contact;
    }

    function _() {
        return E || 0 == E ? "agent-" + E : S;
    }

    function C() {
        return void 0 == F ? "" : F
    }

    function purl() {
        return self.urls.customers.openprofile + businessID + '/' + t.val().thread_id;
    }

    function urls() {
        return void 0 == j ? new {} : j;
    }
    function titles() {
        return void 0 == tt ? new {} : tt;
    }
    function permalink() {
        return self.urls.messages.openlink + "?item_id=" + k;
    }
    this.getCustomer = function () {
        return CI;
    }
    function _deleted() {
        return DL === true ? "disabled" : "";
    }
    function _hidden() {
        return DL === true ? "hidden" : TT === "comment" ? "" : "hidden";
    }
    function _like() {
        return LM === true ? "Unlike" : "Like";
    }
    function formatLink(links, urlService)
    {
        if (links == null || links == undefined || links.length==0)
            return links;        
        var arrs = [];
        for (var i = 0; i < links.length; i++) {
            if (links[i] !=null && (links[i].indexOf("http://") <0) && links[i].indexOf("https://") <0)
            {
                arrs.push(urlService + links[i]);
            }
            else
                arrs.push(links[i])
        }
        return arrs;
    }
    var k = t.key,
        S = t.val().sender_ext_id === t.val().channel_ext_id ? t.val().sender_id : t.val().customer_id,
        CI = t.val().customer_id ? t.val().customer_id : t.val().thread_id,
        SN = t.val().sender_name ? t.val().sender_name : THREADS[t.val().sender_id] ? THREADS[t.val().sender_id].getName() : "",
        x = t.val().message,
        T = t.val().type,
        A = t.val().timestamp,
        D = formatShortDate(dateFromTimestamp(A), !0),
        M = "js-day-" + formatShortDate(dateFromTimestamp(A), !0).replace(/\//g, "-"),
        //E = (t.val().sender_ext_id === t.val().channel_ext_id || t.val().sender_id === t.val().channel_ext_id) ? (t.val().agent_id || (t.val().thread_id && CONTACTS[t.val().thread_id] ? (CONTACTS[t.val().thread_id].getAgent() || SI) : t.val().thread_id)) : undefined, E = !E || E.length == 0 ? undefined : E,
        E = (t.val().sender_ext_id === t.val().channel_ext_id || t.val().sender_id === t.val().channel_ext_id) ? (t.val().agent_id) : undefined, E = !E || E.length == 0 ? undefined : E,
        I = {
            latitude: t.val().latitude,
            longitude: t.val().longitude
        },
        G = t.val().sender_ext_id === t.val().channel_ext_id,
        F = t.val().address,
        R = (t.val().url != null && (t.val().url != null && t.val().url.indexOf("http://") < 0 && t.val().url.indexOf("https://") < 0)) ? (baseUrls_Api + t.val().url ): t.val().url,
        O = t.ref,
        DL = t.val().deleted,
        LM = t.val().liked,
        HM = t.val().hidden,
        P = t.val().readed_at,
        PR = t.val().replied_at,
        L = t.val().size,
        N = t.val().file_name,
        B = t.val().starred,
        j = formatLink(t.val().urls, baseUrls_Api),
        W = t.val().template,
        H = t.val().data,
        U = t.val().title,
        z = t.val().buttons,
        Y = t.val().channel_type,
        TT = t.val().thread_type,
        TI = t.val().thread_id,
        tt = t.val().titles,
        V = t.val().subject;
    
    t.val().read || void 0 != E && 0 != E || (preloaderEnded && newTicketTutorial(currentChat)),
        this.getStarred = function () {
            return B || !1
        }, this.setStarred = function (t, n) {
            return "boolean" == typeof t && ($.post(self.urls.messages.star + businessID + '/' + k + '/?starred=' + t, function (e) { (B = t); n(); }))
        }, this.delete = function () {
            DL = true;
            var t = chatList.find(".js-chat-message[data-key=" + k + "]");
            $(t).find(".wz-trash").remove();
            $(t).find(".wz-caption").last().after('<div class="dropdown wz-trash pull-right"><span data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" class="wz-trash-btn wz-font-s-sm wz-font-c-dark"><i title="This message has been deleted" data-placement="top" data-container="body" class="icon-trash" style="background-color:red"></i></span></div>');
            $(t).addClass("disabled");
            return DL;
        }, this.getLiked = function () {
            return LM || !1
        }, this.setLiked = function (t, n) {
            return "boolean" == typeof t && ($.post(self.urls.messages.like + businessID + '/' + k + '/?liked=' + t, function (e) { (LM = t); n(e); }))
        }, this.renderBefore = function (t) {
            var e = this.getRendered();
            t.before(e && e.hide())
        }, this.getRendered = function (t) {
            var h = !1,
                f = t || T;
            return "text" == f ? h = e() : "image" == f ? h = n() : "multiple_images" == f ? h = o() : "image-full" == f ? h = i() : "audio" == f ? h = l() : "file" == f || "document" == f ? h = r() : "video" == f ? h = s() : "note" == f ? h = c() : "bot" == f ? h = d() : ("menu" == f || "dialog" == f) && (h = u()), h && (DL === true ? h.find(".wz-caption").last().after('<div class="dropdown wz-trash pull-right"><span data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" class="wz-trash-btn wz-font-s-sm wz-font-c-dark"><i title="This message has been deleted" data-placement="top" data-container="body" class="icon-trash" style="background-color:red"></i></span></div>') : ("admin" == role || "agent" == role) && h.find(".wz-caption").last().after($messageTemplateTrash.tmpl({
                key: k
            }))), h

        }, this.render = function () {
            if (currentChat && CONTACTS[currentChat] && CONTACTS[currentChat].getKey() != TI) return;
            var t = this.getRendered();
            t && ("image" == T && t.find("img.js-chat-image").load(scrollMessages), "video" == T && t.find("video.js-chat-video").bind("canplay", scrollMessages)), chatList.append(t), P || !E && 0 != E || g()
        }, this.getTimestamp = function () {
            return A
        }
};