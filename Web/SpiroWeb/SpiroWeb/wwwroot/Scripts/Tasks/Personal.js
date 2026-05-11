function ItemPersonalModel(id, parent_id, label, isDone, attribute, importance, difficulty, isRepeatable) {
    var self = this;

    self.id = ko.observable(id);
    self.parentId = ko.observable(parent_id);
    self.label = ko.observable(label);
    self.isDone = ko.observable(isDone);

    self.attribute = ko.observable(attribute);
    self.importance = ko.observable(importance);
    self.difficulty = ko.observable(difficulty);

    self.isRepeatable = ko.observable(isRepeatable);

    var subscriptionLabel = self.label.subscribe(function (newValue) {
        console.log("The task's new text is - " + newValue);
        globalPersonalRecursiveListViewModel.save();
    });

    var subscriptionDone = self.isDone.subscribe(function (newValue) {
        console.log("The task's new Done Status is - " + newValue);

        var levelsArray = [];
        if (newValue == true) {
            levelsArray = GetTaskUpLevelsArray(self, "personal");
            var pointsToGain = parseInt(self.importance()) * parseInt(self.difficulty());

            //TEMP
            var taskId = self.id();
            if (taskId.toString().startsWith('12811')) {
                taskId = "0";
            }

            UpdatePoints(taskId, self.attribute(), self.importance(), self.difficulty(), "personal");
            AddTaskToHistory(taskId, self.label(), "personal", self.attribute(), true, pointsToGain, levelsArray);
        }
        else {
            if (self.isRepeatable() == undefined || self.isRepeatable() == false) {
                //TEMP
                var taskId = self.id();
                if (taskId.toString().startsWith('12811')) {
                    taskId = "0";
                }

                DeUpdatePoints(taskId, self.attribute(), self.importance(), self.difficulty(), "personal");
                RemoveTaskFromHistory(self.id(), "personal");
            }
        }

        //save
        if (self.isRepeatable() == undefined || self.isRepeatable() == false)
            globalPersonalRecursiveListViewModel.save();
    });
}



function RecursivePersonalListViewModel(tasks) {
    var self = this;

    self.personalItems = ko.observableArray(tasks);
    self.newTaskText = ko.observable();

    self.subitemsOf = function (item) {
        var children = ko.utils.arrayFilter(self.personalItems(), function (arrayItem) {
            var parentItemId = (null === item) ? null : item.id();
            return arrayItem.parentId() == parentItemId;
        });

        return children;
    };

    self.hasSubitems = function (item) {
        var firstMatch = ko.utils.arrayFirst(self.personalItems(), function (arrayItem) {
            return (arrayItem.parentId() == item.id());
        });

        return (null !== firstMatch); // At least one item found in array
    };

    self.removeTask = function (item) {

        $("#dialog-confirm").dialog({
            resizable: false,
            height: 250,
            modal: true,
            buttons: {
                "Delete all items": function () {
                    //$(this).dialog("close");
                    self.removeTaskWithParentID(item);
                    self.personalItems.remove(item);
                    self.save();
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });

        //self.removeTaskWithParentID(item);
        //self.personalItems.remove(item);
        //self.save();
    };

    self.removeTaskWithParentID = function (item) {
        var taskIdsToDelete = [];

        //Find all personalItems to delete
        for (var i = 0; i < self.personalItems().length; i++) {
            if (self.personalItems()[i].parentId() == item.id()) {

                taskIdsToDelete[taskIdsToDelete.length] = self.personalItems()[i].id();
                self.removeTaskWithParentID(self.personalItems()[i]);
            }
        }

        //delete all tasks with Id
        for (var i = 0; i < taskIdsToDelete.length; i++) {

            var task = self.getTaskById(taskIdsToDelete[i]);
            self.personalItems.remove(task);
        }
    };

    self.getTaskById = function (taskId) {
        return _.find(self.personalItems(), function (item) {
            return item.id() == taskId;
        });
    };

    self.getChildTasksByParentId = function (parentId) {
        //return _.find(self.personalItems(), function (item) {
        return _.filter(self.personalItems(), function (item) {
            return item.parentId() == parentId;
        });
    };

    self.addTask = function () {
        var nextId = String(parseInt(personalItems[personalItems.length - 1].id()) + 1);
        console.log(nextId.toString());
        //self.personalItems.push(new ItemPersonalModel(nextId, null, this.newTaskText(), false));
        self.personalItems.push(new ItemPersonalModel(nextId, null, this.newTaskText(), false, "intelligence", 2, 2));
        //alert(this.newTaskText() + " With Id = " + nextId);
        self.newTaskText("");
        $('.inputTaskText').autoGrowInput();

        self.save();
    };

    self.addSubTask = function (item) {
        var nextId = String(parseInt(personalItems[personalItems.length - 1].id()) + 1);
        var newItemPersonalModel = new ItemPersonalModel(nextId, item.id(), "", false, item.attribute(), item.importance(), item.difficulty());

        self.personalItems.push(newItemPersonalModel);

        //$('input:text').autoGrowInput();
        //var subscription = newItemPersonalModel.label.subscribe(function (newValue) {
        //	//debugger
        //	console.log("The task's new text is - " + newValue);
        //});
    };

    self.addDialogTask = function (description, attr, imp, diff, isRepeatable) {
        var nextId = String(parseInt(personalItems[personalItems.length - 1].id()) + 1);
        self.personalItems.push(new ItemPersonalModel(nextId, null, description, false, attr, imp, diff, isRepeatable));
        $('.inputTaskText').autoGrowInput();

        self.save();
    }

    self.showDialogAddSubTask = function (item) {
        addTaskToParentId = item.id();
        createTaskFor = "personal";
        dialog.dialog("open");
    }

    self.showDialogScheduleTask = function (item) {
        scheduleTaskId = item.id();
        ShowDialogScheduleTaskMain(item.label(), "personal", scheduleTaskId);
    }

    self.addDialogSubTask = function (description, attr, imp, diff, isRepeatable) {
        if (addTaskToParentId == -1) return;

        var nextId = String(parseInt(personalItems[personalItems.length - 1].id()) + 1);
        self.personalItems.push(new ItemPersonalModel(nextId, addTaskToParentId, description, false, attr, imp, diff, isRepeatable));

        addTaskToParentId = -1;
        $('.inputTaskText').autoGrowInput();

        self.save();
    }

    self.save = function () {
        $.post("/TasksManager/SavePersonalTasks", { jsonItems: ko.toJSON(self.personalItems) });
    };

    self.isDoneTask = function () {
        var context = ko.contextFor(this);
        if (this.isRepeatable() != undefined && this.isRepeatable() == true && this.isDone() == true) {
            this.isDone(false);
            return false;
        }
        else {
            return true;
        }
    };

    //Load initial state from server, convert it to Task instances, then populate self.tasks
    $.getJSON("/TasksManager/GetPersonalTasks", function (allData) {
        var receivedTasks = JSON.parse(allData);
        personalItems = [];

        var mappedTasks = $.map(receivedTasks, function (item) {
            return new ItemPersonalModel(item.id, item.parentId, item.label, item.isDone, item.attribute, item.importance, item.difficulty, item.isRepeatable);
        });

        personalItems = mappedTasks;
        self.personalItems(mappedTasks);

        $('.inputTaskText').autoGrowInput();

        $('.ui-button-text').removeClass("ui-button-text");
    });
}