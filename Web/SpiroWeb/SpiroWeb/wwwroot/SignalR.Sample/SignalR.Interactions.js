$(function () {

    var ticker = $.connection.stockTicker;

    var _newProducts = 0;
    var _newStoreProducts = 0;
    var _newInteractions = 0;
    var _newUsers = 0;

    var _newProducStoreUpdates = 0;
    var _newProducPricesUpdates = 0;
    var newProductStoreUpdatesFailed = 0;
    var _totalInteractions = 0;
    function init() {
    }

    // Add client-side hub methods that the server will call
    $.extend(ticker.client, {
        updateShoppingCartProductsInQueue: function (valueString) {
            GetProductsQueueIndex(valueString);
        },
        updateTotalInteractions: function (totalInteractions) {
            debugger
            _newInteractions++;
            if (document.getElementById("totalInteractions")){
                document.getElementById("totalInteractions").innerText = totalInteractions;
            }
            if (document.getElementById("totalInteractionsPlus")) {
                document.getElementById("totalInteractionsPlus").innerText = " +" + _newInteractions;

            }
        },
        updateTotalProducts: function (totalProducts) {
            debugger
            _newProducts++;
            document.getElementById("totalProducts").innerText = totalProducts;
            document.getElementById("totalProductsPlus").innerText = " +" + _newProducts;
        },
        updateTotalStoreProducts: function (totalStoreProducts) {
            debugger
            ++_newStoreProducts;
            document.getElementById("totalStoreProducts").innerText = totalStoreProducts;
            document.getElementById("totalStoreProductsPlus").innerText = " +" + _newStoreProducts;
        },
        updateTotalUsers: function (totalUsers) {
            debugger
            ++_newUsers;
            document.getElementById("totalUsers").innerText = totalUsers;
            document.getElementById("totalUsersPlus").innerText = " +" + _newUsers;
        },
        updateStoreProductUpdate: function (updatedStreProduct) {
            debugger
            console.log(updatedStreProduct);

            var table = document.getElementById("ProductsUpdatesTable");
            if (table) {
                var row = table.insertRow(1);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                var cell5 = row.insertCell(4);
                var cell6 = row.insertCell(5);
                var cell7 = row.insertCell(6);
                var cell8 = row.insertCell(7);
                var cell9 = row.insertCell(8);
                cell1.innerHTML = updatedStreProduct.ProductId;
                cell2.innerHTML = updatedStreProduct.Store;
                cell3.innerHTML = "<a target=\"_blank\" href=\"" + updatedStreProduct.StoreUrl + "\">" + updatedStreProduct.Name + "</a>";
                cell4.innerHTML = updatedStreProduct.Brand;
                cell5.innerHTML = updatedStreProduct.OldPrice;
                cell6.innerHTML = updatedStreProduct.NewPrice;
                cell7.innerHTML = updatedStreProduct.PriceUpdateDate;
                cell8.innerHTML = updatedStreProduct.UpdateDate;
                cell9.innerHTML = updatedStreProduct.NeedsUpdate;

                if (updatedStreProduct.NeedsUpdate) {
                    row.style.backgroundColor = "#FFFF00";
                    newProductStoreUpdatesFailed++;
                    document.getElementById("newProductStoreUpdatesFailed").innerText = newProductStoreUpdatesFailed;
                }

                cell1.style.textAlign = "center";
                cell1.style.verticalAlign = "middle";
                cell2.style.textAlign = "center";
                cell2.style.verticalAlign = "middle";
                cell3.style.textAlign = "center";
                cell3.style.verticalAlign = "middle";
                cell4.style.textAlign = "center";
                cell4.style.verticalAlign = "middle";
                cell5.style.textAlign = "center";
                cell5.style.verticalAlign = "middle";
                cell6.style.textAlign = "center";
                cell6.style.verticalAlign = "middle";
                cell7.style.textAlign = "center";
                cell7.style.verticalAlign = "middle";
                cell8.style.textAlign = "center";
                cell8.style.verticalAlign = "middle";
                cell9.style.textAlign = "center";
                cell9.style.verticalAlign = "middle";

                //remove last row
                table.deleteRow(-1);

                _newProducStoreUpdates++;
                document.getElementById("newProducStoreUpdates").innerText = _newProducStoreUpdates;
            }
            //StoreUrl: "https://www.continente.pt/produto/5036092.html"
        },

        updateStoreProductPricesUpdate: function (updatedProductPrice) {
            debugger
            console.log(updatedProductPrice);

            var table = document.getElementById("ProductPricesUpdatesTable");
            if (table) {


                var row = table.insertRow(1);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                var cell5 = row.insertCell(4);
                var cell6 = row.insertCell(5);
                var cell7 = row.insertCell(6);
                var cell8 = row.insertCell(7);
                var cell9 = row.insertCell(8);
                cell1.innerHTML = updatedProductPrice.ProductId;
                cell2.innerHTML = updatedProductPrice.Store;
                cell3.innerHTML = "<a target=\"_blank\" href=\"" + updatedProductPrice.StoreUrl + "\">" + updatedProductPrice.Name + "</a>";
                cell4.innerHTML = updatedProductPrice.Brand;
                cell5.innerHTML = updatedProductPrice.OldPrice;
                cell6.innerHTML = updatedProductPrice.NewPrice;
                cell7.innerHTML = updatedProductPrice.PriceUpdateDate;
                cell8.innerHTML = updatedProductPrice.UpdateDate;
                cell9.innerHTML = updatedProductPrice.NeedsUpdate;

                if (updatedProductPrice.NeedsUpdate) {
                    row.style.backgroundColor = "#FFFF00";
                }

                cell1.style.textAlign = "center";
                cell1.style.verticalAlign = "middle";
                cell2.style.textAlign = "center";
                cell2.style.verticalAlign = "middle";
                cell3.style.textAlign = "center";
                cell3.style.verticalAlign = "middle";
                cell4.style.textAlign = "center";
                cell4.style.verticalAlign = "middle";
                cell5.style.textAlign = "center";
                cell5.style.verticalAlign = "middle";
                cell6.style.textAlign = "center";
                cell6.style.verticalAlign = "middle";
                cell7.style.textAlign = "center";
                cell7.style.verticalAlign = "middle";
                cell8.style.textAlign = "center";
                cell8.style.verticalAlign = "middle";
                cell9.style.textAlign = "center";
                cell9.style.verticalAlign = "middle";

                //remove last row
                table.deleteRow(-1);

                _newProducPricesUpdates++;
                document.getElementById("newProducPricesUpdates").innerText = _newProducPricesUpdates;
            }
        },


        updateAllInteractions: function (interaction) {
            debugger
            console.log(interaction);

            var table = document.getElementById("InteractionsTable");
            if (table) {
                var row = table.insertRow(1);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                //var cell5 = row.insertCell(4);
                //var cell6 = row.insertCell(5);
                //var cell7 = row.insertCell(6);
                //var cell8 = row.insertCell(7);
                //var cell9 = row.insertCell(8);
                cell1.innerHTML = interaction.Id;
                cell2.innerHTML = "<a target=\"_blank\" href=\"/users/details/" + interaction.UserId + "\">" + interaction.UserId + "</a>";
                cell3.innerHTML = interaction.Name;
                cell4.innerHTML = interaction.CreateDate;

                cell1.style.textAlign = "center";
                cell1.style.verticalAlign = "middle";
                cell2.style.textAlign = "center";
                cell2.style.verticalAlign = "middle";
                cell3.style.textAlign = "center";
                cell3.style.verticalAlign = "middle";
                cell4.style.textAlign = "center";
                cell4.style.verticalAlign = "middle";

                //remove last row
                table.deleteRow(-1);

                //_totalInteractions++;
                //document.getElementById("allInteractionsTotal").innerText = _totalInteractions;
            }
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