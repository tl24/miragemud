{
	"Namespace" : "common.error",
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
		"ObjectAlreadyOpen" : new StringMessage ("PlayerError", "ObjectAlreadyOpen", "It's already open.\r\n"),

		"ObjectAlreadyClosed" : new StringMessage ("PlayerError", "ObjectAlreadyClosed", "It's already closed.\r\n"),

		"NotOpenable" : new StringMessage ("PlayerError", "NotOpenable", "You can't open that.\r\n"),

		"NotCloseable" : new StringMessage ("PlayerError", "NotCloseable", "You can't close that.\r\n"),

		/* LockableAttribute */
		"CantLockWhenOpen" : new StringMessage ("PlayerError", "CantLockWhenOpen", "You can't lock that, it's not closed!\r\n"),

		"ObjectAlreadyLocked" : new StringMessage ("PlayerError", "ObjectAlreadyLocked", "It's already locked.\r\n"),

		"ObjectAlreadyUnlocked" : new StringMessage ("PlayerError", "ObjectAlreadyUnlocked", "It's already unlocked.\r\n"),

		"WrongKey" : new StringMessage ("PlayerError", "WrongKey", "You don't have the right key.\r\n"),

		"ObjectLocked" : new StringMessage ("PlayerError", "ObjectLocked", "You can't open that, it's locked.\r\n"),
		
		"SystemError" : new StringMessage ("SystemError", "SystemError", "A system error has occurred executing your command.\r\n")
	}
}