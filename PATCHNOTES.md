## CyberBlitz V1.0 `1.0`

The first version using our new, C# Unity based server. This is a complete rewrite of the game, both client and server. We planned this version extensively before starting and the previous work from the prototype was very helpful.

`1.0` is not in a playable state like the last prototype version, but
it has all the building blocks needed to create a gameloop. The next step
is to finish all core parts and make a playable build.

No live server deployment has been setup yet, so you have to start
the server locally before starting the client game.

[Download client and server](https://changemakereducation-my.sharepoint.com/personal/joel_fallbom_futuregames_nu/_layouts/15/onedrive.aspx?originalPath=aHR0cHM6Ly9jaGFuZ2VtYWtlcmVkdWNhdGlvbi1teS5zaGFyZXBvaW50LmNvbS86ZjovZy9wZXJzb25hbC9qb2VsX2ZhbGxib21fZnV0dXJlZ2FtZXNfbnUvRWwyd1FvWnFTQlpGcWZ0SWNKbEZXN1FCM1BLVlRZMlI2a01oMDA4d3Y1cjh2QT9ydGltZT1NbTVuc1hHSzJVZw&id=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds%2FGP3%5FT7%5FCyberBlitz%5FAlpha%5FV1%2E0%2DServerAndClientWin64%2Ezip&parent=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds)

Commit `bbfa8fb4dd7c2d600dddca2babe7c53115d8fda3`

## V1.2 `Prototype V1.2`

**New features**
* Units can now shoot and do damage to the other teams units (Insta kill)
* Fire-rate implemented for units
* Play against bot option added to player list
* Obsticles have been added (Prevents you from walking over them and prevents shots to go through them)
* First version of Fog of War have been implemented

**Fixes**
* Fixed crutial bug preventing multible games on the server
* Fixed new Ghost bug when shooting another player

[Download client](https://changemakereducation-my.sharepoint.com/personal/joel_fallbom_futuregames_nu/_layouts/15/onedrive.aspx?originalPath=aHR0cHM6Ly9jaGFuZ2VtYWtlcmVkdWNhdGlvbi1teS5zaGFyZXBvaW50LmNvbS86ZjovZy9wZXJzb25hbC9qb2VsX2ZhbGxib21fZnV0dXJlZ2FtZXNfbnUvRWwyd1FvWnFTQlpGcWZ0SWNKbEZXN1FCM1BLVlRZMlI2a01oMDA4d3Y1cjh2QT9ydGltZT1NbTVuc1hHSzJVZw&id=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds%2FGP3%5FT7%5FPrototype%5FV1%2E2%2Ezip&parent=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds)

Commit `d03c0c313358860464881d49c93a78c42bc34308`


## V1.1 `Prototype V1.1`

This version is mostly bug fixes.

**New features**
* Client and Server versions have to match to connect (Also displayed in the UI)

**Fixes**
* Fixed Ghost position bug
* Visual fixes when ending round (Hides aimcones)
* Aimcone works without errors
* You can no longer connect multible times (Creating "dead" users).
* Creating a cone on a Unit with empty timeline correctly shows the cone at the Unit and not at (0,0)


[Download client](https://changemakereducation-my.sharepoint.com/personal/joel_fallbom_futuregames_nu/_layouts/15/onedrive.aspx?originalPath=aHR0cHM6Ly9jaGFuZ2VtYWtlcmVkdWNhdGlvbi1teS5zaGFyZXBvaW50LmNvbS86ZjovZy9wZXJzb25hbC9qb2VsX2ZhbGxib21fZnV0dXJlZ2FtZXNfbnUvRWwyd1FvWnFTQlpGcWZ0SWNKbEZXN1FCM1BLVlRZMlI2a01oMDA4d3Y1cjh2QT9ydGltZT1LMU9vVFptSjJVZw&id=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds%2FGP3%5FT7%5FPrototype%5FV1%2E1%2Ezip&parent=%2Fpersonal%2Fjoel%5Ffallbom%5Ffuturegames%5Fnu%2FDocuments%2FFGSKE20GP3%2FTeam%207%2FBuilds)


Commit `f16f039e0e82705d4b25d6a7b9d66ad3f007b250`

## V1 `Prototype V1.0`

This is the first playable version of the prototype. You can connect, play against another player online and move your units. You can also place guards for your characters. The guards do not shoot yet. You can also edit the timeline.

**Known bugs**
* Ghost position is not the same as the unit position sometimes when playing against another live player.
* Cone aiming is confirmed when pressed through UI
* Sometimes Cone aiming does not work

[Download client](https://changemakereducation-my.sharepoint.com/:u:/r/personal/joel_fallbom_futuregames_nu/Documents/FGSKE20GP3/Team%207/Builds/GP3_T7_Prototype_V1.zip?csf=1&web=1&e=BJlQ8R)

Commit `16b91451617fe9296f93a1ef6345dbd4c71456ef`