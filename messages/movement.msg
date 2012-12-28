{
	"Namespace" : "movement",
	"Messages" : {
		"PlayerArrival" : new ResourceMessage("Information", "PlayerArrival")
		{ Template: "${player} has arrived.\r\n" },
		
		"PlayerLeavesDirection" : new ResourceMessage("Information", "PlayerLeavesDirection")
		{ Template: "${player} leaves ${direction}.\r\n" },

		"YouLeaveDirection" : new ResourceMessage("Information", "YouLeaveDirection")
		{ Template: "You go ${direction}.\r\n" },

		
		"AnonymousCloseDoor" :  new ResourceMessage ("Information", "AnonymousCloseDoor")
		{ Template: "A door closes.\r\n" },

		"AnonymousOpenDoor" :  new ResourceMessage ("Information", "AnonymousOpenDoor")
		{ Template: "A door opens.\r\n" },
		
		"PlayerCloseDoorOthers" :  new ResourceMessage ("Information", "PlayerCloseDoorOthers")
		{ Template: "${player} closes the ${direction} door.\r\n" },

		"PlayerCloseDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerCloseDoorSelf")
		{ Template: "You close the ${direction} door.\r\n" },

		"PlayerOpenDoorOthers" :  new ResourceMessage ("Information", "PlayerOpenDoorOthers")
		{ Template: "${player} opens the ${direction} door.\r\n" },

		"PlayerOpenDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerOpenDoorSelf")
		{ Template: "You open the ${direction} door.\r\n" },

		"PlayerUnlockDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerUnlockDoorSelf")
		{ Template: "You unlock the ${direction} door.\r\n" },

		"PlayerUnlockDoorOthers" :  new ResourceMessage ("Information", "PlayerUnlockDoorOthers")
		{ Template: "${player} unlocks the ${direction} door.\r\n" },

		"PlayerLockDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerLockDoorSelf")
		{ Template: "You lock the ${direction} door.\r\n" },

		"PlayerLockDoorOthers" :  new ResourceMessage ("Information", "PlayerLockDoorOthers")
		{ Template: "${player} locks the ${direction} door.\r\n" },
		
		"InvalidDirection" : new StringMessage("PlayerError", "InvalidDirection", "That's not a valid direction.\r\n"),
		
		"NotInRoom" : new StringMessage("PlayerError", "NotInRoom", "You aren't in a room.\r\nYou need to get out first.\r\n"),
	
		"NoExit" : new StringMessage("PlayerError", "NoExit", "There's no door in that direction.\r\n"),
		
		"CantGoExit" : new StringMessage("PlayerError", "CantGoExit", "You can't go that way.\r\n"),
		
		"NotADoor" : new StringMessage("PlayerError", "NotADoor", "The exit in that direction does not have a door.\r\n"),
		
		"DoorAlreadyOpen" : new StringMessage("PlayerError", "DoorAlreadyOpen", "The door is already open.\r\n"),
		
		"DoorAlreadyClosed" : new StringMessage("PlayerError", "DoorAlreadyClosed", "The door is already closed.\r\n"),
		
		"DoorLocked" : new StringMessage("PlayerError", "DoorLocked", "The door is locked.\r\n"),
		
		"DoorClosed" : new StringMessage("PlayerError", "DoorClosed", "The door is closed.\r\n"),
		
		"DoorNotLockable" : new StringMessage("PlayerError", "DoorNotLockable", "The exit in that direction can not be locked.\r\n")

	}
}