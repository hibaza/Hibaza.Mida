DashboardManager = function() {
    this.params = {}, this.charts = [], this.xhrRequests = [], this.$container = !1, this.config = {}, this.cSel = "#wz-modal-reports .js-reports-table", this.localStorageKey = "dashboardManagerSummary", this.topTenAgents = []
};

DashboardManager.prototype.getStorage = function () {
    var t = localStorage.getItem(this.localStorageKey);
    if (t) {
        var e = t.split(",");
        if (3 == e.length) return e;
        t = !1
    }
    if (!t) {
        var i = "";
        return $.each(this.config, function(t, e) {
            "summary" == e.section && e["default"] && (i.length > 0 && (i += ","), i += t)
        }), localStorage.setItem(this.localStorageKey, i), i.split(",")
    }
}, DashboardManager.prototype.getChartURL = function(t) {
    var e = new URL(self.urls.brands.reports + t + "/" + businessID);
    return e.loadDataParams(this.params), e.getURL()
}, DashboardManager.prototype.setParams = function(t) {
    var e = this;
    $.each(t, function(t, i) {
        e.params[t] = i
    })
}, DashboardManager.prototype.setStorage = function(t, e) {
    var i = localStorage.getItem(this.localStorageKey);
    if (i) {
        this.loadChart(e, this.config[e], t);
        var n = i.split(","),
            o = n.indexOf(t);
        n[o] = e, localStorage.setItem(this.localStorageKey, n.join(",")), this.loadSummaryDropdown(n.join(","))
    }
}, DashboardManager.prototype.deleteParams = function(t) {
    var e = this;
    $.each(t, function(t, i) {
        "undefined" != typeof e.params[i] && delete e.params[i]
    })
}, DashboardManager.prototype.destroyCharts = function() {
    for (var t = 0; t < this.charts.length; t++) {
        try {
            $.each(this.charts[t].data.datasets, function(t, e) {
                e.data = []
            })
        } catch (e) {
            console.log("dashboardManager: Error  clearing chart dataset " + t)
        }
        this.charts[t].destroy(), this.xhrRequests[t] && 4 != this.xhrRequests[t].readyState
    }
    this.charts = [], this.xhrRequests = []
}, DashboardManager.prototype.load = function() {
    this.$container = $(this.cSel), this.config = DASHBOARD, this.params.init && this.params.finish ? this.params = {
        date: "range",
        init: this.params.init,
        finish: this.params.finish
    } : this.params = {}, this.params.agent = this.topTenAgents, this.destroyCharts(), this.loadCharts()
}, DashboardManager.prototype.reset = function() {
    this.reload()
}, DashboardManager.prototype.reload = function() {
    this.destroyCharts(), this.loadCharts()
}, DashboardManager.prototype.loadCharts = function() {
    var t = this;
    this.loadDOM();
    var e = moment(this.params.finish, "x").diff(moment(this.params.init, "x"), "days");
    1 >= e ? this.deleteParams(["period"]) : 14 >= e ? this.setParams({
        period: "day"
    }) : 31 >= e ? this.setParams({
        period: "week"
    }) : this.setParams({
        period: "month"
    });
    var i = this.getStorage(),
        n = 0;
    $.each(i, function(e, i) {
        var o = t.config[i];
        o ? (n += 1, t.loadChart(i, o)) : (console.log("dashboardManager: Error loading chart " + i), localStorage.removeItem(t.localStorageKey))
    }), n > 0 ? $("#wz-modal-reports .wz-dashboard-summary").show() : $("#wz-modal-reports .wz-dashboard-summary").hide(), this.loadSummaryDropdown(i);
    var o = 0;
    $.each(this.config, function(e, i) {
        "main" == i.section && (o += 1, t.loadChart(e, i))
    }), o > 0 ? ($("#wz-modal-reports .wz-dashboard-main").show(), $("#wz-modal-reports .wz-dashboard-summary").removeClass("expanded")) : ($("#wz-modal-reports .wz-dashboard-main").hide(), $("#wz-modal-reports .wz-dashboard-summary").addClass("expanded"))
}, DashboardManager.prototype.loadChart = function(t, e, i) {
    var n, o = this,
        a = $(".js-template-reports-" + e.section).tmpl({
            id: t,
            title: e.title,
            details: e.details
        }),
        r = 0,
        s = [],
        l = e.datasets,
        c = e.options,
        d = 0;
    a.find(".wz-chart-error").hide(), a.find(" .wz-chart-loader").show(), a.find(".js-dashboard-details").hide(), i ? (r = this.getStorage().indexOf(i), $.each(this.charts[r].data.datasets, function(t, e) {
        e.data = []
    }), this.charts[r].destroy(), $old = this.$container.find("#dS-" + i), $old.before(a), $old.remove(), n = this.xhrRequests[r] = $.get(this.getChartURL(t))) : (o.$container.find(".wz-dashboard-" + e.section).append(a), n = this.xhrRequests[this.xhrRequests.length] = $.get(this.getChartURL(t))), n.done(function(t) {
        $.each(t.lines, function(e) {
            s.push(t.lines[e].name), $.each(l, function(i) {
                var n = t.lines[e].data[i];
                n > d && (d = n), l[i].data.push(n)
            })
        }), 10 > d ? c.scales.yAxes[0].ticks.fixedStepSize = 1 : 50 > d ? c.scales.yAxes[0].ticks.fixedStepSize = 5 : 100 > d ? c.scales.yAxes[0].ticks.fixedStepSize = 10 : c.scales.yAxes[0].ticks.fixedStepSize = 25, "summary" == e.section ? c.scales.yAxes[0].ticks.suggestedMax = d + .3 * d : d % c.scales.yAxes[0].ticks.fixedStepSize === 0 && (c.scales.yAxes[0].ticks.suggestedMax = d + .1 * d);
        var n = a.find("canvas");
        Chart.defaults.global.animation.onProgress = function() {
            var t = a.find(".wz-chart-loader"),
                e = a.find(".js-dashboard-details");
            t.fadeOut(100, function() {
                e.fadeIn(100)
            })
        };
        var u = {
            type: e.type,
            data: {
                labels: s,
                datasets: l
            },
            options: e.options
        };
        e.unit !== !1 && a.find("h1").text(t.total + e.unit), i ? o.charts[r] = new Chart(n, u) : o.charts.push(new Chart(n, u))
    }), n.fail(function() {
        a.find(".wz-chart-loader").fadeOut(200, function() {
            a.find("canvas").remove(), a.find(".wz-chart-error").show()
        })
    })
}, DashboardManager.prototype.loadDOM = function() {
    var t = $(".js-template-reports-dashboard");
    this.$container.empty(), this.$container.append(t.tmpl())
}, DashboardManager.prototype.loadSummaryDropdown = function(t) {
    var e = this.$container.find(".js-summary-dropdown");
    e.empty(), $.each(this.config, function(i, n) {
        "summary" == n.section && -1 == t.indexOf(i) && e.each(function(t, e) {
            $(this).append($(".js-template-reports-dropdown").tmpl({
                name: n.title,
                old: $(this).data("chart"),
                id: i
            }))
        })
    })
}, DashboardManager.prototype.dropdownClickHandler = function(t) {
    var e = $(t).data("old"),
        i = $(t).data("new");
    dashboardManager.setStorage(e, i)
}, DashboardManager.prototype.detailsClickHandler = function(t) {
    var e = $(t).data("details");
    $('#wz-modal-reports .wz-crm-menu li[data-menu="' + e + '"] a').trigger("click")
}, DashboardManager.prototype.abortXHR = function() {
    for (var t = 0; t < this.xhrRequests.length; t++) this.xhrRequests[t] && 4 != this.xhrRequests[t].readyState
};
