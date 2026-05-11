
$(function () {

    var ticker = $.connection.stockTicker;

    function init() {
    }

    // Add client-side hub methods that the server will call
    $.extend(ticker.client, {
        updateShoppingCartProductsInQueue: function (valueString) {
            GetProductsQueueIndex(valueString);
        },
        updateShoppingCart: function (productListId) {
            GetShoppingCartListTable(productListId);
        },
        logServerTime: function (serverTime) {
            //$("#spanLogger").text($("#spanLogger").text() + serverTime + "<br>");
            $("#spanLogger").append('<p>Server SignalR Call : Client Time - ' + new Date().toString() + ' , Server Time - ' + serverTime + '</p>');
        }
    });

    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000); // Re-start connection after 5 seconds
    });

    // Start the connection
    $.connection.hub.start()
        .then(init)
        .done(function (state) {
            setInterval(function () {
                $("#spanLogger").append('<p>Client SignalR Call Time : ' + new Date().toString() + '</p>');
                ticker.server.sendMeTheTime();
            }, 600000);
        });
});