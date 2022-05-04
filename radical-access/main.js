import { LiveClientThreeJS } from "@get-rad/radical-live-client-webgl";

const WSSERVER = 'wss://room-handler.live-prod.live.k8s.getrad.co';
const AUTH_TOKEN = '737ecc44-e976-470c-8c32-0b5091a1f059';
const ROOM_ID = '4ad7b61c-ffe3-42a5-bcfd-9cb4406e96fa';
const PLAYER_ID = 'single-player-id';
const RPM_URL = '/Assets/Models/Characters/ReadyPlayerMe/04/dino.glb';

const logData = (data) => console.log(data);

this.client = new LiveClientThreeJS(WSSERVER, AUTH_TOKEN, ROOM_ID);
this.client.connect().then(() => {
    this.client.getPlayerData(PLAYER_ID, logData)

//   this.client.addCharacter(PLAYER_ID, RPM_URL).then((character) => {
//     console.log('character added: ', character);
//     this.add(character);
//     return true;
//   });
}).catch(() => false); 

