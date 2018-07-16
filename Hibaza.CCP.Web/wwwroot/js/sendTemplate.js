var tagchat = "";
function chat_Template() {
    try {
        var channel_type = $(".js-channel-type-chat").val();
        var channel_format = $(".js-channel-format-chat").val();
        var recipient_id = $(".js-customer-input").val();
        tagchat = $(".js-tag-chat").val();

        var para = {};
        //if (channel_type == "facebook") {
            if (channel_format == "text") {
                para = facebook_OneText(recipient_id);
                $(".js-config-chat").val(JSON.stringify({ template: "text" }));
            }
            if (channel_format == "message") {
                para = facebook_OneTextDefault(recipient_id);
                $(".js-config-chat").val(JSON.stringify({ template: "text", tag: tagchat }));
            }
            if (channel_format == "image-text" || channel_format == "image") {
                para = facebook_MultiImageText(recipient_id);
                $(".js-config-chat").val(JSON.stringify({ template: "generic", tag: tagchat }));
            }
            if (channel_format == "orderview" || channel_format == "incart" || channel_format == "orderconfirm") {
                para = facebook_Receipt(recipient_id);
                $(".js-config-chat").val(JSON.stringify({ template: channel_format, tag: tagchat }));
            }
            if (channel_format == "senderactions") {
                para = facebook_SenderActions(recipient_id)
                $(".js-config-chat").val(JSON.stringify({ template: "", tag: tagchat }));
            }
            if (para == null || para == undefined)
                return;
            $(".js-para-chat").val(JSON.stringify(para));
       // }
      
    } catch (e) { console.log(e); }
}

function facebook_OneTextDefault(userid) {
    try {
        var title = $("#js-typearea").val();
        if (title == null || title == undefined || title == "")
            return;
        var message = {
            recipient: {
                id: userid
            },
            message: {
                text: title
            }
            , tag: tagchat
        };
        return message;

    } catch (e) { console.log(e); }
}

function facebook_OneText(userid) {
    try {
        var title = $(".js-image-caption-text").val();
        if (title == null || title == undefined || title == "")
            return;

        var message = {
            recipient: {
                id: userid
            },
            message: {
                text: title
            }
            , tag: tagchat
        };
        return message;

    } catch (e) { console.log(e); }
}


function facebook_MultiImageText(userid) {
    try {
        var gallery = $("#js-gallery-image-upload .js-drop-image");
        var count = 0;
        if (gallery != null && gallery != undefined) {
            var elements = [];
            gallery.each(function (k, v) {
                var title = $(v).parent().find(".textWithImage").val();
                var image = $(v).parent().find(".js-drop-image").attr("value");
                var item = {
                    title: title,
                    image_url: image.indexOf(";base64,") > 0 ? "" : image,
                    buttons: [
                        {
                            type: "postback",
                            title: $(".js-confirm-text").val(),
                            payload: title
                        }
                    ]
                    , default_action: {
                        type: "web_url",
                        url: image.indexOf(";base64,") > 0 ? "" : image,
                    }
                };
                elements.push(item);
            });
            var message = {
                recipient: {
                    id: userid
                },
                message: {
                    attachment: {
                        type: "template",
                        payload: {
                            template_type: "generic",
                            elements: elements
                        }
                    }
                }
                , tag: tagchat
            };
            return message;
        }
        return "";
    } catch (e) { console.log(e); return ""; }
}

function facebook_Receipt(userid) {
    try {
        var elements = [];
        var gallery = $("#js-gallery-image-upload .js-drop-image");

        var count = 0;
        if (gallery != null && gallery != undefined) {
            var createTime = "";
            gallery.each(function (k, v) {
                var title = $(v).parent().find(".js-receipt-productname").val();
                var image = $(v).parent().find(".js-drop-image").attr("value");

                var CreatedDate = $(v).parent().find(".js-drop-image").attr("CreatedDate");
                var price = replaceDotAndVND(removeSpecial($(v).parent().find(".js-receipt-price").val()));

                var t1 = CreatedDate.substring(0, 6);
                var t2 = CreatedDate.substring(6, 20);
                var time = t1 + "20" + t2 + ":00";
                createTime = stringDateToTimestamp(time);
                var item = {
                    title: title,
                    image_url: image.indexOf(";base64,") > 0 ? "" : image,
                    subtitle: "",
                    quantity: 1,
                    price: price,
                    currency: "VND"
                };
                elements.push(item);
            });

            var message = {
                recipient: {
                    id: userid
                },
                message: {
                    attachment: {
                        type: "template",
                        payload: {
                            template_type: "receipt",
                            recipient_name: $(".js-receipt-fullname").val() + " - " + $(".js-receipt-phone").val(),
                            order_number: $(".js-receipt-order-number").val(),
                            currency: "VND",
                            payment_method: "Thanh toán khi nhận hàng",
                            order_url: "http://baza.vn/hot",
                            timestamp: createTime,
                            address: {
                                street_1: $(".js-receipt-address").val(),
                                street_2: $(".js-receipt-province option:selected").text(),
                                city: $(".js-receipt-province option:selected").text(),
                                postal_code: "10000",
                                state: $(".js-receipt-province option:selected").text(),
                                country: "vietnam"
                            },
                            summary: {
                                subtotal: $(".js-receipt-subtotalmoney").val(),
                                total_tax: 0,
                                shipping_cost: replaceDotAndVND(removeSpecial($(".js-receipt-shipping-cost").val())),
                                total_cost: replaceDotAndVND(removeSpecial($(".js-receipt-total-cost").val()))
                            },
                            elements: elements
                        }
                    }
                }
            };

            // them luu lai thong tin khach hang
            var para = "" + $("#id_business").val() + "/" +
                "" + _customer_id + "/" +
                "" + $(".js-receipt-fullname").val() + "/" +
                "" + $(".js-receipt-phone").val() + "/" +
                "" + $(".js-receipt-address").val() + "/" +
                "" + $(".js-receipt-email").val() + "/" +
                "" + $(".js-receipt-province option:selected").val() + "/";
            $.ajax({
                url: "/customers01/edit/" + para,
                type: "POST",
                data: null,
                contentType: 'application/json',
                dataType: 'json',
                async: true,
                success: function (res) {
                }
            });

            return message;
        }
        return null;
    } catch (e) { console.log(e); return null; }
}
//buttons: [{
//    type: "postback",
//    title: $(".js-receipt-confirm").val(),
//    payload: $(".js-receipt-confirm").val()
//}
//]    

function facebook_SenderActions(userid) {
    try {
        var message = {
            recipient: {
                id: userid
            },
            sender_action: typing_on
        };
        return message;
    } catch (e) { console.log(e); }
}

function postOrderToShop() {
    try {
        var ar = [];
        var gallery = $("#js-gallery-image-upload .js-drop-image");
        var para = null;
        if (gallery != null && gallery != undefined) {
            gallery.each(function (k, v) {
                var obj = $(v).parent().find(".js-drop-image").attr("item");
                var item = JSON.parse(obj);

                var products = {
                    ProductId: $(v).parent().find(".js-receipt-product_id").val(),
                    ProductName: $(v).parent().find(".js-receipt-productname").val(),
                    ProductAttribute: $(v).parent().find(".js-receipt-productattribute").val(),
                    ProductSKU: $(v).parent().find(".js-receipt-productsku").val(),
                    ProductPhotoUrl: $(v).parent().find(".js-drop-image").attr("value"),
                    Quantity: parseInt($(v).parent().find(".js-receipt-quantity").val()),
                    Price: replaceDotAndVND(removeSpecial($(v).parent().find(".js-receipt-price").val())),
                    TotalMoney: replaceDotAndVND(removeSpecial($(v).parent().find(".js-receipt-total_money").val())),
                };
                ar.push(products);
                
                if (para == null)
                    para = {
                        Fullname: $(".js-receipt-fullname").val(),
                        Mobiphone: $(".js-receipt-phone").val(),
                        Address: $(".js-receipt-address").val(),
                        Email: $(".js-receipt-email").val(),
                        Note: $(".js-receipt-note").val(),
                        Province: $(".js-receipt-province option:selected").val(),
                        ShippingCost: replaceDotAndVND(removeSpecial($(".js-receipt-shipping-cost").val())),
                        DiscountValue: replaceDotAndVND(removeSpecial($(".js-receipt-discountvalue").val())),
                        TotalMoney: replaceDotAndVND(removeSpecial($(".js-receipt-total-cost").val())),
                        SubTotalMoney: replaceDotAndVND(removeSpecial($(".js-receipt-subtotalmoney").val())),
                        OrderNumber: $(".js-receipt-order-number").val(),
                        CouponCode: $(".js-receipt-couponcode").val(),
                        CouponName: item.coupon_name,
                        Link: $(".js-receipt-link").val(),
                        Message: $(".js-receipt-message").val(),
                        Imei: item.imei,
                        LogonId: $(".js-agent-input").val(),
                        OrderId: item.orderid + "",
                        OrderItems: ar,
                        OrderStatus: $(".js-receipt-status option:selected").text(),
                        OrderStatusInt: parseInt($(".js-receipt-status option:selected").val()),
                        Origin: item.origin,
                        PaymentOption: item.payment_option,
                        ReturnUrl: item.return_url,
                        RewardTicketId: 0,
                        TransactStatus: item.transact_status,
                        TransactStatusInt: item.transact_status_int,
                        PaymentUrl: item.payment_url,
                        Success: item.success,
                        TransType: item.trans_type,
                        PaymentStatus: item.payment_status,
                        PaymentStatusInt: parseInt(item.payment_status_int)
                    };
            });
            para.OrderItems = ar;
            return para
        }
        return null;
    } catch (e) { console.log(e); }
}

function sendButtonOrder(userid, payload) {
    try {
        var para = {
            recipient: {
                id: userid
            },
            message: {
                attachment: {
                    type: "template",
                    payload: {
                        template_type: "button",
                        text: $(".js-receipt-confirm").val(),
                        buttons: [
                            {
                                type: "postback",
                                title: $(".js-receipt-confirm").val(),
                                payload: payload
                            }
                        ]
                    }
                }
            }
        }
        return para;
    } catch (e) { console.log(e); }
}