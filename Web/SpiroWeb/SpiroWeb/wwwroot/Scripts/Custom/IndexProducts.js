

$("#searchForProductOnline").click(function () {
    // Create a new instance of ladda for the specified button
    debugger

    //var l = $('#searchForProductOnline').ladda();
    var l = Ladda.create(document.querySelector("#searchForProductOnline"));

    // Start loading
    l.ladda('start');

    // Will display a progress bar for 50% of the button width
    l.ladda('setProgress', 0.5);

    // Stop loading
    l.ladda('stop');

    // Toggle between loading/not loading states
    l.ladda('toggle');

    // Check the current state
    l.ladda('isLoading');
});
//document.querySelector('#searchForProductOnline').addEventListener('click', function () {
    
//});