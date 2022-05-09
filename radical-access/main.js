//
//
//

import {io}  from "socket.io-client";
import { Client } from "node-osc";
import { MixamoConversion } from "./MixamoConversion.js";

// RADICAL SERVER PARAMS
const WSSERVER = 'wss://room-handler.live-prod.live.k8s.getrad.co';
const AUTH_TOKEN = '737ecc44-e976-470c-8c32-0b5091a1f059';
const ROOM_ID = 'b9f6420e-b615-4ac6-bba0-77fcd6224a3f';
const PLAYER_ID = 'single-player-id';
const RPM_URL = '/Assets/Models/Characters/ReadyPlayerMe/04/dino.glb';

// OSC RELAY PARAMS
const OSC_TARGET_IP = "127.0.0.1";
const OSC_TARGET_PORT = 9000;

const playerList = [];


const logData = (data) => console.log(data);

console.log("Creating OSC client pointing at " + OSC_TARGET_IP + ":" + OSC_TARGET_PORT);
const oscClient = new Client(OSC_TARGET_IP, OSC_TARGET_PORT);


console.log("Connecting to Radical WS Server at " + WSSERVER);


// import { LiveClientThreeJS } from "@get-rad/radical-live-client-webgl";

// var client = new LiveClientThreeJS(WSSERVER, AUTH_TOKEN, ROOM_ID);
// client.connect().then(()=>{
//     console.log("Live client connected");
// });

const wsConnection = io(WSSERVER, {
    rejectUnauthorized: false,
    reconnection: false,
    timeout: 30000,
    transports: ['websocket'],
});

wsConnection.on('connect', ()=>{
    console.log("Connected to Radical WS Server, authenticating with token");
    wsConnection.emit('audience-registration', {
        token : AUTH_TOKEN,
        clientLabel : 'stage3d',
        room : ROOM_ID
    });
});

wsConnection.on('player-connected', (reason)=>{

});

wsConnection.on('prediction-result', (message)=>{

    // console.log("RESULT: ");
    // console.log(message);

    var playerID = message.attendeeId;

    if (playerList.indexOf(playerID) == -1){
        console.log("Found new player " + playerID);
        playerList.push(playerID);
    }

    // loop through result, send OSC message for each bone / landmark
    for (var i=0; i < message.result.length; i++){
        var frame_data = message.result[i].frame_data;
        var address = "/scene/dancer/poseRadical/" + playerID.replace(" ", "_") + "/";
    
        // console.log("###");
        for (var bone in frame_data){
            // console.log("sending for bone " + bone);
            // console.log(frame_data[bone]);
            oscClient.send(address + bone, frame_data[bone]);
            
           // console.log(bone);
        }



    }

    
    

    // if (message.attendeeId == PLAYER_ID){
    //     if (message.result.length > 0){
    //         // console.log("Data for " + PLAYER_ID);
    //         // console.log(message.result[0]);
    //         // console.log(message.timestamp);      
    //         var pose = message.result[0];
            
    //         // console.log(pose);
    //     } else {
    //         console.error("ERROR - no pose results");
    //     }
        
    // }
});

