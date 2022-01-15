import argparse
import audio_pipeline as ap
import random
import time

from pythonosc import udp_client

if __name__ == "__main__":

  parser = argparse.ArgumentParser()
  parser.add_argument("--ip", default="127.0.0.1",
      help="The ip of the OSC server")
  parser.add_argument("--port", type=int, default=7500,
      help="The port the OSC server is listening on")
  args = parser.parse_args()

  client = udp_client.SimpleUDPClient(args.ip, args.port)
  
  audio_sample_length = 10 #seconds
  audio_data = ap.record_audio(audio_sample_length)
  #print(audio_data)  #prints hexadecimal audio data array

  for x in range(10):
    client.send_message("/composition/dashboard/link1", random.random())
    time.sleep(1)