var id, idString, idSharp, dataType, ulId, number01, number02, classes, theItem, icon, labelJoin, content, label, input, inputCheck, formGOpen, formGClose, formOpen, formClose, targetDiv, parentDiv, customModal, element, rClass, rData, rQuestion, rError, rIcon, cQuestion, cError, cCheck, roninCards, arrayCards, numberUlCards, par, z, classItem, groupDepth, gDepth, counter, sortab, disabl, itemposition1, itemposition2, groupposition2, colnumber2, groupdepth2, sField, g, jsonEdited;
arrayCards = [], groupDepth = 0, roninCards = $("#roninCards ul"), numberUlCards = 0;
var botDescription2A = $("#wz-modal-bots-contact .textarea-2A"),
    botDescription2B = $("#wz-modal-bots-contact .textarea-2B"),
    hasGroup, channelInJson, modeId, htmlRonin, ulClasses, liClasses, idUl;
g = 1;
var dataQuestion, dataError, dataCheck, dataSelectField, dataType;
firstColumnOpen = '<div class="row"><div class="col-xs-12">', firstColumnClose = "</div></div>", columnConditionalOpen = '<div class="row row-bool"><span class="bool-border"></span>', columnConditionalClose = "</div>", colxs6Open = '<div class="col-xs-6">', colxs6Close = "</div>";
var itemCounter = 1;

var shortcutsLoaded = {},
    shortcutClicked = function () {
        var t = $(this).data("key");
        t || 0 == t ? (txt.val(txt.val() + shortcutsLoaded[t].name), txt.focus(), mixpanelEvents("clickShortcut")) : (0 == Object.keys(shortcutsLoaded).length ? (cloud.getShortcutForm(""), modalAddShortcut.modal("show")) : modalShortcuts.modal("show"), mixpanelEvents("openShortcuts"))
    },
    loadShortcutsButtons = function (t) {
        shortcutsLoaded = {};
        var e = Object.keys(t),
            i = function (e, i) {
                return t[e].shortcut.toUpperCase() < t[i].shortcut.toUpperCase() ? -1 : t[e].shortcut.toUpperCase() > t[i].shortcut.toUpperCase() ? 1 : 0
            };
        e.sort(i), shorcutBtnContainer.empty();
        //for (var n = 0; 5 > n && n < e.length; n++) {
        //    var o = e[n],
        //        a = $('<div class="btn"  data-key="' + o + '">'),
        //        r = $("<span>").text("/" + t[o].shortcut);
        //    shortcutsLoaded[o] = t[o], a.append(r), a.click(shortcutClicked), shorcutBtnContainer.append(a)
        //}
        //if (role == "admin") {
            
            var a = $('<div class="btn more">'),
                s = e.length > 0 ? trAddMore : trSetShort,
                r = $("<span>").text(s);
            a.append(r), a.click(shortcutClicked), shorcutBtnContainer.append(a)
      //  }
    },
    shortcuts = function (t) {
        setTimeout(function () {
            var t = $(".js-last-ticket li.dropdown a.btn").text();
            sendForm.removeClass(), sendForm.addClass(t)
        }, 500), $("#js-typearea").mention({
            shortcuts: t
        }), $("#js-typearea").typeahead().data("typeahead").source = t
    };
$("#filterShortcut").keyup(function (t) {
    filter("#shortcuts-table .tabulator-row", $(this).val())
}), $(".wz-filter a").click(function (t) {
    t.preventDefault(), $(this).parent().parent().find("a.active").removeClass("active"), $(this).addClass("active")
}), $(".list-inline li").click(function (t) {
    t.preventDefault(), $(this).parent().find(".active").removeClass("active"), $(this).addClass("active")
}), txt.click(newTextareaTutorial);

var newTicketTutorialGoal = 5;
addHow();

var intro = !1,
    flagNewInterfaceExec = !1;
$(".wz-button-check").click(function () {
    $(this).toggleClass("js-checked")
}), $("#js-typearea").focus(function () {
    typeareaFocused = !0, "undefined" != typeof CONTACTS[currentChat] && (CONTACTS[currentChat].isArchived() || CONTACTS[currentChat].getAgent() && CONTACTS[currentChat].getAgent() != myID || $(".shortcuts").addClass("js-focused"))
    }), $("#js-typearea").blur(function () {
        typeareaFocused = !1, shortcutsFocused || $(".shortcuts").removeClass("js-focused")
}), $(".shortcuts").mouseover(function () {
    shortcutsFocused = !0, "undefined" != typeof CONTACTS[currentChat] && (CONTACTS[currentChat].isArchived() || CONTACTS[currentChat].getAgent() && CONTACTS[currentChat].getAgent() != myID || $(".shortcuts").addClass("js-focused"))
}), $(".shortcuts").mouseout(function () {
    shortcutsFocused = !1, typeareaFocused || $(this).removeClass("js-focused")
});
var pricingPeriodSelected, changeBilledPeriod = function (t) {
    var e = $(t).data("period"),
        i = (100 - yearDiscount) / 100,
        n = "A" == e ? roninQuote * i : roninQuote,
        o = "A" == e ? telegramQuote * i : telegramQuote,
        a = "A" == e ? whatsappQuote * i : whatsappQuote,
        r = "A" == e ? facebookQuote * i : facebookQuote,
        s = "A" == e ? smoochQuote * i : smoochQuote;
    pricingRoninTotal.text("€" + n.toFixed(2)), pricingTelegram.text("€" + o.toFixed(2)), pricingWhatsapp.text("€" + a.toFixed(2)), pricingFacebook.text("€" + r.toFixed(2)), pricingSmooch.text("€" + s.toFixed(2))
},
    notificationArray = [];
NotificationPlugin = function (t, e, i, n, o, a) {
    this.title = t || "", this.message = e || "", this.icon = i || "icon-bell", this.button = n, this.callback = o, this.id = "notification-plugin-" + Date.now(), this.elem = $("<div>").attr("id", this.id).attr("class", "wz-notification wz-notification-slim closed"), this.countdown = a;
    var r = $("<div>").addClass("wz-notification-head"),
        s = $("<i>").addClass("js-notification-icon").addClass(this.icon);
    r.append(s);
    var l = $("<div>").addClass("wz-notification-body"),
        c = $("<h3>").addClass("js-notification-title").text(this.title),
        d = $("<p>").addClass("js-notification-message").text(this.message);
    l.append(c, d);
    var u = $("<div>").addClass("wz-notification-foot");
    if ("string" == typeof this.button) {
        var h = $("<div>").attr("class", "btn btn-info").text(this.button);
        h.click(this.close.bind(this)), u.append(h), l.append(u)
    }
    this.elem.append(r, l), this.elem.click(this.close.bind(this)), this.create(), this.show()
}, NotificationPlugin.prototype.create = function () {
    notificationArray.push(this), $("body").append(this.elem)
}, NotificationPlugin.prototype.show = function () {
    setTimeout(function () {
        this.elem.removeClass("closed"), "number" == typeof this.countdown && (this.countdownTimeout = setTimeout(this.close.bind(this), this.countdown)), renderAllNotificationPlugin()
    }.bind(this), 100)
}, NotificationPlugin.prototype.setMessage = function (t) {
    "string" == typeof t && (this.message = t, this.elem.find(".js-notification-message").text(t))
}, NotificationPlugin.prototype.setIcon = function (t) {
    "string" == typeof t && (this.elem.find(".js-notification-icon").removeClass(this.icon).addClass(t), this.icon = t)
}, NotificationPlugin.prototype.setCountdown = function (t) {
    "number" == typeof t && (this.countdown = t, this.show())
}, NotificationPlugin.prototype.setTitle = function (t) {
    "string" == typeof t && (this.title = t, this.elem.find(".js-notification-title").text(t))
}, NotificationPlugin.prototype.close = function () {
    clearTimeout(this.countdownTimeout), this.elem.addClass("closed"), setTimeout(function () {
        this.elem.remove(), "function" == typeof this.callback && this.callback(), deleteNotificationPlugin(this)
    }.bind(this), 400)
}, playNotification = function (t) {
    document.hasFocus() || (audio.play(), notifyMe(t.customer, t.message))
}, scrollMessages = function () {
    firstMessageToScroll ? (lastMessageToScroll = $(".js-list-chat li.js-chat-message[data-key=" + firstMessageToScroll + "]").first(),
        scrollToFirstMessage()) : (listHeight = chatList.height(), messageListContainer.scrollTop(listHeight), $centralScroll.perfectScrollbar("update"))
}, metricsLink = $(".wz-open-metrics"), metricsLink.click(function () {
    mixpanelEvents("openMetrics")
}), openF.click(function () {
    finder.addClass("js-open"), inputFinder.focus(), mixpanelEvents("searchMessages")
}), closeF.click(function () {
    finder.removeClass("js-open"), inputFinder.val("")
});
var s2f = $(".wz-col-sideleft-2 footer");
openF2.click(function () {
    finder2.addClass("js-open"), s2f.addClass("js-f-open"), inputFinder2.focus()
}), closeF2.click(function (t) {
    finder2.removeClass("js-open"), s2f.removeClass("js-f-open"), inputFinder2.val(""), chatFilterSelection(t);
}), finder2.submit(function (t) {
    t.preventDefault();
    chatFilterSelection(t);
}),
inputFinder2.autocomplete({
    source: [],
    minLength: 1,
    select: function (t, e) {
        var i = $(".js-list-contacts .js-contact-" + e.item.id);
        return i.size() > 0 ? (CONTACTS[e.item.id].goToContact(), i.click()) : ("undefined" == typeof CONTACTS[e.item.id] && (CONTACTS[e.item.id] = new
            (e.item)), CONTACTS[e.item.id].isInChatview(chatView, chatViewAgent, chatViewChannel) ? CONTACTS[e.item.id].render(!0) : resetChatview().done(function () {
                var t = $(".js-list-contacts .js-contact-" + e.item.id);
                t.size() > 0 ? (CONTACTS[e.item.id].goToContact(), t.click()) : CONTACTS[e.item.id].render(!0)
            }), $(".js-close-wz-sl2-finder").click()), !1
    }
}).autocomplete("instance")._renderItem = function (t, e) {
    return $("<li>").append($("<i class='wz-margin-r-5 icon-" + e.channel + "'>")).append($("<span>").text(e.value)).appendTo(t)
},
PhonePrefixCombobox = function (t) {
    var e = $(t);
    Date.now();
    e.size() > 0 && $.get(phonePrefixURL).done(function (t) {
        var i = $('<select class="form-control hide">');
        $.each(Object.keys(t), function (e, n) {
            var o = t[n].split(" and ");
            $.each(o, function (t, e) {
                var o = $.trim(e);
                if (o.length > 0) {
                    var a = "+" == o[0] ? o : "+" + o;
                    $("<option>", {
                        value: a,
                        text: a,
                        image: "flag flag-" + n.toLowerCase()
                    }).appendTo(i)
                }
            })
        });
        var n = i.find("option");
        n.sort(function (t, e) {
            var i = t.text.replace(/\D/g, ""),
                n = e.text.replace(/\D/g, "");
            return i = i.length + "" + i, n = n.length + "" + n, i > n ? 1 : n > i ? -1 : 0
        }), i.empty().append(n);
        var o = $('<div class="image-selected">'),
            a = $('<div class="autocomplete-prefix-list">');
        e.addClass("autocomplete-prefix-container").prepend([o, i, a]), i.combobox({
            appendToElem: a
        }), e.find(".custom-combobox-input").on("click focus", function () {
            o.removeClass("flag"), $(this).select()
        }), e.find(".custom-combobox-input").on("blur ", function () {
            for (var t = i.find("option"), e = t.map(function () {
                    return $(this).val()
            }), n = 0; n < e.length; n++)
                if ($(this).val() == e[n]) {
                    var a = t.eq(n).attr("image");
                    return void (o.hasClass("flag") || o.prop("class", "").addClass("image-selected " + a))
                }
            o.removeClass("flag"), $(this).val(""), i.val("")
        }), e.find(".custom-combobox-input").val("+34").blur()
    })
}, $(function () {
    $("[data-toggle=popover]").popover({
        html: !0
    })
}), $("body").on("click", function (t) {
    $('[data-toggle="popover"]').each(function () {
        $(this).is(t.target) || 0 !== $(this).has(t.target).length || 0 !== $(".popover").has(t.target).length || $(this).popover("hide")
    })
}),



Preloader = function (t) {
    this.screenElem = document.getElementById(t), this.promisesToBeDone = [], this.functionsToRunAfter = [], this.timeoutLimit = setTimeout(function () {
        this.close()
    }.bind(this), preloaderLimit)
};

Preloader.prototype.config = function (t, e) {
    this.promisesToBeDone = t, this.functionsToRunAfter = e, $.when.apply($, t).then(this.close.bind(this), function (t) {
        "402" == t.status ? (console.log("[Preloader] promise ended with error 402, payment required!"), this.close()) : (console.log("[Preloader] promise ended with error " + t.status), new NotificationPlugin(trError, trAnErrorOcurred, "icon-cross"))
    }.bind(this))
}, Preloader.prototype.close = function () {
    clearInterval(this.timeoutLimit);
    var t = this.screenElem;
    t && t.parentElement && (t.style.opacity = 1, function i() {
        (t.style.opacity -= .1) < .1 ? t.parentElement && t.parentElement.removeChild(t) : requestAnimationFrame(i)
    }());
    var e = !1;
    this.functionsToRunAfter && $.each(this.functionsToRunAfter, function (t, i) {
        e || i() === !1 && (e = !0)
    }), preloaderEnded = !0
}

function modalWelcome() {
    // If no chats shows modal
    if (Object.keys(CHANNELS).length <= 0) {
        tutorialActive = true;  // Enable tutorial flag

        if (Object.keys(CHANNELS).length <= 0) {
            // If no channels added yet, settigs goes fullscreen
            welcomeModal.removeClass('wz-modal-right').addClass('wz-modal-fullscreen')
            welcomeModal.find('.wz-team').addClass('hide');
        } else {
            welcomeModal.addClass('wz-modal-right').removeClass('wz-modal-fullscreen')
            welcomeModal.find('.wz-team').removeClass('hide');
        }

        welcomeModal.modal('show');

        // Fix to show modal if modalPricing is executed before main.css is finally loaded
        welcomeModal.css('display', 'block');
        //mixpanelEvents('onboardingPopup');


        // Disable share links when no channel exist
        $(".js-share-telegram, .js-share-whatsapp, .js-share-facebook").addClass('hide');

        // Show restricted mode modal if no admin
        if (role != 'admin') {
            $('.js-welcome-subfoot').removeClass('hide');
            $('.js-welcome-panel').addClass('hide');
        }

        // If channels were already created
        if (cloud.getHowManyChannel() > 0) {
            // Share link buttons
            var btnShare, btnShareCopied, shareValue, valueType;
            $.each(CHANNELS, function (index, channel) {
                var ch = CHANNELS[index];
                if (ch) {
                    var $btn = $('.js-welcome-panel .btn.btn-pink[data-channeltype="' + ch.type.toLowerCase() + '"]');
                    var type = ch.type.toLowerCase();
                    if ($btn.size() > 0) {
                        $btn.addClass('actived');
                        shareValue = (type == "facebook") ? "https://m.me/" + ch.facebook_id : ch.phone;
                        valueType = (type == "facebook") ? ch.type + " URL" : ch.type + " number";

                        btnShare = $('.js-share-' + type)
                          .removeClass('hide')
                          .data('channel-id', index)
                          .data('channel-type', type);

                        btnShare.unbind('click', showLink); // Fix when more than one channel
                        btnShare.bind('click', { shareValue: shareValue, valueType: valueType }, showLink);

                        btnShareCopied = $('.js-share-' + type + '-copied')
                          .data('channel-id', index);

                        btnShareCopied.unbind('click', copyToClipboard); // Fix when more than one channel
                        btnShareCopied.bind('click', { shareValue: shareValue, valueType: valueType }, copyToClipboard);

                        // Is active?
                        var isActive = (ch.active);
                        if (!isActive) {
                            $btn.find('.js-channel-status')
                              .removeClass('icon-circle-check')
                              .addClass('icon-notification')
                              .tooltip({
                                  title: "Activation pending",
                                  placement: "top",
                                  container: "body"
                              });
                        }
                    }
                } // end if (channel)

            }); // end CHANNELS.forEach >> TODO: Loop is code-reducible
            $('.js-welcome-header').addClass('js-added');
            addHow();

        } else { // If no channels were created and no messages received, then it might be the first session
            //mixpanelEvents('firstSession');
        }

        return false;
    }
}

$(document).ready(function () { }), toggleBotScreens();
var blockMessageText = $(".js-block-message-text"),
    blockMessageEdit = $(".js-block-message-edit"),
    blockMessageQuestion = $(".js-block-message-question"),
    blockMessageList = $(".js-block-message-list"),
    blockMessageNext = $(".js-block-message-next"),
    blocks;
$(".js-new-block").click(function () {
    newBlock(), clickLoadBlock()
});
var newObject, currentRoninStatus, optionsObject = {};
//clickLoadBlock();


$(document).ready(function () {
    var t = new Preloader("preloader");
    t.config([initFirebase(), loadStylesheet(staticCSSURL), cloud.getShortcuts()], [initReports, emojiPicker, sendMessages, loadOldMessages, loadOldContacts, scrollMessages, modalDismiss, modalWelcome, newVersionTutorial]), createListeners(), tooltips(), modalClientsTable(), modalTicketsTable(), modalCRMTable(), notifications(), responsiveUI(), autoSizeTextarea(), initPerfectScroll();
    //t.config([initFirebase(), loadStylesheet(staticCSSURL), cloud.getShortcuts()], [cloud.verifySuscription, initReports, emojiPicker, sendMessages, loadOldMessages, loadOldContacts, loadSearchContact, scrollMessages, modalDismiss, modalWelcome, newVersionTutorial]), createListeners(), tooltips(), modalClientsTable(), modalTicketsTable(), modalCRMTable(), notifications(), responsiveUI(), autoSizeTextarea(), initPerfectScroll(), initMixpanel({
    //    ID: myID,
    //    firstName: firstName,
    //    lastName: lastName,
    //    created: created,
    //    email: email,
    //    country: country,
    //    company: company,
    //    role: role,
    //    customersCount: customersCountOnInit,
    //    trial: suscriptionTrial,
    //    pricingVolume: pricingVolume,
    //    expired: suscriptionExpired
    //})

});


