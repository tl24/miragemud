This is a MUD framework written in C#.

Features:
  * Multiple Client Types supported (text client, builder client)
  * [Graphical Area Editor](GraphicalEditor.md)
  * Easy command creation.  [Commands](Commands.md) are just methods decorated with attributes.  Argument parsing is unnecessary in most cases.  The framework handles it for you.
  * Area and Player File persistence
  * Multi-threaded I/O
  * Core Services layer that is separate from content and can be used independently
  * Modular design, making it easy to swap out sections of code

Proposed Features:
  * Room commands: certain rooms provide extra commands to the players in the room
  * Mob commands: similar to Room commands except provided by mobs.
  * Object Attributes: Objects, Mobs, etc.  can have any number of attributes attached to them to alter their behavior and that can be detected by commands.  Examples include: items that are Containers, Lockable, Openable, etc.
  * Race inheritance: Race hierarchy can be used for building mobiles that defines default values, skills, etc.  Example: Dragon --> Red Dragon.


News:
  * 1/6/2013 - I've started up coding on this project again and work is moving along nicely
