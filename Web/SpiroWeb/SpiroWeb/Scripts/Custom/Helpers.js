function ProcessAjaxRequestWithLaddaById(serviceUrl, serviceData, ajaxTypeRequest, clickedElement, sucessFunction) {
    var l = Ladda.create(document.querySelector(clickedElement));
    l.start();
    l.isLoading();
    l.setProgress(0 - 1);

    $.ajax({
        url: serviceUrl,
        type: ajaxTypeRequest,
        dataType: 'json',
        data:  serviceData ,

        success: function (data) {
            l.stop();
            if (sucessFunction != undefined) {
                sucessFunction(data);
            }
        }
    });
}

function ProcessAjaxRequestWithLadda(serviceUrl, serviceData, ajaxTypeRequest, sucessFunction) {
    var clickedElement;
    if (event.target.id != undefined) {
        clickedElement = event.target.id;
        if (clickedElement == "") {
            clickedElement = $(event.target.parentNode).attr("id");
        }

        //var l = Ladda.create(document.querySelector(clickedElement));
        var l = Ladda.create(document.querySelector("#" + clickedElement));
        l.start();
        l.isLoading();
        l.setProgress(0 - 1);

        $.ajax({
            url: serviceUrl,
            type: ajaxTypeRequest,
            dataType: 'json',
            data: serviceData,

            success: function (data) {
                l.stop();
                if (sucessFunction != undefined) {
                    sucessFunction(data);
                }
            }
        });
    }
}

function ProcessAjaxRequest(serviceUrl, serviceData, ajaxTypeRequest, clickedElement, sucessFunction) {
    var l = Ladda.create(document.querySelector(clickedElement));

    $.ajax({
        url: serviceUrl,
        type: ajaxTypeRequest,
        dataType: 'json',
        data: serviceData,

        success: function (data) {
            if (sucessFunction != undefined) {
                sucessFunction(data);
            }
            //$("#productQueueSearchText").val($('#searchProductAutocompleteTextbox').val());
        }
    });
}

function ProcessAjaxRequestSimple(serviceUrl, serviceData, ajaxTypeRequest, sucessFunction) {
    $.ajax({
        url: serviceUrl,
        type: ajaxTypeRequest,
        dataType: 'json',
        data: serviceData,

        success: function (data) {
            if (sucessFunction != undefined) {
                sucessFunction(data);
            }
        }
    });
}