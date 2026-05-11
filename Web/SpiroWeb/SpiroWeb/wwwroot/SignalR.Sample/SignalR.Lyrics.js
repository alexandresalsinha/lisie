
$(function () {

    var signal = $.connection.stockTicker;

    function init() {
    }

    // Add client-side hub methods that the server will call
    $.extend(signal.client, {
        newPlayingSong: function () {
            debugger
            window.location.reload();   
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
                signal.server.sendMeTheTime();
            }, 600000);
        });
});