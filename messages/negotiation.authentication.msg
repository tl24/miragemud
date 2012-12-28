{
	"Namespace" : "negotiation.authentication",
	"Messages" : {
		"PlayerAlreadyPlaying" : (ResourceMessage)
		{ MessageType: "PlayerError", 
		  Name: "PlayerAlreadyPlaying", 
		  Template: "That player is already playing.\r\n" 
		},
		
		"NewplayerPassword" : (ResourceMessage)
		{ MessageType: "Prompt", Name: "NewplayerPassword", Template: "New character.\r\nEnter a password for ${player}: " },
		
		"ExistingplayerPassword" : (StringMessage) { 
			MessageType: "Prompt", 
			Name: "ExistingplayerPassword", 
			Text: "Password: "
		},

		"EnterName" : (StringMessage) { 
			MessageType: "Prompt", 
			Name: "EnterName", 
			Text: "Enter Name: "
		},
		"EnterAnotherName" : (StringMessage) { 
			MessageType: "Prompt", 
			Name: "EnterAnotherName", 
			Text: "Ok, what is it, then? "
		},
		
		"ConfirmNewName" : (ResourceMessage)
		{ MessageType: "Prompt", 
		  Name: "ConfirmNewName", 
		  Template: "Is that right, ${player} (Y/N)? " 
		},
					
		"ErrorConfirmName" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorConfirmName", 
			Text: "Please type Yes or No.\r\n"
		},
				
		"ConfirmPassword" : (StringMessage) { 
			MessageType: "Prompt", 
			Name: "ConfirmPassword", 
			Text: "Confirm password: "
		},
		
		"ErrorIllegalName" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorIllegalName", 
			Text: "Illegal name, try another.\r\n"
		},
		
		"ErrorAlreadyPlaying" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorAlreadyPlaying", 
			Text: "That player is already playing.  Try another name.\r\n"
		},
		
		"ErrorWrongPassword" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorWrongPassword", 
			Text: "\r\nWrong Password\r\n"
		},
		
		"ErrorPasswordLength" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorPasswordLength", 
			Text: "Password must be at least five characters long.\n\r"
		},
				
		"ErrorPasswordsDontMatch" : (StringMessage) { 
			MessageType: "PlayerError", 
			Name: "ErrorPasswordsDontMatch", 
			Text: "Passwords don't match.\r\n"
		},
				
		"Login" : (StringMessage) { 
			MessageType: "Confirmation", 
			Name: "Login", 
			Text: "Login Successful"
		}
	}
}