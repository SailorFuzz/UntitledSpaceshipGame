Created by Christopher Olson
Arizona State University
CSE311

--------------------------------
Documents: 
contains the Design/Proposal document and a video showcasing gameplay

PublishedBuild:
Build file of the completed game.  Open the "FinalProject.exe" to play the game

FinalProject:
Contains all the source code, project solution, project files and content.  

GameEngine:
All the components and source code of the game engine.
----------------------------------

---------------------
UntitledSpaceshipGame
---------------------

1 Game Play
*	is 3D, in space, with a spaceship.  And you can shoot with it.  Also doesn't have a title so it's on brand with the name of the game
*	SoundEffects for shooting, hit marks, getting hit marks, and destroying ships
*	SoundEffects play a random sound from that library so sounds aren't monotonous
*	Music, put some chill tracks in for ambience and the music changes at later stages	
-------------
2 User Interface
*	Menu Screen houses 4 buttons to start the game, check the HowTo play, Credits or just Quit
*	HowTo is an animated menu scene of the game's controls and general features
*	Credits includes myself, the maker, and those responsible for this
*	Score menu appears inbetween each level for the player to see current score and continue to the next level
*	Death screen shows final score, and a return to menu
-----------
3 In-Game Help
*	There's a HowTo animated menu, on the main menu.  It can't be more explicit
------------
4 Level Design or Difficulty Progression
*	Enemy behavior is introduced every new level, sometimes its a new enemy, sometimes it's a different combination of enemies, sometimes it's a morph of a enemy, other times it's just a swarm of waves.
*	PowerUps are shown on howto but are introduced as random pickups in the game, their usage is evident through gameplay
*	Each level(scene) has a specific end state where the Score card is displayed, however there is a final end to the game if the player completes level 10.  Though the difficulty progression is high, since it's short 		game of only 10 levels.  Good Luck.
------------
5 Correctness and Completeness
*	No seen bugs that I didn't stomp on, anything you think is a bug is a feature
*	Player is forced inbounds to screen so they cannot leave the play view
*	I have tested it for hours because just trying to beat it is incredibly hard
-----------
6 Other stuff
*	Score is kept and points are awarded for hitting enemies(10pt), killing(100pts) and lost for shooting(-1pt) and over time (-1pt per second).  This is so the player makes more points by destorying the targets in the least amount of shots as possible and in the shortest time
*	Check that credits menu, pretty slick.
-----------
7 Game Video
*	Gameplay video is located inside of the Documentation folder
* 	It's about 4-5min long, and I died without beating it.  I probably made like 10 recordings trying to get a game completion, but it's actually pretty hard.  If you do it, send a screen shot.




8 Extra Effort
*	A bit of combination between game effect and math.  In the coding for the enemies, I used the vector positions of the enemeies and the player to achieve a rotation in the update.  Since Transform.Rotation is a Quanterion and not a vector, vector angles didn't cut it in the Transform.Rotate(axis,angle).  Instead I directly reassigned Quanterion by building a new one with a Matrix utlizing the two vectors (and a little manual rotation) to force the enemy ships to follow the player.  This makes it look as though the enemy ships (especially the ones that follow you around and maintain a distance from you) always look like they're flying in the right direction and toward/away from you.
*	Since the Diffuse color of the material is a vector, I was also able to create a "health" tinting effect on the players/enemies.  By using their original color and then multiplying in a percentage of a "red" vector into it based on hit points.
*	Music Added in, that wasn't shown in class, but I did it, because space games need space music