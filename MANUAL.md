# MIMA Documentation

# System Diagram

                *Visual Sources*            .------------.
                .-------.                   |            |                .-------.
    Entercast->-|  VLC  |----------NDI----->|            |----Spout-----> |  OBS  |--- RTMP --> Vimeo,etc
                '-------'                   |            |                '-------'
                .-----------.               |            |                    ^
    Live VJ-->  |  Resolume |------NDI----->|            |                    |
                '-----------'               |            |                    |
                                            |            |                 Audio Mix
                *Control Inputs*            |            |
                .---------------.           |            |
    Producer--> | TouchDesigner |--OSC----->|            |
                '---------------'           |            |
                .---------------.           |    Unity   |
    CoProducer->| TouchDesigner |--OSC----->|            |
                '---------------'           |            |
                .---------------.           |            |
    Dancer ---->|Posenet/Radical|           |            |
                | web interfaces|           |            |
                '---------------'           |            |
                        ↓ websockets        |            |
                .---------------.           |            |
                |  Node server  |--OSC----->|            |
                '---------------'           |            |
                        ↑ websockets        |            |
                .---------------.           |            |
    Audience--->|    Website    |           |            |
    control     '---------------'           |            |                                            
                                            |            |
                                            '------------'

# First steps
Instructions on getting started with a live broadcast using the MIMA Unity app + OSC control (via TouchDesigner or others)
## Requirements
- Download Unity Hub, use it to install Unity Editor 2020.3.26f1
- Download VLC : https://www.videolan.org/vlc/
- Download NDI Tools : https://www.ndi.tv/tools/
- Download TouchDesigner 2021 : https://derivative.ca/download
- Clone this repository

## Loading a scene, bringing in RTMP stream
MIMA currently consists of a number of systems working together, with a Unity app at the center of it all, which renders the final(?) visual for broadcast.
### Unity
- Run WOMU-Prototype Unity project in `Unity\WOMU-prototype` folder (or use a build). Ensure WOMU-Main scene is loading first.
### Entercast RTSP stream
- Start up an RTSP stream using Entercast, or find an existing test stream
- Launch VLC, ensure the NDI output plugin is enabled (see [here](https://help.ptzoptics.com/support/solutions/articles/13000072212-how-to-turn-an-rtsp-feed-into-an-ndi-source-using-vlc))
- Open the RTSP stream in VLC. It should show a black screen, meaning it's outputting via NDI. 

### TouchDesigner Producer interface
- Run TouchPlayer, and drag `touchdesigner\MIMA-control-prototype-1.toe` onto the spinning cube
- If TouchPlayer interface is on a separate machine, ensure correct IP address is entered in bottom-right corner `MIMA IP Address`. Otherwise it should be `127.0.0.1`
- In TouchDesigner, choose `Producer` from the page list on the left, and click `Load Kulturbrott`. Unity should load the Kulturbrott scene.
- Select `TextureMaps` from the page list, click `List Texture Maps` and then click `List Sources`. The Source List should update to include VLC. 
- Select `MainScreen` as the Target Map, and VLC as the Source. Then click `Update Source MainScreen with MACHINE_NAME#(VLC)`
- The Unity scene should pull in the NDI feed from VLC.



# Inputs & Outputs

## Visual inputs
- MIMA Unity app can receive video feeds via NDI or Spout (Syphon on OSX). These video feeds can be used for:
    - Billboard textures such as flat 'screens' in the virtual world
    - Virtual 'Projectors' which act like real-world projectors, casting images onto 3d geometry in the scene
    - Colour map source for effects, that can choose a random colour based on the video feed as a palette

## Control inputs
- Control inputs (including dancer poses) all arrive via OSC. These can control:
    - Scene changes
    - Camera position / rotation / motion
    - Effects parameters
    - Dancer poses
    - Video feed routing to destinations as above, e.g. NDI input -> a billboard texture.
- Control inputs can come in simultaneously from various sources. To make this easier, we build interfaces for the different 'roles' involved in a performance.
    - Producer has overall control of the current MIMA scene, uses a TouchDesigner interface
    - Co-producer also uses TouchDesigner interface
    - Dancer visits a website which accesses webcam, uses Posenet to generate pose, transfers this information via Websockets to Node server, which relays OSC to Unity
    - Audience controls are provided via a website, which also transfers these commands via websocket to Node server, which relays OSC to Unity.

## Outputs
- The Main Camera in the MIMA Unity broadcasts video via Spout. This can be picked up via OBS and either sent directly broadcast

## Producer interface

