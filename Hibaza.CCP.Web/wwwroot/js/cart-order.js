//$(document).ready(function () {
//    getCart();
//});
var message_link = "";
function getCart(customer_id) {
    try {
        if (_customer_id == null || _customer_id == undefined || _customer_id == "") {
            console.log("customer not null");
            return;
        }
        $(".cart-body").empty();
        $(".order-body").empty();
        //get cart
        var config = {
            mongoconnect: "ConnChat",
            mongodb: "ChatDb",
            collectionname: "Carts",
            type: "search"
        };

        var projection = { _id: 0 };
        var para = { customer_id: _customer_id };
        var json = {
            config: JSON.stringify(config), para: JSON.stringify(para), limit: 50,
            projection: JSON.stringify(projection), order: JSON.stringify({ desc: "timestamp" })
        };
        ExecuteChatBotSyns(json, baseUrls_ApiAi + "api/procedure/execute", "POST", "getCart_result", true);
        getOrder(_customer_id);
    } catch (e) { console.log(e); }
}

function getOrder(customer_id) {
    try {
        //get order
        var config = {
            mongoconnect: "ConnChat",
            mongodb: "ChatDb",
            collectionname: "Orders",
            type: "search"
        };

        var projection = { _id: 0 };
        var para = { customer_id: _customer_id };
        var json = {
            config: JSON.stringify(config), para: JSON.stringify(para), limit: 50,
            projection: JSON.stringify(projection), order: JSON.stringify({ desc: "timestamp" })
        };
        ExecuteChatBotSyns(json, baseUrls_ApiAi + "api/procedure/execute", "POST", "getCart_result", true);

    } catch (e) { console.log(e); }
}

function getPhone(customer_id) {
    try {
        // load tiep tu so dien thoai . xem tren web co dat don khong
        var phone = $(".phoneinfo").attr("phone");
        if (phone == null || phone == undefined || phone == "") {
            var info = {
                async: true,
                customer_id: _customer_id
            };
            upsertDocoments({ $set: info }, { customer_id: _customer_id }, "Orders", "upsert", "", "ConnChat");
            return;
        }
        var url = baseUrls_ApiOrder + "api/order/search?logonId=" + $(".js-agent-input").val() + "&imei=" + _customer_id + "&key=" + phone + "&token=@bazavietnam";
        getPostHtml(null, "GET", url, "getCartWeb_result");


    } catch (e) { console.log(e); }
}

function getCart_result(data, json) {
    try {
        var products = JSON.parse(data);
        var para = JSON.parse(json.config);
        if (products == null || products == undefined || products.length == 0) {
            if (para.collectionName == "Orders") {
                getPhone(_customer_id);

                var info = {
                    async: true,
                    customer_id: _customer_id
                };
                upsertDocoments({ $set: info }, { customer_id: _customer_id }, "Orders", "upsert", "", "ConnChat");
            }
            return;
        }
        drowCart(products, para.collectionName);

    } catch (e) { console.log(e); }
}

function getCartWeb_result(data, jsonpara) {
    try {
        var phone = $(".phoneinfo").attr("phone");
        if (phone == null || phone == undefined || phone == "")
            return;

        $.each(data, function (k, v) {
            var url = baseUrls_ApiOrder + "api/order/detail2?id=" + v.OrderId + "&logonId=" + phone + "&imei=&token=@bazavietnam";
            getPostHtml(null, "GET", url, "getCartDetailWeb_result");
        });
    } catch (e) { console.log(e); }
}

function getCartDetailWeb_result(data, jsonpara) {
    try {
        if (data == null || data == undefined || data.OrderItems == null || data.OrderItems == undefined || data.OrderItems == 0)
            return;
        var para = convertOrderWebToDb01(data);
        if (para == null || para == undefined || para.orderid == null || para.orderid == undefined)
            return;
        para.id = para.orderid;
        para._id = para.orderid;
        para.async = true;
        upsertDocoments({ $set: para }, { orderid: para.orderid }, "Orders", "upsert", "", "ConnChat");

        var product_sku = "";
        $.each(para.products, function (k, v) {
            product_sku += v.product_sku + ","
        });
        $(".order-body").append(
            '<div class="form-group col-sm-12 cart-orderid order-height" orderid="' + para.orderid + '" order_status_int ="' + para.order_status_int + '">' +//onclick="getInfoOrder(this)"

            '<input type="checkbox" class="col-sm-1 checkOrder">' +
            '<span  class="col-sm-4 cart-product">' + (product_sku.substring(0, product_sku.length - 1)) + '</span>' +
            '<span  class="col-sm-4 cart-status">' + para.order_status + '</span>' +//data.OrderStatus
            '<span  class="col-sm-3 cart-created_time">' + formatShortDate(dateFromTimestamp(para.timestamp)) + '</span>' +
            '</div>'
        );

        resizeRow();

    } catch (e) { console.log(e); }
}


function sendToCart() {
    try {
        if (_customer_id == null || _customer_id == undefined || _customer_id == "") {
            console.log("customer not null");
            return;
        }
        var ar = [];
        var sum_sellprice = 0;
        var message_id = "";

        $(".js-list-chat li").each(function (k, v) {
            var author = $(v).attr("data-author");
            if (author != null && author != undefined && author != "" && author.indexOf("contact") >= 0) {
                message_id = $(v).attr("data-key");
            }
        });
        if (message_id != null && message_id != undefined && message_id != "") {
            var url = baseUrls_Api + "messages/openlink/" + $(".js-business-input").val() + "?item_id=" + message_id + "&access_token=";
            var link = getPostHtml(null, "GET", url, "");
            message_link = link.data;
        }

        $(".viewSelect").each(function (i, thiss) {
            try {
                var checked = $($(thiss).find('.icon-p-liked')).length;
                if (checked > 0) {
                    var item = JSON.parse(str_replace_all($(thiss).attr("item"), "'", "\""));

                    var config = {
                        mongoconnect: "ConnAi",
                        mongodb: "AiDb",
                        collectionname: "Products",
                        type: "search"
                    };
                    // get product
                    var projection = { "attributes": 1, "product": 1, productid: 1, "fullname": 1, "websiteurl": 1, "imageurl": 1, "sellprice": 1, "color": 1, _id: 0 };
                    var para = { "product.value": item.product };
                    var json = { config: JSON.stringify(config), para: JSON.stringify(para), limit: 1, projection: JSON.stringify(projection) };
                    var rs = ExecuteChatBotSyns(json, urlServiceAi + "api/procedure/execute", "POST", "", true);
                    var products = JSON.parse(rs);
                    if (products == null || products == undefined || products.length == 0) {
                        alert("Error");
                        return;
                    }

                    var val = products[0];

                    var it = {
                        price: parseInt(val.sellprice.value),
                        product_attribute: val.attributes.desc,
                        product_id: val.productid.value,
                        product_name: val.fullname.desc,
                        product_photo_url: val.imageurl.value,
                        product_sku: val.product.value,
                        product_type: 15,
                        quantity: 1,
                        total_money: parseInt(val.sellprice.value)
                    };
                    ar.push(it);
                    sum_sellprice = sum_sellprice + parseInt(val.sellprice.value);
                }
            } catch (e) { console.log(e); }
        });

        var config = {
            mongoconnect: "ConnChat",
            mongodb: "ChatDb",
            collectionname: "Customers",
            type: "search"
        };
        // get product
        var projection = { _id: 0, phone: 1, name: 1, email: 1, city: 1, address: 1, app_id: 1, weight: 1, height: 1 };
        var para = { id: _customer_id };
        var json = {
            config: JSON.stringify(config), para: JSON.stringify(para), limit: 1,
            projection: JSON.stringify(projection)
        };
        var rs = ExecuteChatBotSyns(json, baseUrls_ApiAi + "api/procedure/execute", "POST", "", true);
        var customers = JSON.parse(rs);
        if (customers == null || customers == undefined || customers.length == 0) {
            new NotificationPlugin("Ok", "Error to cart....", "icon-bell");
            return;
        }

        var timestamp = stringDateToTimestamp(getDateNow());
        var paraInsert = {
            timestamp: timestamp,
            created_time: getDateNow(),
            agent_id: $(".js-agent-input").val(),
            customer_id: _customer_id,
            link: message_link,
            coupon_code: "",
            coupon_name: "",
            discount_value: 0,
            imei: "",
            logon_id: $(".js-agent-input").val(),
            message: "Giao dịch chờ thanh toán",
            note: "",
            orderid: timestamp + "",
            products: ar,

            order_number: timestamp,
            order_status: "Trong giỏ hàng",
            order_status_int: 30,
            origin: "bot",
            payment_option: 0,
            payment_status: "Chưa thanh toán",
            payment_status_int: 0,
            payment_url: "",
            return_url: "",
            shipping_cost: 0,
            subtotal_money: sum_sellprice,
            success: true,
            total_money: sum_sellprice,
            trans_type: "Goods",
            transact_status: "Trong giỏ hàng",
            transact_status_int: 30,

            fullname: customers[0].name,
            mobiphone: customers[0].phone,
            address: customers[0].address,
            email: customers[0].email,
            note: ("weight: " + customers[0].weight + ",height: " + customers[0].height + " (" + $("#wz-dropdown-operator").attr("username") + ")"),
            province: customers[0].city,
            async: true
        };
        paraInsert.id = paraInsert.orderid;
        paraInsert._id = paraInsert.orderid;
        if (paraInsert.orderid == null || paraInsert.orderid == undefined ||
            paraInsert.products == null || paraInsert.products == undefined || paraInsert.products.length == 0 ||
            paraInsert.products[0].product_id == null || paraInsert.products[0].product_id == undefined) {
            new NotificationPlugin("Ok", "Error to cart.", "icon-bell");
            return;
        }
        var rs = upsertDocoments(paraInsert, { orderid: paraInsert.orderid }, "Carts", "insert", "", "ConnChat");
        //drowCart(paraInsert);
        getCart(_customer_id);
        if (rs == "[]")
            new NotificationPlugin("Ok", "Success to cart", "icon-bell");
        else {
            new NotificationPlugin("Ok", "Error to cart..", "icon-bell");
        }
        
    } catch (e) { console.log(e); new NotificationPlugin("Ok", "Error to cart...", "icon-bell"); }
}

function drowCart(data, documentName) {
    try {

        if (data == null || data == undefined || data.length == 0) {
            $(".cart-btn").addClass("hidden");
            return;
        }
        else
            $(".cart-btn").removeClass("hidden");
        $.each(data, function (key, val) {
            if (val.products != null && val.products != undefined) {
                var product = "";
                $.each(val.products, function (k, v) {
                    product += v.product_sku + ","
                });

                $(val.order_status_int == 30 ? ".cart-body" : ".order-body").append(
                    '<div class="form-group  col-sm-12 cart-orderid ' + (val.order_status_int == 30 ? "cart-height" : "order-height") + '" orderid="'
                    + val.orderid + '" order_status_int = "' + val.order_status_int + '">' +//onclick="getInfoOrder(this)"

                    '<input type="checkbox" class="col-sm-1 checkOrder">' +
                    '<span  class="col-sm-4 cart-product">' + (product.substring(0, product.length - 1)) + '</span>' +
                    '<span  class="col-sm-4 cart-status">' + val.order_status + '</span>' +
                    '<span  class="col-sm-3 cart-created_time">' + formatShortDate(dateFromTimestamp(val.timestamp)) + '</span>' +

                    '</div>'
                );
            }
            resizeRow();
        });
        resizeRow();

        //'<a class="btn btn-warning js-submit-save-shop"  onclick="orderSaveToShop()">< i class="fa fa-plus" ></i > Save to shop </a >' +
    } catch (e) { console.log(e); }
}

function getInfoOrder(thiss) {
    try {
        var orderid = $(thiss).attr("orderid");
        var config = {
            mongoconnect: "ConnChat",
            mongodb: "ChatDb",
            collectionname: "Carts",
            type: "search"
        };
        // get product
        var projection = { _id: 0 };
        var para = { customer_id: _customer_id, orderid: orderid };
        var json = {
            config: JSON.stringify(config), para: JSON.stringify(para), limit: 50,
            projection: JSON.stringify(projection), order: JSON.stringify({ desc: "timestamp" })
        };
        ExecuteChatBotSyns(json, baseUrls_ApiAi + "api/procedure/execute", "POST", "getInfoOrder_result", true);

    } catch (e) { console.log(e); }
}
function getInfoOrder_result(data, json) {
    try {
        if (data == null || data == undefined || data == "" || data == "[]")
            return;
        var datas = JSON.parse(data);
        
        var obj = {
            elements: datas,
            message: ""
        };
        loadImageToDropableZone("facebook", "orderview", obj);
        modalImg.modal("show");

        var marge = 0;
        $(".checkOrder").each(function (k, v) {
            var check = $(v).prop('checked');
            if (check)
                marge++;
        })
        if (marge > 1)
            changeQuantity(null);
    } catch (e) { console.log(e); }
}

function orderApply() {
    try {
        if (_customer_id == null || _customer_id == undefined|| _customer_id == "") {
            console.log("customer not null");
            return;
        }
        modalImg.modal("hide");
        var paraUpsertNew = convertOrderAdminCart();
        paraUpsertNew.async = true;
        if (paraUpsertNew.orderid == null || paraUpsertNew.orderid == undefined || paraUpsertNew.orderid == "")
            return;
        //# ket thuc lay ra san pham do
        var rs = null;
        if (paraUpsertNew.order_status_int != 30) {
            paraUpsertNew.id = paraUpsertNew.orderid;
            paraUpsertNew._id = paraUpsertNew.orderid;
            rs = upsertDocoments({ $set: paraUpsertNew }, { orderid: paraUpsertNew.orderid }, "Orders", "upsert", "", "ConnChat");
            rs = upsertDocoments({}, { orderid: paraUpsertNew.orderid }, "Carts", "delete", "", "ConnChat");
        }
        else {
            paraUpsertNew.timestamp = stringDateToTimestamp(getDateNow());
            paraUpsertNew.created_time = getDateNow();
            paraUpsertNew.orderid = stringDateToTimestamp(getDateNow()) + "";
            paraUpsertNew.id = paraUpsertNew.orderid;
            paraUpsertNew._id = paraUpsertNew.orderid;
            rs = upsertDocoments(paraUpsertNew, { orderid: paraUpsertNew.orderid }, "Carts", "insert", "", "ConnChat");
        }
        updateCustomerInfo(paraUpsertNew.address, paraUpsertNew.email, paraUpsertNew.city, paraUpsertNew.customer_id, $(".js-business-input").val());
        // load lai san pham
        getCart(_customer_id);
        if (rs == "[]")
            new NotificationPlugin("Ok", "Success update", "icon-bell");
        else
            new NotificationPlugin("Error", "Error update", "icon-bell");
        defaultCart();

    } catch (e) { console.log(e); }
}

function orderPrivew() {
    try {
        $(".js-channel-format-chat").val("orderview");
        var query = "";
        var order_status_int = 30;
        //{$and:[{products: {$elemMatch: {product:'ax003-02'}}}]}
        $(".checkOrder").each(function (k, v) {
            var check = $(v).prop('checked');
            if (check) {
                var orderid = $(v).parent().attr("orderid");
                order_status_int = parseInt($(v).parent().attr("order_status_int"));
                query += "{\"orderid\": \"" + orderid + "\"},";
            }
        });

        var config = {
            mongoconnect: "ConnChat",
            mongodb: "ChatDb",
            collectionname: order_status_int == 30 ? "Carts" : "Orders",
            type: "search"
        };
        // get product
        var projection = { _id: 0 };
        var para = "{ \"$and\": [{ \"customer_id\": \"" + _customer_id + "\"}, {\"$or\":[" + query + "]}]}";
        var json = {
            config: JSON.stringify(config), para: para, limit: 50,
            projection: JSON.stringify(projection), order: JSON.stringify({ desc: "timestamp" })
        };
        ExecuteChatBotSyns(json, baseUrls_ApiAi + "api/procedure/execute", "POST", "getInfoOrder_result", true);
        
    } catch (e) { console.log(e); }
}

function convertOrderAdminCart() {
    try {
        var elements = postOrderToShop(_customer_id);
        // get product
        var t = convertOrderWebToDb01(elements);
        return t;
    } catch (e) { console.log(e); }
}

function convertOrderWebToDb01(elements) {
    try {
        var array = [];
        var paraUpsertNew = null;
        $.each(elements.OrderItems, function (kk, vv) {
            var item = {
                price: parseInt(replaceDotAndVND(vv.Price)),
                product_attribute: vv.ProductAttribute,
                product_id: vv.ProductId,
                product_name: vv.ProductName,
                product_photo_url: vv.ProductPhotoUrl,
                product_sku: vv.ProductSKU,
                product_type: 15,
                quantity: parseInt(vv.Quantity),
                total_money: parseInt(replaceDotAndVND(vv.TotalMoney)),
            };
            array.push(item);
        });

        if (paraUpsertNew == null && (elements.OrderId != null || elements.OrderNumber != null)) {
            var timestamp = elements.CreatedDate == null || elements.CreatedDate == undefined ? stringDateToTimestamp(getDateNow()) :
                stringDateToTimestamp(formartTime(elements.CreatedDate));
            paraUpsertNew = {
                timestamp: timestamp,
                agent_id: elements.LogonId,
                customer_id: _customer_id,
                coupon_code: elements.CouponCode,
                coupon_name: elements.CouponName,
                created_time: elements.CreatedDate == null || elements.CreatedDate == undefined ? getDateNow() : formartTime(elements.CreatedDate),
                discount_value: parseInt(replaceDotAndVND(elements.DiscountValue)),
                imei: elements.Imei,
                logon_id: elements.LogonId,
                message: elements.Message,
                note: elements.Note,
                orderid: elements.OrderId == null && elements.OrderId == undefined ? elements.OrderNumber : elements.OrderId,
                products: array,

                order_number: elements.OrderNumber,
                order_status: elements.OrderStatus,
                order_status_int: elements.OrderStatusInt,
                origin: elements.Origin,
                payment_option: elements.PaymentOption,
                payment_status: elements.PaymentStatus,
                payment_status_int: elements.PaymentStatusInt,
                payment_url: elements.PaymentUrl,
                return_url: elements.ReturnUrl,
                shipping_cost: parseInt(replaceDotAndVND(elements.ShippingCost)),
                subtotal_money: parseInt(replaceDotAndVND(elements.SubTotalMoney)),
                success: elements.Success,
                total_money: parseInt(replaceDotAndVND(elements.TotalMoney)),
                trans_type: elements.TransType,
                transact_status: elements.TransactStatus,
                transact_status_int: elements.TransactStatusInt,

                fullname: elements.Fullname,
                mobiphone: elements.Mobiphone,
                address: elements.Address,
                email: elements.Email,
                note: elements.Note,
                province: elements.Province,
                link: elements.Link
            }
        }
        return paraUpsertNew;
    } catch (e) { console.log(e); }
}

function resizeRow() {
    reziseButton01($(".war-cart-body"));
    reziseButton01($(".war-order-body"));
    $(".order-body").height(($(".order-height").size() * ($(".order-height").height() + 5)) + $(".order-height").height() + 10);
    $(".cart-body").height(($(".cart-height").size() * ($(".cart-height").height() + 5)) + $(".cart-height").height() + 10);    
}
function reziseButton01(element) {
    try {
        var warbot = $(document).height();
        element.height(warbot - ($(".wz-col-sideright section footer").height() + 160));
    } catch (e) { console.log(e); }
}

function updateCustomerInfo(address, email, city, customer_id, business_id) {
    try {
        var info = {
            address: address,
            email: email,
            city: city
        };

        upsertDocoments({ $set: info }, { id: _customer_id, business_id: business_id }, "Customers", "upsert", "", "ConnChat");
        upsertDocoments({ $set: info }, { id: _customer_id, business_id: business_id }, "CustomerInfo", "upsert", "", "mongoDbAi");
    } catch (e) { console.log(e); }
}

function changeQuantity(thiss) {
    try {
        var sum_Price = 0;
        $(".js-receipt-price").each(function (k, v) {
            var quantity = parseInt($(v).parent().find(".js-receipt-quantity").val());
            var price = parseInt($(v).parent().find(".js-receipt-price").val());
            sum_Price = sum_Price + (price * quantity);
            $(v).parent().find(".js-receipt-total_money").val(price * quantity);
        });
        $(".js-receipt-subtotalmoney").val(sum_Price);
        $(".js-receipt-shipping-cost").val(sum_Price > 500000 ? "0" : "30000");
        sum_Price = (sum_Price + parseInt($(".js-receipt-shipping-cost").val())) - parseInt($(".js-receipt-discountvalue").val())
        $(".js-receipt-total-cost").val(sum_Price);
    } catch (e) { console.log(e); }
}