{
	"Namespace" : "msg:/system/",
	"Messages" : {
		"EchoOn" :  new StringMessage ("UIControl",   "EchoOn",  "\u001b[0m" ),
		"EchoOff" : new StringMessage ("UIControl",   "EchoOff", "\u001b[0;30;40m" ),
		"goodbye" :	new StringMessage ("Information", "goodbye", "Goodbye!\r\n" ),
		"confirmation.prompt" : new StringMessage ("Prompt", "confirmation.prompt", "Are you sure? (y\\n) " ),
		"confirmation.cancel" : new StringMessage ("Information", "confirmation.cancel", "Command cancelled." )
		
	}
}