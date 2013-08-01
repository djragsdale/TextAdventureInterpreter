TextAdventureInterpreter
========================

An interactive command interpreter for text adventures

========================

The goal of this project is to create an interpreter for a Text Adventure creator. Ideally this 
will read available commands (verbs), objects (nouns following non-moving actions), rooms (nouns 
following moving actions), paths (connections between rooms), and requirements (objects required 
specified command) from a data source. While this uses the command line currently, the system 
will eventually be implemented in a GUI format. The GUI will have a grid to select a room, a 
listbox to see the objects in the room, an object editor to create/edit/delete objects, a 
command center to create commands, and a requirement editor for creating command requirements. 
Paths will initially just be adjoining rooms, but reading from a data source will allow shortcuts 
to be created down the road.
