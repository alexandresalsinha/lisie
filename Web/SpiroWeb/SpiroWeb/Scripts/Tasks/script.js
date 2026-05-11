//<![CDATA[ 
var globalWorkRecursiveListViewModel;
var globalSocialRecursiveListViewModel;
var globalPersonalRecursiveListViewModel;

var addTaskToParentId = -1;
var scheduleTaskId = -1;
var scheduleTaskCategory;

var createTaskFor;

var dialog, dialogScheduleTask, form;

//var blurred = false;
//window.onblur = function () { blurred = true; };
//window.onfocus = function () { blurred && (location.reload()); };

//var pointsToGain = 0;
function UpdatePoints(taskId, attr, imp, diff, category) {
    

    var pointsToGain = parseInt(imp) * parseInt(diff);

    var currentPoints = parseInt($("#CurrentPoints").text());
    var currentLevel = parseInt($("#CurrentLevel").text());

    var newPoints = currentPoints;
    var newLevel = currentLevel;

    var splittedAttributes = attr.split(',');

    newPoints = currentPoints + (pointsToGain * splittedAttributes.length);

    var levelApr = Math.floor(newPoints / 100);
    var levelRem = newPoints % 100;

    newLevel = levelApr;

    if (levelRem == 0) {
        currentLevel = levelApr;
    }

    splittedAttributes.forEach(function (item, index) {
        var newAttributePoints = 0;
        switch (item.toLowerCase()) {
            case "intelligence":
                newAttributePoints = parseInt($("#CurrentIntellegencePoints").text()) + pointsToGain;
                $("#CurrentIntellegencePoints").text(newAttributePoints.toString());
                break;
            case "perseverance":
                newAttributePoints = parseInt($("#CurrentPerseverancePoints").text()) + pointsToGain;
                $("#CurrentPerseverancePoints").text(newAttributePoints.toString());
                break;
            case "strength":
                newAttributePoints = parseInt($("#CurrentStrengthPoints").text()) + pointsToGain;
                $("#CurrentStrengthPoints").text(newAttributePoints.toString());

                break;
            case "vitality":
                newAttributePoints = parseInt($("#CurrentVitalityPoints").text()) + pointsToGain;
                $("#CurrentVitalityPoints").text(newAttributePoints.toString());
                break;
            case "creativity":
                newAttributePoints = parseInt($("#CurrentCreativityPoints").text()) + pointsToGain;
                $("#CurrentCreativityPoints").text(newAttributePoints.toString());
                break;
            case "charisma":
                newAttributePoints = parseInt($("#CurrentCharismaPoints").text()) + pointsToGain;
                $("#CurrentCharismaPoints").text(newAttributePoints.toString());
                break;
            default:
                break;
        }

        var newCategoryPoints = 0;
        switch (category) {
            case "social":
                newCategoryPoints = parseInt($("#SocialPoints").text()) + pointsToGain;
                $("#SocialPoints").text(newCategoryPoints.toString());
                break;
            case "personal":
                newCategoryPoints = parseInt($("#PersonalPoints").text()) + pointsToGain;
                $("#PersonalPoints").text(newCategoryPoints.toString());
                break;
            case "work":
                newCategoryPoints = parseInt($("#WorkPoints").text()) + pointsToGain;
                $("#WorkPoints").text(newCategoryPoints.toString());
                break;
            default:
                break;
        }

        //save history
        $.post("/HistoryTasks/AddPointsToHistory", {
            taskId: taskId,
            taskCategory: category,
            taskAttribute: item,
            taskPoints: pointsToGain,
            newAttributePoints: newAttributePoints,
            newCategoryPoints: newCategoryPoints,
            newGlobalPoints: newPoints,
            newGlobalLevel: newLevel
        });
    });

    swal({
        title: "Ohh Yeahhhhh",
        text: "One more step to greatness ;) . Your  " + pointsToGain + " points closer to it, in your " + category + " matters. And you´ve increased your " + attr + ".  Fuck yeah!",
            timer: 10000,
            type: "success",
            showConfirmButton: true
        });

    $("#CurrentPoints").text(newPoints);
    $("#CurrentLevel").text(newLevel);

   
    SavePoints();
}


function DeUpdatePoints(taskId, attr, imp, diff, category) {
    var pointsToGain = parseInt(imp) * parseInt(diff);

    var currentPoints = parseInt($("#CurrentPoints").text());
    var currentLevel = parseInt($("#CurrentLevel").text());

    var newPoints = currentPoints;
    var newLevel = currentLevel;

    var splittedAttributes = attr.split(',');

    newPoints = currentPoints - (pointsToGain * splittedAttributes.length);

    var levelApr = Math.floor(newPoints / 100);
    var levelRem = newPoints % 100;

    newLevel = levelApr;

    if (levelRem == 0) {
        currentLevel = levelApr;
    }

   
    splittedAttributes.forEach(function (item, index) {
        switch (item.toLowerCase()) {
            case "intelligence":
                //$("#CurrentIntellegencePoints").text() = $("#CurrentIntellegencePoints").text() + pointsToGain;
                $("#CurrentIntellegencePoints").text(parseInt($("#CurrentIntellegencePoints").text()) - pointsToGain);
                break;
            case "perseverance":
                $("#CurrentPerseverancePoints").text(parseInt($("#CurrentPerseverancePoints").text()) - pointsToGain);
                break;
            case "strength":
                $("#CurrentStrengthPoints").text(parseInt($("#CurrentStrengthPoints").text()) - pointsToGain);
                break;
            case "vitality":
                $("#CurrentVitalityPoints").text(parseInt($("#CurrentVitalityPoints").text()) - pointsToGain);
                break;
            case "creativity":
                $("#CurrentCreativityPoints").text(parseInt($("#CurrentCreativityPoints").text()) - pointsToGain);
                break;
            case "charisma":
                $("#CurrentCharismaPoints").text(parseInt($("#CurrentCharismaPoints").text()) - pointsToGain);
                break;
            default:

        }

        switch (category) {
            case "social":
                $("#SocialPoints").text(parseInt($("#SocialPoints").text()) - pointsToGain);
                break;
            case "personal":
                $("#PersonalPoints").text(parseInt($("#PersonalPoints").text()) - pointsToGain);
                break;
            case "work":
                $("#WorkPoints").text(parseInt($("#WorkPoints").text()) - pointsToGain);
                break;
            default:

        }

        $.post("/HistoryTasks/RemovePointsFromHistory", {
            taskId: taskId,
            taskCategory: category
        });
    });

    $("#CurrentPoints").text(newPoints);
    $("#CurrentLevel").text(newLevel);

    SavePoints();
}


function SavePoints() {
    var currentPoints = parseInt($("#CurrentPoints").text());
    var currentLevel = parseInt($("#CurrentLevel").text());

    var currentIntelligencePoints = parseInt($("#CurrentIntellegencePoints").text());
    var currentPerseverancePoints = parseInt($("#CurrentPerseverancePoints").text());
    var currentStrengthPoints = parseInt($("#CurrentStrengthPoints").text());
    var currentVitalityPoints = parseInt($("#CurrentVitalityPoints").text());
    var currentCreativityPoints = parseInt($("#CurrentCreativityPoints").text());
    var currentCharismaPoints = parseInt($("#CurrentCharismaPoints").text());

    var currentWorkPoints = parseInt($("#WorkPoints").text());
    var currentSocialPoints = parseInt($("#SocialPoints").text());
    var currentPersonalPoints = parseInt($("#PersonalPoints").text());

    $.post("/TasksManager/SavePoints", {
        points: currentPoints,
        level: currentLevel,
        intelligencePoints: currentIntelligencePoints,
        perseverancePoints: currentPerseverancePoints,
        strengthPoints: currentStrengthPoints,
        vitalityPoints: currentVitalityPoints,
        creativityPoints: currentCreativityPoints,
        charismaPoints: currentCharismaPoints,
        workPoints: currentWorkPoints,
        socialPoints: currentSocialPoints,
        personalPoints: currentPersonalPoints,
    });
}

function AddTaskToHistory(taskToAddId, taskToAddLabel, taskToAddCategory, taskToAddAttribute, taskToAddIsDone, taskToAddPoints, taskToAddlevels) {
    $.post("/HistoryTasks/AddTaskToHistory", {
        taskId: taskToAddId,
        taskLabel: taskToAddLabel,
        taskCategory: taskToAddCategory,
        taskAttribute : taskToAddAttribute,
        isDone: taskToAddIsDone,
        points: taskToAddPoints,
        levels: JSON.stringify(taskToAddlevels)
    });
}

function RemoveTaskFromHistory(taskId, taskCategory) {

    $.post("/HistoryTasks/RemoveTaskFromHistory", {
        taskId: taskId,
        taskCategory: taskCategory
    });
}

window.onload = function () {
    $("#accordion").accordion({
        collapsible: true,
        active: false,
        heightStyle: "auto"
    });

    globalWorkRecursiveListViewModel = new RecursiveWorkListViewModel();
    ko.applyBindings(globalWorkRecursiveListViewModel, document.getElementById("WorkTasksContainer"));

    globalSocialRecursiveListViewModel = new RecursiveSocialListViewModel();
    ko.applyBindings(globalSocialRecursiveListViewModel, document.getElementById("SocialTasksContainer"));

    globalPersonalRecursiveListViewModel = new RecursivePersonalListViewModel();
    ko.applyBindings(globalPersonalRecursiveListViewModel, document.getElementById("PersonalTasksContainer"));

    //Create task Dialog

    progressBar($('#IntellegencePointsPercentage').val(), $('#progressBar_Intellegence'));
    progressBar($('#PerseverancePointsPercentage').val(), $('#progressBar_Perseverance'));
    progressBar($('#StrengthPointsPercentage').val(), $('#progressBar_Strenght'));
    progressBar($('#VitalityPointsPercentage').val(), $('#progressBar_Vitality'));
    progressBar($('#CreativityPointsPercentage').val(), $('#progressBar_Creativity'));
    progressBar($('#CharismaPointsPercentage').val(), $('#progressBar_Charisma'));

    //hide done tasks
    setTimeout(function () {
        $("#showHideTasks").prop('checked', true);
        showHideTasks();
        hideSommeSpam();
    }, 5000);

    function addTask() {
        //if (addTaskToParentId == -1) {
        //	globalWorkRecursiveListViewModel.addDialogTask($('#task-description').val(), $('#AttributeDropDow').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val());
        //}
        //else {
        //	globalWorkRecursiveListViewModel.addDialogSubTask($('#task-description').val(), $('#AttributeDropDow').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val());
        //}


        if (addTaskToParentId == -1) {
            switch (createTaskFor.toLowerCase()) {
                case "work":
                    globalWorkRecursiveListViewModel.addDialogTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                case "social":
                    globalSocialRecursiveListViewModel.addDialogTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                case "personal":
                    globalPersonalRecursiveListViewModel.addDialogTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                default:
                    break;
            }
        }
        else {
            switch (createTaskFor.toLowerCase()) {
                case "work":
                    globalWorkRecursiveListViewModel.addDialogSubTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                case "social":
                    globalSocialRecursiveListViewModel.addDialogSubTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                case "personal":
                    globalPersonalRecursiveListViewModel.addDialogSubTask($('#task-description').val(), $('#mySingleFieldAttributes').val(), $('#ImportanceDropDow').val(), $('#DifficultyDropDow').val(), $('#isRepetable').is(":checked"));
                    break;
                default:
                    break;
            }
        }

        dialog.dialog("close");
        //return valid;
    }

    function scheduleTask() {
        $.post("/WeekCalendar/AddTaskToWeek", {
            id: 0,
            taskId: DialogScheduleTask_TadkId,
            category: $("#dialog_taskCategory").val(),
            title: $("#dialog_taskDescription").val(),
            start: $("#dayToSchedule").val() + 'T' + $("#beginHour").val() + ':00.000+10:00',
            end: $("#dayToSchedule").val() + 'T' + $("#endHour").val() + ':00.000+10:00',
            isDone: false
        }, function (data) {
            dialogScheduleTask.dialog("close");
            var currSrc = $("#WeekCalenderIframe").attr("src");
            $("#WeekCalenderIframe").attr("src", currSrc);

        });
    }

    dialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 500,
        width: 500,
        modal: true,
        buttons: {
            "Create a Task": addTask,
            Cancel: function () {
                dialog.dialog("close");
            }
        },
        close: function () {
            //form[0].reset();
        }
    });

    //Schedule Task Dialog
    dialogScheduleTask = $("#dialog-form-addToWeek").dialog({
        autoOpen: false,
        height: 500,
        width: 500,
        modal: true,
        buttons: {
            "Schedule a Task": scheduleTask,
            Cancel: function () {
                dialogScheduleTask.dialog("close");
            }
        },
        close: function () {
            //form[0].reset();
        }
    });


    //form = dialog.find("form").on("submit", function (event) {
    //    event.preventDefault();
    //    //addUser();
    //    dialog.dialog("close");
    //});

    //$("#create-task").button().on("click", function () {
    $("#create-task").click( function () {
        createTaskFor = "work";
        dialog.dialog("open");
    });

    //$("#create-task-social").button().on("click", function () {
    $("#create-task-social").click( function () {
        createTaskFor = "social";
        dialog.dialog("open");
        $("#AttributeDropDow").val("Vitality");
    });

    //$("#create-task-personal").button().on("click", function () {
    $("#create-task-personal").click( function () {
        createTaskFor = "personal";
        dialog.dialog("open");
        $("#AttributeDropDow").val("Perseverance");
    });

    setTimeout(function () {
        $('.inputTaskText').autoGrowInput();
    }, 3000);

    //Tag-It
    //$("#myTags").tagit({

    //    // Options
    //    fieldName: "skills",
    //    availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"],
    //    autocomplete: { delay: 0, minLength: 2 },
    //    showAutocompleteOnFocus: false,
    //    removeConfirmation: false,
    //    caseSensitive: true,
    //    allowDuplicates: false,
    //    allowSpaces: false,
    //    readOnly: false,
    //    tagLimit: null,
    //    singleField: false,
    //    singleFieldDelimiter: ',',
    //    singleFieldNode: null,
    //    tabIndex: null,
    //    placeholderText: null,

    //    // Events
    //    beforeTagAdded: function (event, ui) {
    //        console.log(ui.tag);
    //    },
    //    afterTagAdded: function (event, ui) {
    //        console.log(ui.tag);
    //    },
    //    beforeTagRemoved: function (event, ui) {
    //        console.log(ui.tag);
    //    },
    //    onTagExists: function (event, ui) {
    //        console.log(ui.tag);
    //    },
    //    onTagClicked: function (event, ui) {
    //        console.log(ui.tag);
    //    },
    //    onTagLimitExceeded: function (event, ui) {
    //        console.log(ui.tag);
    //    }

    //});
    var sampleTags = ['Intelligence', 'Perseverance', 'Strength', 'Vitality', 'Creativity', 'Charisma'];

    $('#AttributesFields').tagit({
        availableTags: sampleTags,
        // This will make Tag-it submit a single form value, as a comma-delimited field.
        singleField: true,
        singleFieldNode: $('#mySingleFieldAttributes'),
        showAutocompleteOnFocus: true,
        allowDuplicates: false,
    });
    
}//]]> 


function hideSommeSpam()
{
    $('center').next().next().next().remove();
}

function showHideTasks() {
    if ($("#showHideTasks").prop('checked') == true) {
        $("input:disabled").parent().hide();
    }
    else {
        $("input:disabled").parent().show();
    }
}

var workTasksToDoTodayList = [];
var workTasksToDoTodayListCounter = 0;

var WorkTasksToDoTodayItems = [
    //new SideBarItemModel(1, "Dashboard", "glyphicon glyphicon-search", false, false, SideBarDashBoardClick),
    //new SideBarItemModel(2, "Apis", "glyphicon glyphicon-wrench", false, false, SideBarApisClick),
    //new SideBarItemModel(3, "Settings", "glyphicon glyphicon-cog", false, false, SideBarSettingsClick),
    //new SideBarItemModel(4, "Account", "glyphicon glyphicon-user", false, false, SideBarAccountClick)
];

function TasksToDoTodayItemModel(id, checkboxRef) {
    var self = this;

    self.id = id
    self.checkboxRef = checkboxRef;
}

function getworkTaskToDoTodayById(itemId) {
    return _.find(WorkTasksToDoTodayItems, function (item) { return item.id == itemId; });
}

function markThisWorkTaskToDoToday() {
    var element = $(event.target);
    var mainTaskDiv = element.parent();
    var clonedElement = mainTaskDiv.clone();

    clonedElement.children("a").remove();

    if (!clonedElement.find("input").is(':disabled')) {
        //check if already exists in "To Do Today Tasks"

        var realCheckbox = mainTaskDiv.find("input:checkbox");
        var clonedCheckbox = clonedElement.find("input:checkbox");
        if (workTasksToDoTodayList.indexOf(realCheckbox) > -1) {
            alert("task already exists in 'To Do Today Tasks");
        }

        clonedCheckbox.attr("data-toDoTodayIndex", workTasksToDoTodayListCounter);

        clonedElement.find("input:checkbox").on("change", function () {
            //alert($(this).attr("data-toDoTodayIndex"));
            //alert($(this).attr('checked'));
            var divParent = $(this).parent();
            var divParentIndex = divParent.index();
            var arrayIndex = $(this).attr("data-toDoTodayIndex");
            var isSelected = $(this).attr('checked');
            if (isSelected != undefined) {
                //then is checked

                var workTasksToDoTodayItem = getworkTaskToDoTodayById(arrayIndex);
                var offset = $(workTasksToDoTodayItem.checkboxRef).offset();
                var event = jQuery.Event("mousedown", {
                    which: 1,
                    pageX: offset.left,
                    pageY: offset.top
                });
                document.elementFromPoint(event.pageX, event.pageY).click();
                clonedElement.remove();

                //remove from To Do Today Tasks
                console.log(workTasksToDoTodayList.length);
                var indexOfArray = WorkTasksToDoTodayItems.indexOf(workTasksToDoTodayItem);
                //var indexOfArray = workTasksToDoTodayList.indexOf(workTasksToDoTodayList[divParentIndex]);
                if (indexOfArray > -1) {
                    WorkTasksToDoTodayItems.splice(indexOfArray, 1);
                }
            }
        });
        clonedElement.appendTo('#WorkTasksTodayContainer');

        //Reference to real checkbox
        WorkTasksToDoTodayItems.push(new TasksToDoTodayItemModel(workTasksToDoTodayListCounter, realCheckbox));
        workTasksToDoTodayListCounter = workTasksToDoTodayListCounter + 1;
    }
}

function simulatedClick(target, options) {

    var event = target.ownerDocument.createEvent('MouseEvents'),
        options = options || {};

    //Set your default options to the right of ||
    var opts = {
        type: options.type || 'click',
        canBubble: options.canBubble || true,
        cancelable: options.cancelable || true,
        view: options.view || target.ownerDocument.defaultView,
        detail: options.detail || 1,
        screenX: options.screenX || 0, //The coordinates within the entire page
        screenY: options.screenY || 0,
        clientX: options.clientX || 0, //The coordinates within the viewport
        clientY: options.clientY || 0,
        ctrlKey: options.ctrlKey || false,
        altKey: options.altKey || false,
        shiftKey: options.shiftKey || false,
        metaKey: options.metaKey || false, //I *think* 'meta' is 'Cmd/Apple' on Mac, and 'Windows key' on Win. Not sure, though!
        button: options.button || 0, //0 = left, 1 = middle, 2 = right
        relatedTarget: options.relatedTarget || null,
    }

    //Pass in the options
    event.initMouseEvent(
        opts.type,
        opts.canBubble,
        opts.cancelable,
        opts.view,
        opts.detail,
        opts.screenX,
        opts.screenY,
        opts.clientX,
        opts.clientY,
        opts.ctrlKey,
        opts.altKey,
        opts.shiftKey,
        opts.metaKey,
        opts.button,
        opts.relatedTarget
    );

    //Fire the event
    target.dispatchEvent(event);
}

var DialogScheduleTask_TadkId;

function ShowDialogScheduleTaskMain(taskText, category, taskId) {
    DialogScheduleTask_TadkId = taskId;
    dialogScheduleTask.dialog("open");
    $("#dialog_taskDescription").val(taskText);
    $("#dialog_taskCategory").val(category);
    $("#dayToSchedule").datepicker({ dateFormat: 'yy-mm-dd' });
    $("#beginHour").timepicker({ 'timeFormat': 'H:i' });
    $("#endHour").timepicker({ 'timeFormat': 'H:i' });
}

function GetTaskUpLevelsArray(task, category) {
    var levelsArray = [];
    if (task.parentId() != null && task.parentId() != undefined) {
        var parentTask = globalPersonalRecursiveListViewModel.getTaskById(task.parentId());

        switch (category) {
            case "personal":
                parentTask = globalPersonalRecursiveListViewModel.getTaskById(task.parentId());
                break;
            case "work":
                parentTask = globalWorkRecursiveListViewModel.getTaskById(task.parentId());
                break;
            case "social":
                parentTask = globalSocialRecursiveListViewModel.getTaskById(task.parentId());
                break;
        }

        while (true) {
            if (parentTask != undefined && parentTask != null) {
                levelsArray.push(parentTask.label());

                if (parentTask.parentId() != null && parentTask.parentId() != undefined) {
                    //parentTask = globalPersonalRecursiveListViewModel.getTaskById(parentTask.parentId());
                    switch (category) {
                        case "personal":
                            parentTask = globalPersonalRecursiveListViewModel.getTaskById(parentTask.parentId());
                            break;
                        case "work":
                            parentTask = globalWorkRecursiveListViewModel.getTaskById(parentTask.parentId());
                            break;
                        case "social":
                            parentTask = globalSocialRecursiveListViewModel.getTaskById(parentTask.parentId());
                            break;
                    }
                }
                else {
                    break;
                }
            }
            else {
                break;
            }
        }
        levelsArray.reverse();
    }
   
    return levelsArray;
}