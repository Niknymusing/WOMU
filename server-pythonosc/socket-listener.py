#!/usr/bin/env python

import asyncio
import websockets


async def hello():
    async with websockets.connect('wss://womu-server.jonasjohansson.repl.co') as websocket:

        greeting = await websocket.recv()
        print("< {}".format(greeting))

asyncio.get_event_loop().run_until_complete(hello())
