$(function () {
    $('.autocomplete-with-hidden').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var url = $(this.element).data('url');

            $.getJSON(url, { term: request.term }, function (data) {
                response(data);
            })
        },
        select: function (event, ui) {
            $(event.target).next('input[type=hidden]').val(ui.item.id);
            AddProductToShoppingCart(ui.item.id, false);
        },
        change: function (event, ui) {
            if (!ui.item) {
                //debugger
                //$(event.target).val('').next('input[type=hidden]').val('');
            }
        }
    });
})

$('#searchProductAutocompleteTextbox').focus(function () {
    if ($("#searchProductAutocompleteTextbox").val() == "Quick Add") {
        $("#searchProductAutocompleteTextbox").val("");
    }
});