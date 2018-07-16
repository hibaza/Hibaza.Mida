CloudService.prototype.getMediaUrl = function () {
    return self.mediaURL
}, CloudService.prototype.getURLs = function () {
    return self.urls
}, CloudService.prototype.setMediaUrl = function (t) {
    self.mediaURL = t
}, CloudService.prototype.setReaded = function (t) {
    if ("object" == typeof CONTACTS[t] && "object" == typeof THREADS[CONTACTS[t].getKey()] && THREADS[CONTACTS[t].getKey()].isUnread() && myID == CONTACTS[t].getAgent()) return $.post(self.urls.threads.read + businessID + '/' + CONTACTS[t].getKey() + '?agent_id=' + CONTACTS[t].getAgent(), function (t) { });
    var e = new $.Deferred;
    return e.reject(), e.promise()
}, CloudService.prototype.searchContacts = function (t, e) {
    $.post(self.urls.customers.search + businessID, {
        keywords: t.term
    }, function (t) {
        if (t.threads && t.threads.length > 0) e($.map(t.threads, function (t) {
            var e = $('#wz-modal-reports  .js-filter-elem[data-key="' + t.id + '"]');
            return 0 == e.length ? (t.label = t.name, t) : null
        }));
        else {
            var i = [{
                label: trNoMatchesFound,
                value: e.term
            }];
            e(i)
        }
    })
}, CloudService.prototype.searchAgents = function (t, e) {
    var i = [];
    $.each(AGENTS, function (e, n) {
        var o = $('#wz-modal-reports  .js-filter-elem[data-key="' + e + '"]');
        0 == o.length && -1 != n.name.toUpperCase().search(t.term.toUpperCase()) && (n.label = n.name, n.id = parseInt(e), i.push(n))
    }), e(i.length > 0 ? i : [{
        label: trNoMatchesFound,
        value: e.term
    }])
}
    , CloudService.prototype.loadProfileContacts = function (customer_id) {
        return $.get(self.urls.threads.list + businessID + '/' + customer_id + '/?limit=50', function (t) {
            if (t.data && t.data.length >= 0) {
                $(document).find(".js-list-conversations").html("");
                $.each(t.data, function (t, e) {
                    NProgress.inc(), e.agent_id = CONTACTS[customer_id] ? CONTACTS[customer_id].getAgent() : "", c = new Contact(e), THREADS[c.getKey()] = c, c.profile_render()
                }), NProgress.inc();
                var e = $(".js-list-conversations .js-contact-" + CONTACTS[currentChat].getKey());
                e.size() > 0 && e.addClass("active")
            }
            tooltips(".wz-col-sideleft-3"), $sideleft3Scroll.perfectScrollbar("update");
        });
    }
    , CloudService.prototype.loadContacts = function (t, e, i, n, o) {
        var a = {
            first: t,
            quantity: e,
            status: '',
            agent_id: '',
            channel_id: '',
            search: '',
            from_date: $(".filter-home .js-filter-date-finish").val(),
            to_date: $(".filter-home .js-filter-date-finish").val(),
        };

        (t || 0 === t) && (a.first = t), e && (a.quantity = e), i && (a.status = i == "all" ? "" : i), n && (a.agent_id = n), o && (a.channel_id = o), a.search = inputFinder2.val();

        return $.get(self.urls.customers.list + businessID + "/?access_token=" + accessToken, a, function (t) {
            if (t.data && t.data.length >= 0) {
                $.each(t.data, function (t, e) {
                    e.avatar = e.avatar == null ? "" : e.avatar.indexOf("http://") < 0 && e.avatar.indexOf("https://") < 0 ? baseUrls_Api + e.avatar : e.avatar;
                    var id = e.id;
                    NProgress.inc(), e.active_thread.agent_id = e.agent_id, THREADS[e.active_thread.id] = new Contact(e.active_thread),
                        e.active_thread.unread = e.unread, e.active_thread.nonreply = e.nonreply, e.active_thread.open = e.open
                    CONTACTS[id] = new Contact(e.active_thread), CONTACTS[id].render(), CONTACTS[id].refresh(["button", "name", "unread", "input", "avatar"])
                }), NProgress.inc();//, sortContacts();
                var e = $(".js-list-contacts .js-contact-" + currentChat);
                e.size() > 0 && e.addClass("active")
            }
            tooltips(".wz-col-sideleft-2"), $("#sideleft2-scroll").perfectScrollbar("update"), noCustomersViewHandler()
        });

    }, CloudService.prototype.saveContact = function (t) {
        $.post(self.urls.customers["new"], {
            customer: t
        })
    }, CloudService.prototype.setContactPicture = function (t) {
        $.post(self.urls.customers.get_picture + t)
    }, CloudService.prototype.getCustomerProfile = function (t) {
        return $.post(createURL(self.urls.customers.profile + businessID, t), function (t) {
            t.ok && t.view && colSiderightClient.html(t.view);
            t.ok && t.data && t.data.customer_id && cloud.loadProfileContacts(t.data.customer_id).done(function () {
                var k = CONTACTS[t.data.customer_id].getKey();
                "object" == typeof THREADS[k] && THREADS[k].profile_refresh();
                cloud.getThreadProfile(k);
                $(document).find(".js-list-conversations li[class*='js-contact-']").removeClass("active");
                $(document).find(".js-list-conversations .js-contact-" + k).addClass("active");
            });
        });

    }, CloudService.prototype.saveLastVisit = function (t) {
        typeof CONTACTS[currentChat] === "object" &&
            $.ajax({
                url: self.urls.threads.last_visit,
                type: "POST",
                data: JSON.stringify({ business_id: businessID, thread_id: CONTACTS[currentChat].getKey(), agent_id: myID, url: t }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (res) {
                }
            })
    }, CloudService.prototype.getThreadProfile = function (t) {
        $sideLeft3.find(".thread-profile").html("");
        if (t || 0 === t) return $.post(self.urls.threads.profile + businessID + "/" + t, function (n) {
            n.ok && n.view && $colSideright.find(".thread-profile").html(n.view);
            n.ok && n.data && $colSideright.find("#wz-tab-products").html('') && $colSideright.find("#wz-tab-products").append('<iframe id="iframeshop" class="shop" src="' + (n.data.last_visit.length > 0 ? n.data.last_visit : "http://m.baza.vn/tim-kiem") + '"></iframe>');
            if (n.ok && n.data) {
                // click vao 1 khach hang
                forwardToBot(""); //$(".js-message-text").html()
            }
        });

        var n = new $.Deferred;
        return n.reject(), n.promise()
    }, CloudService.prototype.getTicketProfile = function (t, e, i) {
        if (t || 0 === t) return $.post(self.urls.tickets.profile + businessID + "/" + t, function (n) {
            n.ok && n.view && (currentTicket == t && n.head_banner && lastTicket.html(n.head_banner), i || colSiderightTicket.html(n.view), e && $.each(e, function (t, e) {
                e()
            }))
        });
        var n = new $.Deferred;
        return n.reject(), n.promise()
    }, CloudService.prototype.getAgentProfile = function (t, e) {
        if (t || 0 === t) return $.post(self.urls.agents.profile + "/" + t, function (t) {
            t.page && (colSiderightAgent.html(t.page), e && $.each(e, function (t, e) {
                e()
            }))
        });
        var i = new $.Deferred;
        return i.reject(), i.promise()
    }, CloudService.prototype.getPaymentProfile = function (t, e) {
        if (t) return $.post(self.urls.payments.view + t + "/", function (t) {
            t.page && (colSiderightPayment.html(t.page), e && $.each(e, function (t, e) {
                e()
            }))
        });
        var i = new $.Deferred;
        return i.reject(), i.promise()
    }, CloudService.prototype.getTicketForm = function (t) {
        !t && 0 !== t || "undefined" == typeof CONTACTS[t] ? new NotificationPlugin(trSelectChat, trSelectChatTicket, "icon-lock", null, null, defNotificationTimer) :
            ($.ajax({
                url: self.urls.tickets.add + businessID + "/" + CONTACTS[t].getKey(),
                type: "POST",
                data: JSON.stringify({
                    'description': '',
                    'short_description': '',
                    'type': 0,
                    'status': 0,
                    'tags': '',
                    'customer_id': '',
                    'customer_name': CONTACTS[t].getName()
                }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (res) {
                    if (res && res.ok && res.view) {
                        modalAddTicket.html(res.view);
                    }
                }
            }),

                $("#wz-modal-add-ticket").addClass("js-hide-optionals"))
    }, CloudService.prototype.getShortcutForm = function (t) {
        void 0 !== typeof t && t && t != "" ?
            $.get(self.urls.brands.views.edit_shortcut + businessID + '/' + t, function (t) {
                t.ok && (modalAddShortcut.html(t.data), modalAddShortcut.modal("show"))
            }) :
            $.get(self.urls.brands.views.new_shortcut + businessID, function (t) {
                t.ok && (modalAddShortcut.html(t.data), modalAddShortcut.modal("show"))
            })
    }, CloudService.prototype.getShortcuts = function () {
    return $.get(baseUrls_Api + "brands/shortcuts/get-by-agent/" + businessID + "/" + myID + "?" + accessToken, function (t) {
        
            t.ok && loadShortcuts(t.data)
        })
    }, CloudService.prototype.removeShortcut = function (t) {
        $.post(self.urls.brands.data.delete_shortcut + businessID + '/' + t, function (t) {
            modalAddShortcut.modal("hide"), cloud.getShortcuts(), modalShortcuts.modal("show"), new NotificationPlugin(trSaved, trShortcutRemoved, "icon-check", null, null, defNotificationTimer)
        })
    }, CloudService.prototype.getPaymentForm = function () {
        "undefined" != typeof CONTACTS[currentChat] ? $.post(createURL(self.urls.payments["new"], currentChat, ""), {}, function (t) {
            t.page && modalPaymentGenerator.html(t.page)
        }) : new NotificationPlugin(trSelectChat, trSelectChatPayment, "icon-lock", null, null, defNotificationTimer)
    }, CloudService.prototype.getEditProfileForm = function (t) {
        $.get(self.urls.customers.edit + businessID + '/' + t, function (t) {
            t.view && modalEditUser.html(t.view);
            //setTimeout(function () {
            $("#id_city").val($("#id_city").attr("value"));
            //}, 10);
        })
    }, CloudService.prototype.getSendCustomerNote = function (t) {
        var e = new URL(self.urls.customers.send_note + businessID + "/" + CONTACTS[currentChat].getKey());
        e.loadDataParams(t), $.post(e.getURL(), function (t) {
            t.ok && t.view && modalAddNote.html(t.view), setTimeout(function () {
                $(".js-focus-on-load").focus()
            }, 500)
        })
    }, CloudService.prototype.getSendTicketNote = function (t) {
        $.post(self.urls.tickets.send_note + "/" + t + "/", function (t) {
            t.page && modalAddNote.html(t.page), setTimeout(function () {
                $(".js-focus-on-load").focus()
            }, 500)
        })
    }, CloudService.prototype.editTicketNote = function (t) {
        $.post(self.urls.tickets.edit_note + t + "/", function (t) {
            t.page && modalAddNote.html(t.page), setTimeout(function () {
                $(".js-focus-on-load").focus()
            }, 500)
        })
    }, CloudService.prototype.removeTicketNote = function (t) {
        $.get(createURL(self.urls.tickets.remove_note, t), function (t) {
            t.error || ($("#wz-modal-add-note").modal("hide"), cloud.getTicketProfile(t.ticket_id), new NotificationPlugin(trNoteDeleted, trNoteDeleted, "icon-trash", null, null, defNotificationTimer))
        })
    }, CloudService.prototype.removeCustomerNote = function (t) {
        $.post(self.urls.customers.remove_note + businessID + '/' + t, function (t) {
            !t.ok || ($("#wz-modal-add-note").modal("hide"), cloud.getCustomerProfile(t.customer_id), new NotificationPlugin(trNoteDeleted, trNoteDeleted, "icon-trash", null, null, defNotificationTimer))
        })
    }, CloudService.prototype.sendForm = function (t, e, i) {
        var n = $.ajax({
            url: e,
            type: "POST",
            data: new FormData(t),
            contentType: !1,
            cache: !1,
            processData: !1,
            success: i
        });
        return modalImgGallery.addClass('hidden'), modalImg.modal("hide"), n
    }, CloudService.prototype.getLastTicket = function (t) {
        return currentTicket = !1, t && $.post(createURL(self.urls.customers.last_ticket + businessID, t), function (t) {
            (lastTicket.html(t.view), t.data && t.data.ticket ? currentTicket = t.data.ticket : currentTicket = void 0)
        })
    }, CloudService.prototype.setShortDescription = function (t, e) {
        var i = e || currentTicket;
        return $.post(createURL(self.urls.tickets.set_short_description, i), {
            business_id: businessID,
            short_description: t
        }, function (e) {
            e.ok && (viewTicketSelected == i && cloud.getTicketProfile(i), mixpanelEvents("changeTicketShortDescription", t))
        })
    }, CloudService.prototype.setDescription = function (t, e) {
        var i = e || currentTicket;
        return $.post(createURL(self.urls.tickets.set_description, i), {
            business_id: businessID,
            description: t
        }, function (e) {
            e.ok && (viewTicketSelected == i && cloud.getTicketProfile(i), mixpanelEvents("changeTicketDescription", t))
        })
    },

    CloudService.prototype.setStatus = function (t, e) {
        var i = e || currentTicket;
        if (i) return $.post(createURL(self.urls.tickets.set_status, i, t), function (e) {
            if (e.ok) {
                viewTicketSelected == i && cloud.getTicketProfile(i), currentTicket == i && ($(".js-status-select").removeClass("selected"), $(".js-status-select[data-status=" + t + "]").addClass("selected"), removeClassByRegExp($("#statusDropdown i"), /^wz\-status\-/), $("#statusDropdown i").addClass("wz-status-" + t));
                var n = $("#wz-tab-pedidos .js-ticket-view[data-ticket=" + i + "]").find(".wz-ticket-status");
                n.size() > 0 && (removeClassByRegExp(n, /^wz\-status\-/), n.addClass("wz-status-" + t)), i != currentTicket || 0 != t && 1 != t || firebaseService.deleteOptionMessages()
            }
        });
        var n = new $.Deferred;
        return n.reject(), n.promise()
    }, CloudService.prototype.editTicketTags = function (t, e) {
        var i = e || currentTicket;
        $.post(self.urls.tickets.edit_tags, {
            'owner_id': i,
            'tags': t
        }, function (e) {
            e.ok && (cloud.getTicketProfile(i), mixpanelEvents("changeTicketTags", t))
        })
    }, CloudService.prototype.setAlert = function () {
        $.post(self.urls.brands.set_alert, function (t) {
            t.ok && modalAddAlert.html(t.page)
        })
    }, CloudService.prototype.getEditTicketForm = function (t) {
        $.post(self.urls.tickets.edit + businessID + "/" + t, function (t) {
            t.ok && t.view && modalAddTicket.html(t.view), mixpanelEvents("editTicket")
        }), $("#wz-modal-add-ticket").removeClass("js-hide-optionals")
    }, CloudService.prototype.getContact = function (t, e) {
        return $.post(self.urls.customers.get + "/" + t, function (i) {
            CONTACTS[t] = new Contact(i.active_thread), e || CONTACTS[t].refresh()
        })
    }, CloudService.prototype.getSendFiles = function () {
        $.post(self.urls.customers.get_send_files + currentChat, function (t) {
            modalGallery.html(t.page)
        })
    }, CloudService.prototype.sendFiles = function (t, p) {
        typeof CONTACTS[currentChat] === "object" &&
            $.ajax({
                url: self.urls.messages.send_files,
                type: "POST",
                data: JSON.stringify({ business_id: businessID, thread_id: CONTACTS[currentChat].getKey(), source_url: p, image_url: t }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (res) {
                }
            })
    }, CloudService.prototype.getReports = function (t) {
        return $.post(self.urls.brands.reports_html + businessID, function (t) {
            t.ok && t.data && (reportsModal.html(t.data), mixpanelEvents("openReports"))
        })
    }, CloudService.prototype.openSettings = function (t, e) {
        var i, n = t || "#user-settings";

        switch (n) {
            case "#user-settings":
                i = this.loadSettingsInfo();
                break;
            case "#company-settings":
                i = this.loadSettingsBrand();
                break;
            case "#channel-settings":
                i = this.loadSettingsChannels();
                break;
            case "#agent-settings":
                i = this.loadSettingsAgents();
                break;
            case "#hotline-setting":
                i = this.loadSettingsHotline();
                break;
            case "#conversation-download":
                i = this.loadConversationDownload();
                break;
            default:
                i = this.loadSettingsInfo()
        }
        return $("#wz-modal-clients .wz-crm-menu .active").removeClass("active"), $('#wz-modal-clients .wz-crm-menu a[href="' + n + '"]').addClass("active"), i.done(function () {
            $("#wz-modal-clients .block-show").removeClass("block-show").addClass("hide"), $(n).removeClass("hide").addClass("block-show"), "function" == typeof e && e()
        }), i
    }, CloudService.prototype.assignCustomer = function (t) {
        return $.post(self.urls.customers.assign + businessID + '/' + myID + '/' + t + "/?access_token=" + accessToken)
    }, CloudService.prototype.assignCustomerTo = function (t, e) {
        return $.post(self.urls.customers.assign_to + businessID + '/' + t + "/" + e + "/?access_token=" + accessToken)
    }, CloudService.prototype.unassignCustomer = function (t) {
        return $.post(self.urls.customers.unassign + businessID + '/' + t + "/?access_token=" + accessToken)
    }, CloudService.prototype.archiveCustomer = function (t) {
        return $.post(self.urls.customers.archive + t)
    }, CloudService.prototype.unarchiveCustomer = function (t) {
        return $.post(self.urls.customers.unarchive + t)
    }, CloudService.prototype.unreadThread = function (t) {
        return $.post(self.urls.threads.unread + businessID + '/' + t)
    }, CloudService.prototype.toggleAdminRights = function (t) {
        return $.post(self.urls.agents.data.admin + businessID + '/' + t, function (e) {
            cloud.loadSettingsAgents(), mixpanelEvents("changePermissions", t)
        })

    }, CloudService.prototype.reloadChannels = function () {
        return $.get(self.urls.channels.data.get_all + businessID, function (t) {
            t && t.data && ($(".js-channel-list:not(.js-channel-list-all)").addClass("js-not-processed"), t.data.forEach(function (t, e) {
                var i = $(".js-channel-list[data-channel=" + t.id + "]");
                var tmp = $channelTemplate.tmpl({
                    key: t.id,
                    name: t.name,
                    type: t.type,
                    active: t.active ? "active" : ""
                });
                if ("undefined" == typeof $(i).html()) {
                    $(".js-channel-list-all").parent().append(tmp);
                    i = tmp;
                    i.on("click", chatFilterSelection),
                        CHANNELS[t.id] = {
                            name: t.name,
                            facebook_id: t.ext_id,
                            phone: t.phone,
                            type: t.type,
                            active: t.active ? "active" : ""
                        }

                    //COUNTERS.channels[t.id] = {}, COUNTERS.channels_unread[t.id] = {};
                }
                else { $(i).html($(tmp).html()) };
                i.removeClass("js-not-processed"), i.find(".wz-channelname").text(t.name), t.active ? i.removeClass("js-channel-offline") : i.addClass("js-channel-offline");
            }), $(".js-channel-list.js-not-processed").each(function (t, e) {
                var i = $(this).data("channel");
                deleteDictKey(COUNTERS.channels, i), deleteDictKey(COUNTERS.channels_unread, i), $(this).hasClass("active") && $(".js-channel-list-all a").trigger("click"), $(this).remove()
            }), $(".all-channel-count").html("(" + Object.keys(t.data).length + ")"),
                cloud.refreshCounters(), tutorialActive && (Object.keys(t.data).length <= 0 ? settingsModal.modal("hide") : (welcomeModal.addClass("wz-modal-right").removeClass("wz-modal-fullscreen"), welcomeModal.find(".wz-team").removeClass("hide"), settingsModal.addClass("wz-modal-right").removeClass("wz-modal-fullscreen modal-firstchannel modal-facebook"), settingsModal.find(".modal-menu").show())))
        })
    },
    //CloudService.prototype.getAgentHotline = function (t) {
    //console.log("dddddddfff");
    //console.log(t);
    //console.log(createURL(self.urls.hotline.get + businessID, t));
    //return $.post(createURL(self.urls.hotline.get + businessID , t), function (t) {
    //    t.ok && t.view
    //        //&& colSiderightClient.html(t.view);
    //        //t.ok && t.data && t.data.customer_id && cloud.loadProfileContacts(t.data.customer_id).done(function () {
    //        //    var k = CONTACTS[t.data.customer_id].getKey();
    //        //    "object" == typeof THREADS[k] && THREADS[k].profile_refresh();
    //        //    cloud.getThreadProfile(k);
    //        //    $(document).find(".js-list-conversations li[class*='js-contact-']").removeClass("active");
    //        //    $(document).find(".js-list-conversations .js-contact-" + k).addClass("active");
    //        //});
    //    });
    //};

    CloudService.prototype.reloadAgents = function () {
        return $.get(self.urls.agents.data.get_all + businessID, function (t) {
            t && t.data && ($(".js-agent-list:not(.js-agent-list-all)").addClass("js-not-processed"), t.data.forEach(function (t, e) {
                var i = $(".js-agent-list[data-agent=" + t.id + "]");
                var tmp = $agentTemplate.tmpl({
                    key: t.id,
                    name: t.name,
                    type: 'head',
                    status: t.status
                });
                if ("undefined" == typeof $(i).html()) {
                    $(".js-agent-list-all").parent().append(tmp);
                    i = tmp;
                    i.on("click", chatFilterSelection), AGENTS[t.id] = {
                        name: t.name,
                        avatar: t.avatar,
                        status: t.status,
                        shogun: t.admin,
                        active: t.active
                    }

                    //COUNTERS.agents[t.id] = {};
                }
                else {
                    $(i).html($(tmp).html());
                }
                i.removeClass("js-not-processed"), i.find(".wz-agent-name").text(t.name), i.removeClass("js-agent-offline"), i.removeClass("js-agent-busy"), i.removeClass("js-agent-online"), i.addClass("js-agent-" + t.status), t.id === myID && i.addClass("active") && i.trigger("click");
            }), $(".js-agent-list.js-not-processed").each(function (t, e) {
                var i = $(this).data("agent");
                deleteDictKey(COUNTERS.agents, i), deleteDictKey(COUNTERS.agents_unread, i), $(this).hasClass("active") && $(".js-agent-list-all a").trigger("click"), $(this).remove()
            }), $(".all-agent-count").html("(" + Object.keys(t.data).length + ")"), cloud.refreshCounters(), loadSearchAgent("#client-status"), loadSearchAgent(".wz-col-central .textarea .assignation"))
        })
    }
    , CloudService.prototype.deleteAgent = function (t) {
        return $.post(self.urls.agents.data["delete"] + businessID + "/" + t, function (t) {
            cloud.loadSettingsAgents(), cloud.reloadAgents()
        })
    }, CloudService.prototype.deleteStripe = function () {
        $.post(self.urls.brands.stripe["delete"], function () {
            loadSettingsPayments(), mixpanelEvents("removePaymentGateway", {
                type: "Stripe"
            })
        })
    }, CloudService.prototype.deleteBraintree = function () {
        $.post(self.urls.brands.braintree["delete"], function () {
            loadSettingsPayments(), mixpanelEvents("removePaymentGateway", {
                type: "Braintree"
            })
        })
    }, CloudService.prototype.roninDisable = function (t) {
        return $.post(self.urls.ronin.disable + t + "/")
    }, CloudService.prototype.roninDelete = function (t) {
        return $.post(self.urls.ronin["delete"] + t + "/")
    }, CloudService.prototype.roninEdit = function (t) {
        return $.post(self.urls.ronin.edit + t + "/", function (t) {
            t.mode && roninEdit(t.mode)
        })
    }, CloudService.prototype.saveRoninMode = function (t) {
        return $.post(self.urls.ronin.save_mode, {
            "new": t
        })
    }, CloudService.prototype.roninManager = function () {
        return $.post(self.urls.ronin.manager, function (t) {
            t.manager && roninManagerModal.html(t.manager)
        })
    }, CloudService.prototype.roninRequest = function (t, e) {
        return "" != t ? $.post(self.urls.ronin.request_bot, {
            bot: t,
            type: e
        }, function (t) {
            t.success || new NotificationPlugin(trErrorSendingData, trPleaseTryAgain, "icon-cross", null, null, null)
        }) : void 0
    }, CloudService.prototype.loadRoninModal = function () {
        return $.post(self.urls.ronin.load, function (t) {
            t.automations && automationsModal.html(t.automations), t.ronin && roninModal.html(t.ronin), initRoninUI(), mixpanelEvents("openAutomate")
        })
    }, CloudService.prototype.verifySuscription = function () {
        return suscriptionExpired ? (removeSettings(), "True" == AGENTS[myID].shogun ? modalPricing() : modalNotification({
            icon: "icon-lock",
            title: trAccountLocked,
            message: trRenewSubscription,
            close: !1
        }), mixpanelEvents("endSubscription"), !1) : (suscriptionExpiredToday && new NotificationPlugin(trSubscriptionExpired, trRenewSubscription, "icon-bell"), !0)
    }, CloudService.prototype.getHowManyChannel = function () {
        for (var t = Object.keys(CHANNELS), e = 0, i = 0; i < t.length; i++) {
            var n = CHANNELS[t[i]];
            "ZamiChat" != n.type && (e += 1)
        }
        return e
    }, CloudService.prototype.saveTicketAttach = function (t, e, i) {
        return $.ajax({
            url: cloud.getURLs().tickets.attach,
            type: "POST",
            data: {
                ticket_id: t,
                type: e,
                key: i
            },
            cache: !1,
            dataType: "json",
            processData: !0
        })
    }, CloudService.prototype.deleteTicketAttach = function (t, e, i) {
        return $.ajax({
            url: cloud.getURLs().tickets.unattach,
            type: "POST",
            data: {
                ticket_id: t,
                type: e,
                key: i
            },
            cache: !1,
            dataType: "json",
            processData: !0
        })
    }, CloudService.prototype.clearChat = function (t) {
        var e = t || CONTACTS[currentChat].getKey();
        return $.post(self.urls.wakizashi.clear + e + "/", function (t) {
            t.success && daisho.clearChatConfirmClose()
        })
    }, CloudService.prototype.clearChatConfirmOpen = function () {
        $("#edit-customer-form .wz-clear-chat-btn").addClass("hide"), $("#edit-customer-form .wz-clear-chat-confirm").removeClass("hide")
    }, CloudService.prototype.clearChatConfirmClose = function () {
        $("#edit-customer-form .wz-clear-chat-confirm").addClass("hide"), $("#edit-customer-form .wz-clear-chat-btn").removeClass("hide")
    }, CloudService.prototype.deleteMessage = function (e, t) {
        return $.post(self.urls.messages.delete + businessID + '/' + t + '/?access_token=' + accessToken,
            function (data) {
                if (data.ok) {
                    MESSAGES[t].delete();
                }
            }
        );
    }, CloudService.prototype.goToMessage = function (t, e, i) {
        var n = chatList.find(".js-chat-message[data-key=" + e + "]");
        if (n.size() > 0) {
            var o = n.position().top - n.outerHeight() + messageListContainer.scrollTop();
            10 > o && (o = 10), messageListContainer.scrollTop(o), $centralScroll.perfectScrollbar("update"), n.removeClass("stick"), setTimeout(function () {
                n.addClass("stick")
            }, 100)
        } else firebaseService.setChat(t, null, i).done(function () {
            var t = chatList.find(".js-chat-message[data-key=" + e + "]");
            t.removeClass("stick"), setTimeout(function () {
                t.addClass("stick")
            }, 100)
        })
    }, CloudService.prototype.changeStatus = function (t) {
        return $.post(self.urls.agents.set_status + myID + '/' + t)
    }, CloudService.prototype.createOptionsMessage = function () {
        return $.post(self.urls.customers.options + currentChat)
    }, CloudService.prototype.createAutoTicket = function () {
        return $.post(self.urls.customers.autoticket + currentChat)
    }, CloudService.prototype.loadCounters = function (countersObj) {
        if (countersObj) {
            countersObj.channels = countersObj.channels || "";
            countersObj.agents = countersObj.agents || "";
            countersObj.pending_unread = countersObj.pending_unread || "";
            countersObj.attention_unread = countersObj.attention_unread || "";
            countersObj.channels_unread = countersObj.channels_unread || "";
            COUNTERS = countersObj;
            cloud.refreshCounters();
        }
    }, CloudService.prototype.getCounters = function () {
        return $.get(self.urls.brands.counters, function (t) {
            t.counters ? cloud.loadCounters(t.counters) : console.log("Get counters no data")
        })
    }, CloudService.prototype.refreshCounters = function () {
        var t = 0;
        CHANNELS && $.each(Object.keys(CHANNELS), function (e, i) {

            var n = COUNTERS.channels_unread && COUNTERS.channels_unread[i] ? COUNTERS.channels_unread[i] : 0;
            t += n;
            if (i.toLowerCase().indexOf("hotline") < 0) {
                $("#wz-acc-c-01 .js-chats-select[data-channel=" + i + "] .badge").text(n).removeClass("loading"), n > 0 ? $("#wz-acc-c-01 .js-channel-list[data-channel=" + i + "]").addClass("unread").removeClass("hidden") : $("#wz-acc-c-01 .js-channel-list[data-channel=" + i + "]").removeClass("unread").addClass("hidden");
            }
            else {
                try {
                    var countStr = $("#wz-acc-c-01 .js-chats-select[data-channel=" + i + "] .badge").text();
                    var phoneCount = countStr != null && countStr != undefined && countStr != "" ? parseInt(countStr) : 0;
                    t = t + phoneCount;
                }
                catch(exx){ }
                $("#wz-acc-c-01 .js-chats-select[data-channel=" + i + "] .badge").removeClass("loading");
                $("#wz-acc-c-01 .js-channel-list[data-channel=" + i + "]").removeClass("unread").addClass("unread").removeClass("hidden");
            }
        }), $("#wz-acc-c-01 .js-channel-list-all .badge").text(t).removeClass("loading"), t > 0 ? $("#wz-acc-c-01 .js-channel-list-all").addClass("unread") : $("#wz-acc-c-01 .js-channel-list-all").removeClass("unread");

        var s = function (t, e) {
            var ac1 = (parseInt($(e).find(".badge").text()) > 0 ? 1 : 0);
            var ac2 = (parseInt($(t).find(".badge").text()) > 0 ? 1 : 0);
            var an1 = $(e).find(".wz-agent-name").text();
            var an2 = $(t).find(".wz-agent-name").text();
            return ac1 > ac2 ? 1 : ac1 === ac2 ? an1 < an2 ? 1 : -1 : -1;
        }


        var e = 0;
        AGENTS && $.each(Object.keys(AGENTS), function (t, i) {
            var n = COUNTERS.attention_unread && COUNTERS.attention_unread[i] ? COUNTERS.attention_unread[i] : 0;
            e += n, $("#wz-acc-a-01 .js-chats-select[data-agent=" + i + "] .badge").text(n).removeClass("loading"), n > 0 ? $("#wz-acc-a-01 .js-agent-list[data-agent=" + i + "]").addClass("unread") : $("#wz-acc-a-01 .js-agent-list[data-agent=" + i + "]").removeClass("unread")
        }), $("#wz-acc-a-01 .js-chats-select[data-agent=all] .badge").text(e).removeClass("loading"), e > 0 ? $("#wz-acc-a-01 .js-agent-list[data-agent=all]").addClass("unread") : $("#wz-acc-a-01 .js-agent-list[data-agent=all]").removeClass("unread");

        $("#wz-acc-a-01 .js-agent-list").sort(s).appendTo($("#wz-acc-a-01 ul"));

        //COUNTERS.pending_unread && Object.keys(COUNTERS.pending_unread).length > 0 ? $(".wz-filter .js-pending a").addClass("unread") : $(".wz-filter .js-pending a").removeClass("unread"),
        //COUNTERS.attention_unread && Object.keys(COUNTERS.attention_unread).length > 0 ? $(".wz-filter .js-active a").addClass("unread") : $(".wz-filter .js-active a").removeClass("unread")
    }, CloudService.prototype.loadSettingsInfo = function () {
        return $.post(self.urls.settings.info + myID, function (t) {
            t.view && settingsUser.html(t.view)
        })
    }, CloudService.prototype.loadSettingsBrand = function () {

        return $.post(self.urls.settings.business + myID + '/' + businessID, function (t) {
            t.ok && t.view && settingsBrand.html(t.view)
        })

    }, CloudService.prototype.loadSettingsChannels = function () {
        return $.post(self.urls.settings.channels + myID + '/' + businessID, function (t) {
            t.ok && t.view && settingsChannels.html(t.view)
        })
    },
    CloudService.prototype.loadSettingsAgents = function () {
        return $.post(self.urls.settings.agents + myID + '/' + businessID, function (t) {
            t.view && settingsAgents.html(t.view)
        })
    }, CloudService.prototype.loadContact = function (t) {
        t && t.id && ("undefined" == typeof CONTACTS[t.id] && (CONTACTS[t.id] = new Contact(t)), CONTACTS[t.id].isInChatview(chatView, chatViewAgent, chatViewChannel) ? CONTACTS[t.id].render(!0) : resetChatview().done(function () {
            var e = $(".js-list-contacts .js-contact-" + t.id);
            e.size() > 0 ? (CONTACTS[t.id].goToContact(), e.click()) : CONTACTS[t.id].render(!0)
        }))
    },
    CloudService.prototype.loadSettingsHotline = function () {
        try {
            return $.post(self.urls.hotline.get + myID, function (t) {
                t.view && settingsHotline.html(t.view)
            })
        } catch (e) { console.log(e); }
    },
    CloudService.prototype.loadConversationDownload = function () {
    try {
        console.log("jjjjjj");
        console.log(self.urls.brands.conversation_download);
        return $.post(self.urls.brands.conversation_download, function (t) {
            console.log("kkk");
            console.log(t);
            console.log(t.view);
                t.view && conversationDownload.html(t.view)
            })
        } catch (e) { console.log(e); }
    }; 