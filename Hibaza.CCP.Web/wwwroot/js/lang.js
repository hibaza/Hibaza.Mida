
function readFileLang(lang, page) {
    try {
        if (lang == null) {
            localStorage.setItem("lang", "en");
            lang = "en";
        }
        if (page == null || page == undefined)
            return;

        $.getJSON("./json/lang/" + page + ".json", function (obj) {
            if (obj == null || obj == undefined)
                return;
            var item = obj[lang];
            $.each(item, function (key, value) {
                if (value.type == "text") {                   
                    $(".lg-" + page  +" .lg-" + key).html(value.text);
                }
            });
        });

    } catch (e) { console.log(e); return []; }
}