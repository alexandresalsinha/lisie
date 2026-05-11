function ItemModel(id, parent_id, label, isDone, attribute, importance, difficulty, isRepeatable) {
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
		globalWorkRecursiveListViewModel.save();
	});

	var subscriptionDone = self.isDone.subscribe(function (newValue) {
		console.log("The task's new Done Status is - " + newValue);

		if (newValue == true) {

		    var levelsArray = [];
		    levelsArray = GetTaskUpLevelsArray(self, "work");
		    var pointsToGain = parseInt(self.importance()) * parseInt(self.difficulty());

		    UpdatePoints(self.id(), self.attribute(), self.importance(), self.difficulty(), "work");
		    AddTaskToHistory(self.id(), self.label(), "work", self.attribute(), true, pointsToGain, levelsArray);
		}
		else {
		    if (self.isRepeatable() == undefined || self.isRepeatable() == false) {
		        DeUpdatePoints(self.id(), self.attribute(), self.importance(), self.difficulty(), "work");
		        RemoveTaskFromHistory(self.id(), "work");
		    }
		}
	    //save
		if (self.isRepeatable() == undefined || self.isRepeatable() == false)
		    globalWorkRecursiveListViewModel.save();
	});
}

function RecursiveWorkListViewModel(tasks) {
	var self = this;

	self.items = ko.observableArray(tasks);
	self.newTaskText = ko.observable();

	self.subitemsOf = function (item) {
		var children = ko.utils.arrayFilter(self.items(), function (arrayItem) {
			var parentItemId = (null === item) ? null : item.id();
			return arrayItem.parentId() == parentItemId;
		});

		return children;
	};

	self.hasSubitems = function (item) {
		var firstMatch = ko.utils.arrayFirst(self.items(), function (arrayItem) {
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
					self.items.remove(item);
					self.save();
					$(this).dialog("close");
				},
				Cancel: function () {
					$(this).dialog("close");
				}
			}
		});

		//self.removeTaskWithParentID(item);
		//self.items.remove(item);
		//self.save();
	};

	self.removeTaskWithParentID = function (item) {
		var taskIdsToDelete = [];

		//Find all items to delete
		for (var i = 0; i < self.items().length; i++) {
			if (self.items()[i].parentId() == item.id()) {

				taskIdsToDelete[taskIdsToDelete.length] = self.items()[i].id();
				self.removeTaskWithParentID(self.items()[i]);
			}
		}

		//delete all tasks with Id
		for (var i = 0; i < taskIdsToDelete.length; i++) {

			var task = self.getTaskById(taskIdsToDelete[i]);
			self.items.remove(task);
		}
	};

	self.getTaskById = function (taskId) {
		return _.find(self.items(), function (item) {
			return item.id() == taskId;
		});
	};

	self.getChildTasksByParentId = function (parentId) {
		//return _.find(self.items(), function (item) {
		return _.filter(self.items(), function (item) {
			return item.parentId() == parentId;
		});
	};

	self.doTaskToday = function(item) {

	}

	self.addTask = function () {
        var nextId = String(parseInt(items[items.length - 1].id()) + 1);
		//self.items.push(new ItemModel(nextId, null, this.newTaskText(), false));
		self.items.push(new ItemModel(nextId, null, this.newTaskText(), false, "intelligence", 2, 2));
		//alert(this.newTaskText() + " With Id = " + nextId);
		self.newTaskText("");
		$('.inputTaskText').autoGrowInput();

		self.save();
	};

	self.addSubTask = function (item) {
	    var nextId = String(parseInt(items[items.length - 1].id()) + 1);
		var newItemModel = new ItemModel(nextId, item.id(), "", false, item.attribute(), item.importance(), item.difficulty());

		self.items.push(newItemModel);

		//$('input:text').autoGrowInput();
		//var subscription = newItemModel.label.subscribe(function (newValue) {
		//	//debugger
		//	console.log("The task's new text is - " + newValue);
		//});
	};

	self.addDialogTask = function (description, attr, imp, diff, isRepeatable) {
	    var nextId = String(parseInt(items[items.length - 1].id()) + 1);
		self.items.push(new ItemModel(nextId, null, description, false, attr, imp, diff, isRepeatable));
		$('.inputTaskText').autoGrowInput();

		self.save();
	}

	self.showDialogAddSubTask = function (item) {
		addTaskToParentId = item.id();
		createTaskFor = "work";
		dialog.dialog("open");
	}

	self.addDialogSubTask = function (description, attr, imp, diff, isRepeatable) {
		if (addTaskToParentId == -1) return;

        var nextId = String(parseInt(items[items.length - 1].id()) + 1);
		self.items.push(new ItemModel(nextId, addTaskToParentId, description, false, attr, imp, diff, isRepeatable));

		addTaskToParentId = -1;
		$('.inputTaskText').autoGrowInput();

		self.save();
	}

	self.showDialogScheduleTask = function (item) {
	    scheduleTaskId = item.id();
	    ShowDialogScheduleTaskMain(item.label(), "work", scheduleTaskId);
	}

	self.save = function () {
		$.post("/TasksManager/SaveTasks", { jsonItems: ko.toJSON(self.items) });
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
	$.getJSON("/TasksManager/GetTasks", function (allData) {
		var receivedTasks = JSON.parse(allData);
		items = [];

		var mappedTasks = $.map(receivedTasks, function (item) {
		    return new ItemModel(item.id, item.parentId, item.label, item.isDone, item.attribute, item.importance, item.difficulty, item.isRepeatable);
		});

		items = mappedTasks;
		self.items(mappedTasks);

		//$('input:text').autoGrowInput();

		$('.ui-button-text').removeClass("ui-button-text");
	});
}