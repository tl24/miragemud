{
	"Namespace" : "communication",
	"Messages" : {
		"SayOthers" :  new ResourceMessage ("Information", "SayOthers")
		{ Template: "${player} says '${message}'.\r\n" },

		"SaySelf" :  new ResourceMessage ("Confirmation", "SaySelf")
		{ Template: "You said '${message}'.\r\n" },
		
		"TellOthers" :  new ResourceMessage ("Information", "TellOthers")
		{ Template: "${player} tells you '${message}'.\r\n" },

		"TellSelf" :  new ResourceMessage ("Confirmation", "TellSelf")
		{ Template: "You tell ${target} '${message}'.\r\n" },
		
		"CantJoinChannel" : new ResourceMessage ("PlayerError", "CantJoinChannel")
		{ Template: "You're not allowed to join the ${channel} channel.\r\n" },
		
		"MemberNotAllowed" : new ResourceMessage( "Information", "MemberNotAllowed" )
		{ Template: "You are no longer allowed in ${channel} channel!\r\n" },
		
		"ChannelText" : new ResourceMessage( "Information", "ChannelText" )
		{ Template: "[${channel}] ${sender}: ${message}" },
		
		"ChannelOn" : new ResourceMessage( "Confirmation", "ChannelOn" )
		{ Template: "Channel ${channel} is now on.\r\n" },

		"ChannelOff" : new ResourceMessage( "Confirmation", "ChannelOff" )
		{ Template: "Channel ${channel} is now off.\r\n" },
		
		"IgnorePlayer" : new ResourceMessage( "Confirmation", "IgnorePlayer" )
		{ Template: "You are now ignoring all communication from ${player}.\r\n" },

		"BeingIgnored" : new ResourceMessage( "PlayerError", "BeingIgnored" )
		{ Template: "${player} is ignoring you.\r\n" },
		
		"UnignorePlayer" : new ResourceMessage( "Confirmation", "UnignorePlayer" )
		{ Template: "You are no longer ignoring ${player}.\r\n" },
		
		"CantIgnoreSelf" : new StringMessage( "PlayerError", "CantIgnoreSelf", "You can't ignore yourself!\r\n" )
	}
}