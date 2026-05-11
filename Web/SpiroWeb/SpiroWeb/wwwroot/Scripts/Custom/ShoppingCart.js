function GetProductsQueueIndex(barCode) {
    SelectedBarCode = barCode;
    $.ajax({
        url: '/ShoppingCart/GetProductsQueueIndex',
        type: 'get',
        dataType: 'json',
        //data: { quizUmbracoNodeId: quizUmbracoId },

        success: function (data) {
            if (data == "" || data == undefined)
                return;

            var _returnedHtml = data;

            var newDiv = $(document.createElement('div'));
            $(newDiv).html(_returnedHtml);
            $(newDiv).dialog({
                modal: true,
                autoResize: true,
                close: function () {
                    closeDialog();
                }
            });
            dialogDiv = $(newDiv).dialog();


            if (SelectedBarCode == undefined) {
                if ($("#selectedBarCodeHidden").val() != undefined) {
                    SelectedBarCode = $("#selectedBarCodeHidden").val();
                }
            }
            //$(dialogDiv).dialog("option", "width", 500);
            //$(dialogDiv).dialog({
            //    width: "auto",
            //    responsive: true,
            //    // maxWidth: 660, // This won't work
            //    create: function (event, ui) {
            //        // Set maxWidth
            //        $(this).css("maxWidth", "700px");
            //    }
            //});

            $(dialogDiv).dialog({
                width: 900,
                autoOpen: false,
                dialogClass: "test",
                modal: false,
                responsive: true
            });
        }
    });
}

function GetProductsQueueSearchHtml(selectedBarCode) {
    SelectedBarCode = selectedBarCode;
    $.ajax({
        url: '/ShoppingCart/GetProductsQueueSearchHtml',
        type: 'post',
        dataType: 'json',
        data: { barCode: selectedBarCode },

        success: function (data) {
            var _returnedHtml = data;
            $(dialogDiv).dialog().html(_returnedHtml);
        }
    });
}

var dialogDiv;
var SelectedBarCode;

function GetProductsQueueSearchResults() {
    ProcessAjaxRequestWithLadda('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#productQueueSearchText').val() }, 'post', (function (data) {
        $(dialogDiv).dialog().html(data);
        //$(dialogDiv).dialog("option", "width", 600);
    }));
    //ProcessAjaxRequestWithLadda('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#productQueueSearchText').val() }, 'post', '#productQueueSearchButton', (function (data) {
    //    $(dialogDiv).dialog().html(data);
    //    //$(dialogDiv).dialog("option", "width", 600);
    //}));

    //$.ajax({
    //    url: '/ShoppingCart/GetProductsQueueSearchResults',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { searchText: $('#productQueueSearchText').val() },

    //    success: function (data) {
    //        var _returnedHtml = data;
    //        $(dialogDiv).dialog().html(_returnedHtml);

    //    }
    //});
}

function GetProductsQueueSearchResultsAgain() {
    debugger
    ProcessAjaxRequestWithLadda('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#productQueueSearchTextAgain').val() }, 'post', (function (data) {
        $(dialogDiv).dialog().html(data);
    }));

    return;
    $.ajax({
        url: '/ShoppingCart/GetProductsQueueSearchResults',
        type: 'post',
        dataType: 'json',
        data: { searchText: $('#productQueueSearchTextAgain').val() },

        success: function (data) {
            var _returnedHtml = data;
            $(dialogDiv).dialog().html(_returnedHtml);
        }
    });
}

var jumboCurrentPage = 1;

function GetProductsQueueSearchMoreResults() {
    $.ajax({
        url: '/ShoppingCart/GetProductsQueueSearchMoreResults',
        type: 'post',
        dataType: 'json',
        data: { searchText: $('#productQueueSearchTextAgain').val(), pageNumber: jumboCurrentPage },

        success: function (data) {
            jumboCurrentPage = jumboCurrentPage + 1;

            var _returnedHtml = data;
            var divInsideDialog = $(dialogDiv).dialog().html();
            $(divInsideDialog).append(_returnedHtml);
        }
    });
}

function GetSelectedProductData(selectedProductUrl, selectedProductStore,  buttonLaddaId) {
    //var eventTargetId = event.target.id;
    //ProcessAjaxRequestWithLadda('/ShoppingCart/GetSelectedProductData', { productUrl: selectedProductUrl }, 'post', '#' + buttonLaddaId, (function (data) {
    ProcessAjaxRequestWithLadda('/ShoppingCart/GetSelectedProductData', { productUrl: selectedProductUrl, store: selectedProductStore }, 'post', (function (data) {
        $(dialogDiv).dialog().html(data);
        $(dialogDiv).dialog("option", "width", 600);
    }));

    //$.ajax({
    //    url: '/ShoppingCart/GetSelectedProductData',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { productUrl: selectedProductUrl },

    //    success: function (data) {

    //    }
    //});
}

function SubmitNewProduct(selectedProductUrl) {
    ProcessAjaxRequestWithLadda('/ShoppingCart/SubmitNewProduct', { productUrl: selectedProductUrl, barCode: SelectedBarCode }, 'post', (function (data) {
        closeDialog();
    }));
    //$.ajax({
    //    url: '/ShoppingCart/SubmitNewProduct',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { productUrl: selectedProductUrl, barCode: SelectedBarCode },

    //    success: function (data) {
    //        var _returnedHtml = data;
    //        //$(dialogDiv).dialog().html(_returnedHtml);

    //        closeDialog()
    //        location.reload();
    //    }
    //});
}

function SubmitNewProductAndToShoppingCart(selectedProductUrl, selectedProductStore ) {
    ProcessAjaxRequestWithLadda('/ShoppingCart/SubmitNewProductAndToShoppingCart', { productUrl: selectedProductUrl, store: selectedProductStore, barCode: SelectedBarCode }, 'post', (function (data) {
        closeDialog();
        GetShoppingCartListTable(data);
    }));


    //$.ajax({
    //    url: '/ShoppingCart/SubmitNewProductAndToShoppingCart',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { productUrl: selectedProductUrl, barCode: SelectedBarCode },

    //    success: function (data) {
    //        var _userProductId = data;
    //        $(dialogDiv).dialog("close");
    //        GetShoppingCartListTable(_userProductId);
    //    }
    //});
}

function AddProductToShoppingCart(selectedProductId, isToAddToHistory) {
    debugger
    var _buttonId = $(event.target).parent().attr('id');
    //ProcessAjaxRequestWithLadda('/ShoppingCart/AddProductToShoppingCart', { productId: selectedProductId, addToHistory: isToAddToHistory }, 'post', '#' + _buttonId, (function (data) {
    ProcessAjaxRequestWithLadda('/ShoppingCart/AddProductToShoppingCart', { productId: selectedProductId, addToHistory: isToAddToHistory }, 'post', (function (data) {
        var _userProductId = data;
        closeDialog();
        GetShoppingCartListTable(_userProductId);
    }));

    //$.ajax({
    //    url: '/ShoppingCart/AddProductToShoppingCart',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { productId: selectedProductId, addToHistory: isToAddToHistory },

    //    success: function (data) {
    //        var _userProductId = data;
    //        //$(dialogDiv).dialog("close");
    //        closeDialog();
    //        GetShoppingCartListTable(_userProductId);
    //    }
    //});
}

//function EndProductsQueue() {
//    $(dialogDiv).dialog("close");
//    location.reload();
//}

function DeleteProductInQueue(selectedBarCode, notWithLadda) {
    if (notWithLadda == undefined) {
        //ProcessAjaxRequestWithLadda('/ShoppingCart/DeleteProductInQueue', { barCode: selectedBarCode }, 'post', '#productQueueRemoveButton', (function (data) {
        ProcessAjaxRequestWithLadda('/ShoppingCart/DeleteProductInQueue', { barCode: selectedBarCode }, 'post', (function (data) {
            var _returnedHtml = data;
            if (_returnedHtml == "") {
                //$(dialogDiv).dialog("close");
                closeDialog();
                location.reload();
                return;
            }
            $(dialogDiv).dialog().html(_returnedHtml);
        }));
    }
    else {

        $.ajax({
            url: '/ShoppingCart/DeleteProductInQueue',
            type: 'post',
            dataType: 'json',
            data: { barCode: selectedBarCode },

            success: function (data) {
                var _returnedHtml = data;
                //TODO - if return data.success == true , remove TR before $(this)
                if (_returnedHtml == "") {
                    $(dialogDiv).dialog("close");
                    location.reload();
                    return;
                }
                $(dialogDiv).dialog().html(_returnedHtml);
            }
        });
    }
}

function GetShoppingCartListTable(productListId) {
    $.ajax({
        url: '/ShoppingCart/GetShoppingCartListTable',
        type: 'post',
        dataType: 'json',
        //data: { barCode: selectedBarCode },

        success: function (data) {
            var _returnedHtml = data;
            //TODO - if return data.success == true , remove TR before $(this)
            if (_returnedHtml != "") {
                $("#mainContainer").html(_returnedHtml);

                if (productListId != undefined && productListId != "") {
                    HighlightUserProduct(productListId);

                }

                return;
            }
        }
    });
}

function HighlightUserProduct(productListId) {
    var jquerySelector = "[data-productlistid='" + productListId + "']";
    var elementToScroll = $(jquerySelector);
    window.scrollTo(0, $(jquerySelector).offset().top);

    setTimeout(function () {
        //elementToScroll.css({ 'background-color': '#ffffff' }).fadeOut();
        elementToScroll.animate({ backgroundColor: "#96DBF2" }, 'slow');
    }, 500);


    setTimeout(function () {
        //elementToScroll.css({ 'background-color': '#ffffff' }).fadeOut();
        elementToScroll.animate({ backgroundColor: "#ffffff" }, 'slow');
    }, 3000);
}

function SearchForProductOnline() {
    SelectedBarCode = undefined;
    //ProcessAjaxRequestWithLadda('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#searchProductAutocompleteTextbox').val() }, 'post', '#searchForProductOnline', (function (data) {
    ProcessAjaxRequestWithLadda('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#searchProductAutocompleteTextbox').val() }, 'post', (function (data) {
        var _returnedHtml = data;

        var newDiv = $(document.createElement('div'));
        $(newDiv).html(_returnedHtml);
        $(newDiv).dialog({
            modal: true,
            autoResize: true
        });
        dialogDiv = $(newDiv).dialog();

        dialogDiv = $(newDiv);
    }));

    //ProcessAjaxRequest('/ShoppingCart/GetProductsQueueSearchResults', { searchText: $('#searchProductAutocompleteTextbox').val() }, 'post', '#searchProductAutocompleteTextbox', (function(data) {
    //        var _returnedHtml = data;

    //        var newDiv = $(document.createElement('div'));
    //        $(newDiv).html(_returnedHtml);
    //        $(newDiv).dialog({
    //            modal: true,
    //            autoResize: true
    //        });
    //        dialogDiv = $(newDiv).dialog();

    //        dialogDiv = $(newDiv);
    //    }));
    //    return;

    //var l = Ladda.create(document.querySelector("#searchForProductOnline"));

    //l.start();
    //l.isLoading();
    //l.setProgress(0 - 1);


    //$.ajax({
    //    url: '/ShoppingCart/GetProductsQueueSearchResults',
    //    type: 'post',
    //    dataType: 'json',
    //    data: { searchText: $('#searchProductAutocompleteTextbox').val() },

    //    success: function (data) {
    //        var _returnedHtml = data;

    //        var newDiv = $(document.createElement('div'));
    //        $(newDiv).html(_returnedHtml);
    //        $(newDiv).dialog({
    //            modal: true,
    //            autoResize: true
    //        });
    //        dialogDiv = $(newDiv).dialog();

    //        dialogDiv = $(newDiv);

    //        l.stop();
    //        //$("#productQueueSearchText").val($('#searchProductAutocompleteTextbox').val());
    //    }
    //});
}

function AddDummyProductToQueue() {
    if ($("#dummyBarcode").val() == '') return;

    $.ajax({
        url: '/ShoppingCart/AddDummyProductToQueue',
        type: 'post',
        dataType: 'json',
        data: { barCode: $("#dummyBarcode").val() },

        success: function (data) {
            //location.reload();
        }
    });
}

function AddQuantityToShoppingListItem(UserProductListId) {
    $.ajax({
        url: '/ShoppingCart/AddQuantityToShoppingListItem',
        type: 'post',
        dataType: 'json',
        data: { userProductListId: UserProductListId },

        success: function (data) {
            GetShoppingCartListTable();
        }
    });

}

//function SubtractQuantityToShoppingListItem(UserProductListId) {
//    $.ajax({
//        url: '/ShoppingCart/SubtractQuantityToShoppingListItem',
//        type: 'post',
//        dataType: 'json',
//        data: { userProductListId: UserProductListId },

//        success: function (data) {
//            GetShoppingCartListTable();
//        }
//    });

//}

function SubtractQuantityToShoppingListItem(UserProductListId, currentQuantity) {
    if (currentQuantity == 1) {
        $("#dialog-confirm").dialog({
            resizable: false,
            height: 140,
            modal: true,
            buttons: {
                "Delete all items": function () {
                    $.ajax({
                        url: '/ShoppingCart/SubtractQuantityToShoppingListItem',
                        type: 'post',
                        dataType: 'json',
                        data: { userProductListId: UserProductListId },

                        success: function (data) {
                            GetShoppingCartListTable();
                            $("#dialog-confirm").dialog("close");
                        }
                    });

                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
    }
    else {
        $.ajax({
            url: '/ShoppingCart/SubtractQuantityToShoppingListItem',
            type: 'post',
            dataType: 'json',
            data: { userProductListId: UserProductListId },

            success: function (data) {
                GetShoppingCartListTable();
            }
        });
    }
}

function closeDialog() {
    $(dialogDiv).dialog("close");
    $('.ui-widget-overlay').remove();
}