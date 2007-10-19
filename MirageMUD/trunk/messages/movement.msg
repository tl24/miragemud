{
	"Namespace" : "msg:/movement/",
	"Messages" : {
		"anonymous.close.door" :  new ResourceMessage ("Information", "anonymous.close.door")
		{ Template: "A door closes.\r\n" },

		"anonymous.open.door" :  new ResourceMessage ("Information", "anonymous.open.door")
		{ Template: "A door opens.\r\n" },
		
		"player.close.door.others" :  new ResourceMessage ("Information", "player.close.door.others")
		{ Template: "${player} closes the ${direction} door.\r\n" },

		"player.close.door.self" :  new ResourceMessage ("Confirmation", "player.close.door.self")
		{ Template: "You close the ${direction} door.\r\n" },

		"player.open.door.others" :  new ResourceMessage ("Information", "player.open.door.others")
		{ Template: "${player} opens the ${direction} door.\r\n" },

		"player.open.door.self" :  new ResourceMessage ("Confirmation", "player.open.door.self")
		{ Template: "You open the ${direction} door.\r\n" },

		"player.unlock.door.self" :  new ResourceMessage ("Confirmation", "player.unlock.door.self")
		{ Template: "You unlock the ${direction} door.\r\n" },

		"player.unlock.door.others" :  new ResourceMessage ("Information", "player.unlock.door.others")
		{ Template: "${player} unlocks the ${direction} door.\r\n" },

		"player.lock.door.self" :  new ResourceMessage ("Confirmation", "player.lock.door.self")
		{ Template: "You lock the ${direction} door.\r\n" },

		"player.lock.door.others" :  new ResourceMessage ("Information", "player.lock.door.others")
		{ Template: "${player} locks the ${direction} door.\r\n" },
		
		"invalid.direction" : new StringMessage("PlayerError", "invalid.direction", "That's not a valid direction.\r\n"),
		
		"not.in.room" : new StringMessage("PlayerError", "not.in.room", "You aren't in a room.\r\nYou need to get out first.\r\n"),
	
		"no.exit" : new StringMessage("PlayerError", "no.exit", "There's no door in that direction.\r\n"),
		
		"cant.go.exit" : new StringMessage("PlayerError", "cant.go.exit", "You can't go that way.\r\n"),
		
		"not.a.door" : new StringMessage("PlayerError", "not.a.door", "The exit in that direction does not have a door.\r\n"),
		
		"door.already.open" : new StringMessage("PlayerError", "door.already.open", "The door is already open.\r\n"),
		
		"door.already.closed" : new StringMessage("PlayerError", "door.already.closed", "The door is already closed.\r\n"),
		
		"door.locked" : new StringMessage("PlayerError", "door.locked", "The door is locked.\r\n"),
		
		"door.closed" : new StringMessage("PlayerError", "door.closed", "The door is closed.\r\n"),
		
		"door.not.lockable" : new StringMessage("PlayerError", "door.not.lockable", "The exit in that direction can not be locked.\r\n")
	}
}