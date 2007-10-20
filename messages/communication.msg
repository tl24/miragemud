{
	"Namespace" : "msg:/communication/",
	"Messages" : {
		"say.others" :  new ResourceMessage ("Communication", "say.others")
		{ Template: "${player} says '${message}'.\r\n" },

		"say.self" :  new ResourceMessage ("Confirmation", "say.self")
		{ Template: "You said '${message}'.\r\n" },
		
		"tell.others" :  new ResourceMessage ("Communication", "tell.others")
		{ Template: "${player} tells you '${message}'.\r\n" },

		"tell.self" :  new ResourceMessage ("Confirmation", "tell.self")
		{ Template: "You tell ${target} '${message}'.\r\n" },
		
		"cant.join.channel" : new ResourceMessage ("PlayerError", "cant.join.channel")
		{ Template: "You're not allowed to join the ${channel} channel.\r\n" },
		
		"member.not.allowed" : new ResourceMessage( "Information", "member.not.allowed" )
		{ Template: "You are no longer allowed in ${channel} channel!\r\n" },
		
		"channel.text" : new ResourceMessage( "Communication", "channel.text" )
		{ Template: "[${channel}] ${sender}: ${message}" },
		
		"channel.on" : new ResourceMessage( "Confirmation", "channel.on" )
		{ Template: "Channel ${channel} is now on.\r\n" }

		"channel.off" : new ResourceMessage( "Confirmation", "channel.off" )
		{ Template: "Channel ${channel} is now off.\r\n" },
		
		"ignore.player" : new ResourceMessage( "Confirmation", "ignore.player" )
		{ Template: "You are now ignoring all communication from ${player}.\r\n" },

		"being.ignored" : new ResourceMessage( "PlayerError", "being.ignored" )
		{ Template: "${player} is ignoring you.\r\n" },
		
		"unignore.player" : new ResourceMessage( "Confirmation", "unignore.player" )
		{ Template: "You are no longer ignoring ${player}.\r\n" },
		
		"cant.ignore.self" : new StringMessage( "PlayerError", "cant.ignore.self", "You can't ignore yourself!\r\n" )
	}
}