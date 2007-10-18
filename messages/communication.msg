{
	"Namespace" : "msg:/communication/",
	"Messages" : {
		"say.others" :  new ResourceMessage ("Communication", "say.others")
		{ Template: "${player} says '${message}'." },

		"say.self" :  new ResourceMessage ("Confirmation", "say.self")
		{ Template: "You said '${message}'." },
		
		"tell.others" :  new ResourceMessage ("Communication", "tell.others")
		{ Template: "${player} tells you '${message}'." },

		"tell.self" :  new ResourceMessage ("Confirmation", "tell.self")
		{ Template: "You tell ${target} '${message}'." }
	}
}