var year = new Date().getFullYear();
var month = new Date().getMonth();
var day = new Date().getDate();

var eventData = {
    events: [
       { "id": 1, "start": new Date(year, month, day, 12), "end": new Date(year, month, day, 13, 35), "title": "Lunch with Mike" },
       { "id": 2, "start": new Date(year, month, day, 14), "end": new Date(year, month, day, 14, 45), "title": "Dev Meeting" },
       { "id": 3, "start": new Date(year, month, day + 1, 18), "end": new Date(year, month, day + 1, 18, 45), "title": "Hair cut" },
       { "id": 4, "start": new Date(year, month, day - 1, 8), "end": new Date(year, month, day - 1, 9, 30), "title": "Team breakfast" },
       { "id": 5, "start": new Date(year, month, day + 1, 14), "end": new Date(year, month, day + 1, 15), "title": "Product showcase" }
    ]
};



$(document).ready(function () {

    $.getJSON("/WeekCalendar/GetWeeklyCalendarJson", function (allData) {
        eventData = JSON.parse(allData);

        $('#calendar').weekCalendar({
            timeslotsPerHour: 4,
            height: function ($calendar) {
                return $(window).height() - $("h1").outerHeight();
            },
            eventRender: function (calEvent, $event) {
                console.log(calEvent.id);
                
                if (calEvent.end.getTime() < new Date().getTime()) {
                    if (!calEvent.isDone) {
                        switch (calEvent.category) {
                            case "work":
                                $event.css("backgroundColor", "#7E86E0");
                                $event.find(".time").css({ "backgroundColor": "#999", "border": "1px solid #888" });
                                break;
                            case "personal":
                                $event.css("backgroundColor", "#ff6a00");
                                $event.find(".time").css({ "backgroundColor": "#999", "border": "1px solid #888" });
                                break;
                            case "social":
                                $event.css("backgroundColor", "#8AC007");
                                $event.find(".time").css({ "backgroundColor": "#999", "border": "1px solid #888" });
                                break;
                            default:
                                break;
                        }
                    }
                    else {
                        $event.css("backgroundColor", "gray");
                        $event.find(".time").css({ "backgroundColor": "#999", "border": "1px solid #888" });
                    }
                }
                var _checkedHtml = "";
                if (calEvent.isDone) {
                    _checkedHtml = " checked";
                }
                $event.append('<label><input id="CheckBox_WeekTaskIsDone" data-id="' + calEvent.id + '" data-category="' + calEvent.category + '" data-taskId="' + calEvent.taskId + '" type="checkbox" name="checkbox" value="value" onClick="ChangeTaskDoneStatus()"' + _checkedHtml +'>Done</label>')
                $event.append('<br><a  href="#" class="glyphicon glyphicon-remove" data-id="' + calEvent.id + '"  title="Delete Schedule Task" onClick="DeleteScheduleTask()">Remove</a>');
            },
            eventNew: function (calEvent, $event) {
                //displayMessage("<strong>Added event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
                alert("You've added a new event. You would capture this event, add the logic for creating a new event with your own fields, data and whatever backend persistence you require.");
            },
            eventDrop: function (calEvent, $event) {
                //console.log(calEvent.id);
                //displayMessage("<strong>Moved Event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
                $.post('/WeekCalendar/UpdateScheduledTask', { id: $event.id, start: calEvent.start, end: calEvent.end });
            },
            eventResize: function (calEvent, $event) {
                //displayMessage("<strong>Resized Event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
            },
            eventClick: function (calEvent, $event) {
                //displayMessage("<strong>Clicked Event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
            },
            eventMouseover: function (calEvent, $event) {
                //displayMessage("<strong>Mouseover Event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
            },
            eventMouseout: function (calEvent, $event) {
                //displayMessage("<strong>Mouseout Event</strong><br/>Start: " + calEvent.start + "<br/>End: " + calEvent.end);
            },
            noEvents: function () {
                //displayMessage("There are no events for this week");
            },
            data: eventData
        });
    });

    //setTimeout(function () {
    //    $('#calendar').css('height', 'auto');
    //    //$('#calendar').css('-webkit-box-sizing', '');
    //    $('#calendar').css('box-sizing', 'content-box !important');
    //}, 5000);
    

    //function displayMessage(message) {
    //    $("#message").html(message).fadeIn();
    //}

    //$("<div id=\"message\" class=\"ui-corner-all\"></div>").prependTo($("body"));

});

function ChangeTaskDoneStatus()
{
    
    var checkBox = $(event.target);
    var checkedBoxValue = false;
    if(checkBox.is(':checked'))
    {
        checkedBoxValue = true;
    }
    $.post('/WeekCalendar/ChangeTaskDoneStatus', { id: checkBox.attr("data-id"), taskId: checkBox.attr("data-taskId"), isDone: checkedBoxValue, category: checkBox.attr("data-category") }, function (data) {
        
        var jsonTasks = JSON.parse(data);
        if (checkedBoxValue == true) {
            UpdatePoints(jsonTasks.attribute, jsonTasks.importance, jsonTasks.checkBox.attr("data-category"));
        }
        else {
            DeUpdatePoints();
        }
        return;
    });
    
}

function DeleteScheduleTask() {
    
    var elementClicked = $(event.target);

    $.post('/WeekCalendar/DeleteScheduleTask', { id: elementClicked.attr("data-id") }, function (data) {
        elementClicked.parent().parent().remove();
    });

}
