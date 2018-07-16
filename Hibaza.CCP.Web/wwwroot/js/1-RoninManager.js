
RoninManager = function () {
    this.$main = roninManagerModal.find(".js-main"), this.$edit = roninManagerModal.find(".js-edit"), this.data = {}
};

RoninManager.prototype.loadBots = function () {
    this.viewMain(), roninManagerModal.find(".wz-loader").fadeIn();
    var t = [{
        id: 0,
        name: "Register",
        run: [1, 2],
        trigger: ["#menu"],
        brand: 1,
        channels: [187, 190, 213],
        disabled: 0,
        init: 0,
        trigger_init: 3
    }, {
        id: 1,
        name: "Autoresponse",
        run: [1],
        trigger: [],
        brand: 1,
        channels: [213],
        disabled: 1,
        init: 0,
        trigger_init: null
    }, {
        id: 2,
        name: "Holidays",
        run: [2],
        trigger: [],
        brand: 1,
        channels: [187, 190],
        disabled: 0,
        init: 0,
        trigger_init: null
    }],
        e = roninManagerModal.find(".js-bot-container");
    e.empty(), $.each(t, function (t, i) {
        e.append($blockTemplateRoninBot.tmpl({
            id: i.id,
            name: i.name,
            disabled: i.disabled,
            channels: []
        }))
    }), roninManagerModal.find(".wz-loader").fadeOut()
}, RoninManager.prototype.editBot = function () {
    this.viewEdit();
    var t = roninManagerModal.find("#wz-bot-blocks"),
        e = roninManagerModal.find("#wz-bot-edit-block");
    return this.data = {
        name: "register",
        run: [1, 2],
        trigger: ["#menu"],
        brand: 1,
        channels: [1, 2, 3],
        disabled: 0,
        init: 0,
        trigger_init: 3,
        status: [{
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "Hola, ¿qué tal? ¿Cómo te llamas?",
                entity: "text"
            }]
        }, {
            "function": "__text__",
            field: "name",
            overwrite: !0
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "No te he entendido. Repíteme tu nombre.",
                entity: "text"
            }]
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "Dime qué quieres de estas opciones:",
                entity: "button",
                buttons: ["Una alcachofa.", "Un pepino."]
            }]
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "No te he entendido.",
                entity: "text"
            }]
        }, {
            "function": "__choice__"
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "Odio las alcachofas, {{customer.name}}. Dime tu email.",
                entity: "text"
            }]
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "Odio los pepinos, {{customer.name}}. Dime tu email.",
                entity: "text"
            }]
        }, {
            "function": "__email__",
            field: "email",
            overwrite: !0
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "repite tu email, please.",
                entity: "text"
            }]
        }, {
            "function": "__send_message__",
            responses: [{
                order: 0,
                text: "Ya no quiero ser tu amigo. Adiós.",
                entity: "text"
            }, {
                order: 1,
                path: "https://mejorconsalud.com/wp-content/uploads/2014/01/Alcachofa-500x375.jpg",
                entity: "image"
            }]
        }],
        relations: [{
            prev: 0,
            next: 1
        }, {
            prev: 1,
            next: 2,
            success: !1
        }, {
            prev: 2,
            next: 1
        }, {
            prev: 1,
            next: 3,
            success: !0
        }, {
            prev: 3,
            next: 5
        }, {
            prev: 5,
            next: 4,
            success: !1
        }, {
            prev: 4,
            next: 5
        }, {
            prev: 5,
            next: 6,
            success: !0,
            keywords: ["alcachofa"]
        }, {
            prev: 5,
            next: 7,
            success: !0,
            keywords: ["pepino"]
        }, {
            prev: 6,
            next: 8
        }, {
            prev: 7,
            next: 8
        }, {
            prev: 8,
            next: 9,
            success: !1
        }, {
            prev: 9,
            next: 8
        }, {
            prev: 9,
            next: 10,
            success: !0
        }, {
            prev: 8,
            next: 3,
            success: !0,
            keywords: ["#menu"]
        }, {
            prev: 3,
            next: 3,
            success: !0,
            keywords: ["#menu"]
        }]
    }, e.empty(), t.empty(), t.append($blockTemplateBlockList.tmpl({
        name: this.data.name
    })), void $.each(this.data.status, function (e, i) {
        t.find("#wz-block-list").append($blockTemplateBlock.tmpl({
            id: e,
            name: "Block #" + e
        }))
    })
}, RoninManager.prototype.editBlock = function (t) {
    var e = roninManagerModal.find("#wz-bot-edit-block");
    e.empty(), e.append($blockTemplateEditBot.tmpl({
        name: "Block #" + t
    })), roninManagerModal.find('.wz-step[data-step="1"]').append($blockTemplateMenuLight.tmpl({
        header: "¿Cómo se activa este bloque?",
        options_text: RONIN_BOT_ORIGINS
    })), roninManagerModal.find('.wz-step[data-step="2"]').append(this.getTemplateById(t)), roninManagerModal.find('.wz-step[data-step="3"]').append($blockTemplateMenuLight.tmpl({
        header: "¿Qué debe ocurrir tras este bloque?",
        options_text: RONIN_BOT_ACTIONS
    }))
}, RoninManager.prototype.getTemplateById = function (t) {
    var e = this.data.status[t]["function"],
        i = $(RONIN_BOT_BLOCK_TYPES[e].template),
        n = RONIN_BOT_BLOCK_TYPES[e].template_data;
    return i.tmpl(n)
}, RoninManager.prototype.viewMain = function () {
    this.$main.removeClass("hide"), this.$edit.addClass("hide"), roninManagerModal.addClass("wz-modal-right wz-modal-sideright").removeClass("wz-modal-fullscreen")
}, RoninManager.prototype.viewEdit = function () {
    this.$main.addClass("hide"), this.$edit.removeClass("hide"), roninManagerModal.removeClass("wz-modal-right wz-modal-sideright").addClass("wz-modal-fullscreen")
}, RoninManager.prototype.nextStep = function () {
    var t = roninManagerModal.find(".wz-step.active"),
        e = t.data("step");
    if (3 > e) {
        var i = parseInt(e) + 1;
        t.removeClass("active"), roninManagerModal.find('.wz-step[data-step="' + i + '"]').addClass("active"), 3 == i ? (roninManagerModal.find(".wz-nav-buttons .btn.next").addClass("hide"), roninManagerModal.find(".wz-nav-buttons .btn.save").removeClass("hide")) : roninManagerModal.find(".wz-nav-buttons .btn.back").removeClass("hide")
    }
}, RoninManager.prototype.prevStep = function () {
    var t = roninManagerModal.find(".wz-step.active"),
        e = t.data("step");
    if (e > 1) {
        var i = e - 1;
        t.removeClass("active"), roninManagerModal.find('.wz-step[data-step="' + i + '"]').addClass("active"), 1 == i ? roninManagerModal.find(".wz-nav-buttons .btn.back").addClass("hide") : (roninManagerModal.find(".wz-nav-buttons .btn.save").addClass("hide"), roninManagerModal.find(".wz-nav-buttons .btn.next").removeClass("hide"))
    }
};

RoninMode = function () {
    this.run = 0, this.register = !1, this.channels = [], this.status = {}, this.relations = [], this.toJson = function () {
        return {
            run: this.run,
            register: this.register,
            channels: this.channels,
            status: this.getStatusJson(),
            relations: this.getRelationsJson()
        }
    }, this.addStatus = function (t) {
        var e;
        return this.status[t] ? e = this.status[t] : (e = new RoninStatus(t), this.status[t] = e), console.log("addStatus" + e), e
    }, this.addRelation = function (t, e) {
        var i = new RoninRelation(t, e);
        console.log("addRelation" + i), this.relations.push(i)
    }, this.getStatusJson = function () {
        var t = [];
        for (var e in this.status) t.push(this.status[e]);
        return t
    }, this.getRelationsJson = function () {
        var t = [];
        return this.relations.forEach(function (e, i, n) {
            t.push(e.toJson())
        }), t
    }
};

RoninStatus = function (t) {
    this.id = t, this.processer = null, this.field = null, this.responses = [], this.wrongs = [], this.setProcesser = function (t) {
        console.log("setProcesser: " + t), this.processer = t
    }, this.setField = function (t) {
        var e, i;
        switch (t) {
            case "Name":
                e = "__text__", i = "name";
                break;
            case "Phone":
                e = "__phone__", i = "phone";
                break;
            case "Email":
                e = "__email__", i = "email";
                break;
            case "Zip":
                e = "__location__", i = "postal_code";
                break;
            case "Address":
                e = "__text__", i = "address";
                break;
            case "Country":
                e = "__text__", i = "country";
                break;
            case "Birthdate":
                e = "__date__", i = "birthdate";
                break;
            case "Other":
                e = "__none__", i = null;
                break;
            default:
                e = "__none__", i = null
        }
        this.processer = e, this.field = i, console.log("setField: " + this.field + "-" + this.processer)
    }, this.toJson = function () {
        return {
            id: this.id,
            processer: this.processer,
            field: this.field,
            responses: this.responses,
            wrongs: this.wrongs
        }
    }, this.addResponse = function (t) {
        console.log("addResponse: " + t), this.responses[0] = t
    }
}, RoninRelation = function (t, e) {
    this.fromId = t, this.toId = e, this.keywords = [], this.toJson = function () {
        return {
            fromId: this.toId,
            toId: this.toId,
            keywords: this.keywords
        }
    }
};
