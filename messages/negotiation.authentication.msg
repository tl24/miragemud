{
	"Namespace" : "msg:/negotiation/authentication/",
	"Messages" : {
		"player.already.playing" :  new ResourceMessage ("PlayerError", "player.already.playing")
		{ Template: "That player is already playing.\r\n" },
		
		"newplayer.password" : new ResourceMessage("Prompt", "newplayer.password")
		{ Template: "New character.\r\nEnter a password for ${player}: " },
		
		"existingplayer.password" : new StringMessage("Prompt", "existingplayer.password", "Password: "),

		"enter.name" :  new StringMessage ("Prompt", "enter.name", "Enter Name: "),
		"enter.another.name" :  new StringMessage ("Prompt", "enter.another.name", "Ok, what is it, then? "),

		"confirm.new.name" : new ResourceMessage("Prompt", "confirm.new.name")
		{ Template: "Is that right, ${player} (Y/N)? " }

		"confirm.new.name" : new ResourceMessage("Prompt", "confirm.new.name")
		{ Template: "Is that right, ${player} (Y/N)? " },
		
		"error.confirm.name" : new StringMessage("PlayerError", "error.confirm.name", "Please type Yes or No.\r\n"),
		
		"confirm.password" :  new StringMessage ("Prompt", "confirm.password", "Confirm password: "),
		
		"error.illegal.name" : new StringMessage("PlayerError", "error.illegal.name", "Illegal name, try another.\r\n"),

		"error.already.playing" : new StringMessage("PlayerError", "error.already.playing", "That player is already playing.  Try another name.\r\n"),

		"error.wrong.password" : new StringMessage("PlayerError", "error.wrong.password", "\r\nWrong Password\r\n"),

		"error.password.length" : new StringMessage("PlayerError", "error.password.length", "Password must be at least five characters long.\n\r"),
		"error.passwords.dont.match" : new StringMessage("PlayerError", "error.passwords.dont.match", "Passwords don't match.\r\n"),
		
		"Login" :  new StringMessage ("Confirmation", "Login", "Login Successful")		
		
	}
}