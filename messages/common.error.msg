{
	"Namespace" : "msg:/common/error/",
	"Messages" : {
		"InvalidActor" :  new ResourceMessage ("PlayerError", "InvalidActor")
		{ Template: "This command can not be executed by a ${ActorType}.\r\n" },
		
		"InvalidCommand" :  new ResourceMessage ("PlayerError", "InvalidCommand")
		{ Template: "Huh?\r\n" },
		
		"NotHere" :  new ResourceMessage ("PlayerError", "NotHere")
		{ Template: "${target} is not here.\r\n" },
		
		"PlayerNotPlaying" :  new ResourceMessage ("PlayerError", "PlayerNotPlaying")
		{ Template: "${player} is not playing right now.\r\n" },
		
		/* OpenableAttribute */
		"object.already.open" : new StringMessage ("PlayerError", "object.already.open", "It's already open.\r\n"),

		"object.already.closed" : new StringMessage ("PlayerError", "object.already.closed", "It's already closed.\r\n"),

		"not.openable" : new StringMessage ("PlayerError", "not.openable", "You can't open that.\r\n"),

		"not.closeable" : new StringMessage ("PlayerError", "not.closeable", "You can't close that.\r\n"),

		/* LockableAttribute */
		"cant.lock.when.open" : new StringMessage ("PlayerError", "cant.lock.when.open", "You can't lock that, it's not closed!\r\n"),

		"object.already.locked" : new StringMessage ("PlayerError", "object.already.locked", "It's already locked.\r\n"),

		"object.already.unlocked" : new StringMessage ("PlayerError", "object.already.unlocked", "It's already unlocked.\r\n"),

		"wrong.key" : new StringMessage ("PlayerError", "wrong.key", "You don't have the right key.\r\n"),

		"object.locked" : new StringMessage ("PlayerError", "object.locked", "You can't open that, it's locked.\r\n"),
		
		"system.error" : new StringMessage ("SystemError", "system.error", "A system error has occurred executing your command.\r\n")
	}
}