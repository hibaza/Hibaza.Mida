
jQuery.expr[":"].Contains = jQuery.expr.createPseudo(function (t) {
    return function (e) {
        return jQuery(e).text().toUpperCase().indexOf(t.toUpperCase()) >= 0
    }
}), String.prototype.capitalizeFirstLetter = function () {
    return this.charAt(0).toUpperCase() + this.slice(1)
};
var deleteDictKey = function (t, e) {
    "undefined" != typeof t && "undefined" != typeof t[e] && delete t[e]
},
    addDictKey = function (t, e) {
        "undefined" != typeof t && (t[e] = !0)
    };
$.fn.scrollBottom = function () {
    return this.scrollTop() + this.height()
}, $.fn.refresh = function () {
    return $(this.selector)
};

var txt = $("#js-typearea"),
    hiddenDiv = $(document.createElement("div")),
    colSiderightClient = $(".wz-sideright-client"),
    colSiderightTicket = $(".wz-sideright-ticket"),
    colSiderightAgent = $(".wz-sideright-agent"),
    colSiderightPayment = $(".wz-sideright-payment"),
    $colSideright = $("#wz-col-sideright"),
    $colCentral = $(".wz-col-central"),
    phoneInput = $(".js-customer-input"),
    channelInput = $(".js-channel-input"),
    channelType = $(".js-channel-type-chat"),
    agentInput = $(".js-agent-input"),
    threadInput = $(".js-thread-input"),
    $emojiBtn = $("#emoji-btn"),
    finder = $("#wz-finder"),
    inputFinder = $("#wz-finder input"),
    openF = $(".js-open-wz-finder"),
    closeF = $(".js-close-wz-finder"),
    finder2 = $("#wz-sl2-finder"),
    inputFinder2 = $("#wz-sl2-finder input"),
    openF2 = $(".js-open-wz-sl2-finder"),
    closeF2 = $(".js-close-wz-sl2-finder"),
    $topBarOption = $(".js-topbar-option"),
    $topBarModals = $("#wz-modal-clients, #wz-modal-reports, #wz-modal-bots-contact, #wz-modal-automations, #wz-modal-ronin-manager"),
    $topBarChats = $(".js-topbar-chats"),
    $topBarSettings = $(".js-topbar-settings"),
    $topBarRoninManager = $(".js-topbar-ronin-manager"),
    audio = document.getElementById("wz-notification-audio");
audio && (audio.volume = .2);
var notificationsList = $(".js-notifications-list"),
    notificationTemplate = $(".js-loadtemplate-notification"),
    iconBell = $(".icon-bell.js-bell"),
    submitImg = $("#wz-modal-upload-image .js-submit-image"),
    resetImg = $("#wz-modal-upload-image .js-reset-image"),
    buttonOpenGallery = $("#wz-modal-upload-image .js-open-image-gallery"),
    selectImgGallery = $("#wz-modal-image-gallery .js-select-image-done"),
    cancelImgGallery = $("#wz-modal-upload-image .cancel"),
    sendForm = $("#send_form"),
    modalImgGallery = $("#wz-modal-image-gallery"),
    modalImg = $("#wz-modal-upload-image"),
    inputModalMessageTag = $("#input-modal-message-tag"),
    txtImage = $(".js-image-caption-text"),
    modalFormImageUpload = $(".js-form-image-upload"),
    inputConfigChat = $(".js-config-chat"),
    inputParaChat = $(".js-para-chat"),
    $modalFullImage = $("#wz-modal-full-image"),
    ticketSelect = $(".js-tickets-select"),
    showTicketList = $(".js-show-tickets-list"),
    ticketsList = $("#wz-modal-tickets"),
    modalAddTicket = $("#wz-modal-add-ticket"),
    modalTicket = $("#wz-modal-ticket"),
    lastTicket = $(".js-last-ticket"),
    ticketTemplate = $(".js-loadtemplate-ticket"),
    ticketsAssignationSelect = $(".js-tickets-assignation-select"),
    tabTickets = $(".js-tab-tickets"),
    ticketsResponsabilitySelect = $(".js-responsability-tickets-select"),
    ticketsStatusSelect = $(".js-status-ticket-select"),
    selectResponsability = $(".js-select-responsability"),
    selectStatus = $(".js-select-status"),
    selectResponsability = $(".js-select-responsability"),
    modalAddShortcut = $("#wz-modal-add-shortcut"),
    modalShortcuts = $("#wz-modal-shortcuts"),
    shorcutBtnContainer = $("#send_form .upper .shortcuts"),
    $sideLeft1 = $(".wz-col-sideleft-1"),
    $sideLeft2 = $(".wz-col-sideleft-2"),
    $sideLeft3 = $(".wz-col-sideleft-3"),
    $addChannel = $(".js-add-channel"),
    $addAgent = $(".js-add-agent"),
    $channelTemplate = $(".js-loadtemplate-channel"),
    $agentTemplate = $(".js-loadtemplate-agent"),
    chatList = $(".js-list-chat"),
    chatsSelect = $(".js-chats-select"),
    chatsStatusSelect = $(".js-status-user-select"),
    tabChats = $(".js-tab-chats"),
    selectClient = $(".js-select-client"),
    selectTicket = $(".js-select-ticket"),
    viewCurrentTicket = $(".js-view-current"),
    unreadCurrent = $(".js-unread-current"),
    nonreplyCurrent = $(".js-nonreply-current"),
    archiveCurrent = $(".js-archive-current"),
    archiveCurrentOption = $(".js-archive-current ul li"),
    openCurrent = $(".js-open-current"),
    sendStatus = $("#send_form .status"),
    checkNote = $("#js-button-checknote"),
    $messageTemplate = $(".js-loadtemplate-message"),
    $messageTemplateAgent = $(".js-loadtemplate-message-agent"),
    $messageTemplateImage = $(".js-loadtemplate-messageImage"),
    $messageTemplateMultipleImages = $(".js-loadtemplate-messageMultipleImages"),
    $messageTemplateAudio = $(".js-loadtemplate-messageAudio"),
    $messageTemplateDocument = $(".js-loadtemplate-messageFile"),
    $messageTemplateVideo = $(".js-loadtemplate-messageVideo"),
    $messageTemplateEmail = $(".js-loadtemplate-messageEmail"),
    $messageTemplateFullImage = $(".js-loadtemplate-messageFullImage"),
    $messageTemplateNote = $(".js-loadtemplate-note"),
    $messageTemplateZami = $(".js-loadtemplate-messageZami"),
    $messageTemplateDialog = $(".js-loadtemplate-dialog"),
    $deleteMessageDropdown = $('.js-loadtemplate-delconfirm'),
    $messageTemplateTrash = $(".js-loadtemplate-trash"),
    inputImage = $("#id_images"),
    inputAudio = $("#js-input-audio"),
    buttonSendimage = $("#js-button-sendimage"),
    buttonSendimageA = $("#js-button-sendimage a"),
    buttonSendaudio = $("#js-button-sendaudio"),
    fillerMessageList = $(".js-filler-chat"),
    messageListContainer = $(".wz-col-central section.main section"),
    olderMessagesLoader = $(".js-see-older-chats"),
    modalAddAlert = $("#wz-modal-add-alert"),
    disconnectionAlert = $("#wz-disconnection-alert"),
    disconnectionTemplate = $(".js-loadtemplate-disconnection"),
    modalAddNote = $("#wz-modal-add-note"),
    emptyCol02 = $(".wz-col-sideleft-2 .empty-state"),
    searchContactContainer = $(".wz-col-sideleft-2 .wz-sideleft2-search"),
    closeContact = $(".wz-col-sideleft-2  .js-close-wz-column"),
    openContact = $(".wz-col-sideleft-3 .js-open-wz-column"),
    contactList = $(".js-list-contacts"),
    conversationList = $(".js-list-conversations"),
    contactListContainer = $(".wz-col-sideleft-2 section section"),
    conversationListContainer = $(".wz-col-sideleft-3 section section"),
    conversationTemplate = $(".js-loadtemplate-conversation")
contactTemplate = $(".js-loadtemplate-contact"),
    listContactsLi = $(".js-list-contacts li"),
    olderContactsLoader = $(".js-see-older-contacts"),
    fillerContactList = $(".js-filler-contact"),
    modalEditUser = $("#wz-modal-edit-user"),
    $assignTemplate = $(".js-loadtemplate-assign"),
    modalPaymentGenerator = $("#wz-modal-payment-generator"),
    modalGallery = $("#wz-modal-gallery"),
    autoresponsesModal = $("#wz-modal-responses"),
    editAutoresponseModal = $("#wz-modal-edit-response"),
    customersModal = $("#wz-modal-clients"),
    reportsModal = $("#wz-modal-reports"),
    pricingSwich = $(".js-pricing-switch"),
    pricingRoninTotal = $(".js-pricing-ronin-total"),
    pricingTelegram = $(".js-pricing-telegram"),
    pricingWhatsapp = $(".js-pricing-whatsapp"),
    pricingFacebook = $(".js-pricing-facebook"),
    pricingSmooch = $(".js-pricing-smooch"),
    pricingModal = $("#wz-modal-pricing"),
    pricingSelectPlan = $(".js-pricing-select-plan"),
    QRCodeModal = $("#wz-modal-qrcode"),
    QRCodeContainer = $("#qrcode-container"),
    QRCodeChannelIcon = $(".js-modal-qrcode-channel-icon"),
    QRCodeChannelName = $(".js-modal-qrcode-channel-name"),
    QRCodeModalClose = $(".js-modal-qrcode-close"),
    QRModalPreloader = $("#wz-modal-qrcode-preload"),
    QRModalPreloading = $(".js-qr-preloader"),
    QRModalSuccess = $(".js-qr-success"),
    accountLockedModal = $("#wz-modal-account-locked"),
    $notificationModal = $("#wz-modal-notification"),
    $notificationModalIcon = $("#wz-modal-notification .js-modalnot-icon"),
    $notificationModalTitle = $("#wz-modal-notification .js-modalnot-title"),
    $notificationModalMessage = $("#wz-modal-notification .js-modalnot-message"),
    $notificationModalButtons = $("#wz-modal-notification .js-modalnot-buttons"),
    $notificationModalSection = $("#wz-modal-notification section"),
    $notificationModalCloseIcon = $("#wz-modal-notification header button"),
    $notificationModalSeparator = $("#wz-modal-notification .wz-separator"),
    welcomeModal = $("#wz-modal-welcome"),
    settingsModal = $("#wz-modal-clients"),
    settingsUser = $("#user-settings"),
    settingsHotline = $("#hotline-setting"),
    conversationDownload = $("#conversation-download"),
    settingsBrand = $("#company-settings"),
    settingsChannels = $("#channel-settings"),
    settingsAgents = $("#agent-settings"),
    showRonin = $(".js-open-ronin"),
    roninModal = $("#wz-modal-ronin"),
    automationsModal = $("#wz-modal-automations"),
    botsContactModal = $("#wz-modal-bots-contact"),
    roninManagerModal = $("#wz-modal-ronin-manager");
var $blockTemplateMenuLight, $blockTemplateMenuHeavy, $blockTemplateSubmenu, $blockTemplateTextarea, $blockTemplateOptions,
    $sideleft1Scroll = $("#sideleft1-scroll"), $sideleft2Scroll = $("#sideleft2-scroll"), $sideleft3Scroll = $("#sideleft3-scroll"), $centralScroll = $("#central-scroll"),
    siderightScrollSelector = "#wz-col-sideright .js-perfect-scroll", $siderightScroll = $(siderightScrollSelector), $leftNavigation = $("#left-navigation");



function introDoneClass() {
    $(".introjs-skipbutton").addClass("js-done")
}

function introSkipClass() {
    $(".introjs-skipbutton").removeClass("js-done")
}

function QR8bitByte(t) {
    this.mode = QRMode.MODE_8BIT_BYTE, this.data = t
}

function QRCode(t, e) {
    this.typeNumber = t, this.errorCorrectLevel = e, this.modules = null, this.moduleCount = 0, this.dataCache = null, this.dataList = new Array
}

function QRPolynomial(t, e) {
    if (void 0 == t.length) throw new Error(t.length + "/" + e);
    for (var i = 0; i < t.length && 0 == t[i];) i++;
    this.num = new Array(t.length - i + e);
    for (var n = 0; n < t.length - i; n++) this.num[n] = t[n + i]
}

function QRRSBlock(t, e) {
    this.totalCount = t, this.dataCount = e
}

function QRBitBuffer() {
    this.buffer = new Array, this.length = 0
}

function URL(t) {
    this.loadDataParams = function (e) {
        void 0 !== e && (t += "?", t += Object.keys(e).map(function (t) {
            return [t, e[t]].map(encodeURIComponent).join("=")
        }).join("&"))
    }, this.getURL = function () {
        return t
    }
}

function CloudService() {
    self.urls = {
        brands: {
            stripe: {
                "delete": "/brands/stripe/delete/",
                save: "/brands/stripe/save/"
            },
            braintree: {
                "delete": "/brands/braintree/delete/",
                save: "/brands/braintree/save/"
            },
            subscriptions: {
                subscription_form: "/brands/subscriptions/subscription_form/",
                has_taxes: "/brands/subscriptions/has_taxes/",
                apply_promo_code: "/brands/subscriptions/check_code/"
            },
            views: {
                new_shortcut: "/shortcuts/new/",
                edit_shortcut: "/shortcuts/edit/"
            },
            data: {
                update_shortcut: apiURL + "brands/shortcuts/update/",
                add_shortcut: apiURL + "brands/shortcuts/create/",
                delete_shortcut: apiURL + "brands/shortcuts/delete/",
                get_shortcuts: apiURL + "brands/shortcuts/list/",
            },
            set_alert: "/brands/alert/",
            save_alert: "/brands/save_alert/",
            unassign: "/brands/unassign/",
            reports_html: "/reports/",
            reports: apiURL + "reports/",
            agents_top: apiURL + "reports/top_agents/",
            customers_top: apiURL + "reports/top_customers/",
            counters: "/brands/counters",
            webwidget: "/brands/webwidget/",
            save_logo: apiURL + "business/save_logo/",
            update: "/business/update/",
            conversation_download:"/business/downloads"
        },
        channels: {
            edit: "/channels/edit/",
            "new": "/channels/new/",
            activate: "/channels/activate/",
            reload: "/channels/edit/",
            upsert_page_zalo: "/channels/UpsertPageZalo/",
            edit_page_zalo: "/channels/EditPageZalo/",
            edit_personal_zalo: "/channels/EditPersonalZalo/",
            upsert_personal_zalo: "/channels/UpsertPersonalZalo/",
            data: {
                get_all: apiURL + "brands/channels/list/",
                update: apiURL + "brands/channels/update/",
                create: apiURL + "brands/channels/create/",
                single: apiURL + "brands/channels/single/",
                "delete": apiURL + "brands/channels/delete/",
            }
        },
        zalos: {
            send: apiURL + "brands/zalos/send/",
            upsert: apiURL + "brands/zalos/upsert/",
        },
        customers: {
            "new": "/customers/new",
            get_picture: "/customers/get_picture/",
            openprofile: "/customers/openprofile/",
            profile: "/customers/profile/",
            edit: "/customers/edit/",
            profileinfo: "/customers/profileinfo/",
            send_note: "/notes/send_customer_note/",
            remove_note: apiURL + "/notes/delete_customer_note/",
            assign: apiURL + "customers/assign/",
            assign_to: apiURL + "customers/assign/",
            unassign: apiURL + "customers/unassign/",
            last_ticket: "/tickets/last/",
            get: "/customers/get",
            get_send_files: "/customers/get_send_files/",
            block: apiURL + "/customers/block/",
            inner_note: "/customers/inner_note/",
            list: apiURL + "customers/list/",
            read: apiURL + "customers/read/",
            unread: apiURL + "customers/unread/",
            archive: "/customers/archive/",
            unarchive: "/customers/unarchive/",
            search: apiURL + "customers/search/",
            options: "/customers/option/",
            autoticket: "/customers/create_autoticket/"
        },
        threads: {
            list: apiURL + "threads/list/",
            get: apiURL + "threads/get/",
            read: apiURL + "threads/read/",
            unread: apiURL + "threads/unread/",
            profile: "/threads/profile/",
            last_visit: apiURL + "threads/last_visit/"
        },
        messages: {
            openlink: appURL + businessID + "/messages/openlink/",
            like: apiURL + "messages/like/",
            hide: apiURL + "messages/hide/",
            read: apiURL + "messages/read/",
            star: apiURL + "messages/star/",
            list: apiURL + "messages/list/",
            send: apiURL + "messages/send/",
            get: apiURL + "messages/get/",
            "delete": apiURL + "messages/delete/",
            send_files: apiURL + "messages/send_files/",
            sendtemplate: apiURL + "messages/sendtemplate/",
            savemessage: apiURL + "messages/savemessage/",
        },
        tickets: {
            add: "/tickets/add/",
            edit: "/tickets/edit/",
            send: "/tickets/send/",
            view: "/tickets/view/",
            set_short_description: apiURL + "tickets/set_short_description",
            set_description: apiURL + "tickets/set_description",
            set_status: apiURL + "tickets/set_status",
            edit_tags: apiURL + "tickets/edit_tags",
            profile: "/tickets/profile/",
            send_note: "/tickets/send_note/",
            edit_note: "/tickets/edit_note/",
            remove_note: "/tickets/remove_note/",
            attach: "/tickets/attach/",
            unattach: "/tickets/unattach/"
        },
        settings: {
            info: "/accounts/settings/",
            business: "/business/settings/",
            channels: "/channels/settings/",
            agents: "/agents/settings/",
            data: {
                update_account: apiURL + "brands/agents/update/"
            }
        },
        shinobi: {
            flights: {
                get_form: "/shinobi/flights/get_form/",
                options: "/shinobi/flights/get_options/"
            }
        },
        ronin: {
            load: "/ronin/load/",
            edit: "/ronin/edit/",
            save_mode: "/ronin/save_mode/",
            manager: "/ronin/manager/",
            request_bot: "/ronin/request_bot/",
            disable: "/ronin/disable/",
            "delete": "/ronin/delete/"
        },
        agents: {
            profile: "/agents/get_agent_view",
            set_status: apiURL + "brands/agents/set_status/",
            edit: "/agents/edit/",
            "new": "/agents/new/",
            register: apiURL + "brands/agents/register/",
            data: {
                get_all: apiURL + "brands/agents/list/",
                update: apiURL + "brands/agents/update/",
                create: apiURL + "brands/agents/create/",
                single: apiURL + "brands/agents/single/",
                admin: apiURL + "brands/agents/toogle_admin/",
                "delete": apiURL + "brands/agents/delete/",
                save_avatar: apiURL + "brands/agents/save_avatar/",
                save_token: apiURL + "brands/agents/save_token/"
            }

        },
        hotline: {
            get: appURL + "hotline/get/",
            upsert: appURL + "hotline/upsert/",
            getCustomerInfo: apiURL + "hotline/getCustomerFormPhone/",
        },
    }, self.createURL = function () {
        var t = "";
        arguments.length > 0 && (t = arguments[0]);
        for (var e = 1; e < arguments.length; e++) t += "/" + arguments[e];
        return t
    }
}

function confirmMsgDel(t, e) {
    var n = $deleteMessageDropdown.tmpl({
        key: e
    }),
        i = $(t).parent();
    i.append(n), i.on("hide.bs.dropdown", function () {
        i.find(".dropdown-menu").remove()
    })
}

function playAlert(t) {
    audio.play(), date = new Date, notificationsList.append(notificationTemplate.tmpl({
        date: date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes(),
        message: t.message,
        tag: t.tag
    })), iconBell.addClass("js-alarm-active"), $(".js-notifications-list li:nth-child(2)").remove(), new NotificationPlugin(trAlert, t.message, "icon-bell")
}

function showNotification(t) {
    new NotificationPlugin(t.title, t.message, t.icon)
}

function processNotification(t) {
    t.type ? "alert" == t.type ? playAlert(t.data) : "browser-notification" == t.type ? playNotification(t.data) : "qr" == t.type ? modalQRCode(t.data) : "notification" == t.type && showNotification(t.data) : playAlert(t)
}

function createListeners() {
    $(".js-invite-agent").click(function () {
        settingsUIRedirect = "#agent-settings", $topBarSettings.trigger("click"), cloud.loadSettingsAgents().done(function () {
            $(".js-show-edit-agent").not("[data-agent]").trigger("click")
        })
    }), messageListContainer.on("click", function (t) {
        ("LI" == t.target.nodeName || "SECTION" == t.target.nodeName) && txt.focus()
    }), inputImage.on("change", function (t) {

        $("#js-gallery-error-upload").addClass("hide"), $(".js-submit-image").prop("disabled", !1), modalImg.modal("show");
        for (var e = 0; 6 > e; e++) imgSendFlags[e] = !1;
        var i = t.target.files;

        i.length <= fileUploadLimit ? $.each(i, function (t, e) {
            var i = new FileReader;
            i.onload = function (e) {
                $(".js-channel-format-chat").val("image-text");
                var data = { elements: [{ image_url: e.target.result, title: "" }] };
                loadImageToDropableZone("facebook", "image-text", data);
                imgSendFlags[t] = !0;
                //$("<div>", {
                //    "class": "col-xs-4"
                //}).append($("<a class='thumbnail' href='#' style='background-image: url(" + e.target.result + ")'><span class='js-thumbnail-close' data-index='" + t + "' href='#'>×</span></a>")).appendTo("#js-gallery-image-upload"), imgSendFlags[t] = !0
                //inputImage.val("")
            }, i.readAsDataURL(e), i.onloadend = function () {
                $(".js-thumbnail-close").unbind("click"), $(".js-thumbnail-close").bind("click", function () {
                    var t = $(this).data("index"),
                        e = $(this).parent().parent();
                    imgSendFlags[t] = !1, e.remove();
                })
            }
        }) : ($("#js-gallery-error-upload").removeClass("hide"), $(".js-submit-image").prop("disabled", !0))

    }), buttonOpenGallery.click(function (t) {
        preventDefault(t), modalImgGallery.hasClass('hidden') ? modalImgGallery.removeClass('hidden') : modalImgGallery.addClass('hidden');
    }), selectImgGallery.click(function () {
        modalImgGallery.addClass('hidden')
    }), cancelImgGallery.click(function () {
        modalImgGallery.addClass('hidden')
    }), resetImg.click(function () {
        sendForm[0].reset(), modalImg.modal("hide"), modalImgGallery.addClass('hidden')
    }), modalImg.on("hidden.bs.modal", function () {
        $("#js-gallery-image-upload").html(""), inputImage.val("")
    }), modalShortcuts.on("shown.bs.modal", function () {
        $("#shortcuts-table").hide()
    }), modalShortcuts.on("shown.bs.modal", function () {
        $("#shortcuts-table").tabulator("redraw", !0)
    }), chatsSelect.click(chatFilterSelection), $(".js-chats-select span").click(function () {
        $topBarModals.trigger("hidden.bs.modal");
        $topBarChats.trigger("click");
    }), chatsStatusSelect.click(function (t) {
        preventDefault(t), selectClient.html($(this).children("span").text() + "<span class='caret'>"), showTicketView(!1), contactStatusView = $(this).data("status"), contactList.empty(), cloud.loadContacts(0, limitContacts, chatView, chatViewAgent, chatViewChannel)
    }), viewCurrentTicket.click(function (t) {
        preventDefault(t), cloud.getTicketProfile(currentTicket), viewProfileTicket(currentTicket)
    }), archiveCurrent.click(function (t) {
        var e = $(".js-status-select.selected");
        if (e.size() > 0) {
            var i = e.data("status");
            if ("0" == i || "1" == i) return void $(this).find("a").attr("data-toggle", "dropdown")
        }
        $(this).find("a").attr("data-toggle", ""), CONTACTS[currentChat].archive()
    }), archiveCurrentOption.click(function (t) {
        var e = $(this).data("status");
        "undefined" != typeof e ? cloud.setStatus(e).done(function () {
            CONTACTS[currentChat].archive(), mixpanelEvents("changeTicketStatusArchive", STATUS[e])
        }) : CONTACTS[currentChat].archive()
    }), txt.on("focus", function (t) {
        currentChat && "object" == typeof CONTACTS[currentChat] && cloud.setReaded(currentChat)
    }), unreadCurrent.click(function (t) {
        preventDefault(t), THREADS[CONTACTS[currentChat].getKey()].unread()
    }), openCurrent.click(function (t) {
        console.log("open linksss");
        console.log(self.urls.messages.openlink + '?item_id=' + CONTACTS[currentChat].getKey());
        preventDefault(t), window.open(self.urls.messages.openlink + '?item_id=' + CONTACTS[currentChat].getKey(), '_blank');
    }), $("#wz-modal-clients .wz-crm-menu li a").click(function (t) {
        t.preventDefault();
        var e = $(this).attr("href");
        cloud.openSettings(e)
    }), $leftNavigation.click(function () {
        $sideLeft1.hasClass("off") ? $sideLeft1.removeClass("off") : $sideLeft1.addClass("off");
        $sideLeft2.hasClass("off") ? $sideLeft2.removeClass("off") : $sideLeft2.addClass("off");
        $sideLeft3.hasClass("off") ? $sideLeft3.removeClass("off") : $sideLeft3.addClass("off");
        $colCentral.hasClass("off") ? $colCentral.removeClass("off") : $colCentral.addClass("off");
        $colSideright.hasClass("off") ? $colSideright.removeClass("off") : $colSideright.addClass("off");
    }), $topBarOption.click(function () {
        if (!$(this).hasClass("loaded")) {
            $topBarOption.removeClass("loaded"), $topBarModals.modal("hide");
            var t = $(this).find("a").attr("href");
            if ("#" != t) {
                switch (t) {
                    case "#wz-modal-reports":
                        cloud.getReports();
                        break;
                    case "#wz-modal-clients":
                        cloud.openSettings(), mixpanelEvents("openSettings");
                        break;
                    case "#wz-modal-automations":
                        cloud.loadRoninModal();
                        break;
                    case "#wz-modal-ronin-manager":
                        cloud.roninManager()
                }
                $(t).modal("show")
            } else tutorialActive && $(".modal:visible").length < 1 && welcomeModal.modal("show");
            $(this).addClass("loaded")
        }
    }), $topBarModals.on("hidden.bs.modal", function () {
        var t = $(".js-topbar-option.active").find("a").attr("href"),
            e = "#" + $(this).attr("id");
        t == e && $topBarChats.trigger("click")
    }), $addAgent.click(function () {
        "admin" == role && (cloud.openSettings("#agent-settings").done(openSettingsModal), mixpanelEvents("addAgentClick"))
    }), $addChannel.click(function (t) {
        "admin" == role && (cloud.openSettings("#channel-settings").done(openSettingsModal), mixpanelEvents("addChannelClick"))
    }), $("input[name='chat-search']").on("input propertychange paste", function (t) {
        t.preventDefault();
        var e = $(this).val();
        e ? ($(".js-chat-date-subhead").fadeOut(), $(".js-chat-message-text:not(:Contains('" + e + "'))").parent().parent().fadeOut(), $(".js-chat-message-text:Contains('" + e + "')").parent().parent().fadeIn(), $(".js-chat-zami .js-chat-message-text:not(:Contains('" + e + "'))").parent().parent().parent().parent().fadeOut(), $(".js-chat-zami .js-chat-message-text:Contains('" + e + "')").parent().parent().parent().parent().fadeIn()) : $(".js-chat-message, .js-chat-date-subhead").fadeIn()
    }), $(".js-close-wz-finder").on("click", function (t) {
        $(".js-chat-message, .js-chat-date-subhead").fadeIn()
    }), $(".js-disable-tutorial").on("click", disableTutorial), $(".js-welcome-panel .btn.btn-pink[data-channeltype]").bind("click", function () {
        var t = $(this).data("channeltype"),
            e = ($(this).data("target"), $(this).hasClass("actived"));
        Object.keys(CHANNELS).length <= 0 ? (settingsModal.removeClass("wz-modal-right").addClass("wz-modal-fullscreen modal-firstchannel"), "facebook" == t && settingsModal.addClass("modal-facebook"), settingsModal.find(".modal-menu").hide()) : (settingsModal.addClass("wz-modal-right").removeClass("wz-modal-fullscreen modal-firstchannel modal-facebook"), settingsModal.find(".modal-menu").show()), settingsUIRedirect = "#channel-settings", cloud.loadSettingsChannels().done(function () {
            $(".js-topbar-option").removeClass("loaded active"), $(".js-topbar-settings").addClass("loaded active"), $("#channels-main").addClass("hide"), $("#channels-edit").removeClass("hide"), settingsModal.modal("show"), e ? $("#channels-group ." + t + ":first").parent().parent().find(".js-edit-channel").trigger("click") : $("#channels-addgroup ." + t).trigger("click")
        })
    }), reportsModal.on("hide.bs.modal show.bs.modal", hideSideright), $(document).ajaxStart(function () {
        NProgress.inc()
    }), $(document).ajaxStop(function () {
        NProgress.done()
    }), $("#wz-modal-bots-contact .btn[data-step]").click(function () {
        var t = $(this).data("step"),
            e = $(this).data("cur-step");
        setBotsContactStep(t, e)
    }), $("#wz-modal-bots-contact .textarea-2A, #wz-modal-bots-contact .textarea-2B").bind("input", function () {
        "" == $(this).val() ? $("#wz-modal-bots-contact .js-next-step").addClass("disabled") : $("#wz-modal-bots-contact .js-next-step").removeClass("disabled")
    }), $("#wz-modal-bots-contact #js-bots-contact-submit").click(function () {
        var t = botDescription2A.val() || botDescription2B.val(),
            e = botDescription2A.val() ? "Current chatbots management" : "New chatbot request";
        setTimeout(function () {
            botDescription2A.val(""), botDescription2B.val(""), $("#wz-modal-bots-contact .js-next-step").addClass("disabled"), setBotsContactStep("1")
        }, 200), cloud.roninRequest(t, e)
    })
}

function sendImageForm() {
    var texts = [];
    $(".textWithImage").each(function (key, v) {
        texts.push($(v).val());
    });
    //txt.val(txtImage.val()), 
    //    inputConfigChat.val(), inputParaChat.val(),

    sendForm.submit();
    //$("#js-gallery-image-upload").html(""), imgSendUrls = [], inputImage.val(""), txtImage.val(""),
    //inputConfigChat.val(""), inputParaChat.val("")

}


function resetChatview() {
    return chatViewAgent = !1, chatViewChannel = "all", chatView = "all", lastChatView = !1, $(".wz-col-sideleft-1 .js-tab-chats, .wz-filter li a").removeClass("active"), $('.wz-col-sideleft-1 .js-tab-chats[data-agent="all"]').addClass("active"), $('.wz-col-sideleft-1 .js-tab-chats[data-channel="all"]').addClass("active"), $(".wz-filter .js-select-all").addClass("active"), cloud.loadContacts(0, limitContacts, chatView, chatViewAgent, chatViewChannel)
}

function showTickets() {
    var t = getTicketsSelectorShow(),
        e = getTicketsSelectorHide();
    $(t).fadeIn(), e && $(e).fadeOut()
}

function noCustomersViewHandler() {
    contactList.find("li").size() > 0 ? emptyCol02.hide() : emptyCol02.show(), contactList.find("li").size() >= limitContacts ? fillerContactList.show() : fillerContactList.hide()
}


function firstLoadAgents() {
    $.get(cloud.getURLs().agents.data.get_all, function (t) {
        t.data && t.data.forEach(function (t, e) {
            $("li.js-agent-list[data-agent=" + t.id + "]").removeClass("js-agent-online"), $("li.js-agent-list[data-agent=" + t.id + "]").removeClass("js-agent-offline"), $("li.js-agent-list[data-agent=" + t.id + "]").removeClass("js-agent-busy"), $("li.js-agent-list[data-agent=" + t.id + "]").addClass("js-agent-" + t.status.toLowerCase())
        })
    })
}

function getTicketsSelectorShow() {
    var t = ".js-list-tickets li";
    return "assigned" == ticketAssignedView ? t += "[data-agent='" + myID + "']" : "unassigned" == ticketAssignedView && (t += "[data-agent='']"), "all" !== ticketStatusView && (t += "[data-status='" + ticketStatusView + "']"), "all" !== ticketResponsabilityView && (t += "[data-responsability='" + ticketResponsabilityView + "']"), t
}

function getTicketsSelectorHide() {
    var t = ".js-list-tickets li:not(";
    return "assigned" == ticketAssignedView ? t += "[data-agent='" + myID + "']" : "unassigned" == ticketAssignedView && (t += "[data-agent='']"), "all" !== ticketStatusView && (t += "[data-status='" + ticketStatusView + "']"), "all" !== ticketResponsabilityView && (t += "[data-responsability='" + ticketResponsabilityView + "']"), t += ")", ".js-list-tickets li:not()" != t ? t : !1
}

function loadTargetMessage(t, e) {
    var i = $(e).data("message-key");
    return firebaseService.getMessage(CUSTOMER_ID, i).done(function (t) {
        if (t) {
            var n = $(e).hasClass("wz-starred-message") ? $(e).find(".wz-starred-text span") : $(e).hasClass("wz-last-message") ? $(e).find(".wz-message-text span") : $(e).find(".wz-note-text span");
            if ("text" == t.type || "note" == t.type) n.text(t.message), linkifyElem(n);
            else if ("image" == t.type) {
                var o = t.file_name || "[VIEW THE PHOTO]";
                if (0 == o.length)
                    if (t.file_name) o = t.file_name;
                    else if (t.url) {
                        var a = t.url.split("/");
                        o = a[a.length - 1]
                    } else o = "Photo";
                var r = $("<span>").text(o).addClass("wz-link wz-cursor-hand").append($("<i class='icon-paper-clip wz-font-c-light-gray-4'></i>")).on("click", function () {
                    openImageUrl(t.url)
                });
                n.append(r)
            } else n.text(" ");
            var s = formatShortDate(dateFromTimestamp(t.timestamp)),
                l = $(e).hasClass("wz-starred-message") ? $(e).find(".wz-starred-date") : $(e).hasClass("wz-last-message") ? $(e).find(".wz-message-date") : $(e).find(".wz-note-date");
            l.text(s), $(e).attr("data-timestamp", t.timestamp);
            var c = $(e).hasClass("wz-starred-message") ? $(e).find(".wz-starred-jump") : $(e).hasClass("wz-last-message") ? $(e).find(".wz-message-jump") : $(e).find(".wz-note-jump");
            var sc = (t.agent_id === undefined || !t.agent_id) && t.sender_ext_id != t.channel_ext_id;
            if (c.bind("click", function () {
                cloud.goToMessage(CUSTOMER_ID, i, t.timestamp)
            }), sc === !0) {
                var d = function () {
                    var ta = CONTACTS[CUSTOMER_ID].getAvatar();
                    $(this).find(".js-picture").css("background-image", "url(" + ta + ")"), $(this).find(".js-picture").addClass("js-picture-" + CUSTOMER_ID);
                    var e = CONTACTS[CUSTOMER_ID].getInitials();
                    $(this).find(".js-initials p").text(e), $(this).find(".js-initials").addClass("js-initials-" + CUSTOMER_ID + " " + getAvatarColor(CUSTOMER_ID)), -1 != ta.indexOf("bot.png") && ($(this).find(".js-initials").removeClass("hide"), $(this).find(".js-picture").addClass("hide")), $(this).find(".wz-starred-author").text(CONTACTS[CUSTOMER_ID].getName()), $(this).find(".wz-message-author").text(CONTACTS[CUSTOMER_ID].getName()), $(this).find(".js-picture").parent().attr("data-toggle", "tooltip").attr("data-original-title", CONTACTS[CUSTOMER_ID].getName())
                };
                CONTACTS[CUSTOMER_ID] ? d.bind(e)() : cloud.getContact(CUSTOMER_ID, !0).done(d.bind(e))
            } else {
                var u = (AGENTS[t.agent_id] && AGENTS[t.agent_id].avatar) ? AGENTS[t.agent_id].avatar : (cloud.getMediaUrl() + "avatars/bot.png");
                AGENTS[t.agent_id] ? ($(e).find(".wz-ticket-avatar, .wz-note-avatar, .wz-starred-avatar, .wz-message-avatar").attr("data-toggle", "tooltip").attr("data-original-title", AGENTS[t.agent_id].name)) : ($(e).find(".wz-ticket-avatar, .wz-note-avatar, .wz-starred-avatar, .wz-message-avatar").attr("data-toggle", "tooltip").attr("data-original-title", t.sender_name)), $(e).find(".js-picture").addClass("js-picture-agent-" + t.agent_id).removeClass("hide"), $(e).find(".js-picture").css("background-image", "url(" + u + ")"), $(e).find(".js-initials").remove(),
                    AGENTS[t.agent_id] ? $(e).find(".wz-starred-author").text(AGENTS[t.agent_id].name) : $(e).find(".wz-starred-author").text(t.sender_name),
                    AGENTS[t.agent_id] ? $(e).find(".wz-message-author").text(AGENTS[t.agent_id].name) : $(e).find(".wz-message-author").text(t.sender_name)
            }
            $(e).removeClass("js-loader-open")
        } else $(e).remove()
    })
}

function viewProfileTicket(t) {
    if (t) {
        if (viewTicketSelected == t) return;
        viewTicketSelected = t
    }
    "ticket" != viewProfile && (viewProfile = "ticket", viewAgentSelected = !1, viewPaymentSelected = !1, colSiderightTicket.show(), colSiderightClient.hide(), colSiderightPayment.hide(), colSiderightAgent.hide()), mixpanelEvents("viewTicket")
}

function viewProfileClient() {
    "client" != viewProfile && (viewProfile = "client", viewTicketSelected = !1, viewAgentSelected = !1, viewPaymentSelected = !1, colSiderightTicket.hide(), colSiderightPayment.hide(), colSiderightAgent.hide(), colSiderightClient.show()
    )
}

function viewProfileAgent(t) {
    if (t) {
        if (viewAgentSelected == t) return;
        viewAgentSelected = t
    }
    "agent" != viewProfile && (viewProfile = "agent", viewTicketSelected = !1, viewPaymentSelected = !1, colSiderightTicket.hide(), colSiderightClient.hide(), colSiderightPayment.hide(), colSiderightAgent.show())
}

function viewProfilePayment(t) {
    if (t) {
        if (viewPaymentSelected == t) return;
        viewPaymentSelected = t
    }
    "payment" != viewProfile && (viewProfile = "payment", viewTicketSelected = !1, viewAgentSelected = !1, colSiderightTicket.hide(), colSiderightClient.hide(), colSiderightAgent.hide(), colSiderightPayment.show())
}

function removeSettings() {
    var t = function (t, e) {
        $(e).hasClass("js-settings-suscription-opt") || $(e).remove()
    };
    $("#wz-modal-clients .modal-header .close").remove(), $(".js-topbar-option").each(t), $("#wz-modal-clients").modal({
        keyboard: !1
    }), $("#wz-modal-clients").modal("hide"), searchContactContainer.css("visibility", "hidden")
}

function saveBrand() {
    cloud.sendForm(document.getElementById("form-settings-brand"), cloud.getURLs().brands.update + businessID, function (t) {
        t.view ? settingsBrand.html(t.view) : (cloud.loadSettingsBrand(), new NotificationPlugin(NP_DATA_SAVE.title, NP_DATA_SAVE.message, NP_DATA_SAVE.icon, null, null, NP_DATA_SAVE.countdown))
    })
}

function openSettingsModal() {
    $topBarOption.removeClass("loaded active"), $topBarSettings.addClass("loaded active"), settingsModal.modal("show")
}

function saveInfo(t) {
    cloud.sendForm(document.getElementById("form-settings-info"), cloud.getURLs().settings.data.update_account + t, function (t) {
       
        t.view ? settingsUser.html(t.view) : (cloud.loadSettingsInfo().done(cloud.reloadAgents()), new NotificationPlugin(NP_DATA_SAVE.title, NP_DATA_SAVE.message, NP_DATA_SAVE.icon, null, null, NP_DATA_SAVE.countdown))
    })
}


function saveAgent(t) {
    void 0 === t ? cloud.sendForm(document.getElementById("form-settings-agent"), cloud.getURLs().agents.data.create + businessID, function (t) {
       
        t.view ? $("#agents-edit").html(t.view) : (t.paid && new NotificationPlugin(NP_PAYMENT.title, NP_PAYMENT.message, NP_PAYMENT.icon, null, null, NP_PAYMENT.countdown), cloud.loadSettingsAgents(), cloud.reloadAgents(), new NotificationPlugin(NP_DATA_SAVE.title, NP_DATA_SAVE.message, NP_DATA_SAVE.icon, null, null, NP_DATA_SAVE.countdown), $("#cancel-settings-agent").click(), mixpanelEvents("addAgent"))
    }) : cloud.sendForm(document.getElementById("form-settings-agent"), cloud.getURLs().agents.data.update + t, function (t) {
           
        t.view ? $("#agents-edit").html(t.view) : (cloud.loadSettingsAgents(), cloud.reloadAgents(), new NotificationPlugin(NP_DATA_SAVE.title, NP_DATA_SAVE.message, NP_DATA_SAVE.icon, null, null, NP_DATA_SAVE.countdown), $("#cancel-settings-agent").click(), mixpanelEvents("editAgent"))
    })
}


function saveChannelFacebook(t, e, b) {
    typeof t != "undefined" && (t.length > 0) ? cloud.sendForm(document.getElementById("channels-facebook-form"), cloud.getURLs().channels.data.update + b + '/' + t, function (t) {
        t.view ? $("#channels-edit").html(t.view) : (refreshChannels(), mixpanelEvents("editFacebookChannel", e))
    }) : cloud.sendForm(document.getElementById("channels-facebook-form"), cloud.getURLs().channels.data.create + "facebook/" + b, function (t) {
        t.view ? $("#channels-edit").html(t.view) : (t.paid && new NotificationPlugin(NP_PAYMENT.title, NP_PAYMENT.message, NP_PAYMENT.icon, null, null, NP_PAYMENT.countdown), 0 == cloud.getHowManyChannel() && mixpanelEvents("addFirstChannel", "Facebook"), refreshChannels(), mixpanelEvents("addFacebookChannel", e), tutorialActive && ($(".js-welcome-panel .btn.btn-pink[data-channeltype='facebook']").addClass("actived"), $topBarChats.trigger("click"), welcomeModal.modal("show"), $(".js-welcome-header").addClass("js-added"), addHow()))
    })
}

function refreshChannels() {
    return cloud.reloadChannels().done(cloud.loadSettingsChannels)
}

function refreshAgents() {
    return cloud.reloadAgents().done(cloud.loadSettingsAgents)
}

function restoreButton(t) {
    var e = $(t);
    e.size() > 0 && e.prop("disabled", !1)
}

function getAvatarColor(t) {
    var e = t % AVATAR_COLOR.length;
    return e >= 0 && e < AVATAR_COLOR.length ? AVATAR_COLOR[e] : AVATAR_COLOR[0]
}

function refreshAgent(t) {
    AGENTS[t] && void 0 !== AGENTS[t].avatar ? ($(".js-picture-agent-" + t).css("background-image", "url(" + cloud.getMediaUrl() + AGENTS[t].avatar + ")"), $(".js-name-agent-" + t).text(AGENTS[t].name.capitalizeFirstLetter())) : $(".js-picture-agent-" + t).css("background-image", "url(" + cloud.getMediaUrl() + "avatars/bot.png)")
}


function setClientInfoBtn(t, e, n) {
    $selector = $(document).find(t), $selector.find(".status").html(e), $selector.find(".dropdown i").removeClass("assigned unassigned archived").addClass(n), $selector.removeClass("hide")
}

function showPaymentModal() {
    modalPaymentGenerator.modal("show")
}

function notificationBellClick() {
    $("#wz-dropdown-notifications").parent().hasClass("open") || mixpanelEvents("openNotifications")
}


function setAssignationListeners(t) {
    if ("object" == typeof CONTACTS[currentChat]) {
        var e = CONTACTS[currentChat].getAssignButton();
        setClientInfoBtn(t, e.text, e["class"]), "assigned" == e["class"] ? $(".js-unassign-footer").css("display", "block") : ($(".js-unassign-footer").css("display", "none"), $(t + " .wz-search-popup ul li:last-child").css("border-bottom-left-radius", "4px"), $(t + " .wz-search-popup ul li:last-child").css("border-bottom-right-radius", "4px"))
    }
    $(".js-unassign-footer").click(function () {
        CONTACTS[currentChat].unassignMe()
    }), $(t + " .dropdown a").click(function () {
        $(t + " .wz-search-popup").toggle(), $(t + " .dropdown .caret ").toggleClass("up")
    }), $(t + " .js-assign-title").click(function () {
        $(t + " .wz-search-popup div").toggle(), $(t + " .js-assign-search input").focus()
    }), $(t + " .js-assign-search i").click(function () {
        $(t + " .wz-search-popup div").toggle(), $(t + " .js-assign-search input").val(""), $(t + " .js-assign-search input").trigger("keyup")
    }), $(t + " .js-assign-search input").keyup(function (e) {
        var n = $(this).val(),
            i = new RegExp(n, "ig");
        $(t + " .wz-search-popup ul li").each(function (t) {
            0 == n.length || $(this).find("span").text().match(i) ? $(this).fadeIn() : $(this).fadeOut()
        })
    }), loadSearchAgent(t)
}

function loadSearchAgent(t) {
    var e = $(t + " .wz-search-popup ul");
    e.empty(), $.each(Object.keys(AGENTS), function (n, i) {
        //if (AGENTS[i] && AGENTS[i].name != undefined && AGENTS[i].status && "online" === AGENTS[i].status.toLowerCase()) {
        if (AGENTS[i] && AGENTS[i].name != undefined) {
            var a = $('<li  data-key="' + i + '">').append('<i class="' + AGENTS[i].status + '">').append("<span>" + AGENTS[i].name + "</span>");
            a.click(function () {
                var e = $(this).data("key");
                CONTACTS[currentChat].assignTo(e), $(t + " .wz-search-popup").toggle(), $(t + " .dropdown .caret ").toggleClass("up")
            }), e.append(a)
        }
    }), $(t + " .js-assign-search input").trigger("keyup")
}


function openImageModal(t) {
    $modalFullImage.find(".modal-body, .modal-header").on("click", function (t) {
        "IMG" == t.target.nodeName ? zoomImageElem($(t.target)) : $modalFullImage.modal("hide")
    }), $modalFullImage.find(".modal-body").empty(), $modalFullImage.find(".modal-body").append(t), $modalFullImage.modal("show")
}

function openImageUrl(t) {
    var e = $('<img class="wz-cursor-hand">').attr("src", t);
    openImageModal(e)
}

function openImageMessage(t, e) {
    var i = firebaseService.getAttach(t, e);
    i.done(function (t) {
        var tt = {
            key: t.id,
            val: function () {
                return t;
            }
        }
        var e = new Message(tt);
        openImageModal(e.getRendered("image-full"))
    })
}

function putText(t) {
    txt.val($(t).text()), txt.focus()
}

function chatFilterSelection(t) {
    t.preventDefault(), $('.js-tab-chats[data-option="' + $(this).data("option") + '"]').removeClass("active");
    var e = $(this).data("option"),
        i = $(this).data("agent"),
        n = $(this).data("channel"),
        o = $('.js-tab-chats[data-option="agent"][data-agent="all"]');
    return $(this).parent().hasClass("panel-title") ? $(".js-tab-chats[data-" + e + '="all"]').addClass("active") : $(this).addClass("active"), "agent" == e ? "all" == i ? (chatViewAgent = !1, $(".js-active a").text(trAssigned), $(".js-all a").text(trAll), $(".wz-filter li a").removeClass("active"), $(".js-all a").addClass("active"), chatView = !1, $(".js-active").fadeOut(200), $(".js-pending, .js-archived").fadeIn(200)) : (chatViewAgent = i, chatView = "active", $(".js-active").removeClass("hide").fadeIn(200), $(".js-pending, .js-archived").fadeOut(200, function () {
        $(".wz-filter li a").removeClass("active"), $(".js-active a").addClass("active"), $(".js-active a").html(AGENTS[i].name || trDeletedAgent)
    })) : "channel" == e ? chatViewChannel = "all" == n ? !1 : n : "all" == e ? (chatView = !1, 0 != chatViewAgent && (chatViewAgent = !1, $(".js-active a").text(trAssigned), $(".js-all a").text(trAll), $('.js-tab-chats[data-option="agent"]').removeClass("active"), o.addClass("active"), $(".js-active").removeClass("hide").fadeOut(200), $(".js-pending, .js-archived").fadeIn())) : chatView = e,
        lastChatView = chatView, ("agent" == e || "channel" == e) && openContact.trigger("click"), contactList.empty(), (e == "channel" && n == "all" || e == "agent" && i == "all") && $(this).parent().addClass('active'),
        cloud.loadContacts(0, limitContacts, chatView, chatViewAgent, chatViewChannel)

}

function showSideright() {
    reportsModal.addClass("wz-modal-cutted"), $colSideright.addClass("wz-tweaked")
}

function hideSideright() {
    reportsModal.removeClass("wz-modal-cutted"), $colSideright.removeClass("wz-tweaked"), reportsModal.find(".tableContainer .selected").removeClass("selected"), (viewAgentSelected || viewPaymentSelected) && viewProfileClient()
}

function generateWidgetSnippet(t, e, i) {
    var n = '!function(a,b,c,d,e,f,g){a.WebWidget=e,a[e]=a[e]||function(){(a[e].q=a[e].q||[]).push(arguments)},a[e].l=1*new Date,f=b.createElement(c),g=b.getElementsByTagName(c)[0],f.async=1,f.src=d,g.parentNode.insertBefore(f,g)}(window,document,"script","/js/webwidget-1.1.min.js","webwidget");',
        o = "var __channel=" + JSON.stringify(t) + ";",
        a = "var __agents=" + JSON.stringify(e) + ";",
        r = "var __content=" + JSON.stringify(i) + ";",
        s = "webwidget(__channel, __agents, __content);";
    return '<script type="text/javascript">' + n.concat(o, a, r, s) + "</script>"
}

function initPerfectScroll() {
    $sideleft1Scroll.perfectScrollbar({
        suppressScrollX: !0,
        wheelPropagation: !0
    }), $sideleft2Scroll.perfectScrollbar({
        suppressScrollX: !0,
        wheelPropagation: !0
    }), $sideleft3Scroll.perfectScrollbar({
        suppressScrollX: !0,
        wheelPropagation: !0
    }), $centralScroll.perfectScrollbar({
        suppressScrollX: !0,
        wheelPropagation: !0
    }), $siderightScroll.perfectScrollbar({
        suppressScrollX: !0,
        wheelPropagation: !0
    })
}

function format0Value(t) {
    return 10 > t ? "0" + t : "" + t
}

function getDateFromTimestamp(t) {
    0 > t && (t *= -1);
    var e = new Date(1e3 * t),
        i = ["January", "February", "March", "April", "May", "June", "July", "Augaust", "September", "October", "November", "December"],
        n = e.getFullYear(),
        o = i[e.getMonth()],
        a = e.getDate(),
        r = e.getHours(),
        s = e.getMinutes(),
        l = e.getSeconds(),
        c = a + "/" + o + "/" + n + " " + r + ":" + s + ":" + l;
    return c
}

function getMonthName(t) {
    if ("number" != typeof t) return !1;
    if (0 > t || t > 11) return !1;
    var e = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    return e[t]
}

function formatShortDatetime(t) {
    return t.getMonth() + 1 + "/" + t.getDate() + " " + format0Value(t.getHours()) + ":" + format0Value(t.getMinutes())
}

function formatShortTime(t) {
    return format0Value(t.getHours()) + ":" + format0Value(t.getMinutes())
}

function formatDateYYMD(t) {
    var e = t.getMonth() + 1;
    1 == e.toString().length && (e = "0" + e);
    var i = t.getDate();
    1 == i.toString().length && (i = "0" + i);
    var n = t.getFullYear() + "-" + e + "-" + i;
    return n
}

function getMonthLastDay(t) {
    return new Date(t.getFullYear(), t.getMonth() + 1, 0)
}

function getMonthFirstDay(t) {
    return new Date(t.getFullYear(), t.getMonth(), 1)
}

function formatShortDate(t, e) {
    this.returnDateFormat = e;
    var i = new Date,
        n = new Date(i.getFullYear(), i.getMonth(), i.getDate()),
        o = t.getMonth() + 1 + "/" + t.getDate() + "/" + t.getFullYear();
    if (!this.returnDateFormat) {
        var a = daydiff(t, n);
        0 >= a ? o = trToday : 1 == a ? o = trYesterday : (o = getMonthName(t.getMonth()) + " " + t.getDate() + "th", t.getFullYear() != i.getFullYear() && (o += ", " + t.getFullYear()))
    }
    return o
}

function dateFromTimestamp(t) {
    return new Date(1e3 * Math.abs(t))
}

function daydiff(t, e) {
    return Math.round((e - t) / 864e5)
}

function setShortcuts(t, e) {
    t.bind("keyup", function (i) {
        13 != i.keyCode || i.shiftKey || (i.altKey ? t.val(t.val() + "\n") : (i.preventDefault(), e.submit()))
    }), t.bind("keypress", function (t) {
        13 != t.keyCode || t.shiftKey || t.preventDefault()
    })
}

function sortContacts() {
    function t(t, e) {
        return $(e).attr("timestamp") > $(t).attr("timestamp") ? 1 : -1;
    }
    $(".js-load-more").size() > 0 ? $(".js-list-contacts li").sort(t).insertBefore($(".js-load-more")) : $(".js-list-contacts li").sort(t).appendTo(".js-list-contacts")
}

function sortStarredMessages() {
    function t(t, e) {
        return $(e).data("timestamp") > $(t).data("timestamp") ? 1 : -1
    }
    $(".js-ticket-starred .wz-starred-message").sort(t).appendTo($(".js-ticket-starred "))
}

function sortTicketsInverse() {
    var t = $(".js-ticket-view");
    for (i = 0; i < t.length; i++) $("#wz-tab-pedidos").prepend($(t[i])), $(t[i]).removeClass("hidden")
}

function createSwitch(t, e) {
    var i = "wz-switch-active",
        n = "wz-switch-inactive";
    if ("string" != typeof t) throw new TypeError("createSwitch(): First arg must be a String.");
    if ("function" != typeof e) throw new TypeError("createSwitch(): Second arg must be a Function.");
    $(t).click(function () {
        $(this).hasClass(n) && ($(t + "." + i).removeClass(i).addClass(n), $(this).removeClass(n).addClass(i)), e(this)
    })
}

function logError(t, e, i) {
    var n = "[" + trError + "]" + t;
    e && (n += " @ " + e), i && (n += ":" + i)
}

function removeClassByRegExp(t, e) {
    var i = t.attr("class");
    if ("string" == typeof i) {
        for (var n = i.split(/\s+/), o = [], a = 0; a < n.length; a++) n[a].match(e) || o.push(n[a]);
        t.attr("class", o.join(" "))
    }
}

function getClassByName(t, e) {
    if (t != null && t != undefined) {
        for (var i = t.attr("class").split(/\s+/), n = 0; n < i.length; n++)
            if (i[n].match(e)) return i[n];
    }
    return ""
}

function createSomeZamiMessages(t, e, i) {
    var n = /\@hibaza\.com$/;
    if (!n.test(email) || !n.test(t)) return messageZamiLastChat = currentChat, !1;
    if (messageZamiLastChat == currentChat) return !1;
    messageZamiTimestamp || (messageZamiTimestamp = Date.now() / 1e3);
    var o = messageZamiTimestamp,
        a = dateFromTimestamp(o),
        r = new Date(formatShortDate(a, !0)),
        s = "js-day-" + formatShortDate(a, !0).replace(/\//g, "-"),
        l = r.getTime() / 1e3;
    if (MESSAGES["zami" + l] = new Message({
        key: function () {
            return "zami" + l
        },
        val: function () {
            return {
                chat: currentChat,
                message: trHeyCustomer + CONTACTS[currentChat].getName() + trHasStarted,
                author: 0,
                date: l,
                day: r,
                dayClass: s,
                timestamp: o,
                agent: 1,
                read: !0,
                type: "zami",
                button_class_1: "hide",
                button_class_2: "hide"
            }
        },
        ref: function () {
            return $("<div>")
        }
    }), MESSAGES["zami" + l].render(), e && $.each(e, function (t, e) {
        if ("" != e && "None" != e) {
            o += .1;
            var i = dateFromTimestamp(o),
                n = new Date(formatShortDate(i, !0)),
                a = "js-day-" + formatShortDate(i, !0).replace(/\//g, "-"),
                r = n.getTime() / 1e3;
            MESSAGES["zami" + r] = new Message({
                key: function () {
                    return "zami" + r
                },
                val: function () {
                    return {
                        chat: currentChat,
                        message: CONTACTS[currentChat].getName() + trHasSet + t + trTo + "*" + e + "*.",
                        author: 0,
                        date: r,
                        day: n,
                        dayClass: a,
                        timestamp: o,
                        agent: 1,
                        read: !0,
                        type: "zami",
                        button_class_1: "hide",
                        button_class_2: "hide"
                    }
                },
                ref: function () {
                    return $("<div>")
                }
            }), MESSAGES["zami" + r].render()
        }
    }), i) {
        o += .1;
        var a = dateFromTimestamp(o),
            r = new Date(formatShortDate(a, !0)),
            s = "js-day-" + formatShortDate(a, !0).replace(/\//g, "-"),
            l = r.getTime() / 1e3;
        MESSAGES["zami" + l] = new Message({
            key: function () {
                return "zami" + l
            },
            val: function () {
                return {
                    chat: currentChat,
                    message: trHey + firstName + "! " + CONTACTS[currentChat].getName() + trDemands + " *" + i.name + "*. " + trIVeCreated,
                    author: 0,
                    date: l,
                    day: r,
                    dayClass: s,
                    timestamp: o,
                    agent: 1,
                    read: !0,
                    type: "zami",
                    button_class_1: "btn-white-pink",
                    button_text_1: trViewTicket,
                    button_click_1: "cloud.getTicketProfile(" + i.id + ", [viewProfileTicket]);",
                    button_class_2: "btn-white-pink",
                    button_text_2: trSendPayment,
                    button_click_2: "cloud.getTicketProfile(" + i.id + ", [viewProfileTicket, cloud.getPaymentForm, showPaymentModal]);"
                }
            },
            ref: function () {
                return $("<div>")
            }
        }), MESSAGES["zami" + l].render()
    }
    scrollMessages(), firebaseService.sortMessages("." + s), messageZamiLastChat = currentChat
}

function zoomImageElem(t) {
    window.open(t.attr("src"))
}

function clearAccents(t) {
    var e = t.replace(/[áàäâ]/g, "a");
    return e = e.replace(/[ÁÀÄÂ]/g, "A"), e = e.replace(/[éèëê]/g, "e"), e = e.replace(/[ÉÈËÊ]/g, "E"), e = e.replace(/[íìïî]/g, "i"), e = e.replace(/[ÍÌÏÎ]/g, "I"), e = e.replace(/[óòöô]/g, "o"), e = e.replace(/[ÓÒÖÔ]/g, "O"), e = e.replace(/[úùüû]/g, "u"), e = e.replace(/[ÚÙÜÛ]/g, "U"), e = e.replace(/ñ/g, "n"), e = e.replace(/Ñ/g, "N")
}

function getInitials(t) {
    var e = "?";
    if ("string" == typeof t && t.length > 0)
        for (var e = "", i = clearAccents(t).toUpperCase(), n = i.replace(/\W/g, " ").replace(/\s+/g, " ").split(" "), o = 0; o < n.length && 2 > o; o++) e += n[o].charAt(0);
    return e
}

function preventDefault(t) {
    t.preventDefault()
}

function preventPaste(t) {
    document.execCommand("insertText", !1, t.originalEvent.clipboardData.getData("text")), t.preventDefault()
}

function preventIntro(t) {
    var e = t.keyCode || t.which;
    13 == e && t.preventDefault()
}

function stopPropagation(t) {
    t.stopPropagation()
}

function showDefaultErrorNotification() {
    new NotificationPlugin(trError, trAnErrorOcurred, "icon-cross")
}

function checkFormChanges() {
    "" == $.trim($(this).val()) ? $(this).addClass("error") : $(this).removeClass("error")
}

function checkFormElements(t) {
    var e = !0;
    return $(t).find("input.form-control, select.form-control").removeClass("error").off("change", checkFormChanges), $(t).find("input.form-control, select.form-control").each(function (t, i) {
        var n = $(i);
        "" == $.trim(n.val()) && (n.addClass("error"), n.on("change", checkFormChanges), e = !1)
    }), e
}

function copyToClipboard(t) {
    var e = t.data.shareValue || $(this).data("share-value"),
        i = t.data.valueType || $(this).data("value-type"),
        n = t.data.shareValueHide || !1,
        o = $("<input>").val(e);
    o.appendTo($("body")), o.select(), document.execCommand("copy"), n ? new NotificationPlugin(i + trCopied, trYour + " " + i + "  " + trHasBeenCopied, "icon-clipboard", null, null, longNotificationTimer) : new NotificationPlugin(i + trCopied, trYour + " " + i + " (" + e + ") " + trHasBeenCopied, "icon-clipboard", null, null, longNotificationTimer), o.remove()
}

function validateCIF(t) {
    if ("string" == typeof t && 9 == t.length) {
        var e = t.toUpperCase(),
            n = "ABCDEFGHJNPQRSUVW";
        if (-1 != n.indexOf(e[0]) && /^\d{2}$/.test(e.slice(1, 3)) && !/^(00)$/.test(e.slice(1, 3)) && /^\d{5}$/.test(e.slice(3, 8))) {
            var o = e.slice(1, 8),
                a = 0,
                r = 0;
            for (i = 0; i < o.length; i++) {
                var s = parseInt(o[i]);
                if (s > 0)
                    if (i % 2 != 0) a += s;
                    else if (i % 2 == 0) {
                        var l = (2 * s).toString();
                        l.length > 1 && (l = parseInt(l[0]) + parseInt(l[1])), r += parseInt(l)
                    }
            }
            var c = (a + r).toString(),
                d = parseInt(c[c.length - 1]),
                u = 0;
            0 != d && (u = 10 - d);
            var h = "JABCDEFGHI",
                f = h.charAt(u);
            return e[8] == f || e[8] == u ? !0 : !1
        }
        return !1
    }
    return !1
}

function addOrUpdateUrlParam(uri, paramKey, paramVal) {
    try {
        var re = new RegExp("([?&])" + paramKey + "=[^&#]*", "i");
        if (re.test(uri)) {
            uri = uri.replace(re, '$1' + paramKey + "=" + paramVal);
        } else {
            var separator = /\?/.test(uri) ? "&" : "?";
            uri = uri + separator + paramKey + "=" + paramVal;
        }
        return uri;
    } catch (e) { console.log(e); return ""; }
}

function changeStatus() {
    onlineStatus = "online" == onlineStatus ? "busy" : "online", cloud.changeStatus(onlineStatus).fail(showDefaultErrorNotification).done(function () {
        refreshAgentStatus(myID, onlineStatus), mixpanelEvents("operatorStatus")
    })
}

function updateStatus(status) {
    //if (status == "offline") {
    //    window.location.href = "/login";
    //}
    //else {
        "online" == status ? ($(".wz-operator-status").removeClass("busy").removeClass("offline"),
            $(".wz-status-text").html(trBusy + '<i class="pull-right icon-ban"></i>')) : "offline" == status ?
                $(".wz-operator-status").addClass("offline") : "busy" == status && ($(".wz-operator-status").addClass("busy"),
                    $(".wz-status-text").html(trActivate + '<i class="pull-right icon-reload"></i>'));
    //}
}


function refreshAgentStatus(t, e) {
    var i = e.toLowerCase();
    (t == myID) && updateStatus(i);
    void 0 === AGENTS[t] && (AGENTS[t] = {}, cloud.reloadAgents()), AGENTS[t].status = i, $("li.js-agent-list[data-agent=" + t + "]").removeClass("js-agent-online"), $("li.js-agent-list[data-agent=" + t + "]").removeClass("js-agent-offline"), $("li.js-agent-list[data-agent=" + t + "]").removeClass("js-agent-busy"), $("li.js-agent-list[data-agent=" + t + "]").addClass("js-agent-" + i), loadSearchAgent("#client-status"), loadSearchAgent(".wz-col-central footer .assignation")
}

function autoSizeTextarea() {
    txt.addClass("txtstuff"), hiddenDiv.addClass("hiddendiv common"), hiddenDiv.css("width", txt.width()), $("body").append(hiddenDiv), txt.on("keyup", function () {
        content = $(this).val(), content = content.replace(/\n/g, "<br>"), hiddenDiv.html(content + '<br class="lbr">'), $(this).css("height", hiddenDiv.height() + 20)
    })
}

function emojiPicker() {
    $emojiBtn.emojiPicker({
        height: "300px",
        width: "450px",
        inputId: "js-typearea"
    })
}

function showGhostChat(t, e) {
    var i = txt.val();
    if (i.length > 0) {
        var n = formatShortTime(new Date),
            o = $(".js-loadtemplate-ghost").tmpl({
                avatar: t,
                name: e,
                date: n,
                message: i
            });
        return $(o).find(".js-resend-message").on("click", function () {
            $(this).parent().parent().parent().remove(), setTimeout(function () {
                var t = $(this).parent().parent().find(".js-chat-message-text").text();
                txt.val($.trim(t)), sendMessage(sendForm)
            }.bind(this), 500)
        }), $(o).find(".js-remove-message").on("click", function () {
            $(this).parent().parent().parent().remove()
        }), chatList.append(o), scrollMessages(), txt.css("height", "42px"), o
    }
    return !1
}

function hideGhostChat() {
    $(".js-chat-ghost").remove()
}

//function initFirebase() {
//    var t = new $.Deferred;
//    console.log(firebaseToken);
//    return FIREBASE_APP = firebase.initializeApp(firebaseConfig, "hibaza"), FIREBASE_APP.auth().signInWithCustomToken(firebaseToken)["catch"](function (t) {
//        var e = t.code;
//        t.message;
//        "auth/invalid-custom-token" === e ? console.log("The token you provided is not valid.") : console.log(t)
//    }).then(function (e) {
//        if (e) {
//            console.log("User is signed in."), firebaseService = new FirebaseService(FIREBASE_APP.database().ref(businessID));
//            var n = function () {
//                var e = contactList.find("li").first().data("key");
//                e && (firebaseService.setChat(e)), cloud.getLastTicket(e), refreshChannels(), refreshAgents(), firebaseService.registerUpdates(), t.resolve();
//            },
//                i = cloud.loadContacts(0, limitContacts, chatView, chatViewAgent, chatViewChannel);
//            $.when(i).done(n)
//        }
//    }), cloud = new CloudService, cloud.setMediaUrl(mediaURL), showTicketList.on("click", function () {
//        cloud.getTicketList()
//    }), t.promise()
//}

function initFirebase() {
    var t = new $.Deferred;
    FIREBASE_APP = firebase.initializeApp(firebaseConfig, "hibaza");
    firebaseService = new FirebaseService(FIREBASE_APP.database().ref(businessID));
    cloud = new CloudService, cloud.setMediaUrl(mediaURL);
    var n = function () {
        var e = contactList.find("li").first().data("key");
        e && (firebaseService.setChat(e)), cloud.getLastTicket(e), refreshChannels(), refreshAgents(), t.resolve();
    },
        i = cloud.loadContacts(0, limitContacts, chatView, chatViewAgent, chatViewChannel);
    return $.when(i).done(n), showTicketList.on("click", function () {
        cloud.getTicketList()
    }), firebaseService.registerUpdates(), t.promise();

}

function initReports() {
    reportManager = new ReportManager, dashboardManager = new DashboardManager, Chart.defaults.global.defaultFontColor = "#FFFFFF", Chart.defaults.global.defaultFontFamily = '"Gotham Rounded", sans-serif, Helvetica, Arial, sans-serif', Chart.defaults.global.animation.duration = 800, Chart.defaults.global.legend.labels.boxWidth = 12, Chart.defaults.global.legend.labels.fontStyle = "bold", moment.locale(LOCALE_LANGUAGE);
    $.get(cloud.getURLs().brands.customers_top + businessID, function (t) {
        reportManager.topTenCustomers = t
    }), $.get(cloud.getURLs().brands.agents_top + businessID, function (t) {
        reportManager.topTenAgents = t;
        var e = [];
        t.lines && $.each(t.lines, function (i) {
            e.push(t.lines[i].id)
        }), dashboardManager.topTenAgents = e
    })
}

function setDateValues(t) {
    $("#wz-modal-reports .js-filter-date-init").val(t.init), $("#wz-modal-reports .js-filter-date-selector-init").val(t.initText), $("#wz-modal-reports .js-filter-date-finish").val(t.finish), $("#wz-modal-reports .js-filter-date-selector-finish").val(t.finishText), $("#wz-modal-reports .js-filter-date-text").text(t.initText + " - " + t.finishText), $("#wz-modal-reports .dropdown-date .dropdown-toggle").removeClass("pick")
}

function getDateRange(t) {
    var e, i, n = t;
    switch (t) {
        case "today":
            e = moment().startOf("day"), i = moment();
            break;
        case "c_week":
            e = moment().startOf("week"), i = moment();
            break;
        case "c_month":
            e = moment().startOf("month"), i = moment();
            break;
        case "l_month":
            e = moment().subtract(1, "months").startOf("month"), i = moment().subtract(1, "months").endOf("month");
            break;
        case "c_year":
            e = moment().startOf("year"), i = moment();
            break;
        default:
            n = "today", e = moment().startOf("day"), i = moment()
    }
    return {
        id: n,
        init: e.valueOf(),
        initText: e.format(DATE_FORMAT),
        finish: i.valueOf(),
        finishText: i.format(DATE_FORMAT)
    }
}

function introJsAssignate() {
    var t = ".js-list-contacts .wz-flex-card.active",
        e = "bottom-left-aligned";
    0 == $(t).size() && (t = "#client-status .dropdown", e = "bottom-right-aligned"), intro && intro.exit(), intro = introJs(), intro.setOptions({
        disableInteraction: !1,
        exitOnOverlayClick: !0,
        steps: [{
            element: t,
            intro: trYouNeed,
            position: e
        }]
    }), $("#wz-modal-upload-image.fade.in").length > 0 ? ($("#wz-modal-upload-image").modal("hide"), $("#wz-modal-upload-image").on("hidden.bs.modal", function () {
        intro.start()
    })) : intro.start()
}

function introJsNewInterface() {
    intro && intro.exit(), intro = introJs(), intro.onexit(function () {
        $("#intercom-container").show()
    });
    var t = trHereIs;
    "admin" == role && (t += "<br><br><u>" + trProTip + "</u>: " + trIfYouWant), intro.setOptions({
        disableInteraction: !1,
        exitOnOverlayClick: !0,
        steps: [{
            step: 1,
            element: ".wz-col-sideleft-1.wz-col-height",
            intro: t,
            position: "right"
        }, {
            step: 2,
            element: ".wz-topbar .wz-col-central",
            intro: trThisIs,
            position: "bottom-right-aligned"
        }, {
            step: 3,
            element: ".wz-col-sideleft-2 header",
            intro: trYouCanFilter,
            position: "right"
        }]
    }), $("#intercom-container").hide(), intro.start(), $(".introjs-tooltip").css("top", "120px")
}

function introJsNewTextarea() {
    intro && intro.exit(), intro = introJs(), intro.onbeforechange(function () {
        shorcutBtnContainer.addClass("introjs-fixParent"), checkNote.hasClass("js-checked") && checkNote.trigger("click")
    }), intro.onafterchange(function () {
        shorcutBtnContainer.addClass("introjs-fixParent")
    }), intro.onexit(function () {
        shorcutBtnContainer.removeClass("introjs-fixParent"), $("#intercom-container").show()
    });
    var t = trFastResponses;
    1 == shorcutBtnContainer.find(".btn").length && (t += "<br><br>" + trSetYourFirst), intro.setOptions({
        disableInteraction: !1,
        exitOnOverlayClick: !0,
        steps: [{
            step: 1,
            element: "#send_form",
            intro: trThisIsOur,
            position: "top"
        }, {
            step: 2,
            element: ".shortcuts",
            intro: t,
            position: "top"
        }, {
            step: 3,
            element: "#js-button-checknote",
            intro: trIfYouCheck,
            position: "top"
        }]
    }),
        $("#intercom-container").hide(), shorcutBtnContainer.addClass("introjs-fixParent"), setTimeout(function () {
        intro.start()
    }, 200)
}

function introJsNewTickets() {
    intro && intro.exit(), intro = introJs(), intro.onexit(function () {
        $("#intercom-container").show()
    }), intro.onafterchange(function () {
        // $(".js-client-view[data-view=tickets]").trigger("click")
    }), intro.onbeforechange(function () {
        // $(".js-client-view[data-view=tickets]").trigger("click")
    }), intro.setOptions({
        disableInteraction: !1,
        exitOnOverlayClick: !0,
        steps: [{
            step: 1,
            element: ".js-last-ticket",
            intro: trEachTime,
            position: "bottom"
        }, {
            step: 2,
            element: "#wz-col-sideright .wz-sideright-client .wz-central",
            intro: trYouCanView,
            position: "left"
        }, {
            step: 3,
            element: '.js-ticket-view[data-ticket="' + currentTicket + '"]',
            intro: trClickOnTicket,
            position: "left"
        }]
    }), $("#intercom-container").hide(), intro.start()
}

function introJsNewTicketView() {
    intro && intro.exit(), intro = introJs(), intro.onexit(function () {
        $("#intercom-container").show()
    }), intro.setOptions({
        disableInteraction: !1,
        exitOnOverlayClick: !0,
        steps: [{
            step: 1,
            element: ".wz-sideright-ticket .wz-ticket-header",
            intro: trYouCanEdit,
            position: "left"
        }, {
            step: 2,
            element: ".wz-sideright-ticket .wz-ticket-body",
            intro: trYouCanAlsoView,
            position: "left"
        }]
    }), $("#intercom-container").hide(), intro.start()
}

function linkify(t) {
    return t.html(function () {
        return $(this).find(".js-chat-message-email").each(function (t) {
            $(this).html($(this).html().replace(/(\S+)(\s+)(\&lt;)((http|https|ftp):\/\/[\w?=&\/-;#~%-]+(\.[\w?=&\/-;#~%-]+)*(?![\w\s?&.\/;#~%"=-]*>))(\&gt;)/gm, '<a href="$4" class="wz-link" target="_blank">$1</a>').replace(/(\n){3,}/g, "\n\n"))
        }), $(this).find(".js-chat-message-text").each(function (t) {
            $(this).html(linkifyString($(this).html()))
        }), $(this).html()
    })
}

function linkifyString(t) {
    return t ? t.replace(/^(.*[\s\w])?((http|https|ftp):\/\/[\w?=&\/-;,#~%-]+(\.[\w?=&\/-;,#~%-]+)*(?![\w\s?&.\/;,#~%"=-]*>))/gm, '$1<a href="$2" class="wz-link" target="_blank">$2</a>') : ""
}

function linkifyElem(t) {
    t.html(linkifyString(t.html()))
}

function strongify(t) {
    return t.html(function () {
        return $(this).find(".js-chat-zami-text").each(function (t) {
            $(this).html($(this).html().replace(/(\*)([\S ]+)(\*)/gm, "<strong>$2</strong>"))
        }), $(this).html()
    })
}

function loadStylesheet(t) {
    var e = new $.Deferred,
        i = document.createElement("style");
    i.textContent = '@import "' + t + '"';
    var n = setInterval(function () {
        try {
            i.sheet.cssRules, e.resolve(), clearInterval(n)
        } catch (t) { }
    }, 250);
    return document.head.appendChild(i), e.promise()
}

function initMixpanel(t) {
    //analytics.identify("agent_" + t.ID, {
    //    name: t.firstName + " " + t.lastName,
    //    email: t.email,
    //    created: t.created,
    //    company: t.company,
    //    country: t.country,
    //    role: t.role,
    //    customersCount: t.customersCount,
    //    trial: t.trial,
    //    pricingVolume: t.pricingVolume,
    //    expired: t.expired
    //}, {
    //    integrations: {
    //        Intercom: {
    //            user_hash: intercom_hash
    //        }
    //    }
    //}), analytics.group(t.company, {
    //    name: t.company
    //}), analytics.track("Start session")
}

function mixpanelEvents(t, e) {
    //"sessionDuration" === t && analytics.track("Session duration", {
    //    Duration: e
    //}), "firstSession" === t && analytics.track("First Session"), "demonError" === t && analytics.track("Demon Error"), "sendRegister" === t && analytics.track("Submit Register Form"), "onboardingPopup" === t && analytics.track("Onboarding Popup"), "messageSent" === t && analytics.track("Message Sent", {
    //    Channel: e
    //}), "privateNoteSent" === t && analytics.track("Private Note Sent", {
    //    Channel: e
    //}), "privateNoteClick" === t && analytics.track("Clicks Private Note"), "messageReceived" === t && analytics.track("Message Received", {
    //    Channel: e
    //}), "firstMessageReceived" === t && analytics.track("First Message Received"), "starredMessage" === t && analytics.track("Starred Message"), "unstarredMessage" === t && analytics.track("Unstarred Message"), "operatorStatus" === t && analytics.track("Change operator status"), "searchMessages" === t && analytics.track("Search messages"), "openSettings" === t && analytics.track("Opens settings"), "openAutomate" === t && analytics.track("Opens Automate"), "openShortcuts" === t && analytics.track("Opens Shortcuts"), "clickShortcut" === t && analytics.track("Clicks Shortcut"), "addShortcut" === t && analytics.track("Adds Shortcut"), "openNotifications" === t && analytics.track("Opens Notifications"), "addAgentClick" === t && analytics.track("Clicks Add Agent"), "addChannelClick" === t && analytics.track("Clicks Add Channel"), "archiveChat" === t && analytics.track("Clicks Archive Chat"), "unarchiveChat" === t && analytics.track("Clicks Unarchive Chat"), "activateTelegramChannel" === t && analytics.track("Activates Telegram Channel"), "editTelegramChannel" === t && analytics.track("Edits Telegram Channel", {
    //    Phone: e
    //}), "addTelegramChannel" === t && analytics.track("Adds Telegram Channel", {
    //    Phone: e
    //}), "editSmoochChannel" === t && analytics.track("Edits Smooch Channel"), "addSmoochChannel" === t && analytics.track("Adds Smooch Channel"), "editFacebookChannel" === t && analytics.track("Edits Facebook Channel", {
    //    Page: e
    //}), "addFacebookChannel" === t && analytics.track("Adds Facebook Channel", {
    //    Page: e
    //}), "addWhatsappChannel" === t && analytics.track("Added Whatsapp Channel", {
    //    Phone: e.phone,
    //    Company: e.company
    //}), "editWhatsappChannel" === t && analytics.track("Edits Whatsapp Channel", {
    //    Phone: e
    //}), "otherChannels" === t && analytics.track("Other Channels"), "addFirstChannel" === t && analytics.track("Adds First Channel", {
    //    Channel: e
    //}), "addAgent" === t && analytics.track("Adds Agent"), "editAgent" === t && analytics.track("Edits Agent"), "changePermissions" === t && analytics.track("Changes Agent Permissions", {
    //    Agent: "agent_" + e
    //}), "createTicketManual" === t && analytics.track("Create ticket Manually"), "createTicketAuto" === t && analytics.track("Create ticket Automatically"), "editTicket" === t && analytics.track("Edit ticket"), "viewTicketsList" === t && analytics.track("View Tickets List"), "viewTicket" === t && analytics.track("View Ticket"), "changeTicketShortDescription" === t && analytics.track("Change ticket short description"), "changeTicketDescription" === t && analytics.track("Change ticket description"), "changeTicketTags" === t && analytics.track("Change ticket tags", {
    //    Tags: e
    //}), "changeTicketStatusTitle" === t && analytics.track("Change ticket status from title", {
    //    Status: e
    //}), "changeTicketStatusArchive" === t && analytics.track("Change ticket status from archiving", {
    //    Status: e
    //}), "changeTicketStatusAuto" === t && analytics.track("Change ticket status automatically", {
    //    Status: e
    //}), "viewClientNotes" === t && analytics.track("View Client Notes"), "addClientNote" === t && analytics.track("Add Client Note"), "viewTicketNotes" === t && analytics.track("View Ticket Notes"), "addTicketNote" === t && analytics.track("Add Ticket Note"), "createPayment" === t && analytics.track("Create payment"), "receivePayment" === t && analytics.track("Receive payment"), "createPaymentGateway" === t && analytics.track("Adds Payment Gateway", {
    //    Type: e.type
    //}), "removePaymentGateway" === t && analytics.track("Removes Payment Gateway", {
    //    Type: e.type
    //}), "initRonin" === t && analytics.track("Init Ronin"), "roninChurn" === t && analytics.track("Ronin client churn"), "addRegisterBot" === t && analytics.track("Creates Register Bot"), "editRegisterBot" === t && analytics.track("Edits Register Bot"), "addAutoresponseBot" === t && analytics.track("Creates Autoresponse Bot"), "editAutoresponseBot" === t && analytics.track("Edits Autoresponse Bot"), "disableAutoresponseBot" === t && analytics.track("Turns off Autoresponse Bot"), "disableRegisterBot" === t && analytics.track("Turns off  Register Bot"), "deleteAutoresponseBot" === t && analytics.track("Deletes Autoresponse Bot"), "deleteRegisterBot" === t && analytics.track("Deletes Register Bot"), "endSubscription" === t && analytics.track("End of subscription"), "paySubscription" === t && analytics.track("Pays Subscription"), "openSubscription" === t && analytics.track("Opens Subscription"), "openSubscriptionForm" === t && analytics.track("Open Subscription Form"), "openSubscriptionForm" === t && analytics.track("Open Subscription Form"), "openReports" === t && analytics.track("Reports 2.0 Open"), "openReportsDashboard" === t && analytics.track("Reports 2.0 > Dashboard"), "openReportsCustomers" === t && analytics.track("Reports 2.0 > Customers"), "openReportsTickets" === t && analytics.track("Reports 2.0 > Tickets"), "openReportsAgents" === t && analytics.track("Reports 2.0 > Agents"), "clickReportsDateChange" === t && analytics.track("Reports 2.0 - Date Change click"), "applyReportsDateChange" === t && analytics.track("Reports 2.0 - Date Change apply")
}

function modalCRMTable() {
    var t = $("#wz-modal-tickets");
    t.on("shown.bs.modal", function () {
        $(".modal-backdrop").addClass("js-modal-tickets")
    })
}

function modalClientsTable() {
    var t = ($("#wz-table-clients"), $("#wz-modal-tickets"));
    $("#wz-acc-h-01,#wz-acc-h-02");
    t.on("shown.bs.modal", function () {
        $(".modal-backdrop").addClass("js-modal-tickets")
    })
}

function modalDismiss() {
    $(".modal").on("shown.bs.modal", function () {
        $(document).off("focusin.modal")
    });
    var t = $(".js-tab-chats");
    return t.click(function () {
        $(".modal").modal("hide")
    }), $(".modal").on("show.bs.modal", function () {
        var t;
        t = this, 1 != $(this).data("modal-stack") && $(".modal").each(function () {
            this !== t && $(this).modal("hide")
        })
    })
}

function modalTicketsTable() {
    var t = $("#js-open-modal-tickets"),
        e = $("#wz-modal-tickets");
    t.click(function () {
        e.modal("show")
    });
    ticketSelect.on("click", function (t) {
        t.preventDefault(), $(this).hasClass("active") ? ($(this).removeClass("active"), $(this).parent().toggleClass("active")) : ($(this).parent().parent().find(".active").removeClass("active"), $(this).toggleClass("active"), $(this).parent().toggleClass("active"));
        var e = $(this).data("filter"),
            i = $(this).data("option");
        switch (e) {
            case "assigned":
                ticketAssignedView == i ? ticketAssignedView = "all" : ticketAssignedView = i;
                break;
            case "responsability":
                ticketResponsabilityView == i ? ticketResponsabilityView = "all" : ticketResponsabilityView = i;
                break;
            case "status":
                ticketStatusView == i ? ticketStatusView = "all" : ticketStatusView = i
        }
    })
}

function modalPricing() {
    changeBilledPeriod($(".js-pricing-switch.wz-switch-active").get()), pricingModal.css("display", "block"), createSwitch(".js-pricing-switch", changeBilledPeriod), pricingSelectPlan.click(function () {
        pricingPeriodSelected = $(".js-pricing-switch.wz-switch-active").data("period"), cloud.openSettings("#suscription-settings", cloud.loadSettingsSuscriptionForm).done(function () {
            pricingModal.modal("hide"), settingsModal.modal("show")
        })
    }), pricingModal.modal("show")
}

function modalQRCode(t) {
    var e, i;
    CHANNELS[t.channel_id] ? (e = CHANNELS[t.channel_id].type, i = CHANNELS[t.channel_id].name + "(" + t.channel_phone + ")") : (e = "whatsapp", i = t.channel_phone), QRCodeModal.modal("show"), QRCodeModal.css("display", "block");
    var n = /icon-/g;
    removeClassByRegExp(QRCodeChannelIcon, n), QRCodeChannelIcon.addClass("icon-" + e), QRCodeChannelName.html(i), QRCodeContainer.empty(), QRCodeContainer.qrcode(t.code), QRCodeModalClose.click(function () {
        QRCodeSuccess(), QRModalPreloader.modal("show")
    })
}

function QRCodeSuccess() {
    QRModalPreloading.addClass("hide"), QRModalSuccess.removeClass("hide")
}

function QRCodeReset() {
    QRModalPreloading.removeClass("hide"), QRModalSuccess.addClass("hide")
}

function modalNotification(t) {
    t && ("string" == typeof t.icon ? (removeClassByRegExp($notificationModalIcon, /^icon-/), $notificationModalIcon.addClass(t.icon)) : $notificationModalSection.hide(), "string" == typeof t.title ? $notificationModalTitle.text(t.title) : ($notificationModalTitle.hide(), $notificationModalSeparator.hide()), "string" == typeof t.message ? $notificationModalMessage.text(t.message) : $notificationModalMessage.hide(), "boolean" == typeof t.close && (t.close || $notificationModalCloseIcon.hide()), $notificationModalButtons.empty(), "object" == typeof t.buttons && $.each(t.buttons, function (e) {
        var i = $("<i class='icon-arrow-right'>"),
            n = $("<strong>").text(t.buttons[e].text),
            o = $("<span>").append(n).append(i),
            a = $("<a class='btn btn-block'>").addClass(t.buttons[e]["class"]).click(t.buttons[e].callback).append(o);
        $notificationModalButtons.append(a)
    }), $notificationModal.modal("show"))
}

function renderAllNotificationPlugin() {
    for (var t = 12, e = t, i = 0; i < notificationArray.length; i++) {
        if (i > 0) {
            var n = parseFloat(notificationArray[i - 1].elem.outerHeight(!0));
            e += n + t
        }
        notificationArray[i].elem.css("top", e + "px")
    }
}

function deleteNotificationPlugin(t) {
    for (var e = 0; e < notificationArray.length; e++)
        if (notificationArray[e].id === t.id) {
            notificationArray.splice(e, 1);
            break
        }
    renderAllNotificationPlugin()
}

function notifications() {
    iconBell.on("click", function () {
        $(this).removeClass("js-alarm-active")
    })
}

function notifyMe(t, e) {
    if ("Notification" in window)
        if ("granted" === Notification.permission) {
            var i = {
                body: e,
                icon: cloud.getMediaUrl() + t.avatar,
                dir: "ltr"
            },
                n = new Notification(t.name, i);
            n.onclick = function () {
                firebaseService.setMainChat(t.id), window.focus()
            }, setTimeout(n.close.bind(n), 2e3)
        } else "denied" !== Notification.permission && Notification.requestPermission(function (i) {
            if ("permission" in Notification || (Notification.permission = i), "granted" === i) {
                var n = {
                    body: e,
                    icon: cloud.getMediaUrl() + t.avatar,
                    dir: "ltr"
                },
                    o = new Notification(t.name, n);
                o.onclick = function () {
                    firebaseService.setMainChat(t.id), window.focus()
                }, setTimeout(o.close.bind(o), 2e3)
            }
        });
    else console.log("This browser does not support desktop notification")
}

function loadOldContacts() {
    contactListContainer.stop(), clearTimeout(timeoutUiAutoLoadContacts), timeoutUiAutoLoadContacts = !1, contactListContainer.bind("scroll", uiAutoLoadContacts)
}

function getContactListHeight() {
    var t = contactListContainer.find(".js-filler-contact").outerHeight() + contactListContainer.find(".js-list-contacts").outerHeight();
    return "none" != olderContactsLoader.css("display") && (t += olderContactsLoader.outerHeight()), t
}

function getContactTop() {
    var t = contactListContainer.scrollTop() + contactListContainer.first().outerHeight();
    return t >= getContactListHeight() - contactListContainer.height() && (t = getContactListHeight() - contactListContainer.height() - 5), t
}

function uiAutoLoadContacts() {
    var t = $(".js-list-contacts li").size(),
        e = $(".js-list-contacts li.js-search").size(),
        i = contactListContainer.scrollBottom() >= lastContactScroll;
    lastContactScroll = contactListContainer.scrollBottom();
    var n = contactListContainer.scrollBottom() >= getContactListHeight(),
        o = cloud.getMediaUrl() + "avatars/bot.png";
    if ($(".js-see-older-contacts .wz-contact-avatar").css("background-image", "url(" + o + ")"), n && i) {
        NProgress.inc(), olderContactsLoader.fadeIn(800), contactListContainer.animate({
            scrollTop: getContactTop()
        }, 800, "swing", function () {
            $sideleft2Scroll.perfectScrollbar("update")
        });
        var a = 808;
        timeoutUiAutoLoadContacts = setTimeout(function () {
            var last = $(".js-list-contacts li").last();
            var timestamp = last && last.length > 0 ? last.attr("timestamp") : 0;
            t >= limitContacts || true ? (lc = cloud.loadContacts(timestamp, limitContacts, chatView, chatViewAgent, chatViewChannel), lc && lc.done(function () {
                $sideleft2Scroll.perfectScrollbar("update"), olderContactsLoader.fadeOut(500, "swing", function () {
                    timeoutUiAutoLoadContacts = !1, NProgress.done(), clearTimeout(timeoutUiAutoLoadContacts)
                }, function () {
                    $sideleft2Scroll.perfectScrollbar("update")
                })
            })) : olderContactsLoader.fadeOut(500, "swing", function () {
                timeoutUiAutoLoadContacts = !1, NProgress.done(), clearTimeout(timeoutUiAutoLoadContacts)
            }, function () {
                $sideleft2Scroll.perfectScrollbar("update")
            })
        }, a)
    }
}

function loadOldMessages() {
    messageListContainer.stop(), olderMessagesLoader.removeClass("js-loader-open"), clearTimeout(timeoutUiAutoScrollMessages), clearTimeout(timeoutUiAutoLoadMessages), clearInterval(intervalUiAutoScrollMessages), timeoutUiAutoScrollMessages = !1, timeoutUiAutoLoadMessages = !1, intervalUiAutoScrollMessages = !1, messageListContainer.bind("scroll", uiAutoLoadMessages)
}

function scrollToFirstMessage() {
    intervalUiAutoScrollMessages || (intervalUiAutoScrollMessages = setInterval(function () {
        if (chatList.children().size() > howManyMessages) {
            chatList.find("li:hidden").fadeIn(1500);
            var t = getMessageTop(lastMessageToScroll);
            messageListContainer.scrollTop(t), $centralScroll.perfectScrollbar("update"), t > 400 ? t -= 400 : t = 10, timeoutUiAutoScrollMessages = setTimeout(function () {
                messageListContainer.stop().animate({
                    scrollTop: t
                }, "500", "swing", function () { })
            }, 200), clearInterval(intervalUiAutoScrollMessages), intervalUiAutoScrollMessages = !1
        }
    }, 100))
}

function getMessageTop(t) {
    var e = t.offset().top;
    return e -= t.hasClass("js-chat-groupped") ? 55 : 70, t.prev() && t.prev().hasClass("js-chat-date-subhead") && (e -= t.prev().outerHeight()), 10 > e && (e = 10), e
}

function isScrolledIntoView(t) {
    var e = messageListContainer.scrollTop(),
        i = e < lastMessageScroll,
        n = 0 == e;
    return lastMessageScroll = e, i && n
}

function uiAutoLoadMessages() {
    if (howManyMessages = $(".js-list-chat li.js-chat-message").size(), lastMessageToScroll = $(".js-list-chat li.js-chat-message").first(), howManyMessages > 0 && howManyMessages >= limitMessages && isScrolledIntoView(olderMessagesLoader)) {
        NProgress.inc(), olderMessagesLoader.addClass("js-loader-open");
        var t = 800;
        timeoutUiAutoLoadMessages = setTimeout(function () {
            if (howManyMessages >= limitMessages) {
                var t = firebaseService.getOlder();
                messageListContainer.find(".stick").removeClass("stick"), $.when(t).done(function () {
                    olderMessagesLoader.removeClass("js-loader-open"), scrollToFirstMessage(), timeoutUiAutoLoadMessages = !1, NProgress.done()
                })
            } else olderMessagesLoader.removeClass("js-loader-open"), scrollToFirstMessage(), timeoutUiAutoLoadMessages = !1, NProgress.done()
        }, t)
    }
}

function loadSearchContact() {
    inputFinder2.autocomplete("option", {
        source: cloud.searchContacts
    })
}

function responsiveUI() {
    var t;
    $(".js-open-wz-column").click(function (e) {
        e.preventDefault(), t = $(this).attr("href"), $(t).addClass("js-open-col")
            , $(".js-close-wz-column").click(function (e) {
                e.preventDefault(), t = $(this).attr("href"), $(t).removeClass("js-open-col")
            })
    });
}

function roninDrag() {
    initSortableCards(), initSortableToolbar()
}

function initSortableToolbar() {
    Sortable.create(roninToolBar, {
        group: {
            name: "roninToolBar",
            pull: "clone"
        },
        sort: !1,
        animation: 100
    })
}

function initSortableCards() {
    roninCards = $("#roninCards ul"), numberUlCards = 1, roninCards.each(function () {
        arrayCards.push($(this).attr("id")), numberUlCards += 1
    }), arrayCards.push("roninToolBar"), roninCards.each(function (t, e) {
        z = t, id = $(this).attr("id"), idString = id.toString(), idSharp = "#" + idString, sortab = !0, disable = !1, $(this).hasClass("bool-li") ? (sortab = !1, disabl = !0) : (sortab = !0, disabl = !1), Sortable.create(e, {
            group: {
                name: idString,
                put: arrayCards
            },
            sort: sortab,
            disabled: disabl,
            draggable: ".item",
            filter: ".ignore",
            animation: 100,
            onMove: function (t) {
                groupposition2 = t.dragged.dataset.groupPosition, colnumber2 = t.related.dataset.colNumber, groupdepth2 = t.related.dataset.groupDepth, t.dragged.dataset.groupPosition = groupposition2, t.dragged.dataset.colNumber = colnumber2, t.dragged.dataset.groupDepth = groupdepth2
            },
            onEnd: function (t) {
                var e = $("#" + t.to.id);
                e.children("li").each(function (t) {
                    $(this).attr("data-item-position", t.toString())
                })
            },
            onAdd: function (t) {
                par = t, par2 = z, onAddFunction(par, par2)
            }
        })
    })
}

function onAddFunction(t, e) {
    var i = $("#roninCards ul").length;
    i += 2, theItem = t.item, theItem.className = "", theItem.classList.add("sort-" + t.newIndex + "-" + e);
    if (gDepth = t.to.dataset.groupdepth, $("ul[data-groupdepth='" + gDepth + "']").find("li").each(function (t) {
        $(this).attr("data-group-depth", gDepth), $(this).attr("data-group-position", $(this).parent().attr("data-position"))
    }), theItem.dataset.itemPosition = t.newIndex, counter = -1, $("li[data-group-depth='" + gDepth + "']").each(function (t) {
        "0" === $(this).attr("data-item-position") && counter++ , $(this).attr("data-col-number", counter)
    }), icon = $(".sort-" + t.newIndex + "-" + e).children("i").attr("class"), dataType = t.item.attributes[0].nodeValue, ulId = $("#" + t.to.id), "conditional" == dataType) {
        if (groupDepth = ulId.attr("data-groupdepth"), groupDepth = parseInt(groupDepth), groupDepth += 1, groupDepth > 4) return new NotificationPlugin(trAddAnother, trIsNotPossible, "icon-lock"), void t.to.removeChild(t.item);
        theItem.classList.add("ignore"), $(ulId).removeClass("dropzone"), number01 = numberUlCards + 1, number02 = numberUlCards + 2;
        var n = parseInt(ulId.attr("data-position"));
        ulId.after('<ul id="n' + t.to.id + '" class="bool bool-li" data-position="' + (n + 1) + '" data-groupdepth="' + (groupDepth - 1) + '"></ul>'), ulId.next().append(t.item), ulId.next().after('<div class="row row-bool"><span class="bool-border"></span><div class="col-xs-6"><ul id="cards' + number01 + '" class="bool bool-yes dropzone" data-position="' + (n + 2) + '" data-groupdepth="' + groupDepth + '"></ul></div><div class="col-xs-6"><ul id="cards' + number02 + '" class="bool bool-no dropzone" data-position="' + (n + 2) + '" data-groupdepth="' + groupDepth + '"></ul></div></div>'), initSortableCards()
    } else if ("options" == dataType) {
        groupDepth = ulId.attr("data-groupdepth"), groupDepth = parseInt(groupDepth), groupDepth += 1, theItem.classList.add("ignore"), $(ulId).removeClass("dropzone"),
            number01 = numberUlCards + 1, number02 = numberUlCards + 2;
        var n = parseInt(ulId.attr("data-position"));
        ulId.after('<ul id="n' + t.to.id + '" class="bool bool-li" data-position="' + (n + 1) + '" data-groupdepth="' + (groupDepth - 1) + '"></ul>'), ulId.next().append(t.item);
        var o = '<li data-type="option-answered" draggable="false" data-group-depth="' + groupDepth + '" data-group-position="' + (n + 2) + '" data-item-position="0" data-col-number="1" data-question="Un café con leche" data-error="" data-check="false" data-selectfield="Otros" class="user-answer"><i class="icon-head"></i><i class="icon-check"></i><span class="text">Un café con leche</span><div class="btn btn-info r-edit" onclick="editCard(this);"><span class="text">' + trEdit + '</span><i class="icon-pencil"></i></div></li>';
        ulId.next().after('<div class="row row-bool"><div class="col-xs-12"><ul id="cards' + number01 + '" class="bool ul-options dropzone" data-position="' + (n + 2) + '" data-groupdepth="' + groupDepth + '">' + o + '</ul></div><div class="col-xs-12"><ul id="cards' + number02 + '" class="bool ul-options dropzone" data-position="' + (n + 2) + '" data-groupdepth="' + groupDepth + '">' + o + "</ul></div></div>"), initSortableCards()
    } else theItem.classList.add("item");
    classItem = ".sort-" + t.newIndex + "-" + e, theItem.dataset.question || createModal(classItem, dataType, icon)
}

function createModal(t, e, i, n, o, a, r) {
    function s() {
        $(".card-check").prop("checked") ? $(".js-check").show() : $(".js-check").hide()
    }
    label = '<label for="id_short_description" class="control-label col-sm-3">' + trQuestion + "</label>", input = '<div class="col-sm-9"><input class="form-control card-question" type="text"></div>', label2 = '<label for="id_short_description" class="control-label col-sm-3">' + trErrorText + "</label>", input2 = '<div class="col-sm-9"><input class="form-control card-error" type="text"></div>';
    var l = "";
    "__none__" === e && (l = "hidden"), inputCheck = '<label class="control-label col-sm-3 ' + l + '"><input class="card-check" name="featured" type="checkbox"> ' + trDoYouWant + "</label>", select = '<label class="control-label col-sm-3 js-check ' + l + '">' + trChooseTheUser + '</label><div class="col-sm-9"><select class="form-control select-field js-check"><option>name</option><option>email</option><option>phone</option><option>birthdate</option><option>postal_code</option><option>address</option><option>city</option><option>country</option></select></div>', formGOpen = '<div class="form-group">', formGClose = "</div>", formOpen = '<form id="edit-question-form" role="form" class="form-horizontal">', formClose = "</form>", content = formOpen + formGOpen + label + input + formGClose, "__none__" !== e && (content += formGOpen + label2 + input2 + formGClose), content += formGOpen + inputCheck + formGClose + formGOpen + select + formGClose + formClose, backgroundDisabled = $('<div class="wz-bg-disabled-ronin"></div>'), customModal = $('<div class="modal fade wz-modal-sideleft ' + e + '" id="myModal" role="dialog" aria-labelledby="myModalLabel" data-keyboard="false"><div class="modal-dialog" role="document"><div class="modal-content"><div class="modal-header"><h4 class="modal-title" id="myModalLabel">' + trEditQuestion + '</h4></div><div class="modal-body">' + content + '</div><div class="modal-footer"><button type="button" class="btn btn-default btn-close" data-dismiss="modal">' + trRemoveItem + '</button><button type="button" class="btn btn-primary save-button">' + trSaveChanges + "</button></div></div></div></div>"), $("body").append(backgroundDisabled), $("body").append(customModal), customModal.on("hidden.bs.modal", function () {
        $(".wz-bg-disabled-ronin").remove(), $("#myModal").remove()
    }), n && $(".card-question").val(n), o && $(".card-error").val(o), "true" === a ? $(".card-check").prop("checked", !0) : $(".card-check").prop("checked", !1), r && $(".select-field").val(r), $("#edit-question-form").submit(function (t) {
        t.preventDefault()
    }), $("#myModal").modal(), $("#myModal").modal("show"), targetDiv = $(t), s(), $(".card-check").click(function () {
        s()
    }), $(".save-button").click(function (t) {
        cQuestion = $(".card-question").val(), cError = $(".card-error").val(), cCheck = $(".card-check").prop("checked") ? "true" : "false", sField = $(".select-field").val(), cQuestion.length && (targetDiv.html('<i class="' + i + '"></i><span class="text">' + cQuestion + '</span><div class="btn btn-info r-edit" onclick="editCard(this);"><span class="text">' + trEdit + '</span><i class="icon-pencil"></i></div>'), targetDiv.attr("data-question", cQuestion).attr("data-error", cError).attr("data-check", cCheck).attr("data-selectField", sField), $("#myModal").modal("hide"), $("#myModal").remove(), $(".wz-bg-disabled-ronin").remove())
    }), $(".btn-close").click(function () {
        parentDiv = targetDiv.parent(), targetDiv.hasClass("ignore") && (parentDiv.next().remove(), parentDiv.prev().addClass("dropzone"), parentDiv.remove()), targetDiv.remove(), $("#roninCards .label").remove(), 0 === parentDiv.children().length && parentDiv.addClass("dropzone"), $("#myModal").remove(), $(".wz-bg-disabled-ronin").remove()
    })
}

function editCard(t) {
    element = t, rClass = $(element).parent().attr("class"), rClass = rClass.replace("item", "").replace(" ", "").replace("ignore", ""), rData = $(element).parent().attr("data-type"), rQuestion = $(element).parent().attr("data-question"), rError = $(element).parent().attr("data-error"), rIcon = $(element).parent().children("i").attr("class"), rField = $(element).parent().attr("data-selectField"), rCheck = $(element).parent().attr("data-check"), createModal("." + rClass, rData, rIcon, rQuestion, rError, rCheck, rField)
}

function saveJson() {
    var t = $(".js-nameRonin").val(),
        e = $("#wz-modal-ronin").attr("data-mode"),
        i = [];
    $(".js-select-channels.checked").each(function () {
        i.push($(this).attr("data-channel"))
    });
    var n, o = [],
        a = [],
        r = [];
    modeId = $(this).attr("data-id"), a = {
        id: modeId,
        setup: {
            name: t,
            mode: e,
            channel: i
        },
        group: {
            type: "regular",
            cards: []
        }
    };
    var s, l, c, d, u;
    $("#roninCards > .row > .col-xs-12 > ul").each(function (t, e) {
        $(e).children("li").each(function (t, e) {
            s = $(e).attr("data-question"), l = $(e).attr("data-error"), c = $(e).attr("data-check"), d = $(e).attr("data-selectField"), u = $(e).attr("data-type");
            var i;
            i = "true" == c ? {
                type: u,
                question: s,
                error: l,
                check: c,
                selectField: d,
                group: " "
            } : {
                    type: u,
                    question: s,
                    error: l,
                    group: " "
                }, a.group.cards[t] = i
        }), $(e).next(".row-bool").children(".col-xs-6").children("ul").length > 0 && $(e).next(".row-bool").children(".col-xs-6").children("ul").each(function (t) {
            n = {
                type: "boolhijo"
            }, r.push(n)
        })
    }), o.push(a);
    var h = cloud.saveRoninMode(JSON.stringify(o));
    $.when(h).done(function () {
        roninModal.modal("hide");
        var t = o[0].id ? "edit" : "add";
        "register" == o[0].setup.mode ? mixpanelEvents(t + "RegisterBot") : "on busy" == o[0].setup.mode && mixpanelEvents(t + "AutoresponseBot")
    })
}

function roninSetup(t) {
    switch (t) {
        case "register":
            $("#wz-modal-ronin").modal("show")
    }
}

function roninLoad() { }

function saveRoninJson() {
    var t = {},
        e = cloud.saveRoninMode(JSON.stringify(t));
    $.when(e).done(function () {
        roninModal.modal("hide")
    })
}

function enableRonin(t) {
    switch (t) {
        case "register":
            $(".wz-ronin-0 .wz-ronin-switch").buttonswitch("setStatus", 1)
    }
}

function disableRonin(t) {
    switch (t) {
        case "register":
            $(".wz-ronin-0 .wz-ronin-switch").buttonswitch("setStatus", 0)
    }
}

function setBotsContactStep(t, e) {
    "1" == t ? (botDescription2A.val(""), botDescription2B.val(""), $("#wz-modal-bots-contact .js-next-step").addClass("disabled")) : "3" == t && $("#wz-modal-bots-contact .js-prev-step").data("step", e), $('#wz-modal-bots-contact [class^="js-bots-contact-"]').addClass("hide"), $("#wz-modal-bots-contact .js-bots-contact-" + t).removeClass("hide")
}

function moveRoninModalToStep(t) {
    $("#wz-modal-ronin .wz-step").removeClass("js-active").removeClass("js-prev").removeClass("js-next");
    var e = $(t);
    e.addClass("js-active"), e.next().length > 0 && e.next().addClass("js-next"), e.prev().length > 0 && e.prev().addClass("js-prev")
}

function roninEdit(t) {
    $.each(t.channels, function (t, e) {
        var i = $(".js-select-channels").filter(function () {
            return $(this).data("channel") == e
        });
        i.addClass("checked")
    }), jsonEdited = t, moveRoninModalToStep("#wz-modal-ronin .wz-step-sortable"), roninHtmlEdit(jsonEdited), $(".js-nameRonin").val(t.name), $(".save-json").attr("data-id", jsonEdited.id), roninDrag();
    var e = $(".js-select-mode").filter(function () {
        var e = "on busy" == t.mode ? "autorresponse" : "register";
        return $(this).data("mode") == e
    }),
        i = e.attr("data-mode");
    $("#wz-modal-ronin").attr("data-mode", i), $("#wz-modal-ronin").modal("show")
}

function roninAdd(t) {
    var e = {
        0: {
            setup: {
                name: "",
                mode: t,
                channel: ""
            },
            group: {
                type: "regular",
                cards: {}
            }
        }
    };
    g = 1, $(".js-select-channels.checked").removeClass("checked"), moveRoninModalToStep("#wz-modal-ronin .wz-step:first-child"), $(".js-nameRonin").val(""), roninHtml(e), $(".save-json").removeAttr("data-id"), roninDrag(), $("#wz-modal-ronin").modal("show")
}

function returnUlLi(t) {
    idUl = "cards" + g, idLi = "sort-" + g, ulOpen = '<ul class="' + ulClasses + '" id="' + idUl + '">', ulClose = "</ul>", li = '<li class="' + liClasses + " " + idLi + '" data-item-position="' + g + '" data-question="' + dataQuestion + '" data-error="' + dataError + '" data-check="' + dataCheck + '" data-selectfield="' + dataSelectField + '" data-type="' + t + '"><i class="icon-align-left"></i><span class="text">' + dataQuestion + '</span><div class="btn btn-info r-edit" onclick="editCard(this);"><span class="text">Edit</span><i class="icon-pencil"></i></div></li>'
}

function eachNested(t, e) {
    liClasses = "item";
    for (var i in t) t[i].group !== !1 ? (dataQuestion = t[i].question, dataError = t[i].error, dataCheck = t[i].check, dataSelectField = t[i].selectField, dataType = t[i].type, ulClasses = "dropzone", hasGroup = !0, returnUlLi(), htmlRonin += ulClose, ulClasses = "bool bool-li", liClasses = "ignore", g++ , returnUlLi(), htmlRonin += ulOpen + li + ulClose + columnConditionalOpen, g++ , ulClasses = "bool bool-yes", Object.keys(t).length > itemCounter && (ulClasses = "bool bool-yes dropzone"), returnUlLi(), htmlRonin += colxs6Open + ulOpen, eachNested(t[i].group.yes.cards, !0), ulClasses = "bool bool-no", Object.keys(t).length > itemCounter && (ulClasses = "bool bool-no dropzone"), itemCounter++ , returnUlLi(), htmlRonin += ulClose + colxs6Close + colxs6Open + ulOpen, eachNested(t[i].group.no.cards, !0), htmlRonin += ulClose + colxs6Close + columnConditionalClose) : (dataQuestion = t[i].question, dataError = t[i].error, dataCheck = t[i].check, dataSelectField = t[i].selectField, dataType = t[i].type, 1 == itemCounter || e === !0 ? (liClasses = "item", g++ , returnUlLi(), htmlRonin += li) : (ulClasses = " ", Object.keys(t).length > itemCounter && (ulClasses = "dropzone next-dropzone"), returnUlLi(), htmlRonin += ulOpen, liClasses = "item", g++ , returnUlLi(), htmlRonin += li + ulClose))
}

function roninHtml(t) {
    $("#wz-modal-ronin").attr("data-mode", t[0].setup.mode), $(".js-select-channels").each(function () {
        channelInJson = $(this).attr("data-channel"), $.inArray(channelInJson, t[0].setup.channel) > -1 && $(this).trigger("click")
    }), ulClasses = "dropzone", Object.keys(t[0].group.cards).length > 1 && (ulClasses = " "), returnUlLi(), htmlRonin = firstColumnOpen + ulOpen, hasGroup = !1, eachNested(t[0].group.cards, !1), htmlRonin += hasGroup === !1 ? ulClose + firstColumnClose : firstColumnClose, $("#roninCards").html(htmlRonin)
}

function roninHtmlEdit(t) {
    modeId = t.id, $("#wz-modal-ronin").attr("data-mode", t.mode), $(".js-select-channels").each(function () {
        channelInJson = $(this).attr("data-channel"), $.inArray(channelInJson, t.channel) > -1 && $(this).trigger("click")
    }), ulClasses = "dropzone", Object.keys(t.status).length > 1 && (ulClasses = " "), returnUlLi(), htmlRonin = firstColumnOpen + ulOpen;
    var e = !1;
    eachNestedEdit(t.status, !1), htmlRonin += e === !1 ? ulClose + firstColumnClose : firstColumnClose, $("#roninCards").html(htmlRonin)
}

function eachNestedEdit(t, e) {
    liClasses = "item";
    for (var i in t) dataQuestion = t[i].questions[0].text, dataError = t[i].errors[0] ? t[i].errors[0].text : "", dataSelectField = t[i].selectField, dataType = t[i].type, dataCheck = null == dataSelectField ? !1 : !0, 1 == itemCounter || e === !0 ? (liClasses = "item", g++ , returnUlLi(dataType), htmlRonin += li) : (ulClasses = " ", Object.keys(t).length > itemCounter && (ulClasses = "dropzone next-dropzone"), returnUlLi(dataType), htmlRonin += ulOpen, liClasses = "item", g++ , returnUlLi(dataType), htmlRonin += li + ulClose)
}

function initRoninUI() {
    roninUI()
}

function roninUI() {
    var t, e, i, n, o;
    selectChannels(), selectLanguage(), selectFields(), tooltips("#wz-modal-ronin"), $(".js-select-mode").click(function () {
        modeAttr = $(this).attr("data-mode"), $("#wz-modal-ronin").attr("data-mode", modeAttr)
    }), $(".wz-step").each(function () {
        $(this).find("form").submit(function (t) {
            t.preventDefault()
        }), $(this).find(".nav-btn").children(".next").click(function () {
            var o = $(this).data("validate");
            if (o) {
                $(o).removeClass("error");
                var a = $.trim($(o).val());
                if (0 == a.length) return setTimeout(function () {
                    $(o).addClass("error"), $(o).focus()
                }, 50), !1;
                $(o).val(a)
            }
            t = $(".js-active"), i = $(".js-prev"), e = $(".js-next"), n = e.next(), e.length > 0 && (t.removeClass("js-active").addClass("js-prev"), i.removeClass("js-prev"), e.removeClass("js-next").addClass("js-active"), n.addClass("js-next")), n.length < 1 && console.log("end"), i.length < 1 && (t.removeClass("js-active").addClass("js-prev"), e.removeClass("js-next").addClass("js-active"), n.addClass("js-next"))
        }), $(this).children(".nav-btn").children(".prev").click(function () {
            t = $(".js-active"), i = $(".js-prev"), e = $(".js-next"), o = i.prev(), i.length > 0 && (t.removeClass("js-active").addClass("js-next"), i.removeClass("js-prev").addClass("js-active"), e.removeClass("js-next"), o.addClass("js-prev")), o.length < 1 && console.log("init"), e.length < 1 && (t.removeClass("js-active").addClass("js-next"), i.removeClass("js-prev").addClass("js-active"), o.addClass("js-prev"))
        })
    })
}

function roninDelete(t, e) {
    var i = cloud.roninDelete(t);
    $.when(i).done(function () {
        cloud.loadRoninModal(), 0 == e ? mixpanelEvents("deleteAutoresponseBot") : 1 == e && mixpanelEvents("deleteRegisterBot")
    })
}

function roninDisable(t, e) {
    var i = cloud.roninDisable(t);
    $.when(i).done(function () {
        cloud.loadRoninModal(), 0 == e ? mixpanelEvents("disableAutoresponseBot") : 1 == e && mixpanelEvents("disableRegisterBot")
    })
}

function selectChannels() {
    $(".js-select-channels").click(function (t) {
        t.preventDefault(), $(this).toggleClass("checked")
    })
}

function selectLanguage() {
    $(".js-select-languages").click(function (t) {
        t.preventDefault(), $(this).parent().find(".js-select-languages").removeClass("checked"), $(this).toggleClass("checked")
    })
}

function selectFields() {
    $(".js-select-fields").click(function (t) {
        t.preventDefault(), "others" !== $(this).data("field") && $(this).toggleClass("checked")
    })
}
function sendTemplate(t) {
    inputAudio.val(""), inputImage.val(""), inputModalMessageTag.val(t);
    for (var e = 0; 6 > e; e++) imgSendUrls[e] = "";
    loadImageToDropableZone("facebook", "image-text", null);
    $(".js-tag-chat").val("SHIPPING_UPDATE");
    $("#js-gallery-error-upload").addClass("hide"), $(".js-submit-image").prop("disabled", !1), modalImg.modal("show");

}
function sendMessages() {
    buttonSendimageA.on("click", function (t) {
        t.preventDefault()
    }), buttonSendimage.on("click", function () {
        defaultCombobox();
        inputAudio.val(""), inputImage.val("");//, inputModalMessageTag.val("sale")
        for (var e = 0; 6 > e; e++) imgSendUrls[e] = "";
        loadImageToDropableZone("facebook", "image-text", null);
        $("#js-gallery-error-upload").addClass("hide"), $(".js-submit-image").prop("disabled", !1), modalImg.modal("show");
    }), buttonSendaudio.on("click", function () {
        inputAudio.click()
    }), submitImg.click(function () {
        sendImageForm()
    }), modalFormImageUpload.on("submit", function (t) {
        t.preventDefault(), sendImageForm()
    }), checkNote.on("click", function (t) {
        $(this).hasClass("js-checked") ? (txt.attr("placeholder", trWriteNote), $("#send_form").addClass("js-note"), mixpanelEvents("privateNoteClick")) : (txt.attr("placeholder", trTypeMessage), $("#send_form").removeClass("js-note")), txt.focus()
    }), sendForm.on("submit", function (t) {

        chat_Template(),
            t.preventDefault(), firstMessageToScroll = !1;

        sendMessage(this);
        successSendAi();
        //var e = $.trim(txt.val()),
        //    i = e.length ;
        //return 0 == i ? !1 : (checkNote.hasClass("js-checked") ? sendInnerNote(this) : sendMessage(this), void txt.val(""))
    })
}

function sendMessage(t) {
    chat_Template();
    var channel_type = $(".js-channel-type-chat").val();

    if ("object" == typeof CONTACTS[currentChat])
        if (CONTACTS[currentChat].getAgent()) {
            if (CONTACTS[currentChat].getAgent() != myID) return void introJsAssignate()
        } else CONTACTS[currentChat].assignMe();
    var e = showGhostChat(AGENTS[myID].avatar || "", AGENTS[myID].name.capitalizeFirstLetter());

    cloud.sendForm(t, channel_type == "facebook" ? cloud.getURLs().messages.send : cloud.getURLs().zalos.send, function (t) {
        if (!t.ok) t.list && t.list.customer && t.list.customer.forEach(function (t) {
            new NotificationPlugin("error", t, "icon-lock", null, null, 2e3)
        });
        else {
            THREADS[CONTACTS[currentChat].getKey()].setReplied(true);
            mixpanelEvents("messageSent", CONTACTS[currentChat].getChannelType().toLowerCase());
            var e = $(".js-status-select.selected");
            if (e.size() > 0) {
                var i = e.data("status");
                "0" == i && cloud.setStatus(1).done(function () {
                    mixpanelEvents("changeTicketStatusAuto", 1)
                })
            }
        }
    }).fail(function () {
        e && e.find(".js-chat-ghost-action").fadeIn()
    }), inputAudio.val(""), inputImage.val("")
}

function sendInnerNote(t) {
    txt.val().length > 0 && (cloud.sendForm(t, cloud.getURLs().customers.inner_note, function (t) {
        t.ok ? mixpanelEvents("privateNoteSent", CONTACTS[currentChat].getChannelType().toLowerCase()) : new NotificationPlugin(trError, trErrorSendingNote, "icon-lock", null, null, 2e3)
    }), checkNote.click())
}

function settingsUI(t) {
    var e = "#wz-modal-clients " + (t || "");
    $(e + " .js-show-edit").click(function () {
        var t = $(this).attr("data-target"),
            e = $(this).attr("data-targethide");
        $(t).removeClass("hide"), $(e).addClass("hide")
    }), $(e + " .dropdown").on("hide.bs.dropdown", function () {
        "none" != $(this).find(".wz-sub-dropdown").css("display") && ($(this).parent().find(".wz-sub-dropdown").hide(), $(this).find(".js-delete span:first-child").removeClass("hide"), $(this).find(".js-delete span:last-child").addClass("hide"))
    }), $(e + " .js-delete").on("click", function (t) {
        t.stopPropagation(), $(this).parent().find(".wz-sub-dropdown").fadeToggle(), $(this).find("span").toggleClass("hide")
    }), $(e + " .js-delete-cancel").on("click", function (t) {
        t.stopPropagation(), $(this).parent().parent().parent().find(".js-delete").click()
    }), $(e + " .js-custom-checkbox").checkbox(),
        $(e + " .js-custom-file").each(function (t) {
            var e = $(this).find('input[type="text"]'),
                i = $(this).find('input[type="file"]'),
                n = $(this).find(".btn.js-upload"),
                o = $(this).find(".btn.js-browse"),
                a = function (t) {
                    i.click(), t.stopPropagation()
                },
                r = function (t) {
                    var i = $(this)[0].files[0];
                    i ? (n.removeClass("hide"), e.val(i.name)) : (n.addClass("hide"), e.val(""))
                };
            e.size() > 0 && i.size() > 0 && (o.unbind("click"), e.unbind("click"), i.unbind("change"), o.bind("click", a), e.bind("click", a), i.bind("change", r), e.prop("readonly", !0).prop("placeholder", trNoFileSelected))
        }), $(e + " .js-hide-edit").click(function () {
            var t = $(this).attr("data-target");
            $(t).addClass("hide"), $(targethide).addClass("hide")
        }), settingsUIRedirect && ($("#wz-modal-clients .wz-crm-menu li a[href=" + settingsUIRedirect + "]").click(), settingsUIRedirect = !1)
}

function loadShortcuts(t) {

    shortcuts(t), setShortcuts(txt, sendForm), loadShortcutsButtons(t), printTable(t);

}

function printTable(t) {
    $("#shortcuts-table").tabulator({
        columns: [{
            title: trShortcut,
            field: "shortcut",
            sortable: !0
        }, {
            title: trText,
            field: "name",
            sortable: !0
        }],
        rowClick: function (t, e, i, n) {
            cloud.getShortcutForm(e + "/")
        },
        fitColumns: !0,
        showLoader: !1,
        sortable: !0,
        renderComplete: function () {
            $("#shortcuts-table").fadeIn(100);
        },
        height: "calc(100vh - 195px)",
        data: t
    })
    $("#shortcuts-table").tabulator("setData", t); $("#shortcuts-table").tabulator("redraw");
}

function filter(t, e) {
    e = $.trim(e), e = e.replace(/ /gi, "|"), $(t).each(function () {
        $(this).text().search(new RegExp(e, "i")) < 0 ? $(this).hide().removeClass("visible") : $(this).show().addClass("visible")
    })
}

function tooltips(t) {
    var e = t || "";
    $(e + ' [data-toggle="tooltip"]').tooltip()
}

function newVersionTutorial() {
    "undefined" != typeof Storage ? 1 != localStorage.hibazaNewVersionTutorial && (localStorage.hibazaNewVersionTutorial = 1, modalNotification({
        icon: "icon-new-interface",
        title: trWelcome,
        message: trWeHaveAdded,
        buttons: [{
            "class": "btn-pink",
            text: trShowMeHow,
            callback: function () {
                $notificationModal.modal("hide"), introJsNewInterface()
            }
        }]
    })) : console.log(" No Web Storage support.")
}

function newTextareaTutorial() {
    "undefined" != typeof Storage ? 1 != localStorage.hibazaNewVersionTextarea && (localStorage.hibazaNewVersionTextarea = 1, introJsNewTextarea()) : console.log(" No Web Storage support.")
}

function newTicketTutorial(t) {
    if ("undefined" != typeof Storage) {
        if ("false" != localStorage.hibazaNewVersionTickets && 1 == localStorage.hibazaNewVersionTutorial && !$notificationModal.hasClass("in")) {
            if (localStorage.hibazaNewVersionTickets) {
                var e = localStorage.hibazaNewVersionTickets.split(","); - 1 == e.indexOf(t) && (localStorage.hibazaNewVersionTickets += "," + t)
            } else localStorage.hibazaNewVersionTickets = t;
            e = localStorage.hibazaNewVersionTickets.split(","), e.length == newTicketTutorialGoal && (localStorage.hibazaNewVersionTickets = "false", modalNotification({
                icon: "icon-new-interface",
                message: trTicketsSimple,
                buttons: [{
                    "class": "btn-pink",
                    text: trLearn,
                    callback: function () {
                        $notificationModal.modal("hide"), introJsNewTickets()
                    }
                }]
            }))
        }
    } else console.log(" No Web Storage support.")
}

function newTicketViewTutorial() {
    "undefined" != typeof Storage ? "false" == localStorage.hibazaNewVersionTickets && 1 != localStorage.hibazaNewVersionTicketsView && "ticket" == viewProfile && (localStorage.hibazaNewVersionTicketsView = 1, introJsNewTicketView()) : console.log(" No Web Storage support.")
}

function addHow() {
    $(".wz-welcome-new .js-added .added .btn-pink").click(function () {
        $(".wz-welcome-new .js-added").addClass("js-how")
    }), $(".wz-how .btn-pink").click(function () {
        $(".wz-welcome-new .js-added").removeClass("js-how")
    })
}

function showLink(t) {
    var e = t.data.shareValue,
        i = (t.data.valueType, $(this).data("channel-type"));
    $("#wz-modal-welcome .js-share-" + i).addClass("hide"), $("#wz-modal-welcome .js-share-" + i + "-copied span").text(e), $("#wz-modal-welcome .js-share-" + i + "-copied").removeClass("hide").trigger("click")
}

function disableTutorial() {
    console.log("disable"), tutorialActive = !1, welcomeModal.modal("hide")
}

function toggleBotScreens() {
    var t, e = $("#wz-modal-bot-edit-block,#wz-modal-bot-edit-tree");
    $(".js-s-block-tree a").click(function (i) {
        i.preventDefault(), $(".js-s-block-tree a").toggleClass("wz-switch-active"), $(".js-s-block-tree a").toggleClass("wz-switch-inactive"), t = $(this).attr("href"), e.hide(), $(t).show()
    })
}

function addOptions() {
    var t = '<div class="wz-edit-footer"><div class="btn btn-pink btn-goto">Go to...</div><div class="wz-icons"><a href=""><i class="icon-align-left"></i></a><a href=""><i class="icon-image"></i></a><a href=""></a><div class="dropdown"><a href="#" data-toggle="dropdown"><span>ADD VARIABLE {}</span></a><ul class="dropdown-menu dropdown-variables"><li><a href="#" class="js-add-variable" data-variable="customer.firstName"><span class="wz-font-s-md wz-font-w-med">First Name<i class="pull-right icon-arrow-right"></i></span></a></li><li><a href="#" class="js-add-variable" data-variable="customer.firstName"><span class="wz-font-s-md wz-font-w-med">Last Name<i class="pull-right icon-arrow-right"></i></span></a></li></ul></div></div></div>';
    $(".js-add-option").click(function () {
        $(this).parent().before('<li class="js-options"><p id="focused" contenteditable="true" placeholder="Write your text"></p><i class="icon-trash pull-right"></i>' + t + "</li>"), document.getElementById("focused").focus(), $("#focused").removeAttr("id"), removeOptions(), gotoOptions(), insertVariables()
    }), removeOptions(), gotoOptions(), insertVariables()
}

function removeOptions() {
    $(".wz-message-list i").click(function () {
        $(this).parent().remove()
    })
}

function gotoOptions() {
    $(".btn-goto").click(function () {
        goToBlock("options", this)
    }), $(".dropdown-goto li a").click(function () {
        var t = $(this).html();
        $(this).parent().parent().prev().html(t);
        var e = $(this).children("span").html(),
            i = e.slice(7, -61);
        $(this).parent().parent().parent().parent().prev().prev().attr("data-next", i)
    })
}

function goToBlock(t, e) {
    if ($(e).parent(".dropdown-goto").length < 1) {
        $(e).wrap("<div class='dropdown dropdown-goto'></div>"), $(e).attr("data-toggle", "dropdown"), $(e).after('<ul class="dropdown-menu"></ul>');
        var i = "";
        $(".wz-bot-block").each(function (n, o) {
            "options" != t && (i = 'onclick="renderBlock7(' + $(o).children(".wz-block-number").html() + ')"'), $(e).next().append('<li><a href="#" ' + i + '><span class="wz-font-s-md.wz-font-w-med">' + $(o).children(".wz-block-text").html() + '<i class="pull-right icon-arrow-right></i>"</span></a></li>')
        }), $(e).dropdown(), $(".dropdown-goto").dropdown("toggle")
    }
    gotoOptions()
}

function replaceTemplateTag(t) {
    var e = $(t).text().replace(/\[\[/g, "{{").replace(/\]\]/g, "}}");
    $(t).text(e)
}

function editMessageText(t, e) {
    "back" == e ? $(t).children().children(".wz-message-text.blocked").click(function () {
        $(this).parent().parent().nextAll("li").remove(), $(this).parent().parent().remove(), renderBlock3()
    }) : $(".wz-message-text.blocked").click(function () {
        var e = $(this).children("p").html(),
            i = {
                id: "1",
                text: e,
                callback: "renderBlock2",
                side: "left"
            };
        replaceTemplateTag(".js-block-message-edit"), content = blockMessageEdit.tmpl(i), $(this).parent().parent().replaceWith(content), document.getElementById("focused1").focus(), $(t).children().children(".wz-message-text.blocked")
    })
}

function renderMessageModel(t, e, i, n) {
    var o;
    i && $(i).hide(), "edit" == t && (replaceTemplateTag(".js-block-message-edit"), o = blockMessageEdit.tmpl(e), $(".wz-edit-block-messages").append(o), document.getElementById("focused1").focus()), "text" == t && (replaceTemplateTag(".js-block-message-text"), o = blockMessageText.tmpl(e), $(".wz-edit-block-messages").append(o)), "question" == t && (replaceTemplateTag(".js-block-message-question"), o = blockMessageQuestion.tmpl(e), $(".wz-edit-block-messages").append(o)), "list" == t && (replaceTemplateTag(".js-block-message-list"), o = blockMessageList.tmpl(e), $(".wz-edit-block-messages").append(o), addOptions()), "next" == t && (replaceTemplateTag(".js-block-message-next"), o = blockMessageNext.tmpl(e), $(".wz-edit-block-messages").append(o)), n && n()
}

function insertVariables() {
    $(".wz-message-edit .js-add-variable").click(function () {
        var t = $(this).attr("data-variable");
        t = '{<span class="wz-font-c-pink">' + t + "</span>} ", $(this).parent().parent().parent().parent().parent().prev().append(t)
    }), $(".wz-message-list .js-add-variable").click(function () {
        var t = $(this).attr("data-variable");
        t = '{<span class="wz-font-c-pink">' + t + "</span>} ", $(this).parent().parent().parent().parent().parent().prev().prev().append(t)
    })
}
function newBlock() {
    $(".wz-edit-block-messages").html(" ");
    renderBlock1(), $(".wz-bot-block.active").removeClass("active");
    var t = $(".wz-bot-block:last");
    blockNumber = parseInt(t.children("i").html()) + 1, $("#wz-modal-bot-edit-block h3").html("Block #" + blockNumber), t.after('<li class="wz-bot-block active" id="bot-block-' + blockNumber + '"><i class="wz-block-number">' + blockNumber + '</i><span class="wz-block-text">Block #' + blockNumber + '</span><span class="wz-block-actions"><i class="icon-trash"></i><i class="icon-move"></i></span></li>')
}

function createObject() {
    newObject = {}, $("#bot-block-" + blockNumber).attr("data-object", "blockObj" + blockNumber), currentRoninStatus = roninMode.addStatus(blockNumber)
}

function renderBlock1() {
    var t = {
        id: "1",
        text: "",
        callback: "renderBlock2",
        side: "left"
    };
    renderMessageModel("edit", t), setTimeout(createObject, 2e3), setTimeout(insertVariables, 2e3)
}

function renderBlock2(t) {
    var e = {
        id: "2",
        text: t,
        side: "left",
        blocked: "true"
    };
    renderMessageModel("text", e, "#message-1", renderBlock3), editMessageText(), newObject.questionType = "text", newObject.question = t, currentRoninStatus.addResponse(t), console.log(newObject)
}

function renderBlock3() {
    var t = {
        id: "3",
        question: "Choose a type of response for your message.",
        buttons: {
            "Give information": "renderBlock4",
            "Choose an option": "renderBlock8",
            Nothing: "renderBlock6"
        },
        side: "right"
    };
    renderMessageModel("question", t)
}

function renderBlock4() {
    var t = {
        id: "4",
        question: "Which type of data do you want in the answer?",
        buttons: {
            Name: "renderBlock5",
            Phone: "renderBlock5",
            Email: "renderBlock5",
            Zip: "renderBlock5",
            Address: "renderBlock5",
            Country: "renderBlock5",
            Birthdate: "renderBlock5",
            Other: "renderBlock5"
        },
        side: "right"
    };
    renderMessageModel("question", t, "#message-3"), newObject.answerType = "give information"
}

function renderBlock5(t) {
    var e = {
        id: "5",
        text: 'My <span class="wz-font-u-lowercase">' + t + '</span> is <span class="wz-font-c-pink wz-font-u-lowercase">{{' + t + "}}</span>...",
        side: "right",
        blocked: "true"
    };
    currentRoninStatus.setField(t), renderMessageModel("text", e, "#message-4", renderBlock6), editMessageText("#message-5", "back"), newObject.dataAnswered = t
}

function renderBlock6(t) {
    var e = {
        id: "6",
        question: "Choose what happens next.",
        buttons: {
            "Go to next block": "renderBlock7",
            "Go to certain block": "goToBlock",
            "Stop conversation": "renderBlock7"
        },
        side: "left"
    };
    "Nothing" == t ? (renderMessageModel("question", e, "#message-3"), currentRoninStatus.setProcesser("__none__"), newObject.answerType = "nothing", console.log(newObject)) : renderMessageModel("question", e)
}

function renderBlock7(t) {
    var e = blockNumber + 1;
    t && "Go to next block" != t && (e = t), "Stop conversation" == t && (e = 0);
    var i = {
        id: "7",
        text: e
    };
    renderMessageModel("next", i, "#message-6"), "Stop conversation" == t && $("#message-7 p.tmpl-message").html('<i class="icon-lock" style="margin-right:10px;"></i>Stop conversation.'), newObject.nextAction = "goToBlock", newObject.nextBlock = e, 0 != e && roninMode.addRelation(currentRoninStatus.id, e);
    "blockObj" + blockNumber;
    window["blockObj" + blockNumber] = newObject
}

function renderBlock8() {
    var t = {
        id: "8",
        options: {},
        side: "right",
        callback: "renderBlock9"
    };
    renderMessageModel("list", t, "#message-3"), $(".js-add-option").trigger("click"), currentRoninStatus.setProcesser("__choice__"), newObject.answerType = "choose an option", console.log(newObject)
}

function renderBlock9() {
    var t = {
        id: "9",
        text: "0"
    };
    renderMessageModel("next", t), $(".wz-message-next .tmpl-message").remove(), $(".js-advanced").addClass("top"), $("#message-8 .wz-message-list").addClass("blocked"), $(".js-options").each(function (t, e) {
        var i = "option" + t,
            n = $(e).children("p").html(),
            o = $(e).children("p").attr("data-next");
        optionsObject[i] = {}, optionsObject[i].id = t, optionsObject[i].type = "text", optionsObject[i].option = n, optionsObject[i].nextAction = "goToBlock", optionsObject[i].nextBlock = o
    }), newObject.options = optionsObject;
    "blockObj" + blockNumber;
    window["blockObj" + blockNumber] = newObject, console.log(newObject)
}

function clickLoadBlock() {
    $(".wz-bot-block").click(function () {
        var t = $(this).attr("id");
        t = "#" + t, loadBlock(t)
    })
}

function loadBlock(t) {
    if ($(t).length < 1) newBlock(), clickLoadBlock();
    else if ($(".wz-bot-block.active").removeClass("active"), $(t).addClass("active"), blockNumber = parseInt($(t).children("i").html()), $("#wz-modal-bot-edit-block h3").html("Block #" + blockNumber), $(".wz-edit-block-messages").html(" "), window[$(t).attr("data-object")]) {
        var e = window[$(t).attr("data-object")];
        loadBlocks(e)
    } else renderBlock1()
}

function str_replace_all(str, str_find, str_replace) {
    try {
        return str.replace(new RegExp(str_find, "gi"), str_replace);
    } catch (ex) { return str; }
}

function str_replace_all01(str, str_find, str_replace) {
    try {
        var sp = str.split('' + str_find + '');
        for (var i = 0; i < sp.length; i++) {
            str = str.replace(str_find, str_replace);
        }
        return str;
    } catch (ex) { return str; }
}

function stringDateToTimestamp(strDate) {
    try {
        //strDate ='02/13/2009 23:31:30'
        if (strDate == null || strDate == undefined || strDate == "")
            return strDate;
        strDate = str_replace_all(strDate, "/", "-");
        var datum = Date.parse(strDate);
        return datum / 1000;
    } catch (e) { return strDate; }
}

function removeSpecial(strObj) {
    try {
        if (strObj == null || strObj == undefined || strObj == "")
            return strObj;
        return strObj.replace(/[^a-zA-Z0-9_-]/g, '');
    } catch (e) { return strObj }
}

function replaceDotAndVND(obj) {
    try {
        if (obj == null || obj == undefined)
            return obj;

        obj = obj.replace('.', '').replace('.', '').replace('.', '').replace('đ', '');
        obj = parseInt(obj);
        return obj;
    } catch (e) { return obj; }
}

function getShortDate() {
    var i = new Date;
    var date = i.getDay() + "/" + (i.getMonth() + 1) + "/" + i.getFullYear().toString().substring(2, 2) + " 00:00";
    return date;
}

function getNoteAgent(e, thiss) {
    try {
        var lst = localStorage.getItem("SuggestionsNoteAgent");
        
        if (lst == null) {
            var obj = ExecuteChatBotSyns(null, baseUrls_Api + "brands/shortcuts/get-by-agent/" + businessID + "/" + myID + "?" + accessToken, "GET",
                "", true);
            
            if (obj != null && obj != undefined) {

                var array = [];
                $.each(obj.data, function (key, val) {
                    console.log(val);
                    array.push({ value: val.shortcut, data:val.name });
                });
                lst = JSON.stringify(array);
                localStorage.setItem("SuggestionsNoteAgent", lst);                
            }
        }
        if (lst == null || lst == "" || lst == "[]")
            return true;

        
        $(thiss).autocomplete({
            source: JSON.parse(lst),
            select: function (event, ui) {
                try {
                    if ($("#wz-tab-datos").attr("gender") == "female") {
                        setTimeout(function () {
                            $(".js-typearea").val(str_replace_all01(ui.item.data, "((gender))", "chị"));
                        }, 100);
                    }
                    if ($("#wz-tab-datos").attr("gender") == "male") {
                        setTimeout(function () {
                            $(".js-typearea").val(str_replace_all01(ui.item.data, "((gender))", "anh"));
                        }, 100);
                    }
                    if ($("#wz-tab-datos").attr("gender") == "") {
                        setTimeout(function () {
                            $(".js-typearea").val(str_replace_all01(ui.item.data, "((gender))", "anh/chị"));
                        },100);
                    }
                } catch (e) { console.log(e); }
            }
        });

    return true;
} catch (e) { console.log(e); }
}

function getDateNow() {
    //return '02/13/2009 23:31:30'
    var t = new Date();
    return t.getMonth() + 1 + "/" + t.getDate() + "/" + t.getFullYear() + " " + format0Value(t.getHours()) + ":" + format0Value(t.getMinutes()) + ":" + format0Value(t.getSeconds());
}

function formartTime(str) {
    //25/10/17 11:58 =>25/10/2017 11:58:00
    try {
        var start = str.substring(0, 6);
        var end = str.substring(6, str.length);
        var t = start + "20" + end + ":00"
        var sp = t.split('/');
        var s = (sp[1].length == 1 ? "0" + sp[1] : sp[1]) + "/" + (sp[0].length == 1 ? "0" + sp[0] : sp[0]) + "/" + sp[2];
        return s;
    } catch (e) { console.log(e); return str; }
}


function upsertDocoments(para, filter, docomentName, type, callback, mongoDb) {
    try {
        var configUpsert = {
            mongoconnect: "ConnChat",
            mongodb: mongoDb,
            collectionname: docomentName,
            type: type
        };
        var json1 = {
            config: JSON.stringify(configUpsert), para: JSON.stringify(para),
            filter: JSON.stringify(filter)
        };

        return ExecuteChatBotSyns(json1, urlServiceAi + "api/procedure/execute", "POST", callback, true);
    } catch (e) { console.log(e); }
}

function getPostHtml(jsonpara, type, url, callback) {
    try {
        var sync = false;
        var list = null;
        if (callback != null && callback != undefined && callback != "")
            sync = true;
        $.ajax({
            url: url,
            type: type,
            data: jsonpara == null ? null : JSON.stringify(jsonpara),
            contentType: 'application/json',
            dataType: 'json',
            async: sync,
            success: function (data) {
                if (sync)
                    eval(callback)(data, jsonpara);
                else {
                    list = data;
                    return data;
                }
            }
        });
        return list
    } catch (e) { console.log(e); }
}

function markAsReply(thiss) {
    try {
        if (_customer_id == null || _customer_id == undefined)
            return;
        var para = { unread: false, nonreply: false };
        upsertDocoments({ $set: para }, { id: _customer_id, business_id: $("#id_business").val() }, "Customers", "upsert", "", "mongoDbHibaza");

    } catch (e) { console.log(e); }
}

