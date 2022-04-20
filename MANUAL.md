# MIMA System
Instructions on getting started with a live broadcast using the MIMA Unity app + OSC control (via TouchDesigner or others)


### Requirements
- Download Unity Hub, use it to install Unity Editor 2020.3.26f1
- Download VLC : https://www.videolan.org/vlc/
- Download NDI Tools : https://www.ndi.tv/tools/
- Download TouchDesigner 2021 : https://derivative.ca/download
- Clone this repository

### Getting Started - loading a scene, bringing in RTMP stream
- Run WOMU-Prototype Unity project in `Unity\WOMU-prototype` folder (or use a build)
- Run TouchPlayer, and drag `touchdesigner\MIMA-control-prototype-1.toe` onto the spinning cube
	- If TouchPlayer interface is on a separate machine, ensure correct IP address is entered in bottom-right corner `MIMA IP Address`. Otherwise it should be `127.0.0.1`
- Start up an RTSP stream using Entercast, or find an existing test stream
- Launch VLC, ensure the NDI output plugin is enabled (see [here](https://help.ptzoptics.com/support/solutions/articles/13000072212-how-to-turn-an-rtsp-feed-into-an-ndi-source-using-vlc))
- Open the RTSP stream in VLC. It should show a black screen, meaning it's outputting via NDI. 
- In TouchDesigner, choose `Producer` from the page list on the left, and click `Load Kulturbrott`. Unity should load the Kulturbrott scene.
- Select `TextureMaps` from the page list, click `List Texture Maps` and then click `List Sources`. The Source List should update to include VLC. 
- Select `MainScreen` as the Target Map, and VLC as the Source. Then click `Update Source MainScreen with MACHINE_NAME#(VLC)`
- The Unity scene should pull in the NDI feed from VLC.


## System Diagram

				*Visual Sources*			.-----------.						
				.-------.					|			|				.-------.
	Entercast->-|  VLC	|----------NDI----->|			|----Spout----->|  OBS  |--- RTMP --> Vimeo,etc
				'-------'					|			|				'-------'
				.-----------.				|			|					^
	Live VJ	--> |  Resolume	|------NDI----->|			|					|
				'-----------'				|			|					|
											|			|			 	Audio Mix
				*Control Inputs*			|			|
				.---------------.			|			|
	Producer-->	| TouchDesigner |--OSC----->|			|
				'---------------'			|			|
				.---------------.			|	Unity	|
	CoProducer->| TouchDesigner |--OSC----->|			|
				'---------------'			|			|
				.---------------.			|			|
	Dancer ---->|Posenet website|			|			|
				'---------------'			|			|
						↓ websockets		|			|
				.---------------.			|			|
				|	Node server	|--OSC----->|			|
				'---------------'			|			|
						↑ websockets		|			|
				.---------------.			|			|
	Audience--->|	Website		|			|			|
	control		'---------------'			|			|											
											|			|
											'-----------'

- The MIMA Unity app receives video feeds and OSC commands

- Visual sources arrive via NDI. This will be normally be an artist using Entercast, or a live VJ. It could also be a local camera feed from the studio where MIMA is physically situated.

- Control inputs all arrive via OSC:
	- Producer has overall control of the current MIMA scene, uses a TouchDesigner interface
	- Co-producer also uses TouchDesigner interface
	- Dancer visits a website which accesses webcam, uses Posenet to generate pose, transfers this information via Websockets to Node server, which relays OSC to Unity
	- Audience controls are provided via a website, which also transfers these commands via websocket to Node server, which relays OSC to Unity.

- MIMA app outputs Spout to OBS, which streams to endpoint (e.g. Vimeo etc)

## Producer interface

