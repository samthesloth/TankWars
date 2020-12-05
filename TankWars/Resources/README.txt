Tank Wars Client And Server Project made by Nicholas Vaskelis and Sam Peters for CS3500 December 2020.

============================
Welcome to Bootleg Tank Wars
============================

==================
CLIENT INFORMATION
==================

======
The UI
======

Top Panel:
- You should see two interactable boxes, one called "Name" and one called "Host".
- You may put a name of up to 16 characters in the "Name" box.
- You may put a website URL or IP Address into the "Host" box with no length restriction.
- Once you have a name entered with a valid host name, you will connect to the server and the game will be displayed on the panel below.
- If the host name is invalid, you will have to relaunch the application and enter in a proper host name.

Game Panel:
- The game is played in this panel. All mouse interactions must be done in this panel.
- If the server goes down when playing, the game will freeze, indicating a disconnect. You can exit out of the application safely if this happens.

========
Controls
========

Movement:
- W to move up.
- S to move down.
- A to move left.
- D to move right.

Shooting:
- Left Mouse Button to fire normal attack.
- Right Mouse Button to fire special attack.

Aiming:
- Move mouse on Game Panel to aim your tank.

============
Game Objects
============
- The red dots on the map are the laser power-up.
- Once you have one, press RMB to fire it.

===================
Other Client Things
===================
- We used some of the given assets for tanks, projectiles, and walls. The animations (beam and death) are both hand-made.
- Connection errors will not crash the game, however they will cause it to hang, meaning that you have to restart the application in order to continue using it.

==================
SERVER INFORMATION
==================
- Allows connection from multiple clients.
- Receives and processes commands from clients.
- Controls game logic, taking in client commands.
- Implements all of the above features--walls, projectiles, tanks, etc--for the server-side.
- Updates world every frame, sending the world to all of the clients each frame.
- Accounts for client disconnections.
- Uses dictionaries and ID's for storing most of the game components.

============
Settings.xml
============
- Allows basic changes to the server:
- The basic UniverseSize, MSPerFrame, RespawnRate, and FramesPerShot.
- Allows modification to ShotSpeed, PowerUpRespawnTime, TankSpeed, and MaxPowerUps.
- Allows user to set coordinates for the walls of the world.
- If any settings are missing, they will be set to a default value.

==========
Game Logic
==========
- Spawn/Respawn tanks and powerups.
- Moves tanks and projectiles.
- Handle collisions between the objects in the game.
- Prevents projectiles from going outside of the game world.
- Wraps players around game world.
- Destroys tanks, beams, powerups, and projectiles when necessary.
- Uses given raytracing method for beam collision.
- Stores number of beams each tank has.

======
Issues
======
- It is very hard to not rapid fire beams if you use our client. We tried many ways of remedying this with an extra boolean and whatnot, but at the end it just didn't end up working.
- With a high number of players in the server (8+) there is some slight flickering with the graphics.
- Ids for objects that aren't tanks are just progressive, once enough objects have spawned, the id will overflow to negative. (only happens after a LONG time for obvious reasons)