{
	"Namespace" : "negotiation.authentication",
	"Messages" : {
		"PlayerAlreadyPlaying" :  new ResourceMessage ("PlayerError", "PlayerAlreadyPlaying")
		{ Template: "That player is already playing.\r\n" },
		
		"NewplayerPassword" : new ResourceMessage("Prompt", "NewplayerPassword")
		{ Template: "New character.\r\nEnter a password for ${player}: " },
		
		"ExistingplayerPassword" : new StringMessage("Prompt", "ExistingplayerPassword", "Password: "),

		"EnterName" :  new StringMessage ("Prompt", "EnterName", "Enter Name: "),
		"EnterAnotherName" :  new StringMessage ("Prompt", "EnterAnotherName", "Ok, what is it, then? "),

		"ConfirmNewName" : new ResourceMessage("Prompt", "ConfirmNewName")
		{ Template: "Is that right, ${player} (Y/N)? " }

		"ConfirmNewName" : new ResourceMessage("Prompt", "ConfirmNewName")
		{ Template: "Is that right, ${player} (Y/N)? " },
		
		"ErrorConfirmName" : new StringMessage("PlayerError", "ErrorConfirmName", "Please type Yes or No.\r\n"),
		
		"ConfirmPassword" :  new StringMessage ("Prompt", "ConfirmPassword", "Confirm password: "),
		
		"ErrorIllegalName" : new StringMessage("PlayerError", "ErrorIllegalName", "Illegal name, try another.\r\n"),

		"ErrorAlreadyPlaying" : new StringMessage("PlayerError", "ErrorAlreadyPlaying", "That player is already playing.  Try another name.\r\n"),

		"ErrorWrongPassword" : new StringMessage("PlayerError", "ErrorWrongPassword", "\r\nWrong Password\r\n"),

		"ErrorPasswordLength" : new StringMessage("PlayerError", "ErrorPasswordLength", "Password must be at least five characters long.\n\r"),
		"ErrorPasswordsDontMatch" : new StringMessage("PlayerError", "ErrorPasswordsDontMatch", "Passwords don't match.\r\n"),
		
		"Login" :  new StringMessage ("Confirmation", "Login", "Login Successful")		
		
	}
}