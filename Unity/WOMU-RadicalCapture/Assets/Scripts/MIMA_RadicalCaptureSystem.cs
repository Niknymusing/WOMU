using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using Klak.Spout;
using Klak.Syphon;
using Radical;
using UnityEngine;


public class MIMA_RadicalCaptureSystem : MonoBehaviour
{
     public MIMA_OSCManager oscManager;
     public MIMA_RadicalCaptureUI radicalUI;
     public RadicalLiveInterface radicalLiveInterface;
     public AnimationPlayback radicalAnimPlayback;

     public GameObject characterPrefab;
     private GameObject character;

     private MIMA_CharacterPoseControl characterOSC;

     public int radicalNumFrames = 0;
     private bool radicalConnected = false;
     private string radicalLastErrorString = "";

     public int oscFramesSent = 0;
     public int characterFrames = 0;

     void Start()
     {
          radicalUI.OnStartRadical += (room, url) =>
          {
               // create character
               if (character != null)
               {
                    Destroy(character);
               }

               Debug.Log($"Spawning new character from prefab {characterPrefab.name}");
               character = GameObject.Instantiate(characterPrefab, Vector3.zero, Quaternion.identity, null);

               characterOSC = character.AddComponent<MIMA_CharacterPoseControl>();
               characterOSC.StartExportFrames();
               characterFrames = 0;

               characterOSC.OnFrame += frame =>
               {
                    characterFrames++;
                    if (oscManager.IsSending)
                    {
                         var messages = MIMA_CharacterPoseControl.FrameToOsc(frame);
                         oscManager.SendMessages(messages);
                         oscFramesSent++;
                    }
               };
               
               
               AIPlayer.Instance.AssignModel(character);
               radicalAnimPlayback.StartPlaybackLive(0);
               
               

               Debug.Log($"Connecting to radical, room: {room}, url : {url}");
               
               // create new interface, connect to room
               radicalLiveInterface = new RadicalLiveInterface(url, room);
               radicalLiveInterface.OnConnected(OnConnectedToRadicalRoom);
               radicalLiveInterface.OnData(OnRadicalData);     
               radicalLiveInterface.OnError((errorString) =>
               {
                    Debug.LogError(errorString);
                    radicalLastErrorString = errorString;
               });
               radicalLiveInterface.Connect();
          };

          radicalUI.OnToggleOSCSend += (enableOSC, IP, portOut) =>
          {
               if (enableOSC)
               {
                    oscManager.StartOSCSending(IP, portOut);
               }
               else
               {
                    oscManager.StopOsc();
               }
          };

          radicalUI.OnStopRadical += () =>
          {
               Debug.Log($"Disconnecting from Radical");
               // radical interface calls Disconnect during deconstructor
               radicalLiveInterface = null;
               radicalConnected = false;
               radicalLastErrorString = "";
               radicalNumFrames = 0;
               oscFramesSent = 0;
               if (character != null) Destroy(character);
               character = null;
          };

          radicalUI.OnToggleSpoutOutput += spoutEnabled =>
          {    
               
               #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
               if (spoutEnabled)
               {
                    var sender = Camera.main.gameObject.AddComponent<SpoutSender>();
                    sender.captureMethod = CaptureMethod.Camera;
                    sender.spoutName = "MIMA_RadicalCapture";
                    sender.sourceCamera = Camera.main;
               }
               else
               {
                    var sender = Camera.main.gameObject.GetComponent<SpoutSender>();
                    if (sender != null){ Destroy(sender);}
               }
               #else
               
               
               if (spoutEnabled)
               {
                    var sender = Camera.main.gameObject.AddComponent<SyphonServer>();
               }
               else
               {
                    var sender = Camera.main.gameObject.GetComponent<SyphonServer>();
                    if (sender != null){ Destroy(sender);}
               }
               #endif
          };
     }

     private void OnConnectedToRadicalRoom()
     {
          
          Debug.Log($"Connected to Radical room ");
          radicalConnected = true;
          
          
     }

     private void OnRadicalData(IList<JsonElement> data)
     {
          radicalNumFrames++;
          radicalAnimPlayback.playLiveFrame(data);
     }

     private void FixedUpdate()
     {

          string oscStatus = oscManager.IsSending ? "sending" : "not sending";
          radicalUI.SetOSCStatus($"{oscStatus} frames : {oscFramesSent}");
          string connStatus = radicalConnected ? "connected" : "disconnected";
          radicalUI.SetRadicalStatus($"{radicalLastErrorString} {connStatus}, frames : {radicalNumFrames}");
     }
}
