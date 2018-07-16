var timeLastReport = 0;
ReportManager = function () {
    this.reportsView = !1, this.chart = !1, this.chartId = !1, this.$chart = !1, this.xhrChart = !1, this.$table = !1, this.cfgTable = {
        fit: !0
    }, this.xhrTable = !1, this.tableHeight = "calc(100vh - 30vh - 30px - 202px)", this.params = {}, this.config = {}, this.cSel = "#wz-modal-reports .js-reports-table", this.tSel = "#wz-reports-table", this.chartLoaded = !1, this.page = 0, this.timeoutOldRecords = !1, this.errorMessageTimeout = !1, this.lastScroll = 0, this.lastRowsPerPage = 0, this.magicNumber = 1, this.topTenAgents = {
        lines: []
    }, this.topTenCustomers = {
        lines: []
    }
};

ReportManager.prototype.getTableURL = function () {
    var t = new URL(self.urls.brands.reports + this.reportsView + "/" + businessID);
    return t.loadDataParams(this.params), t.getURL()
}, ReportManager.prototype.getChartURL = function() {
    var t = this.getChartConfig(),
        e = new URL(self.urls.brands.reports + this.reportsView + "_" + t.type + "/" + businessID);
    return e.loadDataParams(this.params), e.getURL()
}, ReportManager.prototype.getChartConfig = function() {
    if (!this.chartId) return this.chartId = this.config.charts[0].id, this.config.charts[0];
    for (var t = 0; t < this.config.charts.length; t++)
        if (this.config.charts[t].id == this.chartId) return this.config.charts[t]
}, ReportManager.prototype.getTotalPages = function() {
    return this.$table && this.$table.length > 0 ? this.$table.data("pages") : 0
}, ReportManager.prototype.getTotalRows = function() {
    return this.$table && this.$table.length > 0 ? this.$table.data("rows") : 0
}, ReportManager.prototype.getReportHeight = function() {
    var t = this.$table.find(".tabulator-table"),
        e = t.find(".wz-spacer").outerHeight() + t.outerHeight(),
        i = t.find(".wz-preloader");
    return "none" != i.css("display") && (e += i.outerHeight()), e
}, ReportManager.prototype.getTop = function() {
    var t = this.$table.find(".tabulator-tableHolder"),
        e = t.scrollTop() + t.first().outerHeight();
    return e >= this.getReportHeight() - t.height() && (e = this.getReportHeight() - t.height() - 5), e
}, ReportManager.prototype.setParams = function(t) {
    var e = this;
    $.each(t, function(t, i) {
        e.params[t] = i
    })
}, ReportManager.prototype.deleteParams = function(t) {
    var e = this;
    $.each(t, function(t, i) {
        "undefined" != typeof e.params[i] && delete e.params[i]
    })
}, ReportManager.prototype.clearChartData = function() {
    if (this.chart) try {
        $.each(this.chart.data.datasets, function(t, e) {
            e.data = []
        })
    } catch (t) {
        console.log("reportManager: Error  clearing chart dataset " + i)
    }
}, ReportManager.prototype.destroyChart = function() {
    this.chart && (this.clearChartData(), this.chart.stop(), this.chart.destroy(), this.chart = !1, this.chartId = !1, this.$chart = !1, this.xhrChart && 4 != this.xhrChart.readyState && this.xhrChart.abort())
}, ReportManager.prototype.init = function(t, e) {
    if (!e) throw new Error("ReportManager.init(): Second arg must be a config object.");
    this.reportsView = t, this.config = e, this.reset()
    }, ReportManager.prototype.reset = function (t) {
    if (this.$container = $(this.cSel), this.destroyChart(), this.$table = !1, this.chartId = !1, this.page = 1, this.lastScroll = 0, !t && this.params.init && this.params.finish) {
        var i = getDateRange();
        if (this.params = {
            date: "range",
            init: i.init,
            finish: i.finish
                //init: this.params.init,
                //finish: this.params.finish
            }, this.config.filter && "period" == this.config.filter.field) {
            var e = moment(manager.params.finish, "x").diff(moment(manager.params.init, "x"), "days");
            this.chartId = "default", 1 >= e ? (this.deleteParams(["period"]), this.chartId = "total") : 14 >= e ? this.setParams({
                period: "day"
            }) : 31 >= e ? this.setParams({
                period: "week"
            }) : this.setParams({
                period: "month"
            })
        }
    } else {
        var i = getDateRange();
        localStorage.setItem("reportsDateRange", i.id), this.params = {
            date: "range",
            init: i.init,
            finish: i.finish
        }, setDateValues(i)
    }
    this.load()
}, ReportManager.prototype.reload = function() {
    this.$container = $(this.cSel), this.destroyChart(), this.$table.empty(), this.load()
}, ReportManager.prototype.load = function() {
    var t = this;
    t.loadDOM(), t.headerUI(), t.loadTable(), t.loadChart()
}, ReportManager.prototype.loadChart = function() {
    var t = this;
    t.$container.find(".wz-chart-error").hide(), t.$container.find(".wz-chart-loader").show();
    var e = this.getChartConfig(),
        i = [],
        n = [],
        o = e.options;
    return t.xhrChart = $.get(this.getChartURL()).done(function(a) {
        if (t.chart && t.clearChartData(), n = e.datasets, "tickets" == t.reportsView && "default" == t.chartId) {
            var r = a.lines.length.toString();
            t.magicNumber = Math.round(r / 10) + 1, o.scales.xAxes[0].ticks.userCallback = function(t, e) {
                return e % reportManager.magicNumber === 0 ? t : ""
            }
        } else o.scales.xAxes[0].ticks.userCallback = function(t, e) {
            return t
        };
        $.each(a.lines, function(t) {
            a.lines[t] && (i.push(a.lines[t].name || ""), $.each(n, function(e) {
                var i = a.lines[t].data[e];
                n[e].data.push(i)
            }))
        }), t.$chart = t.$container.find("canvas"), t.chartLoaded = !1, Chart.defaults.global.animation.onProgress = function() {
            t.chartLoaded || (t.$container.find(".wz-chart-loader").fadeOut(100), t.chartLoaded = !0)
        }, t.chart ? (t.chart.data.labels = i, t.chart.data.datasets = n, t.chart.update()) : t.chart = new Chart(t.$chart, {
            type: e.type,
            data: {
                labels: i,
                datasets: n
            },
            options: o
        })
    }).fail(function() {
        t.$container.find(".wz-chart-loader").fadeOut(100), t.$container.find(".wz-chart-error").show()
    }), t.xhrChart
}, ReportManager.prototype.loadTable = function() {
    var t = this;
    t.$table = $(t.tSel), t.$container.find(".js-report-nodata").hide(), t.$container.find(".wz-report-loader").show();
    var e = {
        columns: t.config.columns,
        height: t.tableHeight,
        rowClick: t.rowClickHandler.bind(t),
        persistentLayoutID: "reportManager_" + t.reportsView,
        fitColumns: t.cfgTable.fit,
        showLoader: !1,
        sortable: !0,
        dataSorting: function(e, i) {
            return t.$container.find(".wz-report-loader").fadeIn(200, function() {
                var n = "asc" == i ? "" : "-";
                t.setParams({
                    order: n + e[0].field.field
                }), t.loadTable()
            }), !1
        },
        dataLoaded: function(e) {
            t.$container.find(".wz-report-loader").fadeOut(100), 0 == e.length && (t.$container.find(".js-search-apply").addClass("hide"), t.$container.find(".js-report-nodata").show())
        },
        renderComplete: function(e, i) {
            t.tableUI()
        }
    };
    t.cfgTable.fit ? e.persistentLayout = !1 : e.persistentLayout = !0, "payments" == t.reportsView ? e.selectable = !1 : e.selectable = 1, t.setParams({
        page: this.page        
    }), t.xhrTable = $.get(this.getTableURL(), function(i) {
        if (i.data) {
            var last = 0;
            var last1 = 0;
            try {
                last = i.data[i.data.length - 1].date_registered;
                if (last != null && last != undefined)
                    t.setParams({
                        finish: last
                    });
            } catch (ex) { }
            try {
                last1 = i.data[i.data.length - 1].created_at;
                if (last1 != null && last1 != undefined)
                    t.setParams({
                        finish: last1
                    });
            } catch (exx) { }
            t.$table.tabulator(e), t.lastRowsPerPage = i.data.length, t.$table.tabulator("setData", i.data);
            var n = t.$table.find(".tabulator-tableHolder");
            n.off("scroll", t.loadMore.bind(t)), n.on("scroll", t.loadMore.bind(t))
        } else t.loadErrorTable()
    }).fail(function() {
        t.loadErrorTable()
    })
}, ReportManager.prototype.loadMore = function() {
    var t = this.$table.find(".tabulator-tableHolder"),
        e = t.scrollBottom() > this.lastScroll,
        i = t.scrollBottom() >= this.getReportHeight();
    if (this.lastScroll = t.scrollBottom(), this.lastRowsPerPage > 0 && i && e && !this.timeoutOldRecords) {
        NProgress.inc(), t.find(".tabulator-table").append($(".js-template-reports-preloader").tmpl()), t.find(".wz-preloader").fadeIn(800), t.animate({
            scrollTop: this.getTop() + 50
        }, 800, "swing");
        var n = 808;
        this.timeoutOldRecords = setTimeout(function() {
            if (this.lastRowsPerPage > 0 || 1 == this.params.page) {
                var t = this;
                this.loadMoreRecords().done(function() {
                    var e = t.$table.find(".tabulator-tableHolder");
                    e.find(".wz-preloader").fadeOut(500, "swing", function() {
                        this.timeoutOldRecords = !1, NProgress.done(), clearTimeout(this.timeoutOldRecords), e.find(".wz-preloader").remove()
                    }.bind(t))
                }).fail(function() {
                    t.loadErrorTable()
                })
            }
        }.bind(this), n)
    }
}, ReportManager.prototype.loadMoreRecords = function() {
    var t = this;
    return this.setParams({
        page: this.params.page + 1,
        finish: timeLastReport > 0 ? timeLastReport + 1: this.params.finish +1
    }), $.get(this.getTableURL(), function (e) {
        var last = 0;
        var last1 = 0;
        try {
             last = e.data[e.data.length - 1].date_registered;
            if (last != null && last != undefined)
                timeLastReport = last;
        } catch (ex) { }
        try {
            last1 = e.data[e.data.length - 1].created_at;
            if (last1 != null && last1 != undefined)
                timeLastReport = last1;
        } catch (exx) { }
        $.each(e.data, function (e, i) {
            t.$table.tabulator("addRow", i)
        }), t.lastRowsPerPage = e.data.length
    })
}, ReportManager.prototype.loadErrorTable = function() {
    var t = $(".js-template-reports-error");
    this.$table.html(t.tmpl({
        entity: this.reportsView
    })), this.$container.find(".wz-report-loader").hide()
}, ReportManager.prototype.loadFilterTemplate = function(t) {
    var e = t.avatar || "";  - 1 == e.search(cloud.getMediaUrl()) && (e = cloud.getMediaUrl() + t.avatar);
    var i = $(".js-template-reports-advanced-filter").tmpl({
        id: t.id,
        name: t.name,
        avatar: e
    });
    return "customer" == this.config.filter.field && (-1 == e.indexOf("bot.png") ? (i.find(".js-picture").removeClass("hide"), i.find(".js-initials").addClass("hide")) : (i.find(".js-picture").addClass("hide"), i.find(".js-initials").removeClass("hide"), i.find(".js-initials p").text(getInitials(t.name)), i.find(".js-initials").addClass(getAvatarColor(t.id)))), i.find(".js-close").on("click", function(t) {
        stopPropagation(t), $(this).parent().parent().parent().remove(), reportManager.$container.find(".dropdown-filter .js-filter-apply").fadeIn()
    }), i
}, ReportManager.prototype.loadDOM = function() {
    var t = this,
        e = $(".js-template-reports-body"),
        i = $(".js-template-reports-field");
    this.$container.empty(), this.$container.find(".wz-crm-menu li a").removeClass("active"), this.$container.find('.wz-crm-menu li[data-menu="' + this.reportsView + '"] a').addClass("active");
    var n = e.tmpl({
        title: this.config.title || "",
        search: this.params.search || "",
        opened: this.params.search ? "open active" : ""
    });
    if ($.each(this.config.columns, function(t, e) {
            var o = i.tmpl({
                id: e.field,
                name: e.title
            });
            n.find(".js-table-fields").append(o)
        }), this.config.filter) {
        var o = [],
            a = $(".js-template-reports-field-filter").tmpl();
        if (this.config.filter.values) o = this.config.filter.values;
        else switch (this.config.filter.field) {
            case "agent":
                a = $(".js-template-reports-field-filter").tmpl(), o = cloud.searchAgents;
                break;
            case "customer":
                a = $(".js-template-reports-field-filter").tmpl(), o = cloud.searchContacts;
                break;
            case "period":
                a = $(".js-template-reports-period-filter").tmpl({
                    label: this.config.filter.label
                })
        }
        if (a && (a.find(".js-field").text(this.config.filter.label), n.find(".wz-chart-filters").append(a), "period" != this.config.filter.field)) {
            var r = "agent" == this.config.filter.field ? this.topTenAgents : this.topTenCustomers,
                s = [];
            $.each(r.lines, function(e) {
                s.push(r.lines[e].id), n.find(".dropdown-filter .js-filter-values").append(t.loadFilterTemplate({
                    id: r.lines[e].id,
                    name: r.lines[e].name,
                    avatar: r.lines[e].avatar
                }))
            }), t.params[t.config.filter.field] = s, n.find(".js-autocomplete").autocomplete({
                source: o,
                minLength: 1,
                select: function(e, i) {
                    var n = t.$container.find(".dropdown-filter .js-filter-values");
                    if (i.item.id) {
                        var o = t.loadFilterTemplate(i.item),
                            a = n.find(".js-filter-elem.selected").length;
                        a >= ADVANCED_FILTER_LIMIT && o.find("label").removeClass("selected"), t.$container.find(".dropdown-filter .js-filter-values").append(o), t.$container.find(".dropdown-filter .js-filter-apply").fadeIn()
                    }
                    return t.$container.find(".dropdown-filter .js-autocomplete").val("").focus(), e.stopPropagation(), !1
                }
            }).autocomplete("instance")._renderItem = function(t, e) {
                return $("<li>").append($("<i class='wz-margin-r-5 icon-" + e.channel + "'>")).append($("<span>").text(e.value)).appendTo(t)
            }
        }
    }
    this.$container.append(n)
}, ReportManager.prototype.headerUI = function() {
    var t, e = this;
    this.$container.find(".dropdown-fields input").click(function() {
        t = $(this).attr("data-row"), "off" == $(this).attr("data-status") ? ($(this).attr("data-status", "on"), e.$table.tabulator("showCol", t)) : ($(this).attr("data-status", "off"), e.$table.tabulator("hideCol", t))
    });
    var i = this.$container.find(".dropdown-period");
    if (i.size() > 0) {
        var n = i.find(".js-period");
        n.on("click", function(t) {
            var i = $(this).data("period");
            e.destroyChart(), "total" == i ? (e.deleteParams(["period"]), e.chartId = "total") : (e.setParams({
                period: i
            }), e.chartId = "default"), e.loadChart(), e.$container.find(".dropdown-period .dropdown-toggle span").text($(this).text()), e.$container.find(".dropdown-period a.selected").removeClass("selected"), $(this).addClass("selected")
        })
    }
    var i = this.$container.find(".dropdown-filter");
    if (i.length > 0) {
        var o = i.find(".js-filter-values");
        i.on("shown.bs.dropdown", function(t) {
            e.$container.find(".dropdown-filter .js-autocomplete").focus()
        }), i.find(".js-filter-elem").on("click", function(t) {
            i.find(".js-filter-apply").fadeIn()
        }), i.find(".wz-options-icon").on("click", function(t) {
            t.preventDefault(), t.stopPropagation(), i.find(".wz-options").toggle()
        }), i.find(".js-uncheck-all").on("click", function(t) {
            t.preventDefault(), t.stopPropagation(), o.find(".js-filter-elem").removeClass("selected"), i.find(".wz-options").hide(), i.find(".js-filter-apply").fadeIn()
        }), i.find(".js-check-all").on("click", function(t) {
            t.preventDefault(), t.stopPropagation(), o.find(".js-filter-elem").addClass("selected"), i.find(".wz-options").hide(), i.find(".js-filter-apply").fadeIn()
        }), i.find(".js-clear-all").on("click", function(t) {
            t.preventDefault(), t.stopPropagation(), o.find("li").remove(), i.find(".wz-options").hide(), i.find(".js-filter-apply").fadeIn()
        }), i.find(".js-restore").on("click", function(t) {
            o.empty();
            var n = "agent" == reportManager.config.filter.field ? reportManager.topTenAgents : reportManager.topTenCustomers,
                a = [];
            $.each(n.lines, function(t) {
                o.append(e.loadFilterTemplate({
                    id: n.lines[t].id,
                    name: n.lines[t].name,
                    avatar: n.lines[t].avatar
                })), a.push(n.lines[t].id)
            }), e.params[e.config.filter.field] = a, i.find(".dropdown-toggle").addClass("actived"), i.find(".js-field").text(reportManager.config.filter.label), e.loadChart(), i.find(".wz-options").hide(), i.find(".js-filter-apply").fadeOut(100)
        }), i.find(".js-filter-apply").on("click", function(t) {
            var n = o.find(".js-filter-elem.selected").length;
            if (n > 0 && ADVANCED_FILTER_LIMIT >= n) {
                var a = o.find(".js-filter-elem.selected").map(function() {
                    return $(this).data("key")
                }).get();
                e.params[e.config.filter.field] = a, i.find(".dropdown-toggle").addClass("actived"), i.find(".js-field").text(trCustomSelection), e.loadChart(), $(this).fadeOut(100)
            } else n > 0 && (i.find(".js-filter-apply").addClass("vibrate"), i.find(".wz-error").slideDown(200), e.errorMessageTimeout && clearTimeout(e.errorMessageTimeout), e.errorMessageTimeout = setTimeout(function() {
                i.find(".wz-error").slideUp(), i.find(".js-filter-apply").removeClass("vibrate")
            }, 2e3)), stopPropagation(t)
        })
    }
    var a = this.$container.find(".js-report-search"),
        r = a.find("input"),
        s = a.find("span"),
        l = a.find(".icon-cross"),
        c = this.$container.find(".js-search-apply");
    s.text(trSearch + " " + this.config.title.toLowerCase() + "...");
    var d = function(t) {
        var i = $.trim(r.val());
        i != e.params.search && e.$container.find(".wz-report-loader").fadeIn(200, function() {
            r.val(i), e.setParams({
                search: i
            }), e.loadTable()
        }), a.addClass("active")
    };
    a.on("click", function() {
        $(this).addClass("open"), r.focus()
    }), l.on("click", function(t) {
        t.stopPropagation(), e.params.search && e.$container.find(".wz-report-loader").fadeIn(200, function() {
            e.deleteParams(["search"]), e.loadTable()
        }), a.removeClass("open active"), r.val(""), c.addClass("hide")
    }), r.on("keyup", function(t) {
        var i = $.trim($(this).val());
        i != e.params.search ? c.removeClass("hide") : c.addClass("hide")
    }), r.on("keypress", function(t) {
        var e = t.keyCode || t.which;
        13 == e && (t.preventDefault(), t.stopPropagation(), d(t))
    }), c.on("click", d)
}, ReportManager.prototype.rowClickHandler = function(t, e, i, n) {
    switch (this.$table.find(".tabulator-row.selected").removeClass("selected"), this.reportsView) {
        case "tickets":
            this.viewTicketDetail(i), n.addClass("selected");
            break;
        case "agents":
            this.viewAgentDetail(i), n.addClass("selected");
            break;
        case "customers":
            this.viewCustomerDetail(i), n.addClass("selected")
    }
}, ReportManager.prototype.viewCustomerDetail = function(t) {
    currentChat != t.id ? "undefined" != typeof CONTACTS[t.id] ? this.showReportContact(CONTACTS[t.id].getData()) : cloud.getContact(t.id, !0).done(this.showReportContact) : (viewProfileClient(), showSideright())
}, ReportManager.prototype.viewPaymentDetail = function(t) {
    cloud.getPaymentProfile(t.id).done(function() {
        viewProfilePayment(t.id), showSideright()
    })
}, ReportManager.prototype.viewAgentDetail = function(t) {
    cloud.getAgentProfile(t.id).done(function() {
        viewProfileAgent(t.id), showSideright()
    })
}, ReportManager.prototype.viewTicketDetail = function(t) {
    currentChat == t.customer_id ? cloud.getTicketProfile(t.id).done(function() {
        viewProfileTicket(t.id), showSideright()
    }) : viewTicketSelected != t.id && cloud.getTicketProfile(t.id).done(function() {
        firebaseService.setMainChat(t.customer_id), viewProfileTicket(t.id), showSideright()
    })
}, ReportManager.prototype.tableUI = function() {
    var t = this;
    this.$container.find(".js-search-apply").addClass("hide"), tooltips(t.tSel);
    var e = t.$container.find(".dropdown-fields");
    if ($.each(t.config.columns, function(i, n) {
            var o = t.$table.find('.tabulator-cell[data-field="' + n.field + '"]:first'),
                a = "none" != o.css("display");
            e.find('input[data-row="' + n.field + '"]').prop("checked", a).attr("data-status", a ? "on" : "off")
        }), this.config.filter && "period" == this.config.filter.field) {
        var i = this.params.period || "total",
            n = this.$container.find('.js-period[data-period="' + i + '"]');
        this.$container.find(".dropdown-period .dropdown-toggle span").text(n.text()), this.$container.find(".dropdown-period a.selected").removeClass("selected"), n.addClass("selected")
    }
}, ReportManager.prototype.showReportContact = function(t) {
    cloud.loadContact(t), viewProfileClient(), showSideright()
}, ReportManager.prototype.abortXHR = function() {
    this.xhrTable && 4 != this.xhrTable.readyState && this.xhrTable.abort(), this.xhrChart && 4 != this.xhrChart.readyState && this.xhrChart.abort()
}, ReportManager.prototype.chartCustomTooltips = function(t) {
    var e = $("#chartjs-tooltip");
    if (!t.x) return void e.css({
        opacity: 0
    });
    e.removeClass("above below"), e.addClass(t.yAlign);
    var i = "";
    i += ['<div class="chartjs-tooltip-section">', '	<span class="chartjs-tooltip-key" style="background-color:red"></span>', '	<span class="chartjs-tooltip-value">' + t.title + "</span>", "</div>"].join(""), e.html(i), e.css({
        opacity: 1,
        left: t.x + "px",
        top: t.y + "px",
        fontFamily: t.fontFamily,
        fontSize: t.fontSize,
        fontStyle: t.fontStyle
    })
};
