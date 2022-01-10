#!/usr/bin/env python3
import argparse
import random
import time
import json
import websocket

from pythonosc import udp_client

server_address = 'wss://womu-server.jonasjohansson.repl.co'
ip = "127.0.0.1"
port = 7500

client = udp_client.SimpleUDPClient(ip, port)


def on_message(wsapp, message):
    msg = json.loads(message)
    addr = msg["address"]
    val = msg["value"]
    print(addr, val)
    client.send_message(addr, val)


wsapp = websocket.WebSocketApp(
    server_address, on_message=on_message)
wsapp.run_forever()
