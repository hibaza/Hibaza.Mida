$(function() {
    $.widget("custom.buttonswitch", {
        options: {
            initialStatus: 0,
            inputID: "switch-default",
            textON: "ON",
            textOFF: "OFF"
        },
        _create: function() {
            var t = this;
            this.element.addClass("wz-switch-button"), this.input = $("<input>").attr({
                type: "checkbox",
                id: t.options.inputID
            }).addClass("wz-switch-button-input").appendTo(t.element), this.setStatus(this.options.initialStatus), this.label = $("<label>").attr({
                "for": t.options.inputID
            }).addClass("wz-switch-button-label").appendTo(t.element), this.inner = $("<span>").addClass("wz-switch-button-inner").appendTo(t.label), this.setInnerText(this.options.textON, this.options.textOFF), this.slider = $("<span>").addClass("wz-switch-button-slider").appendTo(t.label)
        },
        _destroy: function() {
            this.element.removeClass("wz-switch-button"), this.input.remove(), this.label.remove()
        },
        _setOption: function(t, e) {
            "status" === t && this.setStatus(e), this["super"](t, e)
        },
        _setOptions: function(t) {
            var e = this;
            $.each(t, function(t, i) {
                e._setOptions(t, i)
            })
        },
        getStatus: function() {
            return this.input[0].checked
        },
        setStatus: function(t) {
            this.input[0].checked = t
        },
        setID: function(t) {
            this.options.inputID = t, this.input.attr("id", t)
        },
        setInnerText: function(t, e) {
            this.options.textON = t, this.options.textOFF = e, this.inner.attr("data-before", t).attr("data-after", e)
        },
        enable: function() {
            this.input.prop("disabled", !0)
        },
        disable: function() {
            this.input.prop("disabled", !1)
        }
    })
}), $(function() {
    $.widget("custom.checkbox", {
        options: {
            icon: !1,
            label: !1
        },
        _create: function() {
            var t = this;
            this.element.addClass("wz-checkbox-container").on("click", this.clickHandler.bind(this)), this.element.data("icon") && (this.options.icon = this.element.data("icon")), this.element.data("label") && (this.options.label = this.element.data("label")), this.wrapper = $("<i>").addClass("icon-none wz-check").appendTo(t.element), this.input = this.element.find('input[type="checkbox"]').appendTo(this.wrapper), this.input[0].checked && this.wrapper.addClass("active"), this.options.icon && (this.icon = $("<i>").addClass("wz-check-icon").addClass(this.options.icon).appendTo(t.element)), this.options.label && (this.label = $("<span>").text(this.options.label).appendTo(t.element))
        },
        _destroy: function() {
            this.element.removeClass("wz-checkbox-container"), this.input.appendTo(this.element), this.icon && this.icon.remove(), this.label.remove()
        },
        _setOption: function(t, e) {
            "status" === t && this.setStatus(e), this["super"](t, e)
        },
        _setOptions: function(t) {
            var e = this;
            $.each(t, function(t, i) {
                e._setOptions(t, i)
            })
        },
        getStatus: function() {
            return this.input[0].checked
        },
        setStatus: function(t) {
            this.input[0].checked = t;
        },
        setID: function(t) {
            this.input.attr("id", t)
        },
        setText: function(t) {
            this.options.label = t, this.label.text(t)
        },
        clickHandler: function(t) {
            this.setStatus(!this.getStatus()), this.wrapper.toggleClass("active")
        }
    })
}), $(function() {
    $.widget("custom.combobox", {
        options: {
            appendToElem: this.element
        },
        _create: function() {
            this.wrapper = $("<span>").addClass("custom-combobox").insertAfter(this.element), this.element.hide(), this._createAutocomplete(), this._createShowAllButton()
        },
        _setOption: function(t, e) {
            "appendToElem" === t && this._super(t, e)
        },
        _createAutocomplete: function() {
            var t = this.element.children(":selected"),
                e = t.val() ? t.text() : "",
                i = this;
            this.input = $("<input>").appendTo(this.wrapper).val(e).attr("title", "").addClass("form-control custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left").autocomplete({
                delay: 0,
                minLength: 0,
                source: $.proxy(this, "_source"),
                appendTo: i.options.appendToElem
            }).on("click", function() {
                $(this).autocomplete("search", "")
            }), this.input.autocomplete("instance")._renderItem = function(t, e) {
                var i = $("<li>");
                return e.image && i.append($("<span>").addClass(e.image)), i.append($("<span>").text(e.value)), i.appendTo(t), i
            }, this._on(this.input, {
                autocompleteselect: function(t, e) {
                    if (e.item.option.selected = !0, this._trigger("select", t, {
                            item: e.item.option
                        }), e.item.image) {
                        var i = $(this.input).parent().parent().find(".image-selected");
                        i.size() > 0 && (i.prop("class", ""), i.addClass("image-selected " + e.item.image)), $(this.input).val(e.item.value), $(this.input).blur()
                    }
                },
                autocompletechange: "_removeIfInvalid"
            })
        },
        _createShowAllButton: function() {
            var t = this.input,
                e = !1;
            $("<a>").appendTo(this.wrapper).html("<span class='caret'></span>").removeClass("ui-corner-all").addClass("form-control btn btn-default custom-combobox-toggle ui-corner-right").on("mousedown", function() {
                e = t.autocomplete("widget").is(":visible")
            }).on("click", function() {
                t.trigger("focus"), e || t.autocomplete("search", "")
            })
        },
        _source: function(t, e) {
            var i = new RegExp($.ui.autocomplete.escapeRegex(t.term), "i");
            e(this.element.children("option").map(function() {
                var e = $(this).text(),
                    n = $(this).attr("image");
                return !this.value || t.term && !i.test(e) ? void 0 : {
                    label: "<b>" + e + "</b>",
                    value: e,
                    image: n || !1,
                    option: this
                }
            }))
        },
        _removeIfInvalid: function(t, e) {
            if (!e.item) {
                var i = this.input.val(),
                    n = i.toLowerCase(),
                    o = !1;
                this.element.children("option").each(function() {
                    return $(this).text().toLowerCase() === n ? (this.selected = o = !0, !1) : void 0
                }), o || (this.input.val(""), this.element.val(""), this.input.autocomplete("instance").term = "")
            }
        },
        _destroy: function() {
            this.wrapper.remove(), this.element.show()
        }
    })
}), $(function() {
    $.widget("custom.timepicker", {
        variables: {
            cursorPosition: 0
        },
        options: {
            hours: 0,
            minutes: 0
        },
        _create: function() {
            var t = this;
            this.element.addClass("wz-timepicker").val(this.formatTime(this.options.hours, this.options.minutes)).on("click", function() {
                this.selectionStart > 2 ? (t.selectMinutes(), t.variables.cursorPosition = 3) : (t.selectHours(), t.variables.cursorPosition = 0)
            }).on("keydown", function(e) {
                var i = e.keyCode || e.which;
                switch (e.preventDefault(), i) {
                    case 39:
                        t.selectMinutes(), t.variables.cursorPosition = 3;
                        break;
                    case 37:
                        t.selectHours(), t.variables.cursorPosition = 0;
                        break;
                    case 38:
                        t.variables.cursorPosition < 3 ? t.increaseHours() : t.increaseMinutes();
                        break;
                    case 40:
                        t.variables.cursorPosition < 3 ? t.decreaseHours() : t.decreaseMinutes();
                        break;
                    case 8:
                        t.variables.cursorPosition < 3 ? (t.deleteHours(this), t.variables.cursorPosition = 0) : (t.deleteMinutes(this), t.variables.cursorPosition = 3)
                }
                i >= 48 && 57 >= i ? t.enterValue(i - 48) : i >= 96 && 105 >= i && t.enterValue(i - 96)
            }), this.checkHours(), this.checkMinutes(), this._refresh()
        },
        _setOption: function(t, e) {
            "hours" === t ? (this._super(t, e), this.checkHours()) : "minutes" === t && (this._super(t, e), this.checkMinutes()), this._refresh()
        },
        _setOptions: function(t) {
            var e = this;
            $.each(t, function(t, i) {
                e._setOption(t, i)
            })
        },
        _refresh: function() {
            var t = this;
            this.element.val(t.formatTime())
        },
        formatTime: function() {
            var t = this.options.hours,
                e = this.options.minutes;
            return t = 10 > t ? "0" + t : "" + t, e = 10 > e ? "0" + e : "" + e, t + ":" + e
        },
        checkHours: function() {
            this.options.hours > 23 && (this.options.hours = 23)
        },
        checkMinutes: function() {
            this.options.minutes > 59 && (this.options.minutes = 59)
        },
        increaseHours: function() {
            var t = this.options.hours;
            this.options.hours = 23 > t ? t + 1 : 0, this._refresh(), this.selectHours()
        },
        decreaseHours: function() {
            var t = this.options.hours;
            this.options.hours = t > 0 ? t - 1 : 23, this._refresh(), this.selectHours()
        },
        increaseMinutes: function() {
            var t = this.options.minutes;
            this.options.minutes = 59 > t ? t + 1 : 0, this._refresh(), this.selectMinutes()
        },
        decreaseMinutes: function() {
            var t = this.options.minutes;
            this.options.minutes = t > 0 ? t - 1 : 59, this._refresh(), this.selectMinutes()
        },
        enterValue: function(t) {
            switch (this.variables.cursorPosition) {
                case 0:
                    this.options.hours = this.options.hours % 10 + 10 * t, this.variables.cursorPosition = 1, this.checkHours(), this._refresh(), this.selectHours();
                    break;
                case 1:
                    this.options.hours = 10 * parseInt(this.options.hours / 10) + t, this.variables.cursorPosition = 3, this.checkHours(), this._refresh(), this.selectMinutes();
                    break;
                case 3:
                    this.options.minutes = this.options.minutes % 10 + 10 * t, this.variables.cursorPosition = 4, this.checkMinutes(), this._refresh(), this.selectMinutes();
                    break;
                case 4:
                    this.options.minutes = 10 * parseInt(this.options.minutes / 10) + t, this.variables.cursorPosition = 3, this.checkMinutes(), this._refresh(), this.selectMinutes()
            }
        },
        deleteHours: function() {
            this.options.hours = 0, this._refresh(), this.selectHours()
        },
        deleteMinutes: function() {
            this.options.minutes = 0, this._refresh(), this.selectMinutes()
        },
        selectHours: function() {
            this.element[0].setSelectionRange(0, 2)
        },
        selectMinutes: function() {
            this.element[0].setSelectionRange(3, 5)
        },
        _destroy: function() {
            this.element.removeClass("wz-timepicker").val("")
        }
    })
});