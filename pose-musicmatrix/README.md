# MusicMatrixProject
Code for the Music Matrix project.

## Open Sound Control (OSC) Web Bridge

Creates a simple bridge between Pose and Unity.

    .----------.              .----------------------.    .------------------.                 .------.
    |   Unity  | --tcp/udp--> | bridge.js OSC server | => | socket.io client | --websockets--> | Pose |
    `--(3334)--'              `-------( 3333 )-------'    `------------------'                 .------.
         ^                                                                                          |
         |                                                                                          |
         |                                                                                          |
         |                .----------------------.    .------------------.                          |
         `---tcp/udp----- | bridge.js OSC client | <= | socket.io server | <-------websockets-------'
                          `----------------------'    `-----( 8081 )-----'

## Pose

Body movements are detected via web camera using PoseNet and sent via OSC to Unity for visualization.

## Unity

Unity is used to render body movements and music.

## Usage

Install dependencies and run the bridge app on your machine (localhost):

```
$ cd MusicMatrixHackathon
$ node bridge.js
```

Build and run Pose:

```
$ cd pose
$ yarn
$ yarn watch
```

Run Unity
