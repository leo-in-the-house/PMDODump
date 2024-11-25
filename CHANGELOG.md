# 0.8.7 Changes #

* Friend Rescues will tell you the destination floor
* Minor tweaks to trap spawning
* Minor display fix for evo moves
* Partial fix for music spoilering (tracks will be renamed next version to exclude numbering in the title)
* RC: Fixed various boss room settings for Nature Power
* Defeated enemies are no longer affected by Magnet Pull
* Dev: Added support for passive events from the tiles the player stands on
* Dev: Added support for InteractOrder, to control which ground entities are interacted with first if they are both being touched/interacted with
* Dev: GAME:FadeOutFront and GAME:FadeInFront, for fading in front of textbox and menu elements
* Dev: Added debug UI for Mouse Coord in Textbox/menu UI space
* Dev: Fixed Spawnlist editor crash
* Dev: Improved dungeon spawn editor visuals
* Dev: Moved traps away from the pokemon area in the test floor
* Dev: RandomRoomSpawnStep has changed its constructor signature to include a successPercent argument.  Recheck any lua scripts that may call this.