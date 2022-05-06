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


## Adding & configuring Scenes

MIMA contains the concept of a `MIMA_SceneCollection`, which contains a number of `MIMA_Scene`s. Both are ScriptableObjects in Unity, that can be authored to add new scenes, or new configurations of existing scenes in the project.

The default Collection is `TestCollection`.

a `MIMA_Scene` Scriptable object contains the following:
### Texture Maps
A list of places that can receive textures
- `Target Name` - the name of the map (no spaces allowed)
- `Target Type` - the type of target
- `Target Mat` - if target type == MATERIAL, which material will be overwritten
- `Target Light Name` - if target type == LIGHT_COOKIE, finds a light with this name and sets the Cookie parameter to the texture
- `Target Projector Name` - if target type == DECAL_PROJECTOR, finds a Projector with this name and sets the Decal texture 
- `Target Effect Name` - if targettype == VISUAL_EFFECT, looks in the list of scene Effects and sets textures accordingly.
- `Texture Names` - a list of texture params on Materials to overwrite with the new Textures.
- `Offset` - sets the texture offset, where possible
- `Scale` - sets the texture scale, where possible
`SourceName` - the name of the NDI or Spout source that this mapping is pulling from.

### Effects
A list of prefabs containing a `MIMA_Effect` class, that can be controlled via OSC, texture-mapped etc.

*IMPORTANT NOTE* - Dancers are a special kind of Effect, that response to pose + some other specific commands.

- `Name` - the name by which it will be accessed via OSC (no spaces allowed)
- `Prefab` - the Unity prefab which gets loaded by this Effect. Must have a `MIMA_Effect` behaviour.

### Camera Positions
A list of transforms which are found by name, and stored as camera positions, which can be jumped to via OSC.

### Reflection Probe Refresh Mode
A performance thing, having the Mode on Every Frame is very heavy, so most of the time you'll want it On Awake, unless there's a particular visual effect you're after.

### Scene
A reference to a Unity scene that gets loaded.


## OSC API

### Scenes
`/scene/load [scene name]` - Loads a scene by name from the current Collection

`/scene/blackout [toValue] [timeSeconds]` - either blacks out (`toValue = 1.0`) or fades in (`toValue = 0.0`) over `timeSeconds`

### Cameras
`/scene/listCameras` - tells Unity to output a list of the current camera positions in the Scene via OSC.


`/scene/camera/[which]/setRandomMotion [amount]` - tells camera `which` ('main', or a number) to set the amount of shake to `amount`
`/scene/camera/[which]/move [x] [y] [z]` - moves camera `which` ('main', or a number) by x, y, z


### Texture Maps
`/scene/map/[map Name]/setSource [name]` - sets the source of Texture Map [map Name] to [name]

### Effects

`/scene/effect/[effect name]/set/[paramName] [value]` - set [paramName] on a Visual Effect called [effect name].

For the Generic Particle effect, these include :

- `attachToMainCamera` - 1 or 0
- `sphericalSpread` - any float number - size of sphere
- `spawnRate` - float 0.0 - 1.0
- `edgeThickness` float 0.0 - 1.0 - whether the particles spawn on the edge of the sphere or completely inside it
- `gridInterval` - int between 5 and 2000 - does a little angular dance
- `trailThickness` - float 0.0 - 1.0 - width of trails
- `speed` - float 0.0 - 1.0 - speed of particles
- `turbulence` - float 0.0 - 1.0 - amount of turbulent noise
- `turbulenceFrequency` - float - small values like 0.001 produce smooth noise, higher values produce chaos


### Dancers

`/scene/dancer/pose[Radical/Mediapipe]/[clientID]/[componentName] [x] [y] [z] ([w])` - sends a Pose to the Dancer Effect that is currently set to listen to `clientId`. 

When `poseRadical` is used, it expects either 4 arguments to construct a Quaternion that is used to set the rotation of bone `componentName`, or 3 arguments to set the root (hip) position.

When `poseMediapipe` is used, it expects 3 arguments x,y,z to position the landmark numbered `componentName`.

`/scene/dancer/setId/[dancerName] [newId]` - sets the Dancer Effect to listen to Pose commands with the `clientId` of `newId`.

`/scene/dancer/scale/[dancerName] [scale]` - sets the named Dancer Effect's position scale to `scale`



